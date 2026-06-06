# Contributing to SmoothLingua

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Build

```bash
dotnet build src
```

## Run tests

```bash
dotnet test src
```

## Code formatting

The project enforces consistent formatting via `dotnet format`. Before opening a PR, run:

```bash
dotnet format src
```

CI will reject PRs where `dotnet format --verify-no-changes` fails.

## Submitting changes

1. Fork the repository and create a branch from `main`.
2. Make your changes and ensure all tests pass.
3. Run `dotnet format src` to ensure consistent formatting.
4. Open a pull request using the provided template.

## Versioning

Versions follow [Semantic Versioning](https://semver.org/). The authoritative version is set by the git tag at release time — do not change `PackageVersion` in the csproj manually.
