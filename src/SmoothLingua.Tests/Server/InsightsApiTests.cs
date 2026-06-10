namespace SmoothLingua.Tests.Server;

using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

public class InsightsApiTests : IClassFixture<ConversationsApiTests.ApiFixture>
{
    private readonly HttpClient client;

    public InsightsApiTests(ConversationsApiTests.ApiFixture fixture) => client = fixture.CreateClient();

    [Fact]
    public async Task GetInsights_AfterSomeTurns_ReflectsAggregatedActivity()
    {
        var convA = Guid.NewGuid().ToString();
        var convB = Guid.NewGuid().ToString();

        await client.PostAsJsonAsync($"/conversations/{convA}/messages", new { text = "Hello" });
        await client.PostAsJsonAsync($"/conversations/{convA}/messages", new { text = "Bye" });
        await client.PostAsJsonAsync($"/conversations/{convB}/messages", new { text = "Hi" });

        var response = await client.GetAsync("/insights");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var snapshot = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(snapshot.GetProperty("totalMessages").GetInt32() >= 3);
        Assert.True(snapshot.GetProperty("totalConversations").GetInt32() >= 2);
        Assert.True(snapshot.GetProperty("intents").GetArrayLength() >= 1);
        Assert.True(snapshot.GetProperty("averageConfidence").GetSingle() >= 0f);
    }

    [Fact]
    public async Task GetInsights_NeverExposesConversationIds()
    {
        var conv = Guid.NewGuid().ToString();
        await client.PostAsJsonAsync($"/conversations/{conv}/messages", new { text = "Hello" });

        var raw = await client.GetStringAsync("/insights");

        Assert.DoesNotContain(conv, raw);
    }
}
