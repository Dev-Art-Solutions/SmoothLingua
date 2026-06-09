namespace SmoothLingua.Abstractions.Conversations;

/// <summary>
/// Persistence layer for conversation state.
/// Implementations may store state in memory, on disk, or in a remote database.
/// </summary>
public interface IConversationStore
{
    /// <summary>Returns the persisted state for the given conversation, or <c>null</c> if none exists.</summary>
    ConversationState? Get(string conversationId);

    /// <summary>Persists the current state of the given conversation.</summary>
    void Save(string conversationId, ConversationState state);

    /// <summary>Removes the persisted state for the given conversation (equivalent to a hard reset).</summary>
    void Reset(string conversationId);
}
