using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    readonly List<Popup> _probePopups = [];
    readonly Dictionary<Popup, TopLevel> _probePopupRoots = [];

    void AttachProbePopups(Window window)
    {
        foreach (var popup in window.GetLogicalDescendants().OfType<Popup>()) AttachProbePopup(popup);
    }

    void AttachProbePopup(Popup popup)
    {
        if (_probePopups.Contains(popup)) return;
        _probePopups.Add(popup); popup.Opened += OnProbePopupOpened; popup.Closed += OnProbePopupClosed;
    }

    void DetachProbePopup(Popup popup)
    {
        popup.Opened -= OnProbePopupOpened; popup.Closed -= OnProbePopupClosed;
        _probePopups.Remove(popup); _probePopupRoots.Remove(popup);
    }

    void OnProbePopupOpened(object? sender, EventArgs e)
    {
        if (sender is not Popup popup || popup.Child is not Visual child || TopLevel.GetTopLevel(child) is not { } root) return;
        _probePopupRoots[popup] = root; AttachProbeRoot(root);
    }

    void OnProbePopupClosed(object? sender, EventArgs e)
    {
        if (sender is not Popup popup) return;
        var root = _probePopupRoots.TryGetValue(popup, out var tracked) ? tracked :
            popup.Child is Visual child ? TopLevel.GetTopLevel(child) : null;
        if (root is null) return;
        ClearProbeForRoot(root); if (!ReferenceEquals(root, _topLevel)) DetachProbeRoot(root);
        _probePopupRoots.Remove(popup);
    }
}
