namespace SmoothLingua.Conversations;

using System.Collections.Concurrent;
using SmoothLingua.Abstractions.Conversations;

/// <summary>
/// Default <see cref="IConversationStore"/> that keeps state in memory.
/// State is lost when the process exits; use <see cref="FileConversationStore"/> for durability.
/// </summary>
public sealed class InMemoryConversationStore : IConversationStore
{
    private readonly ConcurrentDictionary<string, ConversationState> store = new();

    /// <inheritdoc/>
    public ConversationState? Get(string conversationId)
        => store.TryGetValue(conversationId, out var state) ? state : null;

    /// <inheritdoc/>
    public void Save(string conversationId, ConversationState state)
        => store[conversationId] = state;

    /// <inheritdoc/>
    public void Reset(string conversationId)
        => store.TryRemove(conversationId, out _);
}
