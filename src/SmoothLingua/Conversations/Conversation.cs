namespace SmoothLingua.Conversations;

using Abstractions.Conversations;
using Abstractions.Rules;
using Abstractions.Stories;
using SmoothLingua.Abstractions;

public class Conversation : IConversation
{
    private readonly char[] symbols = { '!', '?', '.', ',', ':', ';', ' ' };

    private readonly IRuleManager ruleManager;
    private readonly IStoryManager storyManager;
    private readonly Domain domain;
    private readonly IConversationStore store;
    private Dictionary<string, string>? slots;

    public Conversation(
        string id,
        IRuleManager ruleManager,
        IStoryManager storyManager,
        Domain domain,
        IConversationStore store,
        ConversationState? initialState = null)
    {
        Id = id;
        this.ruleManager = ruleManager;
        this.storyManager = storyManager;
        this.domain = domain;
        this.store = store;

        if (initialState != null)
        {
            slots = initialState.Slots != null
                ? new Dictionary<string, string>(initialState.Slots)
                : null;
            storyManager.LoadState(initialState.ActiveStep, initialState.ActiveStoryNames);
        }
        else
        {
            slots = domain.Slots?.ToDictionary(x => x.Name, x => x.DefaultValue ?? string.Empty);
        }
    }

    public string Id { get; }

    public List<string> HandleIntent(string intentName, string input)
    {
        var slot = domain
            .Slots?
            .FirstOrDefault(x => x.MappedFromIntent == intentName);

        if (slot != null)
        {
            if (slot.Entity != null)
            {
                var words = input.Split(symbols);

                foreach (var word in words)
                {
                    var entity = domain.Entities!.First(x => x.Name == slot.Entity);

                    if (entity.Examples.Contains(word))
                    {
                        slots![slot.Name] = word;
                    }
                }
            }
            else
            {
                slots![slot.Name] = intentName;
            }
        }

        List<string> result = new List<string>();

        if (ruleManager.TryGetResponse(intentName, out var response))
        {
            storyManager.ClearState();
            result.Add(response ?? string.Empty);
            store.Save(Id, GetState());
            return result;
        }

        storyManager.TryGetResponse(intentName, out result);

        if (slots != null)
        {
            for (int i = 0; i < result.Count; i++)
            {
                foreach (var s in slots)
                {
                    result[i] = result[i].Replace("{" + s.Key + "}", s.Value);
                }
            }
        }

        store.Save(Id, GetState());
        return result;
    }

    public void Reset()
    {
        storyManager.ClearState();
        slots = domain.Slots?.ToDictionary(x => x.Name, x => x.DefaultValue ?? string.Empty);
        store.Reset(Id);
    }

    public ConversationState GetState()
    {
        var (activeStep, activeStoryNames) = storyManager.GetState();
        return new ConversationState(slots != null ? new Dictionary<string, string>(slots) : null, activeStep, activeStoryNames);
    }
}
