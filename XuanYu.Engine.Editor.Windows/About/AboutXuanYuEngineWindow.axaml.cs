using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace XuanYu.Engine.Editor.Windows.About;

public partial class AboutXuanYuEngineWindow : Window
{
    public AboutXuanYuEngineWindow()
    {
        InitializeComponent();
    }

    private void OnCloseClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
