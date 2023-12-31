namespace SmoothLingua.Rules.Internal;

using Abstractions.Rules;

internal class RuleValidator
{
    public static void Validate(Rule rule)
    {
        if (rule == null)
        {
            throw new ArgumentNullException(nameof(rule));
        }

        if(string.IsNullOrEmpty(rule.Name))
        {
            throw new ArgumentException($"Rule name can't be null or empty.");
        }

        if (string.IsNullOrEmpty(rule.IntentName))
        {
            throw new ArgumentException($"Rule intent can't be null or empty.");
        }

        if (string.IsNullOrEmpty(rule.Response))
        {
            throw new ArgumentException($"Rule response can't be null or empty.");
        }
    }
}
