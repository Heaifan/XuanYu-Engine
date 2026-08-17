using Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

// 主窗口数据模型（x:DataType 编译绑定需要具名类型）
public sealed record MainWindowModel(IReadOnlyList<PaletteSection> Sections);

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowModel(PaletteCatalog.BuildSections(dark: false));
    }
}
