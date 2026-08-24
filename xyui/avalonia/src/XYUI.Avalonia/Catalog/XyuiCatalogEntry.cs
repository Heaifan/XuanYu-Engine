namespace XYUI.Avalonia.Catalog;

public sealed record XyuiCatalogEntry(
    string Module,
    string SourceItemId,
    string CanonicalId,
    string Name,
    string Title,
    string Description,
    string Preview,
    string Variants,
    string States,
    string Usage,
    string SpecificationPath,
    string AvaloniaType,
    IReadOnlyList<string> ApiRefs,
    XyuiCatalogStatus Status,
    bool SourcePresent)
{
    public string CanonicalIdentity { get; init; } = "";
    public string KnownGap { get; init; } = "";
    public string StateText => !Status.Ready ? "NOT READY" : string.IsNullOrEmpty(KnownGap) ? "READY FOR VISUAL ACCEPTANCE" : "READY WITH GAP";

    public string AvaloniaText => !SourcePresent
        ? "SOURCE NOT PRESENT IN CURRENT REPOSITORY"
        : (string.IsNullOrEmpty(AvaloniaType) ? "AVALONIA SOURCE NOT IMPLEMENTED" : AvaloniaType);

    public string ApiText => ApiRefs.Count == 0 ? "Canonical spec" : string.Join(" · ", ApiRefs);
}

public sealed record XyuiCatalogStatus(
    bool Designed,
    bool Canonical,
    bool Avalonia,
    bool Gallery,
    bool Documented)
{
    public bool Ready => Designed && Canonical && Avalonia && Gallery && Documented;

    public string ToText() => string.Join(" / ", new[]
    {
        Designed ? "DESIGNED" : "—",
        Canonical ? "CANONICAL" : "—",
        Avalonia ? "AVALONIA" : "—",
        Gallery ? "GALLERY" : "—",
        Documented ? "DOCUMENTED" : "—",
    });
}
