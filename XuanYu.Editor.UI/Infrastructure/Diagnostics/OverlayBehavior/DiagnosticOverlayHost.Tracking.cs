using Avalonia;
using Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    public DiagnosticElementSnapshot? TrackedSnapshot { get; private set; }
    public Rect? LastKnownBounds { get; private set; }

    void CaptureTrackedTarget(DiagnosticProbeResult result)
    {
        TrackedSnapshot = DiagnosticElementSnapshot.Capture(result);
        LastKnownBounds = TryGetFloatingBounds(result.DeepVisual, out var bounds) ? bounds : result.Bounds;
    }

    bool TryGetTrackedBounds(DiagnosticProbeResult result, out Rect bounds)
    {
        if (TopLevel.GetTopLevel(result.DeepVisual) is not null &&
            TryGetFloatingBounds(result.DeepVisual, out bounds))
        {
            if (IsProbeLocked) LastKnownBounds = bounds;
            return true;
        }
        if (IsProbeLocked && LastKnownBounds is { } known)
        {
            bounds = known;
            return true;
        }
        bounds = default;
        return false;
    }
}
