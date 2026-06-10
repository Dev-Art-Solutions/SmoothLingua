namespace SmoothLingua.Abstractions.Analytics;

/// <summary>
/// Aggregated statistics for a single intent across all observed turns.
/// </summary>
/// <param name="IntentName">The intent name. The fallback intent is reported alongside the rest.</param>
/// <param name="Count">Total number of turns in which this intent was the effective answer.</param>
/// <param name="AverageConfidence">Mean raw confidence across those turns, in the range [0, 1].</param>
public record IntentStat(string IntentName, int Count, float AverageConfidence);
