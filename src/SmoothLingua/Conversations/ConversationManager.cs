namespace SmoothLingua.Conversations;

using System.Collections.Concurrent;

using Abstractions.Conversations;
using SmoothLingua.Abstractions;
using SmoothLingua.Abstractions.Rules;
using SmoothLingua.Abstractions.Stories;

public class ConversationManager(IRuleManagerFactory ruleManagerFactory, IStoryManagerFactory storyManagerFactory, Domain domain) : IConversationManager
{
    private readonly IRuleManagerFactory ruleManagerFactory = ruleManagerFactory;
    private readonly IStoryManagerFactory storyManagerFactory = storyManagerFactory;
    private readonly Domain domain = domain;
    private readonly ConcurrentDictionary<string, Conversation> conversations = new();

    public IConversation Get(string conversationId)
        => conversations.GetOrAdd(conversationId,
            (x) => new Conversation(conversationId,
                ruleManagerFactory.Create(),
                storyManagerFactory.Create(),
                domain));
}
