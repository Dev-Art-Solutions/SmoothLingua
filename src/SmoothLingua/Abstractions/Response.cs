namespace SmoothLingua.Abstractions;

/// <summary>The bot's reply to a single user message.</summary>
/// <param name="IntentName">The intent that was predicted for the user's input.</param>
/// <param name="Messages">One or more reply messages produced by the matching story or rule.</param>
public record Response(string IntentName, List<string> Messages);

