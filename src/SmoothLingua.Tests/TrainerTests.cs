namespace SmoothLingua.Tests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Moq;
using Xunit;

using SmoothLingua.Abstractions.NLU;
using SmoothLingua.Abstractions.Stories;
using SmoothLingua.Abstractions;

public class TrainerTests
{
    [Fact]
    public async Task Train_CallsITrainerAndStoresDomain()
    {
        // Arrange
        var domain = new Domain(
            Intents: [new("Greeting", ["Hello", "Hi"]), new("Bye", ["Bye", "Goodbye"])],
            Rules: [new("Rule1", "Greeting", "Response1")],
            Stories: [new("Story1", [new IntentStep("Bye"), new ResponseStep("Response1")])]
        );

        var trainerMock = new Mock<SmoothLingua.Abstractions.NLU.ITrainer>();
        var stream = new MemoryStream();

        trainerMock.Setup(t => t.Train(It.IsAny<List<Intent>>(), It.IsAny<Stream>()));

        var trainer = new Trainer(trainerMock.Object);

        // Act
        await trainer.Train(domain, stream, default);

        // Assert
        trainerMock.Verify(t => t.Train(domain.Intents, stream), Times.Once);
    }

    [Fact]
    public async Task Train_ThrowsException_WhenDomainIsNull()
    {
        // Arrange
        var trainer = new Trainer();

        // Act & Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        await Assert.ThrowsAsync<ArgumentNullException>(() => trainer.Train(default, "path", default));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Fact]
    public async Task Train_ThrowsException_WhenIntentValidationFails()
    {
        // Arrange
        var domain = new Domain(
            Intents:
            [
            new(string.Empty, ["Hello"])
            ],
            Rules: [],
            Stories: []
        );

        var trainerMock = new Mock<SmoothLingua.Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => trainer.Train(domain, "path", default));
    }

    [Fact]
    public async Task Train_ThrowsException_WhenRuleValidationFails()
    {
        // Arrange
        var domain = new Domain(
            Intents:
            [
                new("Greeting", ["Hello"])
            ],
            Rules:
            [
                new(string.Empty, "Greeting", "Response1")
            ],
            Stories: []
        );

        var trainerMock = new Mock<SmoothLingua.Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path", default));
    }

    [Fact]
    public async Task Train_ThrowsException_WhenStoryValidationFails()
    {
        // Arrange
        var domain = new Domain(
            Intents:
            [
                new("Greeting", ["Hello"])
            ],
            Rules: [],
            Stories:
            [
                new("Story1", [new IntentStep("Greeting")])
            ]
        );

        var trainerMock = new Mock<SmoothLingua.Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path", default));
    }

    [Fact]
    public async Task Train_ThrowsException_WhenRuleHasSameIntentAsAnotherRule()
    {
        // Arrange
        var domain = new Domain(
            Intents: [new("Greeting", ["Hello"])],
            Rules: [new("Rule1", "Greeting", "Response1"),
                new("Rule2", "Greeting", "Response2")],
            Stories: []
        );

        var trainerMock = new Mock<SmoothLingua.Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path", default));
    }


    [Fact]
    public async Task Train_ThrowsException_WhenIntentIsMissingInStory()
    {
        // Arrange
        var domain = new Domain(
            Intents:
            [
                new("Greeting", ["Hello"])
            ],
            Rules: [],
            Stories:
            [
                new("Story1", [new ResponseStep("Response1")])
            ]
        );

        var trainerMock = new Mock<SmoothLingua.Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path", default));
    }

    [Fact]
    public async Task Train_ThrowsException_WhenIntentIsMissingInSecondStory()
    {
        // Arrange
        var domain = new Domain(
            Intents:
            [new("Greeting", ["Hello"])],
            Rules: [],
            Stories:[new("Story1", [new IntentStep("Greeting"), new ResponseStep("Response1")]), new("Story2", [new ResponseStep("Response2")])]
        );

        var trainerMock = new Mock<SmoothLingua.Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path", default));
    }

