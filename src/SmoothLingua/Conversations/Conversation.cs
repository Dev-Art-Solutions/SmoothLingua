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
    private Dictionary<string, string>? slots;

    public Conversation(string id, IRuleManager ruleManager, IStoryManager storyManager, Domain domain)
    {
        Id = id;
        this.ruleManager = ruleManager;
        this.storyManager = storyManager;
        this.domain = domain;
        slots = domain.Slots?.ToDictionary(x => x.Name, x => x.DefaultValue ?? string.Empty);
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

        return result;
    }

    public void Reset()
    {
        storyManager.ClearState();
        slots = domain.Slots?.ToDictionary(x => x.Name, x => x.DefaultValue ?? string.Empty);
    }
}
