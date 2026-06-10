namespace SmoothLingua.Tests.Server;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

public class PlaygroundApiTests : IClassFixture<ConversationsApiTests.ApiFixture>
{
    private readonly HttpClient client;

    public PlaygroundApiTests(ConversationsApiTests.ApiFixture fixture) => client = fixture.CreateClient();

    [Fact]
    public async Task Predict_ReturnsIntentAndConfidence_ForUserSuppliedExamples()
    {
        var body = new
        {
            intents = new[]
            {
                new { name = "Greeting", examples = new[] { "Hello", "Hi", "Hey there" } },
                new { name = "Bye",      examples = new[] { "Goodbye", "Bye", "See you" } },
            },
            text = "Hi there"
        };

        var response = await client.PostAsJsonAsync("/playground/predict", body);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Greeting", json.GetProperty("intentName").GetString());
        Assert.InRange(json.GetProperty("confidence").GetSingle(), 0f, 1f);
    }

    [Fact]
    public async Task Predict_TooFewIntents_Returns400()
    {
        var body = new
        {
            intents = new[]
            {
                new { name = "Greeting", examples = new[] { "Hello" } },
            },
            text = "Hi"
        };

        var response = await client.PostAsJsonAsync("/playground/predict", body);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Predict_EmptyText_Returns400()
    {
        var body = new
        {
            intents = new[]
            {
                new { name = "Greeting", examples = new[] { "Hi" } },
                new { name = "Bye",      examples = new[] { "Bye" } },
            },
            text = ""
        };

        var response = await client.PostAsJsonAsync("/playground/predict", body);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
