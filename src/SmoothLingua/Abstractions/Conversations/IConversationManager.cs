namespace SmoothLingua.Abstractions.Conversations;

public interface IConversationManager
{
    IConversation Get(string conversationId);
}
