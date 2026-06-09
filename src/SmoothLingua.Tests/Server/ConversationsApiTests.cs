namespace SmoothLingua.Tests.Server;

using System.IO;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using SmoothLingua.Abstractions;
using SmoothLingua.Abstractions.Stories;

public class ConversationsApiTests : IClassFixture<ConversationsApiTests.ApiFixture>
{
    private readonly HttpClient client;

    public ConversationsApiTests(ApiFixture fixture) => client = fixture.CreateClient();

    [Fact]
    public async Task Health_Returns200()
    {
        var response = await client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostMessage_Returns200WithIntentAndMessages()
    {
        var convId = Guid.NewGuid().ToString();
        var response = await client.PostAsJsonAsync($"/conversations/{convId}/messages", new { text = "Hello" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.TryGetProperty("intentName", out _), "Response should contain intentName");
        Assert.True(json.TryGetProperty("messages", out _), "Response should contain messages");
        Assert.True(json.TryGetProperty("confidence", out _), "Response should contain confidence");
    }

    [Fact]
    public async Task PostMessage_EmptyText_Returns400()
    {
        var convId = Guid.NewGuid().ToString();
        var response = await client.PostAsJsonAsync($"/conversations/{convId}/messages", new { text = "" });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostReset_Returns204()
    {
        var convId = Guid.NewGuid().ToString();
        await client.PostAsJsonAsync($"/conversations/{convId}/messages", new { text = "Hello" });

        var resetResponse = await client.PostAsync($"/conversations/{convId}/reset", null);
        Assert.Equal(HttpStatusCode.NoContent, resetResponse.StatusCode);
    }

    // ── Test fixture: builds a model in memory and wires a real Agent ─────────

    public class ApiFixture : WebApplicationFactory<Program>
    {
        private static readonly byte[] ModelBytes = BuildModel();

        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Replace the agent registered via AddSmoothLingua with one backed by our in-memory model.
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAgent));
                if (descriptor != null) services.Remove(descriptor);

                services.AddSingleton<IAgent>(_ =>
                    AgentLoader.Load(new MemoryStream(ModelBytes)).GetAwaiter().GetResult());
            });
        }

        private static byte[] BuildModel()
        {
            var domain = new Domain(
                [new("Greeting", ["Hello", "Hi"]), new("Bye", ["Goodbye", "Bye"])],
                [new("Greet", [new IntentStep("Greeting"), new ResponseStep("Hello!")])],
                [new("Bye", "Bye", "Goodbye")]
            );

            var ms = new MemoryStream();
            new Trainer().Train(domain, ms, default).GetAwaiter().GetResult();
            return ms.ToArray();
        }
    }
}
