using Avalonia.Controls;

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
        if (DataContext is XYUI1ComponentDocument document) PreviewHost.Content = document.PreviewFactory();
    }
}
