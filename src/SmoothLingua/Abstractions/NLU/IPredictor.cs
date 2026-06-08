namespace SmoothLingua.Abstractions.NLU;

public interface IPredictor
{
    /// <summary>Predicts the intent for <paramref name="text"/>.</summary>
    /// <returns>A tuple of the predicted intent name and a confidence score in [0, 1].</returns>
    (string IntentName, float Confidence) Predict(string text);
}