using System.ComponentModel;
using Avalonia;
using Avalonia.Styling;

namespace XYUI.Avalonia.Gallery;

public sealed class PaletteViewModel : INotifyPropertyChanged
{
    public IReadOnlyList<PaletteSection> Sections { get; private set; }
    public event PropertyChangedEventHandler? PropertyChanged;

    public PaletteViewModel()
    {
        Sections = BuildSections();
        if (Application.Current is { } app) app.ActualThemeVariantChanged += OnThemeChanged;
    }

    public static PaletteViewModel Create() => new();

    void OnThemeChanged(object? sender, EventArgs e)
    {
        Sections = BuildSections();
        PropertyChanged?.Invoke(this, new(nameof(Sections)));
    }

    static IReadOnlyList<PaletteSection> BuildSections() =>
        PaletteCatalog.BuildSections(Application.Current?.ActualThemeVariant == ThemeVariant.Dark);
}
