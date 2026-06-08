using SmoothLingua.Abstractions.NLU;
using SmoothLingua.Abstractions.Rules;
using SmoothLingua.Abstractions.Stories;

namespace SmoothLingua.Abstractions;

/// <summary>
/// Represents the complete definition of a conversational agent.
/// Pass a <see cref="Domain"/> to <see cref="ITrainer.Train"/> to produce a deployable model.
/// </summary>
/// <param name="Intents">All intents the agent should recognise.</param>
/// <param name="Stories">Multi-turn conversation flows composed of intent and response steps.</param>
/// <param name="Rules">Always-active single-turn shortcuts that take precedence over stories.</param>
/// <param name="Slots">Optional slot definitions for extracting named values from user messages.</param>
/// <param name="Entities">Optional entity definitions that list known values for each slot.</param>
/// <param name="ConfidenceThreshold">Minimum confidence required to use the predicted intent. Predictions below this value trigger the fallback intent. Default is <c>0.4</c>.</param>
/// <param name="FallbackIntentName">Intent name used when the predicted confidence is below <see cref="ConfidenceThreshold"/>. Default is <c>"nlu_fallback"</c>.</param>
public record Domain(
    List<Intent> Intents,
    List<Story> Stories,
    List<Rule> Rules,
    List<Slot>? Slots = default,
    List<Entity>? Entities = default,
    float ConfidenceThreshold = 0.4f,
    string FallbackIntentName = "nlu_fallback");
