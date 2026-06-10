namespace SmoothLingua.Server.Playground;

using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using SmoothLingua.Abstractions;
using SmoothLingua.Abstractions.NLU;

/// <summary>
/// Trains and caches small per-request intent classifiers for the demo playground.
/// </summary>
/// <remarks>
/// Training is bounded by <see cref="MaxIntents"/> and <see cref="MaxExamplesPerIntent"/> and the
/// cache is capped by <see cref="MaxCacheEntries"/> so a curious visitor can't exhaust memory or CPU.
/// </remarks>
public sealed class PlaygroundService
{
    public const int MaxIntents = 10;
    public const int MaxExamplesPerIntent = 20;
    public const int MaxTextLength = 500;
    private const int MaxCacheEntries = 64;

    private readonly ConcurrentDictionary<string, CachedPredictor> cache = new();
    private long lastUsedTick;

    /// <summary>
    /// Trains (or reuses a cached classifier) and predicts the intent of <paramref name="text"/>
    /// against the supplied <paramref name="intents"/>. Returns <c>null</c> when the supplied data
    /// is rejected by validation.
    /// </summary>
    public async Task<PlaygroundResult> PredictAsync(IReadOnlyList<Intent> intents, string text, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(intents);
        ArgumentNullException.ThrowIfNull(text);

        if (intents.Count < 2)
            return PlaygroundResult.Failure("At least two intents are required so the model can distinguish them.");
        if (intents.Count > MaxIntents)
            return PlaygroundResult.Failure($"At most {MaxIntents} intents are accepted in the playground.");
        if (text.Length > MaxTextLength)
            return PlaygroundResult.Failure($"Text must be at most {MaxTextLength} characters long.");

        var distinctNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (var intent in intents)
        {
            if (string.IsNullOrWhiteSpace(intent.Name))
                return PlaygroundResult.Failure("Every intent must have a non-empty name.");
            if (!distinctNames.Add(intent.Name))
                return PlaygroundResult.Failure($"Intent name '{intent.Name}' is duplicated.");
            if (intent.Examples is null || intent.Examples.Count == 0)
                return PlaygroundResult.Failure($"Intent '{intent.Name}' needs at least one example.");
            if (intent.Examples.Count > MaxExamplesPerIntent)
                return PlaygroundResult.Failure($"Intent '{intent.Name}' has too many examples — keep it to {MaxExamplesPerIntent}.");
            if (intent.Examples.Any(string.IsNullOrWhiteSpace))
                return PlaygroundResult.Failure($"Intent '{intent.Name}' has an empty example.");
        }

        var key = ComputeKey(intents);
        if (!cache.TryGetValue(key, out var cached))
        {
            var predictor = await BuildPredictorAsync(intents, cancellationToken);
            EvictIfFull();
            cached = new CachedPredictor(predictor, NextTick());
            cache[key] = cached;
        }
        else
        {
            cached.Touch(NextTick());
        }

        (string IntentName, float Confidence) prediction;
        lock (cached.SyncRoot)
        {
            prediction = cached.Predictor.Predict(text);
        }

        return PlaygroundResult.Ok(prediction.IntentName, prediction.Confidence);
    }

    private static async Task<IPredictor> BuildPredictorAsync(IReadOnlyList<Intent> intents, CancellationToken cancellationToken)
    {
        var domain = new Domain([.. intents], new List<Abstractions.Stories.Story>(), new List<Abstractions.Rules.Rule>());

        // Trainer's internal ZipArchive disposes the train stream when finalising the model,
        // so capture the bytes and load the predictor from a fresh stream.
        var trainStream = new MemoryStream();
        await new Trainer().Train(domain, trainStream, cancellationToken);
        var bytes = trainStream.ToArray();

        using var loadStream = new MemoryStream(bytes);
        return new NLU.Predictor(loadStream);
    }

    private long NextTick() => Interlocked.Increment(ref lastUsedTick);

    private void EvictIfFull()
    {
        if (cache.Count < MaxCacheEntries) return;

        var victims = cache
            .OrderBy(kvp => kvp.Value.LastUsedTick)
            .Take(cache.Count - MaxCacheEntries + 1)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in victims) cache.TryRemove(key, out _);
    }

    private static string ComputeKey(IReadOnlyList<Intent> intents)
    {
        var normalised = intents
            .Select(i => new { i.Name, Examples = i.Examples.OrderBy(e => e, StringComparer.Ordinal).ToList() })
            .OrderBy(i => i.Name, StringComparer.Ordinal)
            .ToList();

        var json = JsonConvert.SerializeObject(normalised);
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(bytes);
    }

    private sealed class CachedPredictor
    {
        public CachedPredictor(IPredictor predictor, long tick)
        {
            Predictor = predictor;
            LastUsedTick = tick;
            SyncRoot = new object();
        }

        public IPredictor Predictor { get; }
        public long LastUsedTick { get; private set; }
        public object SyncRoot { get; }

        public void Touch(long tick) => LastUsedTick = tick;
    }
}

/// <summary>Outcome of a playground prediction. Either a successful prediction or a validation error.</summary>
public sealed record PlaygroundResult(bool Success, string? IntentName, float Confidence, string? Error)
{
    public static PlaygroundResult Ok(string intentName, float confidence) => new(true, intentName, confidence, null);
    public static PlaygroundResult Failure(string error) => new(false, null, 0f, error);
}
