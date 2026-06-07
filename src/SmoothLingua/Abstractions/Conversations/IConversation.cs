namespace SmoothLingua.Abstractions.Conversations;

public interface IConversation
{
    string Id { get; }

    List<string> HandleIntent(string intentName, string input);

    void Reset();
}
