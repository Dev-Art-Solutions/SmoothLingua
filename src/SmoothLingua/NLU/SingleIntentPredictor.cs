namespace SmoothLingua.NLU;

using Abstractions.NLU;

public class SingleIntentPredictor(string intentName) : IPredictor
{
    public (string IntentName, float Confidence) Predict(string text) => (intentName, 1.0f);
}
