using Avalonia.Controls;
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
            PaletteCatalog.BuildSections(dark: false), XyuiCatalogSource.Load(), XYUI1GalleryCatalog.Build());
    }
}
