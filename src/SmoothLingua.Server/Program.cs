using SmoothLingua.Abstractions;
using SmoothLingua.Server.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSmoothLingua(builder.Configuration);

var app = builder.Build();

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

app.Run();

// Required for WebApplicationFactory in integration tests
public partial class Program { }

// ── Request models ─────────────────────────────────────────────────────────────

public record MessageRequest(string Text);
