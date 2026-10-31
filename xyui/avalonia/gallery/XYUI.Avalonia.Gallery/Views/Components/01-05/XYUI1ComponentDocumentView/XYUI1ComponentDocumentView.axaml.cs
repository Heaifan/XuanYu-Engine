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
        PreviewHost.HorizontalContentAlignment = HorizontalAlignment.Left;
        PreviewHost.Content = document.PreviewFactory();
        var liveHost = this.FindControl<ContentControl>("LiveExamplesHost");
        if (liveHost != null && document.LiveExamplesFactory != null)
        {
            liveHost.HorizontalContentAlignment = HorizontalAlignment.Left;
            liveHost.Content = document.LiveExamplesFactory();
        }
    }

    public void ScrollToExamples()
    {
        var sv = this.FindControl<ScrollViewer>("DocScrollViewer");
        if (sv != null) sv.Offset = new global::Avalonia.Vector(0, 520);
    }
}
