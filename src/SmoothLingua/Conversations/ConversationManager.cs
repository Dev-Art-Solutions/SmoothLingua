namespace SmoothLingua.Conversations;

using System.Collections.Concurrent;

using Abstractions.Conversations;
using SmoothLingua.Abstractions;
using SmoothLingua.Abstractions.Rules;
using SmoothLingua.Abstractions.Stories;

public class ConversationManager : IConversationManager
{
    private readonly IRuleManagerFactory ruleManagerFactory;
    private readonly IStoryManagerFactory storyManagerFactory;
    private readonly Domain domain;
    private readonly IConversationStore store;
    private readonly ConcurrentDictionary<string, Conversation> conversations = new();

    public ConversationManager(
        IRuleManagerFactory ruleManagerFactory,
        IStoryManagerFactory storyManagerFactory,
        Domain domain,
        IConversationStore? store = null)
    {
        this.ruleManagerFactory = ruleManagerFactory;
        this.storyManagerFactory = storyManagerFactory;
        this.domain = domain;
        this.store = store ?? new InMemoryConversationStore();
    }

    public IConversation Get(string conversationId)
    {
        if (conversations.TryGetValue(conversationId, out var existing))
            return existing;

        var savedState = this.store.Get(conversationId);
        var conversation = new Conversation(
            conversationId,
            ruleManagerFactory.Create(),
            storyManagerFactory.Create(),
            domain,
            this.store,
            savedState);

        conversations.TryAdd(conversationId, conversation);
        return conversation;
    }
}
