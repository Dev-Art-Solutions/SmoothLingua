using SmoothLingua;
using SmoothLingua.Abstractions;
using SmoothLingua.Abstractions.Stories;

MemoryStream memoryStream = new();

Trainer trainer = new();

trainer.Train(new Domain(
    [
        new ("Greeting",
        ["Hello", "Hi"]),
        new ("Good",
        ["I am fine", "I am good, thank you"]),
        new ("Bad",
        ["I am feeling bad", "I am not good"]),
        new ("Bye",
        ["Good bye", "Bye"])
    ],
    [
        new ("Good",
        [
            new IntentStep("Greeting"),
            new ResponseStep("Hello from bot!"),
            new IntentStep("Good"),
            new ResponseStep("I am glad to hear that!")
        ]),
        new ("Bad",
        [
            new IntentStep("Greeting"),
            new ResponseStep("Hello from bot!"),
            new IntentStep("Bad"),
            new ResponseStep("I am sorry to hear that!")
        ])
    ],
    [
        new ("Bye","Bye","Bye")
    ]
    ), memoryStream, default).Wait();

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
