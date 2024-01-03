namespace SmoothLingua.Tests.Rules;

using SmoothLingua.Abstractions.Rules;
using SmoothLingua.Rules;

public class RuleManagerTests
{
    [Fact]
    public void TryGetResponse_ReturnsTrueAndSetsResponse_WhenIntentExists()
    {
        // Arrange
        var rules = new List<Rule>
            {
                new("Greeting", "GreetingIntent", "Hello!"),
                new("Farewell", "FarewellIntent", "Goodbye!")
            };

        var ruleManager = new RuleManager(rules);

        // Act
        var result = ruleManager.TryGetResponse("GreetingIntent", out var response);

        // Assert
        Assert.True(result);
        Assert.Equal("Hello!", response);
    }

    [Fact]
    public void TryGetResponse_ReturnsFalseAndNullResponse_WhenIntentDoesNotExist()
    {
        // Arrange
        var rules = new List<Rule>
            {
                new("Greeting", "GreetingIntent", "Hello!"),
                new("Farewell", "FarewellIntent", "Goodbye!")
            };

        var ruleManager = new RuleManager(rules);

        // Act
        var result = ruleManager.TryGetResponse("UnknownIntent", out var response);

        // Assert
        Assert.False(result);
        Assert.Null(response);
    }
}
