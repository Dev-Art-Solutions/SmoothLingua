namespace SmoothLingua.Abstractions;

public record Slot(string Name, string? MappedFromIntent, string? Entity, string? DefaultValue);
