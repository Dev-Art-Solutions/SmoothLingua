namespace SmoothLingua.Abstractions.Stories;

public interface IStoryManager
{
    void ClearState();

    bool TryGetResponse(string intentName, out List<string> responses);
}
