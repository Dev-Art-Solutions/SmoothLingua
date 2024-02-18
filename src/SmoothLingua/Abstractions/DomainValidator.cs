namespace SmoothLingua.Abstractions;

using Abstractions.Stories;
using SmoothLingua.NLU.Internal;
using SmoothLingua.Rules.Internal;
using SmoothLingua.Stories.Internal;

public class DomainValidator
{
    public static void Validate(Domain domain)
    {
        ArgumentNullException.ThrowIfNull(domain);

        if(domain.Intents == null)
        {
            throw new ArgumentException($"{nameof(domain.Intents)} is null", nameof(domain));
        }

        if(domain.Intents.Count == 0)
        {
            throw new ArgumentException($"You can't have domain with 0 intents", nameof(domain));
        }

        if (domain.Stories == null)
        {
            throw new ArgumentException($"{nameof(domain.Stories)} is null", nameof(domain));
        }

        if (domain.Rules == null)
        {
            throw new ArgumentException($"{nameof(domain.Rules)} is null", nameof(domain));
        }

        foreach (var rule in domain.Rules)
        {
            RuleValidator.Validate(rule);
        }

        foreach (var story in domain.Stories)
        {
            StoryValidator.Validate(story);
        }

        foreach (var intent in domain.Intents)
        {
            IntentValidator.Validate(intent);
        }

        foreach (var rule in domain.Rules)
        {
            if (domain.Rules.Any(x => x.Name != rule.Name &&
                x.IntentName == rule.IntentName))
            {
                var currentRule = domain.Rules.First(x => x.Name != rule.Name &&
                x.IntentName == rule.IntentName);

                throw new ArgumentException($"Two rules can't have same intent. Rules {rule.Name} and {currentRule.Name} have same intent.");
            }

            if (!domain.Intents.Any(x => x.Name == rule.IntentName))
            {
                throw new ArgumentException($"Intent {rule.IntentName} is missing!");
            }

            if (domain.Stories.Any(x => x.Steps.Any(s => s is IntentStep si && si.IntentName == rule.IntentName)))
            {
                throw new ArgumentException($"Story can't have intent which is part of the rule.");
            }
        }

        if (domain.Stories.Count > 1)
        {
            for (var i = 0; i < domain.Stories.Count - 1; i++)
            {
                var story = domain.Stories[i];

                if (story.Steps.Any(s => s is IntentStep si && !domain.Intents.Any(x => x.Name == si.IntentName)))
                {
                    throw new ArgumentException($"Intent is missing!");
                }

                for (var j = i + 1; j < domain.Stories.Count; j++)
                {
                    var secondStory = domain.Stories[j];

                    if (secondStory.Steps.Any(s => s is IntentStep si && !domain.Intents.Any(x => x.Name == si.IntentName)))
                    {
                        throw new ArgumentException($"Intent is missing!");
                    }

                    var minStepCount = story.Steps.Count < secondStory.Steps.Count ?
                        story.Steps.Count :
                        secondStory.Steps.Count;

                    for (var s = 0; s < minStepCount; s++)
                    {
                        var step = story.Steps[s];
                        var secondStep = secondStory.Steps[s];

                        if (step is ResponseStep)
                        {
                            continue;
                        }

                        if (step is IntentStep si && secondStep is IntentStep sii)
                        {
                            if (si.IntentName == sii.IntentName)
                            {
                                var responseSteps = story
                                    .Steps
                                    .Skip(s + 1)
                                    .TakeWhile(x => x is ResponseStep)
                                    .Select(x => x as ResponseStep)
                                    .ToList();

                                var secondResponseSteps = secondStory
                                    .Steps
                                    .Skip(s + 1)
                                    .TakeWhile(x => x is ResponseStep)
                                    .Select(x => x as ResponseStep!)
                                    .ToList();

                                if (responseSteps.Count != secondResponseSteps.Count)
                                {
                                    throw new ArgumentException($"The response after same intents should be same for story {story.Name} and {secondStory.Name}");
                                }

                                for (int r = 0; r < responseSteps.Count; r++)
                                {
                                    var reponseStep = responseSteps[r];
                                    var secondResponseStep = secondResponseSteps[r];

                                    if (reponseStep!.Text != secondResponseStep!.Text)
                                    {
                                        throw new ArgumentException($"The response after same intents should be same for story {story.Name} and {secondStory.Name}");
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
