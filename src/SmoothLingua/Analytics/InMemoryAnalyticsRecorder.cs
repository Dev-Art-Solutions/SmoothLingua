namespace SmoothLingua.Analytics;

using System.Collections.Concurrent;
using SmoothLingua.Abstractions.Analytics;

/// <summary>
/// Default <see cref="IAnalyticsRecorder"/> that keeps counters in memory. Lightweight, lock-free,
/// no external dependencies. State is lost when the process exits.
/// </summary>
public sealed class InMemoryAnalyticsRecorder : IAnalyticsRecorder
{
    private readonly ConcurrentDictionary<string, IntentAccumulator> intents = new();
    private readonly ConcurrentDictionary<string, byte> conversations = new();
    private long totalMessages;
    private long fallbackHits;
    private double confidenceSum;
    private readonly object confidenceLock = new();

    /// <inheritdoc/>
    public void Record(AnalyticsEvent evt)
    {
        Interlocked.Increment(ref totalMessages);
        if (evt.IsFallback) Interlocked.Increment(ref fallbackHits);

        lock (confidenceLock)
        {
            confidenceSum += evt.Confidence;
        }

        conversations.TryAdd(evt.ConversationId, 0);
        intents.AddOrUpdate(
            evt.IntentName,
            _ => new IntentAccumulator(1, evt.Confidence),
            (_, acc) => acc.Add(evt.Confidence));
    }

    /// <inheritdoc/>
    public AnalyticsSnapshot GetSnapshot()
    {
        var total = (int)Interlocked.Read(ref totalMessages);
        var fallback = (int)Interlocked.Read(ref fallbackHits);
        var conversationCount = conversations.Count;

        double avgConfidence;
        lock (confidenceLock)
        {
            avgConfidence = total == 0 ? 0d : confidenceSum / total;
        }

        var fallbackRate = total == 0 ? 0f : (float)fallback / total;
        var avgConversationLength = conversationCount == 0 ? 0f : (float)total / conversationCount;

        var intentStats = intents
            .Select(kvp => new IntentStat(kvp.Key, kvp.Value.Count, kvp.Value.AverageConfidence))
            .OrderByDescending(s => s.Count)
            .ThenBy(s => s.IntentName, StringComparer.Ordinal)
            .ToList();

        return new AnalyticsSnapshot(
            TotalMessages: total,
            TotalConversations: conversationCount,
            FallbackHits: fallback,
            FallbackRate: fallbackRate,
            AverageConfidence: (float)avgConfidence,
            AverageConversationLength: avgConversationLength,
            Intents: intentStats);
    }

    private sealed class IntentAccumulator
    {
        private readonly object gate = new();
        private int count;
        private double confidenceSum;

        public IntentAccumulator(int initialCount, float initialConfidence)
        {
            count = initialCount;
            confidenceSum = initialConfidence;
        }

        public int Count { get { lock (gate) return count; } }

        public float AverageConfidence
        {
            get
            {
                lock (gate)
                {
                    return count == 0 ? 0f : (float)(confidenceSum / count);
                }
            }
        }

        public IntentAccumulator Add(float confidence)
        {
            lock (gate)
            {
                count++;
                confidenceSum += confidence;
                return this;
            }
        }
    }
}
