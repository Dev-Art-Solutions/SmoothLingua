namespace SmoothLingua.NLU;

using Microsoft.ML;
using System.IO;

using Abstractions.NLU;
using Microsoft.ML.Data;

public class Trainer : ITrainer
{
    private readonly string Features = "Features";
    private readonly string Label = "Label";
    private readonly string PredictedLabel = "PredictedLabel";

    public void Train(List<Intent> intents, string filePath)
    {
        var context = new MLContext();
        var trainingData = PrepareData(context, intents);
        var transformer = TrainModel(context, trainingData);

        context.Model.Save(transformer, trainingData.Schema, filePath);
    }

    public void Train(List<Intent> intents, Stream stream)
    {
        var context = new MLContext();
        var trainingData = PrepareData(context,intents);
        var transformer = TrainModel(context, trainingData);

        context.Model.Save(transformer, trainingData.Schema, stream);
    }

    private static IDataView PrepareData(MLContext context, List<Intent> intents)
    {
        var data = intents.SelectMany(i => i.Examples.Select(e => new IntentTrainingData(e, i.Name)));

        return context.Data.LoadFromEnumerable(data);
    }

    private TransformerChain<Microsoft.ML.Transforms.KeyToValueMappingTransformer> TrainModel(MLContext context, IDataView trainingData)
    {
        var pipeline = context.Transforms.Text.FeaturizeText(Features, nameof(IntentTrainingData.Text))
            .Append(context.Transforms.Conversion.MapValueToKey(Label))
            .Append(context.MulticlassClassification.Trainers.SdcaNonCalibrated())
            .Append(context.Transforms.Conversion.MapKeyToValue(PredictedLabel));

        return pipeline.Fit(trainingData);
    }
}
