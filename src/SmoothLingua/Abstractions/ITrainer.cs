namespace SmoothLingua.Abstractions;

public interface ITrainer
{
    Task Train(Domain domain, string path);

    Task Train(Domain domain, Stream stream);
}
