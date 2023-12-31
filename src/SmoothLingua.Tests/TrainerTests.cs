namespace SmoothLingua.Tests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Moq;
using Xunit;

using Abstractions.NLU;
using Abstractions.Stories;
using Abstractions;
using Abstractions.Rules;

public class TrainerTests
{
    [Fact]
    public async Task Train_CallsITrainerAndStoresDomain()
    {
        // Arrange
        var domain = new Domain(
            Intents: new List<Intent>
            {
            new Intent("Greeting", new List<string> { "Hello", "Hi" }),
             new Intent("Bye", new List<string> { "Bye", "Goodbye" })
            },
            Rules: new List<Rule>
            {
            new Rule("Rule1", "Greeting", "Response1")
            },
            Stories: new List<Story>
            {
            new Story(
                "Story1",
                new List<Step> { new IntentStep("Bye"), new ResponseStep("Response1") }
            )
            }
        );

        var trainerMock = new Mock<Abstractions.NLU.ITrainer>();
        var stream = new MemoryStream();

        trainerMock.Setup(t => t.Train(It.IsAny<List<Intent>>(), It.IsAny<Stream>()));

        var trainer = new Trainer(trainerMock.Object);

        // Act
        await trainer.Train(domain, stream);

        // Assert
        trainerMock.Verify(t => t.Train(domain.Intents, stream), Times.Once);
    }

