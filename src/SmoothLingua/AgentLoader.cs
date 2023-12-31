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
    public static async Task<IAgent> Load(string modelPath)
    {
        Domain domain;
        using (FileStream file = new FileStream(modelPath, FileMode.Open))
        {
            domain = await GetDomain(file);
        }

        var predictor = new Predictor(modelPath);
        var ruleManagerFactory = new RuleManagerFactory(domain.Rules);
        var storyManagerFactory = new StoryManagerFactory(domain.Stories);

        return new Agent(predictor, new ConversationManager(ruleManagerFactory, storyManagerFactory));
    }

    public static async Task<IAgent> Load(Stream stream)
    {
        var domain = await GetDomain(stream);
        var predictor = new Predictor(stream);
        var ruleManagerFactory = new RuleManagerFactory(domain.Rules);
        var storyManagerFactory = new StoryManagerFactory(domain.Stories);

        return new Agent(predictor, new ConversationManager(ruleManagerFactory, storyManagerFactory));
    }

    private static async Task<Domain> GetDomain(Stream stream)
    {
        ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read);
        var entry = archive.GetEntry("domain.json");

        if (entry == null)
        {
            throw new ArgumentException("Domain file is missing");
        }

        StreamReader reader = new StreamReader(entry.Open());
        return JsonConvert.DeserializeObject<Domain>(await reader.ReadToEndAsync(), new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        })!;
    }
}
