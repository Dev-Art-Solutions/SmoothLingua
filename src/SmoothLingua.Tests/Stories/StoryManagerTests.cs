namespace SmoothLingua.Tests.Stories;

using SmoothLingua.Abstractions.Stories;
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
        var activeStory = new Story("Story",
                [
                    new IntentStep("Greeting"),
                    new ResponseStep("Hello!") ]);

        // Use reflection to set the internal state for testing
        SetInternalState(storyManager, "activeStories", new List<Story> { activeStory });
        SetInternalState(storyManager, "activeStep", 0);

        // Act
        var result = storyManager.TryGetResponse("Greeting", out var responses);

        // Assert
        Assert.True(result);
        Assert.Equal(["Hello!"], responses);
        Assert.Equal(2, GetInternalState<int>(storyManager, "activeStep")); // Assert that activeStep is updated
    }

    private static void SetInternalState<T>(object obj, string fieldName, T value)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(obj, value);
    }

    private static T? GetInternalState<T>(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field != null ? (T?)field.GetValue(obj) : default;
    }
}
