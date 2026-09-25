namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    void ClearProbeVisuals()
    {
        _nativeViewportHost = null;
        ClearProbeHighlight();
        HideToolWindow();
    }

    void ClearProbeHighlight()
    {
        if (_probeHighlight is null) return;
        ProbeOwner.Children.Remove(_probeHighlight);
        _probeHighlight = null;
    }
}
