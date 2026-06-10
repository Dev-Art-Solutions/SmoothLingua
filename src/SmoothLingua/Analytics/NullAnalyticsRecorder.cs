namespace SmoothLingua.Analytics;

using SmoothLingua.Abstractions.Analytics;

/// <summary>
/// No-op <see cref="IAnalyticsRecorder"/> used as the default so existing hosts incur no overhead
/// until they opt in to analytics.
/// </summary>
public sealed class NullAnalyticsRecorder : IAnalyticsRecorder
{
    /// <summary>Shared singleton instance.</summary>
    public static readonly NullAnalyticsRecorder Instance = new();

    private static readonly AnalyticsSnapshot EmptySnapshot =
        new(0, 0, 0, 0f, 0f, 0f, new List<IntentStat>());

    /// <inheritdoc/>
    public void Record(AnalyticsEvent evt) { }

    /// <inheritdoc/>
    public AnalyticsSnapshot GetSnapshot() => EmptySnapshot;
}
