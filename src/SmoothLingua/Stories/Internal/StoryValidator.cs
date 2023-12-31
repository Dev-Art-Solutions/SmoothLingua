namespace SmoothLingua.Stories.Internal;

using Abstractions.Stories;

internal class StoryValidator
{
    public static void Validate(Story story)
    {
        if (story == null)
        {
            throw new ArgumentNullException(nameof(story));
        }

        if (string.IsNullOrEmpty(story.Name))
        {
            throw new ArgumentException($"Story name can't null or empty.");
        }

        if (story.Steps.Count < 2)
        {
            throw new ArgumentException($"Story should have at least two steps.");
        }

        if (story.Steps[0] is not IntentStep)
        {
            throw new ArgumentException($"First step of the story should be intent.");
        }

        bool isIntent = false;

        for(int i = 0; i < story.Steps.Count; i++)
        {
            if(isIntent && story.Steps[i] is IntentStep)
            {
                throw new ArgumentException($"Two consecutive can't be intents.");
            }

            if (story.Steps[i] is IntentStep)
            {
                isIntent = true;
            }
            else
            {
                isIntent = false;
            }
        }

        if (story.Steps[story.Steps.Count - 1] is not ResponseStep)
        {
            throw new ArgumentException($"Last step of the story should be response.");
        }
    }
}
