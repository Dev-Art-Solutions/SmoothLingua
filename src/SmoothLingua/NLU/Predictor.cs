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

    public (string IntentName, float Confidence) Predict(string text)
    {
        var prediction = predictionEngine.Predict(new IntentTrainingData(text, string.Empty));
        return (prediction.PredictedLabel, ComputeConfidence(prediction.Score));
    }

    // Softmax over raw SDCA scores to produce a value in (0, 1].
    private static float ComputeConfidence(float[] scores)
    {
        if (scores.Length == 0)
            return 0f;

        var max = scores.Max();
        var expScores = scores.Select(s => Math.Exp(s - max)).ToArray();
        return (float)(expScores.Max() / expScores.Sum());
    }
}
