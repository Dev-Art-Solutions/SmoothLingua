namespace SmoothLingua;

using Abstractions;
using Abstractions.Conversations;
using Abstractions.NLU;

/// <summary>
/// Default implementation of <see cref="IAgent"/>. Predicts the user's intent, delegates to the
/// conversation manager for story/rule resolution, and returns the bot's reply.
/// Obtain an instance via <see cref="AgentLoader.Load(string, CancellationToken)"/>.
/// </summary>
public class Agent(IPredictor predictor, IConversationManager conversationManager) : IAgent
{
    private readonly IPredictor predictor = predictor;
    private readonly IConversationManager conversationManager = conversationManager;

    /// <inheritdoc/>
    public Response Handle(string conversationId, string input)
    {
        var intentName = predictor.Predict(input);
        var conversation = conversationManager.Get(conversationId);
        var messages = conversation.HandleIntent(intentName, input);

        return new Response(intentName, messages);
    }

    /// <inheritdoc/>
    public void Reset(string conversationId)
    {
        var conversation = conversationManager.Get(conversationId);
        conversation.Reset();
    }
}