    [Fact]
    public async Task Train_ThrowsException_WhenResponsesAfterSameIntentsAreDifferent()
    {
        // Arrange
        var domain = new Domain(
            Intents:
            [
                new("Greeting", ["Hello"])
            ],
            Rules: [],
            Stories:
            [
                new("Story1", [new IntentStep("Greeting"), new ResponseStep("Response1")]),
                new("Story2", [new IntentStep("Greeting"), new ResponseStep("Response2")])
            ]
        );

        var trainerMock = new Mock<SmoothLingua.Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path", default));
    }

    [Fact]
    public async Task Train_ThrowsException_WhenResponsesCountAfterSameIntentsAreDifferent()
    {
        // Arrange
        var domain = new Domain(
            Intents:
            [
                new("Greeting", ["Hello"])
            ],
            Rules: [],
            Stories:
            [
                new("Story1", [new IntentStep("Greeting"), new ResponseStep("Response1")]),
                new("Story2", [new IntentStep("Greeting"), new ResponseStep("Response2"), new ResponseStep("Response3")])
            ]
        );

        var trainerMock = new Mock<SmoothLingua.Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path", default));
    }

    [Fact]
    public async Task Train_ThrowsException_WhenRuleIntentIsMissingInDomain()
    {
        // Arrange
        var domain = new Domain(
            Intents: [new("Greeting", ["Hello"])],
            Rules: [new("Rule1", "Farewell", "Response1")],
            Stories: []
        );

        var trainerMock = new Mock<SmoothLingua.Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path", default));
        Assert.Contains("Intent Farewell is missing", exception.Message);
    }

    [Fact]
    public async Task Train_ThrowsException_WhenStoryContainsIntentFromRule()
    {
        // Arrange
        var domain = new Domain(
            Intents:
            [
                new("Greeting", ["Hello"])
            ],
            Rules:
            [
                new("Rule1", "Greeting", "Response1")
            ],
            Stories:
            [
                new("Story1", [new IntentStep("Greeting"), new ResponseStep("Response1")])
            ]
        );

        var trainerMock = new Mock<SmoothLingua.Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path", default));
        Assert.Contains("Story can't have intent which is part of the rule", exception.Message);
    }

    [Fact]
    public async Task Train_ThrowsException_WhenTwoConsecutiveStepsAreIntents()
    {
        // Arrange
        var domain = new Domain(
            Intents: [new("Greeting", ["Hello"])],
            Rules: [],
            Stories: [new("Story1", [new IntentStep("Greeting"), new IntentStep("Farewell")])]
        );

        var trainerMock = new Mock<SmoothLingua.Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path", default));
        Assert.Contains("Two consecutive can't be intents.", exception.Message);
    }

    [Fact]
    public async Task Train_ThrowsException_StoryShouldHaveAtLeastTwoSteps()
    {
        // Arrange
        var domain = new Domain(
            Intents:
            [
                new("Greeting", ["Hello"])
            ],
            Rules: [],
            Stories:
            [
                new("Story1", [new IntentStep("Greeting")])
            ]
        );

        var trainerMock = new Mock<SmoothLingua.Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path", default));
        Assert.Contains("Story should have at least two steps.", exception.Message);
    }

    [Fact]
    public async Task Train_ThrowsException_WhenLastStepOfStoryIsNotResponse()
    {
        // Arrange
        var domain = new Domain(
            Intents:
            [
                new("Greeting", ["Hello"])
            ],
            Rules: [],
            Stories:
            [
                new("Story1", [ new IntentStep("Greeting"),
                new ResponseStep("Greeting"),
                new IntentStep("Greeting")])
            ]
        );

        var trainerMock = new Mock<SmoothLingua.Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path", default));
        Assert.Contains("Last step of the story should be response", exception.Message);
    }
}
