namespace SmoothLingua.NLU;

using Abstractions.NLU;

public class SingleIntentPredictor(string intentName) : IPredictor
{
    public string Predict(string text) => intentName;
}
