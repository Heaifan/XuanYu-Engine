using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    void Reconcile()
    {
        CloseAll();
        if (_vm is not null && !_vm.IsDiagnosticMode)
        {
            ClearProbeVisuals();
            _probeResult = null;
            _lockedProbeResult = null;
            return;
        }
        if (_vm?.IsDiagnosticMode != true) ClearProbeVisuals();
        if (_vm?.IsDiagnosticRegionBoundsMode != true) return;
        foreach (var target in DiagnosticRegistry.Targets.Values)
        {
            if (!target.IsEffectivelyVisible || TopLevel.GetTopLevel(target) is null) continue;
            var popup = new Popup
            {
                PlacementTarget = target,
                Placement = PlacementMode.TopEdgeAlignedLeft,
                IsLightDismissEnabled = false,
                TakesFocusFromNativeControl = false,
                ShouldUseOverlayLayer = true,
                Child = new DiagnosticBadge(target, _clipboard),
            };
            PopupOwner.Children.Add(popup);
            _popups.Add(popup);
            popup.IsOpen = true;
        }
        RenderTargetBounds();
    }

    void CloseAll()
    {
        foreach (var popup in _popups) popup.IsOpen = false;
        PopupOwner.Children.Clear();
        _popups.Clear();
        ClearTargetBounds();
    }
}
