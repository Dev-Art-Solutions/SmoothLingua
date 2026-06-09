namespace SmoothLingua.Abstractions.Conversations;

public interface IConversation
{
    string Id { get; }

    List<string> HandleIntent(string intentName, string input);

    void Reset();

    /// <summary>Returns a snapshot of this conversation's current mutable state.</summary>
    ConversationState GetState();
}
