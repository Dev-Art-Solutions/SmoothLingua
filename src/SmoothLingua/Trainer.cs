namespace SmoothLingua;

using System.IO;
using System.IO.Compression;

using Abstractions;
using Internal;
using Newtonsoft.Json;

public class Trainer : ITrainer
{
    private readonly Abstractions.NLU.ITrainer trainer;

    public Trainer()
        : this(new NLU.Trainer())
    {
    }

    public Trainer(Abstractions.NLU.ITrainer trainer)
    {
        this.trainer = trainer;
    }

    public async Task Train(Domain domain, string path)
    {
        DomainValidator.Validate(domain);
        trainer.Train(domain.Intents, path);

        using (FileStream zipToOpen = new FileStream(path, FileMode.Open))
        {
            await StoreDomain(domain, zipToOpen);
        }
    }

    public async Task Train(Domain domain, Stream stream)
    {
        DomainValidator.Validate(domain);
        trainer.Train(domain.Intents, stream);
        await StoreDomain(domain, stream);
    }

    private async Task StoreDomain(Domain domain, Stream stream)
    {
        using ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Update);
        ZipArchiveEntry readmeEntry = archive.CreateEntry("domain.json");
        using StreamWriter writer = new StreamWriter(readmeEntry.Open());
        writer.AutoFlush = true;
        await writer.WriteAsync(JsonConvert.SerializeObject(domain, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        }));
    }
}
