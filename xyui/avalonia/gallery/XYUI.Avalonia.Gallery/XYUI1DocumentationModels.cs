using Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public sealed record XYUIDocProperty(string Name, string Type, string DefaultValue, string Description);
public sealed record XYUIDocVariant(string Name, string Description, string Usage);
public sealed record XYUIDocState(string Name, string Description);
public sealed record XYUIDocToken(string Name, string Value, string Description);

public sealed record XYUI1ComponentDocument(
    string Id, string ChineseName, string EnglishName, string Overview, string WhenToUse,
    Func<Control> PreviewFactory, IReadOnlyList<string> Usages, IReadOnlyList<XYUIDocVariant> Variants,
    IReadOnlyList<XYUIDocState> States, IReadOnlyList<XYUIDocProperty> Properties,
    IReadOnlyList<XYUIDocToken> Tokens, string AvaloniaType)
{
    public string CanonicalDisplay => $"XY.{EnglishName} · {EnglishName}";
    public bool HasVariants => Variants.Count > 0;
    public bool HasStates => States.Count > 0;
}

public sealed record XYUI1NavigationItem(
    string Id, string ChineseName, string CanonicalName, XYUI1ComponentDocument? Document);
