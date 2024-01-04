namespace SmoothLingua.Rules;

using Abstractions.Rules;

public class RuleManager(List<Rule> rules) : IRuleManager
{
    private readonly List<Rule> rules = rules;

    public bool TryGetResponse(string intentName, out string? response)
    {
        response = default;
        var rule = rules.FirstOrDefault(x=> x.IntentName == intentName);

        if(rule != null)
        {
            response = rule.Response;
        }

        return rule != null;
    }
}
