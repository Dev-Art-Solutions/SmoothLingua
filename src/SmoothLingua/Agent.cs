namespace SmoothLingua;

using Abstractions;
using Abstractions.Analytics;
using Abstractions.Conversations;
using Abstractions.NLU;
using SmoothLingua.Analytics;

/// <summary>
/// Default implementation of <see cref="IAgent"/>. Predicts the user's intent, delegates to the
/// conversation manager for story/rule resolution, and returns the bot's reply.
/// Obtain an instance via <see cref="AgentLoader.Load(string, CancellationToken)"/>.
/// </summary>
public class Agent : IAgent
{
    private static readonly char[] EntitySeparators = ['!', '?', '.', ',', ':', ';', ' '];

    private readonly IPredictor predictor;
    private readonly IConversationManager conversationManager;
    private readonly Domain domain;
    private readonly IAnalyticsRecorder analyticsRecorder;

    /// <summary>Creates an agent without analytics recording. Equivalent to passing <see cref="NullAnalyticsRecorder"/>.</summary>
    public Agent(IPredictor predictor, IConversationManager conversationManager, Domain domain)
        : this(predictor, conversationManager, domain, NullAnalyticsRecorder.Instance)
    {
    }

    /// <summary>Creates an agent that records every turn to the supplied <paramref name="analyticsRecorder"/>.</summary>
    public Agent(IPredictor predictor, IConversationManager conversationManager, Domain domain, IAnalyticsRecorder analyticsRecorder)
    {
        this.predictor = predictor;
        this.conversationManager = conversationManager;
        this.domain = domain;
        this.analyticsRecorder = analyticsRecorder;
    }

    /// <inheritdoc/>
    public Response Handle(string conversationId, string input)
    {
        var (intentName, confidence) = predictor.Predict(input);

        var isFallback = confidence < domain.ConfidenceThreshold;
        var effectiveIntent = isFallback ? domain.FallbackIntentName : intentName;

        var conversation = conversationManager.Get(conversationId);
        var messages = conversation.HandleIntent(effectiveIntent, input);
        var entities = ExtractEntities(input);

        analyticsRecorder.Record(new AnalyticsEvent(
            ConversationId: conversationId,
            IntentName: effectiveIntent,
            Confidence: confidence,
            IsFallback: isFallback,
            Timestamp: DateTimeOffset.UtcNow));

        return new Response(effectiveIntent, messages, confidence, entities);
    }

    /// <inheritdoc/>
    public void Reset(string conversationId)
    {
        var conversation = conversationManager.Get(conversationId);
        conversation.Reset();
    }

    private Dictionary<string, string>? ExtractEntities(string input)
    {
        if (domain.Entities is not { Count: > 0 })
            return null;

        var words = input.Split(EntitySeparators, StringSplitOptions.RemoveEmptyEntries);
        Dictionary<string, string>? result = null;

        foreach (var entity in domain.Entities)
        {
            foreach (var word in words)
            {
                if (entity.Examples.Contains(word))
                {
                    (result ??= [])[entity.Name] = word;
                    break;
                }
            }
        }

        return result;
    }
}
