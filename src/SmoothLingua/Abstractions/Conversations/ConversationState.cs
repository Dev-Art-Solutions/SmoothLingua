namespace SmoothLingua.Abstractions.Conversations;

/// <summary>Snapshot of a conversation's mutable state — slots and story progress — suitable for serialisation.</summary>
public record ConversationState(
    Dictionary<string, string>? Slots,
    int ActiveStep,
    List<string> ActiveStoryNames);
