using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using XuanYu.Editor.UI.Diagnostic;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    const string ProbeRevision = "eaaac921";
    const string ProbeBranch = "integration/diag-final-fix13-3-coverage";

    void LogProbeHit(Visual hit, DiagnosticProbeResult result)
    {
        if (!ProbeEnabled) return;
        Console.WriteLine($"[DIAG-HIT] Hit={hit.GetType().FullName}; Selected={result.ControlType}; " +
            $"Name={result.Name}; Text={result.Text}; DebugId={result.DebugId}; Mode={result.ProbeMode}");
        foreach (var (visual, rank) in DiagnosticProbeResolver.TraceCandidates(hit))
            Console.WriteLine($"[DIAG-HIT] Candidate={visual.GetType().FullName}; Rank={rank}; " +
                $"Name={(visual as Control)?.Name ?? "N/A"}; DebugId={DebugId(visual)}");
    }

    void LogProbeState(string phase, DiagnosticProbeResult? result,
        DiagnosticPlacementTargetKind? targetKind = null)
    {
        if (!ProbeEnabled) return;
        var owner = _topLevel as Window;
        var handle = _toolWindow?.TryGetPlatformHandle()?.Handle ?? nint.Zero;
        Console.WriteLine($"[DIAG-STATE] Phase={phase}; Locked={IsProbeLocked}; " +
            $"Target={TrackedSnapshot?.TargetDisplayName ?? "N/A"}; Bounds={LastKnownBounds}; " +
            $"Probe={result?.ControlType ?? "N/A"}; NativeHost={_nativeViewportHost is not null}; " +
            $"PlacementMode={_cardPlacementMode}; TargetKind={targetKind?.ToString() ?? "N/A"}; " +
            $"Pointer={_lastProbePointer}; ToolExists={_toolWindow is not null}; " +
            $"ToolVisible={_toolWindow?.IsVisible == true}; Position={_toolWindow?.Position}; " +
            $"Handle=0x{handle.ToInt64():X}; OwnerActive={owner?.IsActive}; OwnerState={owner?.WindowState}");
    }

    void LogProbeVersion() => Console.WriteLine(
        $"[DIAG-VERSION] Revision={ProbeRevision}; Branch={ProbeBranch}");

    static string DebugId(Visual visual) => visual is Control control
        ? XYDiagnostic.GetDebugId(control) ?? "N/A" : "N/A";
}
