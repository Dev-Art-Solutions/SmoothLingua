# SmoothLingua

[![build and test](https://github.com/Dev-Art-Solutions/SmoothLingua/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/Dev-Art-Solutions/SmoothLingua/actions/workflows/build-and-test.yml)
[![NuGet](https://img.shields.io/nuget/v/SmoothLingua.svg)](https://www.nuget.org/packages/SmoothLingua)

SmoothLingua is a .NET library for building conversational agents powered by intent recognition, story-based dialogue management, and rule-based responses. It is designed for teams that want a lightweight, code-first alternative to heavy conversational AI frameworks — you define your agent entirely in C# and run it anywhere .NET runs, with no external services required.

## Concepts

**Intent** — a named category of user utterances. You supply example phrases and the ML trainer learns to recognise them. An `Intent` named `Greeting` with examples `["Hello", "Hi"]` will match any similar greeting at runtime.

**Story** — a multi-turn conversation flow defined as an ordered list of `IntentStep` and `ResponseStep` entries. Stories model sequences: "after the user greets, reply; if they say they're fine, reply differently than if they say they're sad."

**Rule** — an always-active, single-turn shortcut. A rule fires whenever its intent is predicted, regardless of conversation context. Use rules for commands like `Bye` that should always produce the same response.

**Domain** — the bundle that groups all intents, stories, rules, slots, and entities into one trainable unit. You hand a `Domain` to `Trainer.Train`, which serialises it (together with the trained ML model) into a zip archive you can store anywhere.

**Agent** — the runtime object that handles user input. Obtain one via `AgentLoader.Load`. It predicts the intent, advances the conversation state, and returns a `Response` containing the matched intent name and the bot's reply messages.

## Quick Start

### Installation

```bash
dotnet add package SmoothLingua
```

### Minimal working example

```csharp
using SmoothLingua;
using SmoothLingua.Abstractions;
using SmoothLingua.Abstractions.Stories;

// 1. Define and train the domain in memory
MemoryStream memoryStream = new();
Trainer trainer = new();

await trainer.Train(new Domain(
    [
        new("Greeting", ["Hello", "Hi"]),
        new("Good",     ["I am fine", "I am good, thank you"]),
        new("Bad",      ["I am feeling bad", "I am not good"]),
        new("Bye",      ["Good bye", "Bye"]),
        new("IAmFrom",  ["I am from USA", "I am from Bulgaria", "I am from Germany"])
    ],
    [
        new("Good",
        [
            new IntentStep("Greeting"),
            new ResponseStep("Hello! How are you?"),
            new IntentStep("Good"),
            new ResponseStep("Glad to hear that! Where are you from?"),
            new IntentStep("IAmFrom"),
            new ResponseStep("It is nice in {country}!")
        ]),
        new("Bad",
        [
            new IntentStep("Greeting"),
            new ResponseStep("Hello! How are you?"),
            new IntentStep("Bad"),
            new ResponseStep("I am sorry to hear that.")
        ])
    ],
    [new("Bye", "Bye", "Bye")],
    [new Slot("country", "IAmFrom", "country", "")],
    [new Entity("country", new HashSet<string> { "Bulgaria", "USA", "Germany" })]
), memoryStream, default);

// 2. Load the trained agent and start a conversation
var agent = await AgentLoader.Load(new MemoryStream(memoryStream.GetBuffer()));
var conversationId = Guid.NewGuid().ToString();

var r = agent.Handle(conversationId, "bye");
Console.WriteLine($"{r.IntentName}: {string.Join(", ", r.Messages)}");
// Bye: Bye

r = agent.Handle(conversationId, "hello");
Console.WriteLine($"{r.IntentName}: {string.Join(", ", r.Messages)}");
// Greeting: Hello! How are you?

r = agent.Handle(conversationId, "I am fine");
Console.WriteLine($"{r.IntentName}: {string.Join(", ", r.Messages)}");
// Good: Glad to hear that! Where are you from?

r = agent.Handle(conversationId, "I am from Bulgaria");
Console.WriteLine($"{r.IntentName}: {string.Join(", ", r.Messages)}");
// IAmFrom: It is nice in Bulgaria!
```

### Load a model from file

Train once and persist the model to disk; no retraining needed on subsequent starts:

```csharp
// Train and save
await trainer.Train(domain, "model.zip", default);

// Load on every restart
var agent = await AgentLoader.Load("model.zip");
```

## When SmoothLingua, when not

**Good fit**
- .NET applications that need embedded conversational logic with no external services
- Prototypes and internal tools where you want full control over training data and model lifecycle
- Chatbots with a finite, well-defined intent set (up to ~50 intents)
- Offline or air-gapped environments

**Not a good fit**
- Large-scale open-domain conversation (use a hosted LLM instead)
- Use cases that require deep semantic understanding beyond the ML.NET SDCA classifier
- Teams that need a visual or no-code dialogue editor
