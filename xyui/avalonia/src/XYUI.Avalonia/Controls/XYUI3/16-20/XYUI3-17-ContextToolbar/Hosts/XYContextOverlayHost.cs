using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Controls;

public sealed class XYContextOverlayHost : Canvas
{
    public XYContextOverlayHost() { ClipToBounds = true; ZIndex = 10000; HorizontalAlignment = HorizontalAlignment.Stretch; VerticalAlignment = VerticalAlignment.Stretch; }
    public static XYContextOverlayHost? Attach(Control target)
    {
        if (TopLevel.GetTopLevel(target) is not Window window || window.Content is not Control content) return null;
        window.HorizontalContentAlignment = HorizontalAlignment.Stretch; window.VerticalContentAlignment = VerticalAlignment.Stretch;
        var existing = window.GetVisualDescendants().OfType<XYContextOverlayHost>().FirstOrDefault();
        if (existing is not null) { existing.IsHitTestVisible = true; return existing; }
        var overlay = content as Grid ?? new Grid();
        var host = overlay.Children.OfType<XYContextOverlayHost>().FirstOrDefault();
        if (host is null)
        {
            if (!ReferenceEquals(overlay, content)) { window.Content = null; overlay.Children.Add(content); window.Content = overlay; }
            overlay.HorizontalAlignment = HorizontalAlignment.Left; overlay.VerticalAlignment = VerticalAlignment.Top; overlay.Width = window.ClientSize.Width; overlay.Height = window.ClientSize.Height;
            host = new XYContextOverlayHost { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top }; overlay.Children.Add(host); overlay.SizeChanged += host.OnOwnerSizeChanged; window.SizeChanged += (_, e) => { overlay.Width = e.NewSize.Width; overlay.Height = e.NewSize.Height; };
        }
        host.SyncToOwner(window.ClientSize); host.IsHitTestVisible = true; return host;
    }
    void OnOwnerSizeChanged(object? sender, SizeChangedEventArgs e) => SyncToOwner(e.NewSize);
    internal void SyncToOwner(Size size) { Width = size.Width; Height = size.Height; }
    public void AddOverlay(Control control) { if (!Children.Contains(control)) Children.Add(control); }
    public void RemoveOverlay(Control control) { if (Children.Contains(control)) Children.Remove(control); }
    public void ClearOverlays() => Children.Clear();
}
