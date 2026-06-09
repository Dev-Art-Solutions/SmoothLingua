namespace SmoothLingua.Conversations;

using System.Text.Json;
using SmoothLingua.Abstractions.Conversations;

/// <summary>
/// <see cref="IConversationStore"/> that persists each conversation as a JSON file inside <paramref name="directory"/>.
/// State survives process restarts. Thread-safe via a single lock.
/// </summary>
public sealed class FileConversationStore : IConversationStore
{
    private readonly string directory;
    private readonly object fileLock = new();

    /// <param name="directory">Directory where conversation JSON files are written. Created on first use if absent.</param>
    public FileConversationStore(string directory)
    {
        this.directory = directory;
        Directory.CreateDirectory(directory);
    }

    /// <inheritdoc/>
    public ConversationState? Get(string conversationId)
    {
        var path = FilePath(conversationId);
        lock (fileLock)
        {
            if (!File.Exists(path))
                return null;

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<ConversationState>(json);
        }
    }

    /// <inheritdoc/>
    public void Save(string conversationId, ConversationState state)
    {
        var path = FilePath(conversationId);
        lock (fileLock)
        {
            File.WriteAllText(path, JsonSerializer.Serialize(state));
        }
    }

    /// <inheritdoc/>
    public void Reset(string conversationId)
    {
        var path = FilePath(conversationId);
        lock (fileLock)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }

    private string FilePath(string conversationId)
        => Path.Combine(directory, $"{conversationId}.json");
}
