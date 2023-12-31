using SmoothLingua.Abstractions.Stories;

namespace SmoothLingua.Stories;

public class StoryManagerFactory : IStoryManagerFactory
{
    private readonly List<Story> stories;

    public StoryManagerFactory(List<Story> stories)
        => this.stories = stories;

    public IStoryManager Create()
        => new StoryManager(this.stories);
}
