namespace SmoothLingua.Stories;

using Abstractions.Stories;

public class StoryManager : IStoryManager
{
    private readonly List<Story> stories;

    private int activeStep = 0;
    private List<Story> activeStories = new List<Story>();

    public StoryManager(List<Story> stories)
        => this.stories = stories;

    public void ClearState()
    {
        activeStories.Clear();
        activeStep = 0;
    }

    public bool TryGetResponse(string intentName, out List<string> responses)
    {
        if (activeStories.Count > 0 && TryGetResponseFromStories(activeStories, intentName, out responses))
        {
            return true;
        }

        ClearState();

        return TryGetResponseFromStories(stories, intentName, out responses);
    }

    private bool TryGetResponseFromStories(List<Story> currentStories, string intentName, out List<string> responses) 
    {
        responses = new List<string>();
        var currentActiveStories = currentStories.Where(x => x.Steps.Count > activeStep &&
         ((IntentStep)x.Steps[activeStep]).IntentName == intentName).ToList();

        if (currentActiveStories.Count > 0)
        {
            activeStories = currentActiveStories;
            responses = activeStories[0]
                .Steps
                .Skip(activeStep + 1)
                .TakeWhile(x => x is ResponseStep).Select(x => ((ResponseStep)x).Text)
                .ToList();
            activeStep = activeStep + responses.Count + 1;

            return true;
        }

        return false;
    }
}
