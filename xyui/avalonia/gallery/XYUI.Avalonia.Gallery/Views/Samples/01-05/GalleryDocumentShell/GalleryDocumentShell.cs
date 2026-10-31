using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace XYUI.Avalonia.Gallery.Views;

public class GalleryDocumentShell : ScrollViewer
{
    protected override Type StyleKeyOverride => typeof(ScrollViewer);

    public GalleryDocumentShell()
    {
        HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
        VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
    }

    public void ScrollTo(double y)
    {
        Offset = new global::Avalonia.Vector(0, y);
    }
}

public class UniversalDocumentShell : GalleryDocumentShell {}
