using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Gallery.Views;

public partial class XYUI3LiveExamplesSection : UserControl
{
    public XYUI3LiveExamplesSection()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is not XYUI1ComponentDocument doc || doc.LiveExamplesFactory is null) return;
        LiveHost.HorizontalContentAlignment = HorizontalAlignment.Left;
        LiveHost.Content = doc.LiveExamplesFactory();
    }
}
