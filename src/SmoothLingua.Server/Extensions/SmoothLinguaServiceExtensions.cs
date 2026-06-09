namespace SmoothLingua.Server.Extensions;

using SmoothLingua.Abstractions;
using SmoothLingua.Conversations;

public static class SmoothLinguaServiceExtensions
{
    /// <summary>
    /// Registers <see cref="IAgent"/> as a singleton and wires up a conversation store.
    /// Reads <c>SmoothLingua:ModelPath</c> and optionally <c>SmoothLingua:StoreDirectory</c>
    /// from configuration. When <c>StoreDirectory</c> is set, a <see cref="FileConversationStore"/>
    /// is used so conversation state survives restarts; otherwise an in-memory store is used.
    /// </summary>
    public static IServiceCollection AddSmoothLingua(this IServiceCollection services, IConfiguration configuration)
    {
        var modelPath = configuration["SmoothLingua:ModelPath"]
            ?? throw new InvalidOperationException("SmoothLingua:ModelPath is required in configuration.");

        var storeDirectory = configuration["SmoothLingua:StoreDirectory"];

        var store = storeDirectory is { Length: > 0 }
            ? (Abstractions.Conversations.IConversationStore)new FileConversationStore(storeDirectory)
            : new InMemoryConversationStore();

        services.AddSingleton(store);
        services.AddSingleton<IAgent>(_ =>
            AgentLoader.Load(modelPath, store: store).GetAwaiter().GetResult());

        return services;
    }
}
