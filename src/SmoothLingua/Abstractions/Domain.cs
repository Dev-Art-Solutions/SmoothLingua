using SmoothLingua.Abstractions.NLU;
using SmoothLingua.Abstractions.Rules;
using SmoothLingua.Abstractions.Stories;

namespace SmoothLingua.Abstractions;

public record Domain(List<Intent> Intents, List<Story> Stories, List<Rule> Rules);
