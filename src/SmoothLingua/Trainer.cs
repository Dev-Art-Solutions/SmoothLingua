namespace SmoothLingua;

using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;

using Abstractions;
using NLU.Internal;

/// <summary>
/// Trains a <see cref="Abstractions.Domain"/> using ML.NET and serialises the resulting model to a zip archive
/// that can later be loaded by <see cref="AgentLoader"/>.
/// </summary>
public class Trainer(Abstractions.NLU.ITrainer trainer) : ITrainer
{


    private readonly Abstractions.NLU.ITrainer trainer = trainer;

    public Trainer()
        : this(new NLU.Trainer())
    {
    }

    /// <inheritdoc/>
    public async Task Train(Domain domain, string path, CancellationToken cancellationToken)
    {
        DomainValidator.Validate(domain);

        if (domain.Intents.Count >= NLUContants.MinIntentCount)
        {
            trainer.Train(domain.Intents, path);
        }

        using FileStream zipToOpen = new(path, FileMode.Open);
        await StoreDomain(domain, zipToOpen, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task Train(Domain domain, Stream stream, CancellationToken cancellationToken)
    {
        DomainValidator.Validate(domain);

        if (domain.Intents.Count >= NLUContants.MinIntentCount)
        {
            trainer.Train(domain.Intents, stream);
        }

        await StoreDomain(domain, stream, cancellationToken);
    }

    private static async Task StoreDomain(Domain domain, Stream stream, CancellationToken cancellationToken)
    {
        using ZipArchive archive = new(stream, ZipArchiveMode.Update);
        ZipArchiveEntry readmeEntry = archive.CreateEntry("domain.json");
        using StreamWriter writer = new(readmeEntry.Open());
        writer.AutoFlush = true;
        await writer.WriteAsync(JsonConvert.SerializeObject(domain, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        }).ToArray(), cancellationToken);
    }
}
