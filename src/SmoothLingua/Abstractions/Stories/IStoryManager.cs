namespace SmoothLingua.Abstractions.Stories;

public interface IStoryManager
{
    void ClearState();

    bool TryGetResponse(string intentName, out List<string> responses);

    /// <summary>Returns the current story-progress snapshot (active step index and active story names).</summary>
    (int ActiveStep, List<string> ActiveStoryNames) GetState();

    /// <summary>Restores a previously saved story-progress snapshot.</summary>
    void LoadState(int activeStep, List<string> activeStoryNames);
}
