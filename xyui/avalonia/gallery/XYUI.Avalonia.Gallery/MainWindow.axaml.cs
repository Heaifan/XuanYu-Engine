using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using XYUI.Avalonia.Catalog;

namespace XYUI.Avalonia.Gallery;

// 主窗口数据模型（x:DataType 编译绑定需要具名类型）
public sealed record MainWindowModel(
    IReadOnlyList<PaletteSection> Sections,
    IReadOnlyList<XyuiCatalogEntry> CatalogEntries,
    IReadOnlyList<XYUI1GalleryItem> XYUI1Entries);

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowModel(
            PaletteCatalog.BuildSections(dark: false), XyuiCatalogSource.Load(), Array.Empty<XYUI1GalleryItem>());
        if (Program.InitialComponentId is { Length: > 0 } id &&
            DocumentationView.DataContext is XYUI1DocumentationViewModel documentation)
            documentation.Select(id);
        if (Application.Current is { } app)
        {
            app.ActualThemeVariantChanged += OnActualThemeVariantChanged;
            Closed += (_, _) => app.ActualThemeVariantChanged -= OnActualThemeVariantChanged;
            UpdateThemeSwitch();
        }
    }

    void OnActualThemeVariantChanged(object? sender, EventArgs e) => UpdateThemeSwitch();

    void OnThemeSwitchClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current is not { } app) return;
        app.RequestedThemeVariant = app.ActualThemeVariant == ThemeVariant.Dark
            ? ThemeVariant.Light : ThemeVariant.Dark;
    }

    void UpdateThemeSwitch()
    {
        var dark = Application.Current?.ActualThemeVariant == ThemeVariant.Dark;
        ThemeStateText.Text = dark ? "Theme：Dark" : "Theme：Light";
        ThemeSwitchButton.Content = dark ? "切换 Light" : "切换 Dark";
    }
}
