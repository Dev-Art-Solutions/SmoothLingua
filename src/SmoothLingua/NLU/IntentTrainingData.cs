namespace SmoothLingua.NLU;

using Microsoft.ML.Data;

internal class IntentTrainingData(string text, string label)
{
    [LoadColumnName("Text")]
    [LoadColumn(0)]
    public string Text { get; } = text;

    [LoadColumnName("Label")]
    [LoadColumn(1)]
    public string Label { get; } = label;
}
