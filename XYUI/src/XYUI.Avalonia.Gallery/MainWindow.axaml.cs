using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace XYUI.Avalonia.Gallery;

public sealed partial class MainWindow : Window
{
    public MainWindow() => AvaloniaXamlLoader.Load(this);
}
