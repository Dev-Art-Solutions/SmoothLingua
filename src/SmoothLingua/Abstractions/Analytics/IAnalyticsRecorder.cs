namespace SmoothLingua.Abstractions.Analytics;

/// <summary>
/// Captures anonymous per-turn signals and exposes aggregate insights about the agent's behaviour.
/// Implementations must be safe to call concurrently from multiple conversations.
/// </summary>
public interface IAnalyticsRecorder
{
    /// <summary>Records a single turn. Called by the agent after every <c>Handle</c>.</summary>
    void Record(AnalyticsEvent evt);

    /// <summary>Returns a snapshot of aggregated insights over everything recorded so far.</summary>
    AnalyticsSnapshot GetSnapshot();
}
