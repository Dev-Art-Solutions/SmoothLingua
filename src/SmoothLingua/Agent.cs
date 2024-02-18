namespace SmoothLingua;

using Abstractions;
using Abstractions.Conversations;
using Abstractions.NLU;

public class Agent(IPredictor predictor, IConversationManager conversationManager) : IAgent
{
    private readonly IPredictor predictor = predictor;
    private readonly IConversationManager conversationManager = conversationManager;

    public Response Handle(string conversationId, string input)
    {
        var intentName = predictor.Predict(input);
        var conversation = conversationManager.Get(conversationId);
        var messages = conversation.HandleIntent(intentName);

        return new Response(intentName, messages);
    }

    public void Reset(string conversationId)
    {
        var conversation = conversationManager.Get(conversationId);
        conversation.Reset();
    }
}
