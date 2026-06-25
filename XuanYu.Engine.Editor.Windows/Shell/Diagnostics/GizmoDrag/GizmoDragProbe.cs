namespace XuanYu.Engine.Editor.Windows.Shell.Diagnostics;

/// <summary>Gizmo 拖动审计探针。单帧内聚合 UI/WorldState/Diagnostics/Inspector 刷新标记。</summary>
public static class GizmoDragProbe
{
    static readonly AsyncLocal<GizmoDragProbeFrame?> _current = new();

    public static GizmoDragProbeFrame? Current => _current.Value;

    public static GizmoDragProbeFrame BeginFrame(string trigger)
    {
        var frame = new GizmoDragProbeFrame(trigger);
        _current.Value = frame;
        return frame;
    }

    public static void EndFrame()
    {
        var frame = _current.Value;
        if (frame is not null)
        {
            frame.Log("帧结束");
            _current.Value = null;
        }
    }

    public static void MarkUiRefreshed() => Current?.MarkUiRefreshed();
    public static void MarkWorldStateWritten() => Current?.MarkWorldStateWritten();
    public static void MarkDiagnosticsRefreshed() => Current?.MarkDiagnosticsRefreshed();
    public static void MarkInspectorRefreshed() => Current?.MarkInspectorRefreshed();

    public static void Log(string stage, double? elapsedMs = null)
    {
        var frame = _current.Value;
        if (frame is null) return;
        frame.Log(stage, elapsedMs);
    }
}
