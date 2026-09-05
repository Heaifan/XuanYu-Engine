using Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public sealed record XYUIDocProperty(string Name, string Type, string DefaultValue, string Description);
public sealed record XYUIDocVariant(string Name, string Description, string Usage);
public sealed record XYUIDocState(string Name, string Description);
public sealed record XYUIDocToken(string Name, string Value, string Description);

public sealed record XYUIDocRule(string Title, string Content);
public sealed record XYUIDocFoundationItem(string Concept, string FoundationToken, string Description);
public sealed record XYUIDocGuideItem(string Category, string Description);

public sealed record XYUI1ComponentDocument(
    string Id, string ChineseName, string EnglishName, string Overview, string WhenToUse,
    Func<Control> PreviewFactory, IReadOnlyList<string> Usages, IReadOnlyList<XYUIDocVariant> Variants,
    IReadOnlyList<XYUIDocState> States, IReadOnlyList<XYUIDocProperty> Properties,
    IReadOnlyList<XYUIDocToken> Tokens, string AvaloniaType)
{
    public string CanonicalDisplay => $"{CanonicalIdentity} · {EnglishName}";
    public string CanonicalIdentity { get; init; } = "";
    public string KnownGap { get; init; } = "";
    public string Category { get; init; } = "Canonical Stable · Typography / Text";
    public string QuickStartXaml { get; init; } = "";
    // 验收状态由目录注入：Phase 1A/1B 基线已收口，处于审查中。
    public string Acceptance { get; init; } = "BASELINE ACCEPTED · MIGRATION REVIEW";
    public string StatusText => string.IsNullOrEmpty(KnownGap) ? Acceptance : $"{Acceptance} · GAP RETAINED";
    public string VisualStatus => int.TryParse(Id.Replace("XYUI-1-", ""), out var idx) && idx <= 24 ? "USER VISUAL ACCEPTED" : "PENDING REVIEW";
    public bool HasKnownGap => !string.IsNullOrEmpty(KnownGap);
    public bool HasVariants => Variants.Count > 0;
    public bool HasStates => States.Count > 0;
    public bool HasInteractionGuide => Id == "XYUI-2-09";
    public bool HasNumberFieldInteractionGuide => Id == "XYUI-2-10";
    public IReadOnlyList<XYUIDocRule> CoreRules { get; init; } = [];
    public IReadOnlyList<XYUIDocFoundationItem> FoundationMappings { get; init; } = [];
    public IReadOnlyList<XYUIDocGuideItem> HowToUse { get; init; } = [];
    public Func<Control>? LiveExamplesFactory { get; init; }
    public bool HasCoreRules => CoreRules.Count > 0;
    public bool HasFoundationMappings => FoundationMappings.Count > 0;
    public bool HasHowToUse => HowToUse.Count > 0;
    public bool HasLiveExamples => LiveExamplesFactory != null;
}

public sealed record XYUI1NavigationItem(
    string Id, string ChineseName, string CanonicalName, XYUI1ComponentDocument? Document);

public sealed record FoundationNavigationItem(string Id, string ChineseName, string CanonicalName);
