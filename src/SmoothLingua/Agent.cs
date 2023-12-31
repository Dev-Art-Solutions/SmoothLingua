namespace SmoothLingua;

using Abstractions;
using Abstractions.Conversations;
using Abstractions.NLU;

public class Agent : IAgent
{
    private IPredictor predictor;
    private IConversationManager conversationManager;

    public Agent(IPredictor predictor, IConversationManager conversationManager)
    {
        this.predictor = predictor;
        this.conversationManager = conversationManager;
    }

    public Response Handle(string conversationId, string input)
    {
        var intentName = predictor.Predict(input);
        var conversation = conversationManager.Get(conversationId);
        var messages = conversation.HandleIntent(intentName);

        return new Response(intentName, messages);
    }
}
