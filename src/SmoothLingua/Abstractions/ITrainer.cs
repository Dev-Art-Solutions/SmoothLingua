namespace SmoothLingua.Abstractions;

public interface ITrainer
{
    Task Train(Domain domain, string path, CancellationToken cancellationToken = default);

    Task Train(Domain domain, Stream stream, CancellationToken cancellationToken = default);
}
