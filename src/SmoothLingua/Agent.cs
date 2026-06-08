namespace SmoothLingua;

using Abstractions;
using Abstractions.Conversations;
using Abstractions.NLU;

/// <summary>
/// Default implementation of <see cref="IAgent"/>. Predicts the user's intent, delegates to the
/// conversation manager for story/rule resolution, and returns the bot's reply.
/// Obtain an instance via <see cref="AgentLoader.Load(string, CancellationToken)"/>.
/// </summary>
public class Agent(IPredictor predictor, IConversationManager conversationManager, Domain domain) : IAgent
{
    private static readonly char[] EntitySeparators = ['!', '?', '.', ',', ':', ';', ' '];

    private readonly IPredictor predictor = predictor;
    private readonly IConversationManager conversationManager = conversationManager;
    private readonly Domain domain = domain;

    /// <inheritdoc/>
    public Response Handle(string conversationId, string input)
    {
        var (intentName, confidence) = predictor.Predict(input);

        var effectiveIntent = confidence >= domain.ConfidenceThreshold
            ? intentName
            : domain.FallbackIntentName;

        var conversation = conversationManager.Get(conversationId);
        var messages = conversation.HandleIntent(effectiveIntent, input);
        var entities = ExtractEntities(input);

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
