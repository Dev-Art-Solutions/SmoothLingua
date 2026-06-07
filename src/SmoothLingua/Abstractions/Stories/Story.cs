namespace SmoothLingua.Abstractions.Stories;

/// <summary>
/// A named multi-turn conversation flow composed of alternating <see cref="IntentStep"/> and <see cref="ResponseStep"/> entries.
/// At runtime the agent advances through the story as the user's intents match the expected steps.
/// </summary>
/// <param name="Name">Unique name for this story (used for debugging and logging).</param>
/// <param name="Steps">Ordered sequence of intent and response steps that define the flow.</param>
public record Story(string Name, List<Step> Steps);
