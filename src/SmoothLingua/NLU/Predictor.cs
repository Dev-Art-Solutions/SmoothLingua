namespace SmoothLingua.NLU;

using Microsoft.ML;

using Abstractions.NLU;

public class Predictor : IPredictor
{
    private readonly PredictionEngine<IntentTrainingData, IntentPrediction> predictionEngine;

    public Predictor(string filePath) 
    {
        var context = new MLContext();
        predictionEngine = context.Model.CreatePredictionEngine<IntentTrainingData, IntentPrediction>(context.Model.Load(filePath, out var _));
    }

    public Predictor(Stream stream) 
    {
        var context = new MLContext();
        predictionEngine = context.Model.CreatePredictionEngine<IntentTrainingData, IntentPrediction>(context.Model.Load(stream, out var _));
    }

    public string Predict(string text)
    {
        return predictionEngine.Predict(new IntentTrainingData(text, string.Empty)).PredictedLabel;
    }
}
