namespace SmoothLingua.Conversations;

using System.Collections.Concurrent;

using Abstractions.Conversations;
using SmoothLingua.Abstractions.Rules;
using SmoothLingua.Abstractions.Stories;

public class ConversationManager(IRuleManagerFactory ruleManagerFactory, IStoryManagerFactory storyManagerFactory) : IConversationManager
{
    private readonly IRuleManagerFactory ruleManagerFactory = ruleManagerFactory;
    private readonly IStoryManagerFactory storyManagerFactory = storyManagerFactory;
    private readonly ConcurrentDictionary<string, Conversation> conversations = new();

    public IConversation Get(string conversationId)
        => conversations.GetOrAdd(conversationId,
            (x) => new Conversation(conversationId,
                ruleManagerFactory.Create(), 
                storyManagerFactory.Create()));
}
