namespace SmoothLingua.Conversations;

using System.Collections.Concurrent;

using Abstractions.Conversations;
using SmoothLingua.Abstractions.Rules;
using SmoothLingua.Abstractions.Stories;

public class ConversationManager : IConversationManager
{
    private readonly IRuleManagerFactory ruleManagerFactory;
    private readonly IStoryManagerFactory storyManagerFactory;
    private readonly ConcurrentDictionary<string, Conversation> conversations = new ConcurrentDictionary<string, Conversation>();

    public ConversationManager(IRuleManagerFactory ruleManagerFactory, IStoryManagerFactory storyManagerFactory)
    {
        this.ruleManagerFactory = ruleManagerFactory;
        this.storyManagerFactory = storyManagerFactory;
    }

    public IConversation Get(string conversationId)
        => conversations.GetOrAdd(conversationId,
            (x) => new Conversation(conversationId,
                ruleManagerFactory.Create(), 
                storyManagerFactory.Create()));
}
