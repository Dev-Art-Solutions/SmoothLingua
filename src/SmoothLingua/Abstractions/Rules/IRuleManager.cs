namespace SmoothLingua.Abstractions.Rules;

public interface IRuleManager
{
    bool TryGetResponse(string intentName, out string? response);
}
