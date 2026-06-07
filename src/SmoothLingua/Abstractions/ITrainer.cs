namespace SmoothLingua.Abstractions;

/// <summary>Trains a <see cref="Domain"/> and serialises the resulting model so it can be loaded by <see cref="AgentLoader"/>.</summary>
public interface ITrainer
{
    /// <summary>Trains the domain and writes the model archive to the specified file path.</summary>
    /// <param name="domain">The domain containing intents, stories, and rules to train.</param>
    /// <param name="path">File path where the model zip archive will be written.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task Train(Domain domain, string path, CancellationToken cancellationToken = default);

    /// <summary>Trains the domain and writes the model archive to the specified stream.</summary>
    /// <param name="domain">The domain containing intents, stories, and rules to train.</param>
    /// <param name="stream">Stream that will receive the model zip archive.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    Task Train(Domain domain, Stream stream, CancellationToken cancellationToken = default);
}
