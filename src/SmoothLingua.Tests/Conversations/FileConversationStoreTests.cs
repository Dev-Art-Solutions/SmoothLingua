namespace SmoothLingua.Tests.Conversations;

using System.IO;
using Xunit;
using SmoothLingua.Abstractions.Conversations;
using SmoothLingua.Conversations;

public class FileConversationStoreTests : IDisposable
{
    private readonly string tempDir = Path.Combine(Path.GetTempPath(), $"sl-store-test-{Guid.NewGuid()}");

    public void Dispose() => Directory.Delete(tempDir, recursive: true);

    [Fact]
    public void Get_ReturnsNull_WhenNoStateHasBeenSaved()
    {
        var store = new FileConversationStore(tempDir);
        Assert.Null(store.Get("conv-1"));
    }

    [Fact]
    public void Save_ThenGet_ReturnsPersistedState()
    {
        var store = new FileConversationStore(tempDir);
        var state = new ConversationState(
            Slots: new Dictionary<string, string> { ["city"] = "Sofia" },
            ActiveStep: 2,
            ActiveStoryNames: ["story-a"]);

        store.Save("conv-1", state);
        var loaded = store.Get("conv-1");

        Assert.NotNull(loaded);
        Assert.Equal(2, loaded.ActiveStep);
        Assert.Equal(["story-a"], loaded.ActiveStoryNames);
        Assert.Equal("Sofia", loaded.Slots!["city"]);
    }

    [Fact]
    public void State_SurvivesNewStoreInstance()
    {
        // Simulates a process restart: first store instance saves, second loads.
        var state = new ConversationState(
            Slots: new Dictionary<string, string> { ["country"] = "Bulgaria" },
            ActiveStep: 3,
            ActiveStoryNames: ["greet-story"]);

        new FileConversationStore(tempDir).Save("session-42", state);

        // New instance — equivalent to process restart.
        var loaded = new FileConversationStore(tempDir).Get("session-42");

        Assert.NotNull(loaded);
        Assert.Equal(3, loaded.ActiveStep);
        Assert.Equal("Bulgaria", loaded.Slots!["country"]);
    }

    [Fact]
    public void Reset_RemovesPersistedState()
    {
        var store = new FileConversationStore(tempDir);
        var state = new ConversationState(null, 1, []);
        store.Save("conv-2", state);

        store.Reset("conv-2");

        Assert.Null(store.Get("conv-2"));
    }

    [Fact]
    public void Reset_DoesNotThrow_WhenNoStateExists()
    {
        var store = new FileConversationStore(tempDir);
        var ex = Record.Exception(() => store.Reset("nonexistent"));
        Assert.Null(ex);
    }
}
