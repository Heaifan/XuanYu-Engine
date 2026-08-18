namespace XYUI.Avalonia.Gallery;

public sealed record PaletteViewModel(IReadOnlyList<PaletteSection> Sections)
{
    public static PaletteViewModel Create() => new(PaletteCatalog.BuildSections(dark: false));
}
