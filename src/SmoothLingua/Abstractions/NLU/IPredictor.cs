namespace SmoothLingua.Abstractions.NLU;

public interface IPredictor
{
    string Predict(string text);
}