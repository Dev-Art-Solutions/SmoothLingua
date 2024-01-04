using SmoothLingua.Abstractions.Stories;

namespace SmoothLingua.Stories;

public class StoryManagerFactory(List<Story> stories) : IStoryManagerFactory
{
    private readonly List<Story> stories = stories;

    public IStoryManager Create()
        => new StoryManager(this.stories);
}
