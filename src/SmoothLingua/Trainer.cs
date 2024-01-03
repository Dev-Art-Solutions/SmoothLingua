﻿namespace SmoothLingua;

using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;

using Abstractions;

public class Trainer(Abstractions.NLU.ITrainer trainer) : ITrainer
{
    private readonly Abstractions.NLU.ITrainer trainer = trainer;

    public Trainer()
        : this(new NLU.Trainer())
    {
    }

    public async Task Train(Domain domain, string path)
    {
        DomainValidator.Validate(domain);
        trainer.Train(domain.Intents, path);

        using FileStream zipToOpen = new(path, FileMode.Open);
        await StoreDomain(domain, zipToOpen);
    }

    public async Task Train(Domain domain, Stream stream)
    {
        DomainValidator.Validate(domain);
        trainer.Train(domain.Intents, stream);
        await StoreDomain(domain, stream);
    }

    private static async Task StoreDomain(Domain domain, Stream stream)
    {
        using ZipArchive archive = new(stream, ZipArchiveMode.Update);
        ZipArchiveEntry readmeEntry = archive.CreateEntry("domain.json");
        using StreamWriter writer = new(readmeEntry.Open());
        writer.AutoFlush = true;
        await writer.WriteAsync(JsonConvert.SerializeObject(domain, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        }));
    }
}
