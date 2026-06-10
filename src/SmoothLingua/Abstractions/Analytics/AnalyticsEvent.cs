namespace SmoothLingua.Abstractions.Analytics;

/// <summary>
/// A single anonymous record of one user turn: which intent was selected, with what confidence,
/// and whether the agent fell back. Conversation identifiers are used only to compute aggregate
/// counts (e.g. unique conversations, average length) and are never exposed in snapshots.
/// </summary>
/// <param name="ConversationId">Opaque identifier supplied by the host (e.g. session id). Used only for aggregation.</param>
/// <param name="IntentName">The intent that the agent acted on for this turn (already resolved to fallback when applicable).</param>
/// <param name="Confidence">Raw confidence reported by the predictor for this turn, in the range [0, 1].</param>
/// <param name="IsFallback">True when the raw prediction was below <c>Domain.ConfidenceThreshold</c> and the fallback intent was used.</param>
/// <param name="Timestamp">UTC timestamp at which the turn was handled.</param>
public record AnalyticsEvent(
    string ConversationId,
    string IntentName,
    float Confidence,
    bool IsFallback,
    DateTimeOffset Timestamp);
