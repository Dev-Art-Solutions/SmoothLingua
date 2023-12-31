namespace SmoothLingua.Abstractions.NLU;

public interface ITrainer
{
    void Train(List<Intent> intents, string path);

    void Train(List<Intent> intents, Stream stream);
}
