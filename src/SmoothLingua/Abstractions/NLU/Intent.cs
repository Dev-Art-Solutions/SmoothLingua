namespace SmoothLingua.Abstractions.NLU;

/// <summary>A named category of user utterances used to train the intent classifier.</summary>
/// <param name="Name">Unique identifier for this intent (e.g. <c>"Greeting"</c>).</param>
/// <param name="Examples">Representative phrases that should be classified as this intent.</param>
public record Intent(string Name, List<string> Examples);