    [Fact]
    public async Task Train_ThrowsException_WhenDomainIsNull()
    {
        // Arrange
        var trainer = new Trainer();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => trainer.Train(null, "path"));
    }

    [Fact]
    public async Task Train_ThrowsException_WhenIntentValidationFails()
    {
        // Arrange
        var domain = new Domain(
            Intents: new List<Intent>
            {
            new Intent(null, new List<string> { "Hello" })
            },
            Rules: new List<Rule>(),
            Stories: new List<Story>()
        );

        var trainerMock = new Mock<Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => trainer.Train(domain, "path"));
    }

    [Fact]
    public async Task Train_ThrowsException_WhenRuleValidationFails()
    {
        // Arrange
        var domain = new Domain(
            Intents: new List<Intent>
            {
                new Intent("Greeting", new List<string> { "Hello" })
            },
            Rules: new List<Rule>
            {
                new Rule(null, "Greeting", "Response1")
            },
            Stories: new List<Story>()
        );

        var trainerMock = new Mock<Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path"));
    }

    [Fact]
    public async Task Train_ThrowsException_WhenStoryValidationFails()
    {
        // Arrange
        var domain = new Domain(
            Intents: new List<Intent>
            {
                new Intent("Greeting", new List<string> { "Hello" })
            },
            Rules: new List<Rule>(),
            Stories: new List<Story>
            {
                new Story("Story1", new List<Step> { new IntentStep("Greeting") })
            }
        );

        var trainerMock = new Mock<Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path"));
    }

    [Fact]
    public async Task Train_ThrowsException_WhenRuleHasSameIntentAsAnotherRule()
    {
        // Arrange
        var domain = new Domain(
            Intents: new List<Intent>
            {
                new Intent("Greeting", new List<string> { "Hello" })
            },
            Rules: new List<Rule>
            {
                new Rule("Rule1", "Greeting", "Response1"),
                new Rule("Rule2", "Greeting", "Response2")
            },
            Stories: new List<Story>()
        );

        var trainerMock = new Mock<Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path"));
    }


    [Fact]
    public async Task Train_ThrowsException_WhenIntentIsMissingInStory()
    {
        // Arrange
        var domain = new Domain(
            Intents: new List<Intent>
            {
                new Intent("Greeting", new List<string> { "Hello" })
            },
            Rules: new List<Rule>(),
            Stories: new List<Story>
            {
                new Story("Story1", new List<Step> { new ResponseStep("Response1") })
            }
        );

        var trainerMock = new Mock<Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path"));
    }

    [Fact]
    public async Task Train_ThrowsException_WhenIntentIsMissingInSecondStory()
    {
        // Arrange
        var domain = new Domain(
            Intents: new List<Intent>
            {
                new Intent("Greeting", new List<string> { "Hello" })
            },
            Rules: new List<Rule>(),
            Stories: new List<Story>
            {
                new Story("Story1", new List<Step> { new IntentStep("Greeting"), new ResponseStep("Response1") }),
                new Story("Story2", new List<Step> { new ResponseStep("Response2") })
            }
        );

        var trainerMock = new Mock<Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path"));
    }

    [Fact]
    public async Task Train_ThrowsException_WhenResponsesAfterSameIntentsAreDifferent()
    {
        // Arrange
        var domain = new Domain(
            Intents: new List<Intent>
            {
                new Intent("Greeting", new List<string> { "Hello" })
            },
            Rules: new List<Rule>(),
            Stories: new List<Story>
            {
                new Story("Story1", new List<Step> { new IntentStep("Greeting"), new ResponseStep("Response1") }),
                new Story("Story2", new List<Step> { new IntentStep("Greeting"), new ResponseStep("Response2") })
            }
        );

        var trainerMock = new Mock<Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path"));
    }

    [Fact]
    public async Task Train_ThrowsException_WhenResponsesCountAfterSameIntentsAreDifferent()
    {
        // Arrange
        var domain = new Domain(
            Intents: new List<Intent>
            {
                new Intent("Greeting", new List<string> { "Hello" })
            },
            Rules: new List<Rule>(),
            Stories: new List<Story>
            {
                new Story("Story1", new List<Step> { new IntentStep("Greeting"), new ResponseStep("Response1") }),
                new Story("Story2", new List<Step> { new IntentStep("Greeting"), new ResponseStep("Response2"), new ResponseStep("Response3") })
            }
        );

        var trainerMock = new Mock<Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path"));
    }

    [Fact]
    public async Task Train_ThrowsException_WhenRuleIntentIsMissingInDomain()
    {
        // Arrange
        var domain = new Domain(
            Intents: new List<Intent>
            {
                new Intent("Greeting", new List<string> { "Hello" })
            },
            Rules: new List<Rule>
            {
                new Rule("Rule1", "Farewell", "Response1")
            },
            Stories: new List<Story>()
        );

        var trainerMock = new Mock<Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path"));
        Assert.Contains("Intent Farewell is missing", exception.Message);
    }

    [Fact]
    public async Task Train_ThrowsException_WhenStoryContainsIntentFromRule()
    {
        // Arrange
        var domain = new Domain(
            Intents: new List<Intent>
            {
                new Intent("Greeting", new List<string> { "Hello" })
            },
            Rules: new List<Rule>
            {
                new Rule("Rule1", "Greeting", "Response1")
            },
            Stories: new List<Story>
            {
                new Story("Story1", new List<Step> { new IntentStep("Greeting"), new ResponseStep("Response1") })
            }
        );

        var trainerMock = new Mock<Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path"));
        Assert.Contains("Story can't have intent which is part of the rule", exception.Message);
    }

    [Fact]
    public async Task Train_ThrowsException_WhenTwoConsecutiveStepsAreIntents()
    {
        // Arrange
        var domain = new Domain(
            Intents: new List<Intent>
            {
                new Intent("Greeting", new List<string> { "Hello" })
            },
            Rules: new List<Rule>(),
            Stories: new List<Story>
            {
                new Story("Story1", new List<Step> { new IntentStep("Greeting"), new IntentStep("Farewell") })
            }
        );

        var trainerMock = new Mock<Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path"));
        Assert.Contains("Two consecutive can't be intents.", exception.Message);
    }

    [Fact]
    public async Task Train_ThrowsException_StoryShouldHaveAtLeastTwoSteps()
    {
        // Arrange
        var domain = new Domain(
            Intents: new List<Intent>
            {
                new Intent("Greeting", new List<string> { "Hello" })
            },
            Rules: new List<Rule>(),
            Stories: new List<Story>
            {
                new Story("Story1", new List<Step> { new IntentStep("Greeting") })
            }
        );

        var trainerMock = new Mock<Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path"));
        Assert.Contains("Story should have at least two steps.", exception.Message);
    }

    [Fact]
    public async Task Train_ThrowsException_WhenLastStepOfStoryIsNotResponse()
    {
        // Arrange
        var domain = new Domain(
            Intents: new List<Intent>
            {
                new Intent("Greeting", new List<string> { "Hello" })
            },
            Rules: new List<Rule>(),
            Stories: new List<Story>
            {
                new Story("Story1", new List<Step> { new IntentStep("Greeting"),
                new ResponseStep("Greeting"),
                new IntentStep("Greeting")})
            }
        );

        var trainerMock = new Mock<Abstractions.NLU.ITrainer>();
        var trainer = new Trainer(trainerMock.Object);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => trainer.Train(domain, "path"));
        Assert.Contains("Last step of the story should be response", exception.Message);

        // Additional assertions can be added based on your specific requirements
    }
}
