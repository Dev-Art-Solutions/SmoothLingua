namespace SmoothLingua.Tests.Analytics;

using SmoothLingua.Abstractions.Analytics;
using SmoothLingua.Analytics;

public class InMemoryAnalyticsRecorderTests
{
    [Fact]
    public void GetSnapshot_OnEmpty_ReturnsZeroes()
    {
        var recorder = new InMemoryAnalyticsRecorder();

        var snapshot = recorder.GetSnapshot();

        Assert.Equal(0, snapshot.TotalMessages);
        Assert.Equal(0, snapshot.TotalConversations);
        Assert.Equal(0, snapshot.FallbackHits);
        Assert.Equal(0f, snapshot.FallbackRate);
        Assert.Equal(0f, snapshot.AverageConfidence);
        Assert.Equal(0f, snapshot.AverageConversationLength);
        Assert.Empty(snapshot.Intents);
    }

    [Fact]
    public void Record_AggregatesCountsAcrossConversations()
    {
        var recorder = new InMemoryAnalyticsRecorder();

        recorder.Record(NewEvent("c1", "Greeting", 0.9f, false));
        recorder.Record(NewEvent("c1", "Bye", 0.8f, false));
        recorder.Record(NewEvent("c2", "Greeting", 0.6f, false));
        recorder.Record(NewEvent("c2", "nlu_fallback", 0.2f, true));

        var snapshot = recorder.GetSnapshot();

        Assert.Equal(4, snapshot.TotalMessages);
        Assert.Equal(2, snapshot.TotalConversations);
        Assert.Equal(1, snapshot.FallbackHits);
        Assert.Equal(0.25f, snapshot.FallbackRate, 0.001f);
        Assert.Equal(2f, snapshot.AverageConversationLength);
        Assert.Equal((0.9f + 0.8f + 0.6f + 0.2f) / 4f, snapshot.AverageConfidence, 0.001f);
    }

    [Fact]
    public void GetSnapshot_OrdersIntentsByCountDescending()
    {
        var recorder = new InMemoryAnalyticsRecorder();

        recorder.Record(NewEvent("c1", "Bye", 0.7f, false));
        recorder.Record(NewEvent("c1", "Greeting", 0.9f, false));
        recorder.Record(NewEvent("c2", "Greeting", 0.5f, false));
        recorder.Record(NewEvent("c3", "Greeting", 0.4f, false));

        var snapshot = recorder.GetSnapshot();

        Assert.Equal(2, snapshot.Intents.Count);
        Assert.Equal("Greeting", snapshot.Intents[0].IntentName);
        Assert.Equal(3, snapshot.Intents[0].Count);
        Assert.Equal((0.9f + 0.5f + 0.4f) / 3f, snapshot.Intents[0].AverageConfidence, 0.001f);
        Assert.Equal("Bye", snapshot.Intents[1].IntentName);
        Assert.Equal(1, snapshot.Intents[1].Count);
    }

    [Fact]
    public void Record_IsThreadSafe()
    {
        var recorder = new InMemoryAnalyticsRecorder();
        const int threads = 8;
        const int perThread = 250;

        Parallel.For(0, threads, t =>
        {
            for (var i = 0; i < perThread; i++)
            {
                recorder.Record(NewEvent($"c{t}", "Greeting", 0.5f, false));
            }
        });

        var snapshot = recorder.GetSnapshot();

        Assert.Equal(threads * perThread, snapshot.TotalMessages);
        Assert.Equal(threads, snapshot.TotalConversations);
        Assert.Equal(threads * perThread, snapshot.Intents.Single().Count);
    }

    private static AnalyticsEvent NewEvent(string convId, string intent, float confidence, bool isFallback)
        => new(convId, intent, confidence, isFallback, DateTimeOffset.UtcNow);
}
