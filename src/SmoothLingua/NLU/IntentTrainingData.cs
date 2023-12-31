namespace SmoothLingua.NLU;

using Microsoft.ML.Data;

internal class IntentTrainingData
{
    public IntentTrainingData(string text, string label)
    {
        Text = text;
        Label = label;
    }

    [LoadColumnName("Text")]
    [LoadColumn(0)]
    public string Text { get; }

    [LoadColumnName("Label")]
    [LoadColumn(1)]
    public string Label { get; }
}
