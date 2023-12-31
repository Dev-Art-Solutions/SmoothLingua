namespace SmoothLingua.Tests.Stories;

using Abstractions.Stories;
using SmoothLingua.Stories;

public class StoryManagerTests
{
    [Fact]
    public void TryGetResponse_ReturnsTrueAndSetsResponses_WhenIntentExistsInActiveStories()
    {
        // Arrange
        var stories = new List<Story>
        {
            // Set up stories as needed
        };

        var storyManager = new StoryManager(stories);

        // Set up active stories
        var activeStory = new Story("Story", new List<Step>
                {
                    new IntentStep("Greeting"),
                    new ResponseStep("Hello!") });

        // Use reflection to set the internal state for testing
        SetInternalState(storyManager, "activeStories", new List<Story> { activeStory });
        SetInternalState(storyManager, "activeStep", 0);

        // Act
        var result = storyManager.TryGetResponse("Greeting", out var responses);

        // Assert
        Assert.True(result);
        Assert.Equal(new List<string> { "Hello!" }, responses);
        Assert.Equal(2, GetInternalState<int>(storyManager, "activeStep")); // Assert that activeStep is updated
    }

    // Add more tests using reflection as needed

    private static void SetInternalState<T>(object obj, string fieldName, T value)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(obj, value);
        }
    }

    private static T? GetInternalState<T>(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            return (T?)field.GetValue(obj);
        }

        return default;
    }
}
