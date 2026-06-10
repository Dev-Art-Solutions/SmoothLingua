namespace SmoothLingua.Tests;

using Moq;

using Xunit;

using SmoothLingua.Abstractions;
using SmoothLingua.Abstractions.Analytics;
using SmoothLingua.Abstractions.Conversations;
using SmoothLingua.Abstractions.NLU;
using SmoothLingua.Abstractions.Stories;
using SmoothLingua.Analytics;

public class AgentTests
{
    private static Domain MinimalDomain() => new(
        Intents: [new("Greeting", ["Hello"]), new("Bye", ["Goodbye"])],
        Stories: [new("Story1", [new IntentStep("Greeting"), new ResponseStep("Hi!")])],
        Rules: []
    );

    [Fact]
    public void Handle_CallsPredictorAndConversationManager()
    {
        // Arrange
        var conversationId = "123";
        var input = "Hello, chatbot!";
        var intentName = "Greeting";
        var confidence = 0.9f;
        var predictedMessages = new List<string> { "Hello back!" };

        var predictorMock = new Mock<IPredictor>();
        predictorMock.Setup(p => p.Predict(input)).Returns((intentName, confidence));

        var conversationManagerMock = new Mock<IConversationManager>();
        var conversationMock = new Mock<IConversation>();
        conversationMock.Setup(c => c.HandleIntent(intentName, input)).Returns(predictedMessages);
        conversationManagerMock.Setup(cm => cm.Get(conversationId)).Returns(conversationMock.Object);

        var agent = new Agent(predictorMock.Object, conversationManagerMock.Object, MinimalDomain());

        // Act
        var response = agent.Handle(conversationId, input);

        // Assert
        Assert.Equal(intentName, response.IntentName);
        Assert.Equal(predictedMessages, response.Messages);
        Assert.Equal(confidence, response.Confidence);

        predictorMock.Verify(p => p.Predict(input), Times.Once);
        conversationManagerMock.Verify(cm => cm.Get(conversationId), Times.Once);
        conversationMock.Verify(c => c.HandleIntent(intentName, input), Times.Once);
    }

    [Fact]
    public void Handle_LowConfidence_UsesFallbackIntent()
    {
        // Arrange
        var conversationId = "123";
        var input = "asdfghjkl";
        var predictedIntent = "Greeting";
        var lowConfidence = 0.2f;
        var fallbackIntent = "nlu_fallback";
        var fallbackMessages = new List<string> { "I did not understand that." };

        var domain = MinimalDomain();

        var predictorMock = new Mock<IPredictor>();
        predictorMock.Setup(p => p.Predict(input)).Returns((predictedIntent, lowConfidence));

        var conversationManagerMock = new Mock<IConversationManager>();
        var conversationMock = new Mock<IConversation>();
        conversationMock.Setup(c => c.HandleIntent(fallbackIntent, input)).Returns(fallbackMessages);
        conversationManagerMock.Setup(cm => cm.Get(conversationId)).Returns(conversationMock.Object);

        var agent = new Agent(predictorMock.Object, conversationManagerMock.Object, domain);

        // Act
        var response = agent.Handle(conversationId, input);

        // Assert
        Assert.Equal(fallbackIntent, response.IntentName);
        Assert.Equal(lowConfidence, response.Confidence);
        conversationMock.Verify(c => c.HandleIntent(fallbackIntent, input), Times.Once);
        conversationMock.Verify(c => c.HandleIntent(predictedIntent, input), Times.Never);
    }

    [Fact]
    public void Handle_ExtractsEntities_WhenDomainHasEntities()
    {
        // Arrange
        var conversationId = "123";
        var input = "I want pizza";
        var intentName = "order_food";
        var confidence = 0.95f;

        var domain = new Domain(
            Intents: [new("order_food", ["I want pizza"]), new("cancel", ["cancel"])],
            Stories: [],
            Rules: [],
            Entities: [new Entity("food", ["pizza", "pasta", "salad"])]
        );

        var predictorMock = new Mock<IPredictor>();
        predictorMock.Setup(p => p.Predict(input)).Returns((intentName, confidence));

        var conversationManagerMock = new Mock<IConversationManager>();
        var conversationMock = new Mock<IConversation>();
        conversationMock.Setup(c => c.HandleIntent(intentName, input)).Returns([]);
        conversationManagerMock.Setup(cm => cm.Get(conversationId)).Returns(conversationMock.Object);

        var agent = new Agent(predictorMock.Object, conversationManagerMock.Object, domain);

        // Act
        var response = agent.Handle(conversationId, input);

        // Assert
        Assert.NotNull(response.ExtractedEntities);
        Assert.Equal("pizza", response.ExtractedEntities["food"]);
    }

    [Fact]
    public void Handle_NoEntities_WhenDomainHasNoEntityDefinitions()
    {
        // Arrange
        var input = "Hello";
        var intentName = "Greeting";

        var predictorMock = new Mock<IPredictor>();
        predictorMock.Setup(p => p.Predict(input)).Returns((intentName, 0.9f));

        var conversationManagerMock = new Mock<IConversationManager>();
        var conversationMock = new Mock<IConversation>();
        conversationMock.Setup(c => c.HandleIntent(intentName, input)).Returns([]);
        conversationManagerMock.Setup(cm => cm.Get("1")).Returns(conversationMock.Object);

        var agent = new Agent(predictorMock.Object, conversationManagerMock.Object, MinimalDomain());

        // Act
        var response = agent.Handle("1", input);

        // Assert
        Assert.Null(response.ExtractedEntities);
    }

    [Fact]
    public void Handle_RecordsAnalyticsEvent_WithEffectiveIntentAndFallbackFlag()
    {
        var predictorMock = new Mock<IPredictor>();
        predictorMock.Setup(p => p.Predict("hello")).Returns(("Greeting", 0.9f));
        predictorMock.Setup(p => p.Predict("???")).Returns(("Greeting", 0.1f));

        var conversationManagerMock = new Mock<IConversationManager>();
        var conversationMock = new Mock<IConversation>();
        conversationMock.Setup(c => c.HandleIntent(It.IsAny<string>(), It.IsAny<string>())).Returns([]);
        conversationManagerMock.Setup(cm => cm.Get(It.IsAny<string>())).Returns(conversationMock.Object);

        var recorder = new InMemoryAnalyticsRecorder();
        var agent = new Agent(predictorMock.Object, conversationManagerMock.Object, MinimalDomain(), recorder);

        agent.Handle("conv-1", "hello");
        agent.Handle("conv-1", "???");

        var snapshot = recorder.GetSnapshot();
        Assert.Equal(2, snapshot.TotalMessages);
        Assert.Equal(1, snapshot.TotalConversations);
        Assert.Equal(1, snapshot.FallbackHits);
        Assert.Contains(snapshot.Intents, i => i.IntentName == "Greeting" && i.Count == 1);
        Assert.Contains(snapshot.Intents, i => i.IntentName == "nlu_fallback" && i.Count == 1);
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

        var agent = new Agent(predictorMock.Object, conversationManagerMock.Object, MinimalDomain());

        // Act
        agent.Reset(conversationId);

        // Assert
        conversationMock.Verify(c => c.Reset(), Times.Once);
    }
}
