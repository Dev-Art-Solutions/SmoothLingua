# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [2.2.0] - 2026-06-10

### Added

- **`IAnalyticsRecorder` abstraction** — captures anonymous per-turn signals (intent, confidence,
  fallback flag, conversation id) and exposes them as aggregates. Lives in the core package and
  has no external dependencies.
- **`InMemoryAnalyticsRecorder`** — thread-safe default implementation; computes totals, fallback
  rate, average confidence, average conversation length, and per-intent stats on demand.
- **`NullAnalyticsRecorder`** — zero-cost no-op used by default so existing hosts opt in to
  analytics rather than paying for it implicitly.
- **`AnalyticsEvent` / `AnalyticsSnapshot` / `IntentStat`** — public records that describe a single
  turn and the aggregate view over the whole stream.
- **`GET /insights`** on `SmoothLingua.Server` — returns the snapshot as JSON, never exposes
  conversation identifiers or message text.
- **`POST /playground/predict`** on `SmoothLingua.Server` — trains a small per-request model from
  visitor-supplied intent examples and predicts the matching intent for a piece of text. Includes
  an LRU cache over trained predictors and hard caps on intent / example counts.
- **CORS support** on `SmoothLingua.Server` — configurable via `SmoothLingua:Cors:AllowedOrigins`;
  the demo subdomain (`chat.smooth-lingua.com`) and the main site are allowed by default.
- **Interactive demo at `chat.smooth-lingua.com`** — vanilla JS + Bootstrap chat page that talks
  to the public API, surfaces the predicted intent and confidence on every reply, hosts a
  Playground tab for user-defined intents, and an Insights tab for live aggregates.
- Tests: aggregation correctness and thread-safety for `InMemoryAnalyticsRecorder`, integration
  tests for `/insights` (including a "never leaks conversation IDs" check) and `/playground/predict`.

### Changed

- `AgentLoader.Load(...)` gains an optional `analyticsRecorder:` parameter. When omitted, the
  no-op recorder is used and existing callers see no behaviour change.
- `Agent` exposes a second constructor that accepts an `IAnalyticsRecorder`; the existing
  constructor remains and forwards to the no-op recorder.
- `AddSmoothLingua` registers an `IAnalyticsRecorder` (`InMemoryAnalyticsRecorder`) alongside the
  agent and conversation store.

## [2.1.0] - 2026-06-09

### Added

- **`IConversationStore` abstraction** — lives in the core package with no ASP.NET dependency.
  Exposes `Get`, `Save`, and `Reset`; any host (console, desktop, web service) uses the same types.
- **`InMemoryConversationStore`** — default implementation, preserves existing behaviour exactly.
- **`FileConversationStore`** — durable implementation that serialises each conversation as a JSON
  file. Conversation state survives process restarts. Thread-safe via an internal lock; no external
  dependencies required.
- **`ConversationState`** record — serialisable snapshot of a conversation (slot values, active
  story step, active story names).
- **`IStoryManager.GetState` / `LoadState`** — allows story progress to be captured and restored.
- **`IConversation.GetState`** — exposes the current state snapshot for persistence.
- **`SmoothLingua.Server`** — new minimal ASP.NET Core Web API project:
  - `POST /conversations/{id}/messages` — processes a user message and returns a `Response`.
  - `POST /conversations/{id}/reset` — resets conversation state.
  - `GET /health` — liveness check.
  - `services.AddSmoothLingua(configuration)` DI extension; automatically selects
    `FileConversationStore` when `SmoothLingua:StoreDirectory` is configured.
- **`Dockerfile`** — multi-stage build for `SmoothLingua.Server`; mounts `/app/data` as a volume
  for the model file and conversation store.
- **Console persistence demo** — QuickStart Example 3 demonstrates a conversation that resumes
  from `FileConversationStore` after a process restart.
- Unit tests for `FileConversationStore` (including restart-survival test).
- Integration tests for all three server endpoints via `WebApplicationFactory`.

### Changed

- `AgentLoader.Load(...)` gains an optional `store:` parameter. When omitted, `InMemoryConversationStore`
  is used — no behaviour change for existing callers.
- `ConversationManager` gains an optional `IConversationStore` constructor parameter with the same
  default, ensuring full backward compatibility.

## [2.0.0] - 2026-06-08

### Added

- **Confidence score** — every `Response` now carries a `Confidence` property (0–1). For domains with a
  single intent `SingleIntentPredictor` returns `1.0`; for multi-intent domains the raw SDCA scores are
  normalised with softmax.
