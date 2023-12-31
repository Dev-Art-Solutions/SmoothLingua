namespace SmoothLingua.Rules;

using Abstractions.Rules;

public class RuleManagerFactory : IRuleManagerFactory
{
    private readonly List<Rule> rules;

    public RuleManagerFactory(List<Rule> rules)
       => this.rules = rules;


    public IRuleManager Create()
        => new RuleManager(rules);
}
