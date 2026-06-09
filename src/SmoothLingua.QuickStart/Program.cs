using SmoothLingua;
using SmoothLingua.Abstractions;
using SmoothLingua.Abstractions.Stories;
using SmoothLingua.Conversations;

// ── Shared domain ────────────────────────────────────────────────────────────

var domain = new Domain(
    [
        new("Greeting", ["Hello", "Hi"]),
        new("Good",     ["I am fine", "I am good, thank you"]),
        new("Bad",      ["I am feeling bad", "I am not good"]),
        new("Bye",      ["Good bye", "Bye"]),
        new("IAmFrom",  ["I am from USA", "I aam from Bulgaria", "I am from Germany"])
    ],
    [
        new("Good",
        [
            new IntentStep("Greeting"),
            new ResponseStep("Hello from bot!"),
            new IntentStep("Good"),
            new ResponseStep("I am glad to hear that! \n Where are you from?"),
            new IntentStep("IAmFrom"),
            new ResponseStep("It is nice in {country}")
        ]),
        new("Bad",
        [
            new IntentStep("Greeting"),
            new ResponseStep("Hello from bot!"),
            new IntentStep("Bad"),
            new ResponseStep("I am sorry to hear that!")
        ])
    ],
    [new("Bye", "Bye", "Bye")],
    new List<Slot>() { new Slot("country", "IAmFrom", "country", "") },
    new List<Entity>() { new Entity("country", new HashSet<string>() { "Bulgaria", "USA", "Germany" }) }
);

// ── Example 1: train into a MemoryStream ─────────────────────────────────────

Console.WriteLine("=== Example 1: in-memory model ===");

MemoryStream memoryStream = new();
Trainer trainer = new();
await trainer.Train(domain, memoryStream, default);

var agent = await AgentLoader.Load(new MemoryStream(memoryStream.GetBuffer()));
var conversationId = Guid.NewGuid().ToString();

PrintResponse(agent.Handle(conversationId, "bye"));
PrintResponse(agent.Handle(conversationId, "hello"));
PrintResponse(agent.Handle(conversationId, "I am fine"));
PrintResponse(agent.Handle(conversationId, "I am from Bulgaria"));
PrintResponse(agent.Handle(conversationId, "hello"));
PrintResponse(agent.Handle(conversationId, "I am bad"));

// ── Example 2: train to a file, then load from file ──────────────────────────

Console.WriteLine("\n=== Example 2: file-based model ===");

const string modelPath = "model.zip";
await trainer.Train(domain, modelPath, default);

var agentFromFile = await AgentLoader.Load(modelPath);
var conversationId2 = Guid.NewGuid().ToString();

PrintResponse(agentFromFile.Handle(conversationId2, "hello"));
PrintResponse(agentFromFile.Handle(conversationId2, "I am fine"));
PrintResponse(agentFromFile.Handle(conversationId2, "I am from USA"));

// ── Example 3: persistent conversation (survives process restart) ─────────────

Console.WriteLine("\n=== Example 3: FileConversationStore (persistent state) ===");

const string storeDir = "conversation-store";
const string persistentConvId = "demo-persistent-session";
var fileStore = new FileConversationStore(storeDir);
var persistentAgent = await AgentLoader.Load(modelPath, store: fileStore);

var existingState = fileStore.Get(persistentConvId);
if (existingState != null)
{
    Console.WriteLine($"Resuming existing conversation (story step {existingState.ActiveStep})...");
}
else
{
    Console.WriteLine("Starting a fresh conversation...");
}

PrintResponse(persistentAgent.Handle(persistentConvId, "hello"));
Console.WriteLine($"State saved to '{storeDir}/{persistentConvId}.json' — re-run to continue from here.");

// ── Helpers ───────────────────────────────────────────────────────────────────

static void PrintResponse(Response response)
{
    Console.WriteLine($"Intent: {response.IntentName}");
    foreach (var text in response.Messages)
        Console.WriteLine($"  > {text}");
}
