using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Gallery.Views;

public partial class XYUI3QuickStartSection : UserControl
{
    public XYUI3QuickStartSection()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is not XYUI1ComponentDocument doc) return;
        QuickStartHost.HorizontalContentAlignment = HorizontalAlignment.Left;
        QuickStartHost.Content = doc.PreviewFactory();
    }
}
