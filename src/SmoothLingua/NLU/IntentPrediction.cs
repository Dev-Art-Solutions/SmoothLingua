namespace SmoothLingua.NLU;

using Microsoft.ML.Data;

internal class IntentPrediction
{
    [ColumnName("PredictedLabel")]
    public string PredictedLabel { get; set; } = default!;
}
