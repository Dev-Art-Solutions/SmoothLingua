# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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

[Unreleased]: https://github.com/Dev-Art-Solutions/SmoothLingua/compare/v1.2.0...HEAD
[1.2.0]: https://github.com/Dev-Art-Solutions/SmoothLingua/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/Dev-Art-Solutions/SmoothLingua/compare/v1.0.6...v1.1.0
[1.0.6]: https://github.com/Dev-Art-Solutions/SmoothLingua/releases/tag/v1.0.6
