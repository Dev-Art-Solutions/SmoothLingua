namespace SmoothLingua.Abstractions.Rules;

/// <summary>
/// An always-active single-turn response that fires whenever <paramref name="IntentName"/> is predicted,
/// taking precedence over any active story.
/// </summary>
/// <param name="Name">Unique name for this rule.</param>
/// <param name="IntentName">The intent that triggers this rule.</param>
/// <param name="Response">The fixed reply message returned when the rule fires.</param>
public record Rule(string Name, string IntentName, string Response);