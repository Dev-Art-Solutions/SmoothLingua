namespace SmoothLingua.Abstractions;

/// <summary>The bot's reply to a single user message.</summary>
/// <param name="IntentName">The intent that was predicted for the user's input (may be the fallback intent when confidence is below threshold).</param>
/// <param name="Messages">One or more reply messages produced by the matching story or rule.</param>
/// <param name="Confidence">How confident the NLU model is in the predicted intent, in the range [0, 1]. Always <c>1.0</c> for single-intent domains.</param>
/// <param name="ExtractedEntities">Entities found in the user's input, keyed by entity name. <c>null</c> when the domain defines no entities or none matched.</param>
public record Response(string IntentName, List<string> Messages, float Confidence, Dictionary<string, string>? ExtractedEntities = null);

