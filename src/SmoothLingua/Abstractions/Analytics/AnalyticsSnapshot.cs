namespace SmoothLingua.Abstractions.Analytics;

/// <summary>
/// Anonymous, aggregate view over recorded turns. Designed to answer
/// "where is the agent unsure?" and "which intents need more examples?" — never
/// to expose individual conversations.
/// </summary>
/// <param name="TotalMessages">Total number of recorded turns.</param>
/// <param name="TotalConversations">Number of distinct conversation identifiers observed.</param>
/// <param name="FallbackHits">Number of turns that resolved to the fallback intent.</param>
/// <param name="FallbackRate">Share of turns that fell back, in [0, 1]. Zero when no turns have been recorded.</param>
/// <param name="AverageConfidence">Mean confidence across all turns, in [0, 1]. Zero when no turns have been recorded.</param>
/// <param name="AverageConversationLength">Mean number of turns per conversation. Zero when no conversations exist.</param>
/// <param name="Intents">Per-intent counts and mean confidence, sorted by <see cref="IntentStat.Count"/> descending.</param>
public record AnalyticsSnapshot(
    int TotalMessages,
    int TotalConversations,
    int FallbackHits,
    float FallbackRate,
    float AverageConfidence,
    float AverageConversationLength,
    List<IntentStat> Intents);
