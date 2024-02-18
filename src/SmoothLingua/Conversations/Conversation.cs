namespace SmoothLingua.Conversations;

using Abstractions.Conversations;
using Abstractions.Rules;
using Abstractions.Stories;

public class Conversation : IConversation
{
    private IRuleManager ruleManager;
    private IStoryManager storyManager;

    public Conversation(string id, IRuleManager ruleManager, IStoryManager storyManager)
    {
        Id = id;
        this.ruleManager = ruleManager;
        this.storyManager = storyManager;
    }

    public string Id { get; }

    public List<string> HandleIntent(string intentName)
    {
        List<string> result = new List<string>();

        if (ruleManager.TryGetResponse(intentName, out var response))
        {
            storyManager.ClearState();
            result.Add(response ?? string.Empty);

            return result;
        }

        storyManager.TryGetResponse(intentName, out result);

        return result;
    }

    public void Reset()
    {
        storyManager.ClearState();
    }
}
