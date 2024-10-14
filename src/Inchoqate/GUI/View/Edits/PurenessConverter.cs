using Inchoqate.GUI.View.Converters;

namespace Inchoqate.GUI.View.Edits;

public class PurenessConverter() : SelectConverter<int?, string?>(convertBack: s => int.TryParse(s, out var value) ? value : null);