- **Fallback intent** — when the predicted confidence falls below a configurable threshold the agent
  automatically routes to a fallback intent instead of an uncertain prediction. Two new fields on `Domain`
  control this behaviour:
  - `ConfidenceThreshold` (default `0.4`) — minimum confidence required to accept a prediction.
  - `FallbackIntentName` (default `"nlu_fallback"`) — the intent used when confidence is too low.
- **Entity extraction** — `Response` now carries an optional `ExtractedEntities` dictionary
  (`Dictionary<string, string>?`). At runtime the agent scans the user input for words that match any
  entity's example list and returns each match keyed by entity name. Requires `Entities` to be defined on
  the `Domain`; returns `null` otherwise.

### Changed

- `IPredictor.Predict` return type changed from `string` to `(string IntentName, float Confidence)`.
- `Agent` constructor gains a third parameter: `Domain domain`.

### Breaking changes and migration from v1.x

**`Response` has a new required positional parameter `Confidence`.**

```csharp
// v1.x
Response r = new("Greeting", messages);

// v2.0
Response r = new("Greeting", messages, 0.9f);
```

If you pattern-match or deconstruct `Response` you need to handle the third element.

**`IPredictor.Predict` now returns a tuple.**

Custom `IPredictor` implementations must be updated:

```csharp
// v1.x
public string Predict(string text) => "myIntent";

// v2.0
public (string IntentName, float Confidence) Predict(string text) => ("myIntent", 1.0f);
```

**`Agent` requires `Domain` in its constructor.**

If you instantiate `Agent` directly (rather than via `AgentLoader`) pass the `Domain` as the third argument.

## [1.2.0] - 2026-06-07

### Added
- Rewritten README with hero paragraph, Concepts section (Intent/Story/Rule/Domain/Agent), Quick Start, and "When SmoothLingua, when not" guidance.
- `<GenerateDocumentationFile>` enabled in `SmoothLingua.csproj` — NuGet consumers now get IntelliSense descriptions.
- XML documentation comments on all public types: `IAgent`, `ITrainer`, `Agent`, `AgentLoader`, `Trainer`, `Domain`, `Intent`, `Story`, `Rule`, `Response`.
- Second QuickStart example demonstrating model training and loading from a file (`model.zip`) instead of a `MemoryStream`.

### Changed
- Website (`smooth-lingua.com`) content updated to match the new library documentation: Concepts section, updated Quick Start, When/Not guidance, and refreshed Changelog.

## [1.1.0] - 2026-06-06

### Added
- Automated release pipeline: pushing a `v*` tag now builds, tests, packs, and publishes to NuGet automatically.
- GitHub Release is created automatically with generated release notes on each tag push.
- `CHANGELOG.md` (this file) following Keep a Changelog format.
- `CONTRIBUTING.md` with build and test instructions.
- GitHub issue templates for bug reports and feature requests.
- Pull request template.
- Build status and NuGet version badges in README.
- Code coverage collection via Coverlet, uploaded as a CI artifact on every build.
- `dotnet format --verify-no-changes` check in CI to enforce consistent formatting.

### Changed
- CI workflow split into `build-and-test.yml` (runs on push/PR) and `release.yml` (runs on version tags).
- `PackageVersion` in `SmoothLingua.csproj` is now a fallback — the authoritative version comes from the git tag during release.

## [1.0.6] - 2024-01-01

### Added
- Initial public release.
- Intent recognition, story management, rule-based responses.
- `Trainer`, `Agent`, `AgentLoader`, `Conversation`, `Domain` core types.
- NuGet package `SmoothLingua`.

[Unreleased]: https://github.com/Dev-Art-Solutions/SmoothLingua/compare/v2.2.0...HEAD
[2.2.0]: https://github.com/Dev-Art-Solutions/SmoothLingua/compare/v2.1.0...v2.2.0
[2.1.0]: https://github.com/Dev-Art-Solutions/SmoothLingua/compare/v2.0.0...v2.1.0
[2.0.0]: https://github.com/Dev-Art-Solutions/SmoothLingua/compare/v1.2.0...v2.0.0
[1.2.0]: https://github.com/Dev-Art-Solutions/SmoothLingua/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/Dev-Art-Solutions/SmoothLingua/compare/v1.0.6...v1.1.0
[1.0.6]: https://github.com/Dev-Art-Solutions/SmoothLingua/releases/tag/v1.0.6
