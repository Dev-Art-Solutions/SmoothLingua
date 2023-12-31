namespace SmoothLingua.NLU.Internal;

using Abstractions.NLU;

public class IntentValidator
{
    public static void Validate(Intent intent)
    {
        if(intent == null)
        {
            throw new ArgumentNullException(nameof(intent));
        }

        if(string.IsNullOrEmpty(intent.Name))
        {
            throw new ArgumentNullException(nameof(intent.Name));
        }

        if(intent.Examples == null)
        {
            throw new ArgumentNullException(nameof(intent.Examples));
        }

        if(intent.Examples.Count == 0)
        {
            throw new ArgumentException("Examples should have at least one example");
        }

        foreach(var example in intent.Examples)
        {
            if (string.IsNullOrEmpty(example))
            {
                throw new ArgumentNullException(nameof(example));
            }
        }
    }
}
