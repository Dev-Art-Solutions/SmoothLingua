namespace SmoothLingua.Abstractions;

/// <summary>Represents a trained conversational agent that handles user input and maintains per-conversation state.</summary>
public interface IAgent
{
    /// <summary>Processes a user message within the given conversation and returns the bot's response.</summary>
    /// <param name="conversationId">A unique identifier for the conversation (e.g. a session or user ID).</param>
    /// <param name="input">The raw text message from the user.</param>
    /// <returns>A <see cref="Response"/> containing the predicted intent name and the bot's reply messages.</returns>
    Response Handle(string conversationId, string input);

    /// <summary>Resets the conversation state for the specified conversation, starting it from scratch.</summary>
    /// <param name="conversationId">The conversation to reset.</param>
    void Reset(string conversationId);
}
