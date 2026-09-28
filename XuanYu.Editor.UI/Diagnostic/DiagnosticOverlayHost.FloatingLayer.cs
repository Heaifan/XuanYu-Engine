using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    void AttachFloatingLayer(Window window)
    {
        var root = EnsureWindowRoot(window);
        if (root is null) return;
        if (_floatingLayer?.Parent is Panel) return;
        _floatingLayer = new Canvas { HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch, IsHitTestVisible = false };
        _floatingLayer.SetValue(Panel.ZIndexProperty, -1); root.Children.Insert(0, _floatingLayer);
        window.UpdateLayout();
    }

    void DetachFloatingLayer()
    {
        if (_floatingLayer?.Parent is Panel root) root.Children.Remove(_floatingLayer);
        _floatingLayer = null;
    }
}
