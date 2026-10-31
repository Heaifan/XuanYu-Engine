using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Gallery.Views;

public partial class XYUI3CompositionSection : UserControl
{
    public XYUI3CompositionSection()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is not XYUI1ComponentDocument doc || doc.CompositionFactory is null) return;
        CompositionHost.HorizontalContentAlignment = HorizontalAlignment.Left;
        CompositionHost.Content = doc.CompositionFactory();
    }
}
