using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed class XYContextOverlayHost : Canvas
{
    XYContextOverlayHost() { ClipToBounds = true; ZIndex = 10000; HorizontalAlignment = HorizontalAlignment.Stretch; VerticalAlignment = VerticalAlignment.Stretch; }
    public static XYContextOverlayHost? Attach(Control target)
    {
        if (TopLevel.GetTopLevel(target) is not Window window || window.Content is not Control content) return null;
        var overlay = content as Grid ?? new Grid();
        var host = overlay.Children.OfType<XYContextOverlayHost>().FirstOrDefault();
        if (host is null)
        {
            if (!ReferenceEquals(overlay, content)) { window.Content = null; overlay.Children.Add(content); window.Content = overlay; }
            host = new XYContextOverlayHost(); overlay.Children.Add(host);
        }
        host.IsHitTestVisible = true; return host;
    }
    public void AddOverlay(Control control) { if (!Children.Contains(control)) Children.Add(control); }
    public void RemoveOverlay(Control control) { if (Children.Contains(control)) Children.Remove(control); }
    public void ClearOverlays() => Children.Clear();
}
