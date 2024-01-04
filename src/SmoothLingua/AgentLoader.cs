namespace SmoothLingua;

using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;

using Abstractions;
using Conversations;
using NLU;
using Rules;
using Stories;

public class AgentLoader
{
    public static async Task<IAgent> Load(string modelPath, CancellationToken cancellationToken = default)
    {
        Domain domain;
        using (FileStream file = new(modelPath, FileMode.Open))
        {
            domain = await GetDomain(file, cancellationToken);
        }

        var predictor = new Predictor(modelPath);
        var ruleManagerFactory = new RuleManagerFactory(domain.Rules);
        var storyManagerFactory = new StoryManagerFactory(domain.Stories);

        return new Agent(predictor, new ConversationManager(ruleManagerFactory, storyManagerFactory));
    }

    public static async Task<IAgent> Load(Stream stream, CancellationToken cancellationToken = default)
    {
        var domain = await GetDomain(stream, cancellationToken);
        var predictor = new Predictor(stream);
        var ruleManagerFactory = new RuleManagerFactory(domain.Rules);
        var storyManagerFactory = new StoryManagerFactory(domain.Stories);

        return new Agent(predictor, new ConversationManager(ruleManagerFactory, storyManagerFactory));
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
