using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Gallery.Views;

public partial class XYUI1ComponentDocumentView : UserControl
{
    public XYUI1ComponentDocumentView()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is not XYUI1ComponentDocument document) return;
        var preview = document.PreviewFactory();
        PreviewHost.HorizontalContentAlignment = HorizontalAlignment.Left;
        PreviewHost.Content = preview;
    }
}
