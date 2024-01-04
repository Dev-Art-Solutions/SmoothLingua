namespace SmoothLingua.NLU.Internal;

using Abstractions.NLU;

public class IntentValidator
{
    public static void Validate(Intent intent)
    {
        ArgumentNullException.ThrowIfNull(intent);

        if (string.IsNullOrEmpty(intent.Name))
        {
            throw new ArgumentException($"{nameof(intent.Name)} is null", nameof(intent));
        }

        if(intent.Examples == null)
        {
            throw new ArgumentException($"{nameof(intent.Examples)} is null", nameof(intent));
        }

        if(intent.Examples.Count == 0)
        {
            throw new ArgumentException("Examples should have at least one example");
        }

        foreach(var example in intent.Examples)
        {
            if (string.IsNullOrEmpty(example))
            {
                throw new ArgumentException($"Intent example is empty {nameof(example)}", intent.Name);
            }
        }
    }
}
