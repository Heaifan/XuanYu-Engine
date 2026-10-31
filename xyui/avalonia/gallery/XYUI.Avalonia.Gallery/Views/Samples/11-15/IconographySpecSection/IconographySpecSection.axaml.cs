using Avalonia.Controls;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery.Views;

public partial class IconographySpecSection : UserControl
{
    public IconographySpecSection()
    {
        InitializeComponent();

        var compact = XyuiIconSizeMetrics.For(XyuiIconSize.Compact);
        TxtCompactSize.Text = $"{compact.SizeDip:0} DIP · Compact";
        TxtCompactStroke.Text = $"Stroke {compact.StrokeWidth:0.00} DIP";

        var def = XyuiIconSizeMetrics.For(XyuiIconSize.Default);
        TxtDefaultSize.Text = $"{def.SizeDip:0} DIP · Default";
        TxtDefaultStroke.Text = $"Stroke {def.StrokeWidth:0.00} DIP";

        var comf = XyuiIconSizeMetrics.For(XyuiIconSize.Comfortable);
        TxtComfortableSize.Text = $"{comf.SizeDip:0} DIP · Comfortable";
        TxtComfortableStroke.Text = $"Stroke {comf.StrokeWidth:0.00} DIP";

        var touch = XyuiIconSizeMetrics.For(XyuiIconSize.Touch);
        TxtTouchSize.Text = $"{touch.SizeDip:0} DIP · Touch";
        TxtTouchStroke.Text = $"Stroke {touch.StrokeWidth:0.00} DIP";

        var search = XyuiVectorIcons.GetMetrics(XyuiVectorIcon.Search);
        TxtSharedMetrics.Text = $"Canonical Search · Logical Viewport {search.LogicalViewport:0} DIP · Geometry Bounds {search.GeometryBounds.Width:0.#} DIP";
    }
}
