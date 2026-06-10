namespace SmoothLingua;

using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;

using Abstractions;
using Abstractions.Analytics;
using Abstractions.Conversations;
using Abstractions.NLU;
using Analytics;
using Conversations;
using NLU;
using NLU.Internal;
using Rules;
using Stories;

/// <summary>Loads a trained model produced by <see cref="Trainer"/> and returns an <see cref="IAgent"/> ready to handle conversations.</summary>
public class AgentLoader
{
    /// <summary>Loads the model from a file on disk.</summary>
    /// <param name="modelPath">Path to the model zip archive written by <see cref="Trainer.Train(Abstractions.Domain, string, CancellationToken)"/>.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <param name="store">Optional conversation store. Defaults to <see cref="InMemoryConversationStore"/> when not provided.</param>
    /// <param name="analyticsRecorder">Optional analytics recorder. Defaults to <see cref="NullAnalyticsRecorder"/> (no overhead) when not provided.</param>
    public static async Task<IAgent> Load(string modelPath, CancellationToken cancellationToken = default, IConversationStore? store = null, IAnalyticsRecorder? analyticsRecorder = null)
    {
        Domain domain;
        using (FileStream file = new(modelPath, FileMode.Open))
        {
            domain = await GetDomain(file, cancellationToken);
        }

        var predictor = NLUContants.MinIntentCount <= domain.Intents.Count ? new Predictor(modelPath) : (IPredictor)new SingleIntentPredictor(domain.Intents[0].Name);
        var ruleManagerFactory = new RuleManagerFactory(domain.Rules);
        var storyManagerFactory = new StoryManagerFactory(domain.Stories);

        return new Agent(
            predictor,
            new ConversationManager(ruleManagerFactory, storyManagerFactory, domain, store),
            domain,
            analyticsRecorder ?? NullAnalyticsRecorder.Instance);
    }

    /// <summary>Loads the model from an in-memory or arbitrary stream.</summary>
    /// <param name="stream">A readable stream containing the model zip archive.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <param name="store">Optional conversation store. Defaults to <see cref="InMemoryConversationStore"/> when not provided.</param>
    /// <param name="analyticsRecorder">Optional analytics recorder. Defaults to <see cref="NullAnalyticsRecorder"/> (no overhead) when not provided.</param>
    public static async Task<IAgent> Load(Stream stream, CancellationToken cancellationToken = default, IConversationStore? store = null, IAnalyticsRecorder? analyticsRecorder = null)
    {
        var domain = await GetDomain(stream, cancellationToken);
        var predictor = NLUContants.MinIntentCount <= domain.Intents.Count ? new Predictor(stream) : (IPredictor)new SingleIntentPredictor(domain.Intents[0].Name);
        var ruleManagerFactory = new RuleManagerFactory(domain.Rules);
        var storyManagerFactory = new StoryManagerFactory(domain.Stories);

        return new Agent(
            predictor,
            new ConversationManager(ruleManagerFactory, storyManagerFactory, domain, store),
            domain,
            analyticsRecorder ?? NullAnalyticsRecorder.Instance);
    }

    private static async Task<Domain> GetDomain(Stream stream, CancellationToken cancellationToken = default)
    {
        ZipArchive archive = new(stream, ZipArchiveMode.Read);
        ZipArchiveEntry entry = archive.GetEntry("domain.json") ?? throw new ArgumentException("Domain file is missing");
        StreamReader reader = new(entry.Open());
        return JsonConvert.DeserializeObject<Domain>(await reader.ReadToEndAsync(cancellationToken), new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        })!;
    }
}
