namespace SmoothLingua.Tests.Conversations;

using Xunit;
using Moq;

using SmoothLingua.Conversations;
using SmoothLingua.Abstractions;
using SmoothLingua.Abstractions.Rules;
using SmoothLingua.Abstractions.Stories;

public class ConversationManagerTests
{
    [Fact]
    public void Get_CreatesNewConversationWhenNotExists()
    {
        // Arrange
        var conversationId = "456";
        var ruleManagerFactoryMock = new Mock<IRuleManagerFactory>();
        var storyManagerFactoryMock = new Mock<IStoryManagerFactory>();
        var domain = new Domain([], [], []);

        var conversationManager = new ConversationManager(ruleManagerFactoryMock.Object, storyManagerFactoryMock.Object, domain);

        // Act
        var result = conversationManager.Get(conversationId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Conversation>(result);
        Assert.Equal(conversationId, result.Id);
    }
}
