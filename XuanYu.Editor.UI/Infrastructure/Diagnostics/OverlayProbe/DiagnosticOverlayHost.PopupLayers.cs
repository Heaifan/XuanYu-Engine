using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    readonly Dictionary<TopLevel, Canvas> _probeRootLayers = [];

    public int ActivePopupProbeLayerCount => _probeRootLayers.Count;
    public int ActivePopupProbeHighlightCount => _probeHighlight?.Parent is Canvas parent &&
        _probeRootLayers.Values.Contains(parent) ? 1 : 0;

    Canvas? GetProbeLayer(TopLevel root)
    {
        if (ReferenceEquals(root, _topLevel)) return ProbeOwner;
        if (_probeRootLayers.TryGetValue(root, out var existing)) return existing;
        if (root.GetVisualDescendants().OfType<Panel>().FirstOrDefault() is { } panel)
            return AddProbeLayer(root, panel);
        if (root is not ContentControl content || content.Content is not Control child) return null;
        var wrapper = new Grid { HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch };
        var layer = NewProbeLayer();
        content.Content = wrapper; wrapper.Children.Add(child); wrapper.Children.Add(layer);
        _probeRootLayers[root] = layer; return layer;
    }

    Canvas AddProbeLayer(TopLevel root, Panel panel)
    {
        var layer = NewProbeLayer(); panel.Children.Add(layer); _probeRootLayers[root] = layer; return layer;
    }

    static Canvas NewProbeLayer() => new()
    {
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Stretch,
        IsHitTestVisible = false
    };

    void RemoveProbeLayer(TopLevel root)
    {
        if (!_probeRootLayers.Remove(root, out var layer)) return;
        if (layer.Parent is Panel panel) panel.Children.Remove(layer);
    }
}
