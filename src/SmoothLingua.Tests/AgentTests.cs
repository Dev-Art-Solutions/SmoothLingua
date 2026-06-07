namespace SmoothLingua.Tests;

using Moq;

using Xunit;

using SmoothLingua.Abstractions.Conversations;
using SmoothLingua.Abstractions.NLU;

public class AgentTests
{
    [Fact]
    public void Handle_CallsPredictorAndConversationManager()
    {
        // Arrange
        var conversationId = "123";
        var input = "Hello, chatbot!";
        var intentName = "Greeting";
        var predictedMessages = new List<string> { "Hello back!" };

        var predictorMock = new Mock<IPredictor>();
        predictorMock.Setup(p => p.Predict(input)).Returns(intentName);

        var conversationManagerMock = new Mock<IConversationManager>();
        var conversationMock = new Mock<IConversation>();
        conversationMock.Setup(c => c.HandleIntent(intentName, input)).Returns(predictedMessages);
        conversationManagerMock.Setup(cm => cm.Get(conversationId)).Returns(conversationMock.Object);

        var agent = new Agent(predictorMock.Object, conversationManagerMock.Object);

        // Act
        var response = agent.Handle(conversationId, input);

        // Assert
        Assert.Equal(intentName, response.IntentName);
        Assert.Equal(predictedMessages, response.Messages);

        // Verify that methods were called with the correct arguments
        predictorMock.Verify(p => p.Predict(input), Times.Once);
        conversationManagerMock.Verify(cm => cm.Get(conversationId), Times.Once);
        conversationMock.Verify(c => c.HandleIntent(intentName, input), Times.Once);
    }

    [Fact]
    public void Handle_Reset()
    {
        // Arrange
        var conversationId = "123";
        var predictorMock = new Mock<IPredictor>();

        var conversationManagerMock = new Mock<IConversationManager>();
        var conversationMock = new Mock<IConversation>();
        conversationManagerMock.Setup(cm => cm.Get(conversationId)).Returns(conversationMock.Object);

        var agent = new Agent(predictorMock.Object, conversationManagerMock.Object);

        // Act
        agent.Reset(conversationId);

        // Assert
        conversationMock.Verify(c => c.Reset(), Times.Once);
    }
}
