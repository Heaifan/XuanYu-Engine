using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FluidWarfare.Editor.Windows.About;

public partial class AboutFluidWarfareWindow : Window
{
    public AboutFluidWarfareWindow()
    {
        InitializeComponent();
    }

    private void OnCloseClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
