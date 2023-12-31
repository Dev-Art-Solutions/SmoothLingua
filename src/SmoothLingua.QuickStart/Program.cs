using SmoothLingua;
using SmoothLingua.Abstractions;
using SmoothLingua.Abstractions.Stories;

MemoryStream memoryStream = new MemoryStream();

Trainer trainer = new Trainer();

trainer.Train(new Domain(
    new List<SmoothLingua.Abstractions.NLU.Intent>()
    {
        new SmoothLingua.Abstractions.NLU.Intent("Greeting",
        new List<string>(){ "Hello", "Hi" }),
        new SmoothLingua.Abstractions.NLU.Intent("Good",
        new List<string>(){ "I am fine", "I am good, thank you" }),
        new SmoothLingua.Abstractions.NLU.Intent("Bad",
        new List<string>(){ "I am feeling bad", "I am not good" }),
        new SmoothLingua.Abstractions.NLU.Intent("Bye",
        new List<string>(){ "Good bye", "Bye" })
    },
    new List<Story>()
    {
        new Story("Good", new List<SmoothLingua.Abstractions.Stories.Step>()
        {
            new IntentStep("Greeting"),
            new ResponseStep("Hello from bot!"),
            new IntentStep("Good"),
            new ResponseStep("I am glad to hear that!")
        }),
        new Story("Bad", new List<SmoothLingua.Abstractions.Stories.Step>()
        {
            new IntentStep("Greeting"),
            new ResponseStep("Hello from bot!"),
            new IntentStep("Bad"),
            new ResponseStep("I am sorry to hear that!")
        })
    },
    new List<SmoothLingua.Abstractions.Rules.Rule>()
    {
        new SmoothLingua.Abstractions.Rules.Rule("Bye","Bye","Bye")
    }
    ), memoryStream).Wait();

var conversationId = Guid.NewGuid().ToString();
var agent = await AgentLoader.Load(new MemoryStream(memoryStream.GetBuffer()));

var response = agent.Handle(conversationId, "bye");
Console.WriteLine($"Intent:{response.IntentName}");

foreach (var text in response.Messages)
{
    Console.WriteLine($"Response:{text}");
}

response = agent.Handle(conversationId, "hello");
Console.WriteLine($"Intent:{response.IntentName}");

foreach (var text in response.Messages)
{
    Console.WriteLine($"Response:{text}");
}

response = agent.Handle(conversationId, "I am fine");
Console.WriteLine($"Intent:{response.IntentName}");

foreach (var text in response.Messages)
{
    Console.WriteLine($"Response:{text}");
}

response = agent.Handle(conversationId, "hello");
Console.WriteLine($"Intent:{response.IntentName}");

foreach (var text in response.Messages)
{
    Console.WriteLine($"Response:{text}");
}

response = agent.Handle(conversationId, "I am bad");
Console.WriteLine($"Intent:{response.IntentName}");

foreach (var text in response.Messages)
{
    Console.WriteLine($"Response:{text}");
}
