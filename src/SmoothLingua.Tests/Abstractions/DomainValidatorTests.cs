namespace SmoothLingua.Tests.Abstractions;

using SmoothLingua.Abstractions.NLU;
using SmoothLingua.Abstractions.Stories;
using SmoothLingua.Abstractions;
using SmoothLingua.Abstractions.Rules;

public class DomainValidatorTests
{
    [Fact]
    public void Validate_ThrowsException_WhenDomainIsNull()
    {
        // Arrange
        Domain? domain = null;

        // Act & Assert
#pragma warning disable CS8604 // Possible null reference argument.
        Assert.Throws<ArgumentNullException>(() => DomainValidator.Validate(domain));
#pragma warning restore CS8604 // Possible null reference argument.
    }

    [Fact]
    public void Validate_ThrowsException_WhenIntentsIsNull()
    {
        // Arrange
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        var domain = new Domain(null, [], []);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        // Act & Assert
        Assert.Throws<ArgumentException>(() => DomainValidator.Validate(domain));
    }

    [Fact]
    public void Validate_ThrowsException_WhenIntentIsMissing()
    {
        // Arrange
        var intent = new Intent("TestIntent", ["example1", "example2"]);
        var domain = new Domain([intent], [], [new("Missing","Test2", "Wrong")]);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => DomainValidator.Validate(domain));
    }

    [Fact]
    public void Validate_ThrowsException_WhenResponseAfterSameIntentsIsNotSame()
    {
        // Arrange
        var intent = new Intent("TestIntent", ["example1", "example2"]);
        var responseStep1 = new ResponseStep("Response1");
        var responseStep2 = new ResponseStep("Response2");
        var intentStep = new IntentStep("TestIntent");

        var story1 = new Story("Story1", [intentStep, responseStep1]);
        var story2 = new Story("Story2", [intentStep, responseStep2]);

        var domain = new Domain([intent], [story1, story2], []);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => DomainValidator.Validate(domain));
    }

    [Fact]
    public void Validate_NoExceptionThrown_WhenDomainIsValid()
    {
        // Arrange
        var intent = new Intent("TestIntent", ["example1", "example2"]);
        var responseStep = new ResponseStep("Response1");
        var intentStep = new IntentStep("TestIntent");

        var story = new Story("Story1", [intentStep, responseStep]);

        var domain = new Domain([intent], [story], []);

        // Act & Assert
        DomainValidator.Validate(domain);
    }

    [Fact]
    public void Validate_ThrowsException_WhenIntentIsMissingInRule()
    {
        // Arrange
        var intent = new Intent("TestIntent", ["example1", "example2"]);
        var rule = new Rule("TestRule", "NonExistentIntent", "Response");
        var domain = new Domain([intent], [], [rule]);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => DomainValidator.Validate(domain));
    }

    [Fact]
    public void Validate_ThrowsException_WhenStoryHasSameIntentMultipleTimes()
    {
        // Arrange
        var intent = new Intent("TestIntent", ["example1", "example2"]);
        var intentStep = new IntentStep("TestIntent");

        var story = new Story("Story1", [intentStep, intentStep]);
        var domain = new Domain([intent], [story], []);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => DomainValidator.Validate(domain));
    }

    [Fact]
    public void Validate_NoExceptionThrown_WhenDomainIsValidWithUniqueIntents()
    {
        // Arrange
        var intent1 = new Intent("TestIntent1", ["example1", "example2"]);
        var intent2 = new Intent("TestIntent2", ["example3", "example4"]);

        var story1 = new Story("Story1", [new IntentStep("TestIntent1"), new ResponseStep("Response1")]);
        var story2 = new Story("Story2", [new IntentStep("TestIntent2"), new ResponseStep("Response2")]);

        var domain = new Domain([intent1, intent2], [story1, story2], []);

        // Act & Assert
        DomainValidator.Validate(domain);
    }
}
