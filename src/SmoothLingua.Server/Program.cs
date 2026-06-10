using SmoothLingua.Abstractions;
using SmoothLingua.Abstractions.Analytics;
using SmoothLingua.Abstractions.NLU;
using SmoothLingua.Server.Extensions;
using SmoothLingua.Server.Playground;

const string CorsPolicy = "SmoothLinguaDemo";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSmoothLingua(builder.Configuration);
builder.Services.AddSingleton<PlaygroundService>();

var allowedOrigins = builder.Configuration.GetSection("SmoothLingua:Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "https://smooth-lingua.com", "https://chat.smooth-lingua.com" };

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .WithMethods("GET", "POST"));
});

var app = builder.Build();

app.UseCors(CorsPolicy);

// ── Health ────────────────────────────────────────────────────────────────────

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

// ── Conversations ─────────────────────────────────────────────────────────────

app.MapPost("/conversations/{id}/messages", (string id, MessageRequest request, IAgent agent) =>
{
    if (string.IsNullOrWhiteSpace(request.Text))
        return Results.BadRequest(new { error = "text is required" });

    var response = agent.Handle(id, request.Text);
    return Results.Ok(response);
});

app.MapPost("/conversations/{id}/reset", (string id, IAgent agent) =>
{
    agent.Reset(id);
    return Results.NoContent();
});

// ── Insights ──────────────────────────────────────────────────────────────────

app.MapGet("/insights", (IAnalyticsRecorder analytics) => Results.Ok(analytics.GetSnapshot()));

// ── Playground ────────────────────────────────────────────────────────────────

app.MapPost("/playground/predict", async (PlaygroundRequest request, PlaygroundService playground, CancellationToken ct) =>
{
    if (request.Intents is null || request.Intents.Count == 0)
        return Results.BadRequest(new { error = "intents is required" });
    if (string.IsNullOrWhiteSpace(request.Text))
        return Results.BadRequest(new { error = "text is required" });

    var intents = request.Intents
        .Select(i => new Intent(i.Name ?? string.Empty, i.Examples ?? new List<string>()))
        .ToList();

    var result = await playground.PredictAsync(intents, request.Text, ct);
    if (!result.Success)
        return Results.BadRequest(new { error = result.Error });

    return Results.Ok(new { intentName = result.IntentName, confidence = result.Confidence });
});

app.Run();

// Required for WebApplicationFactory in integration tests
public partial class Program { }

// ── Request models ─────────────────────────────────────────────────────────────

public record MessageRequest(string Text);
public record PlaygroundRequest(List<PlaygroundIntentDto>? Intents, string Text);
public record PlaygroundIntentDto(string? Name, List<string>? Examples);
