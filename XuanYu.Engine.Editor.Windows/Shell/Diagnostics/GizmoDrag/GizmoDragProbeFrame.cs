using System.Diagnostics;

namespace XuanYu.Engine.Editor.Windows.Shell.Diagnostics;

/// <summary>单帧审计上下文。记录从 PointerMoved 到 Render 的完整生命周期。</summary>
public sealed class GizmoDragProbeFrame : IDisposable
{
    readonly long _startTicks;
    readonly string _trigger;
    long _lastLogTicks;

    public bool UiRefreshed { get; private set; }
    public bool WorldStateWritten { get; private set; }
    public bool DiagnosticsRefreshed { get; private set; }
    public bool InspectorRefreshed { get; private set; }

    public GizmoDragProbeFrame(string trigger)
    {
        _trigger = trigger;
        _startTicks = Stopwatch.GetTimestamp();
        _lastLogTicks = _startTicks;
    }

    public void MarkUiRefreshed() => UiRefreshed = true;
    public void MarkWorldStateWritten() => WorldStateWritten = true;
    public void MarkDiagnosticsRefreshed() => DiagnosticsRefreshed = true;
    public void MarkInspectorRefreshed() => InspectorRefreshed = true;

    public void Log(string stage, double? elapsedMs = null)
    {
        var now = Stopwatch.GetTimestamp();
        var ms = elapsedMs ?? (now - _lastLogTicks) * 1000.0 / Stopwatch.Frequency;
        _lastLogTicks = now;
        var totalMs = (now - _startTicks) * 1000.0 / Stopwatch.Frequency;
        EditorProbe.Write(
            "Gizmo拖动",
            stage,
            $"触发={_trigger} 本阶段={ms:F3}ms 累计={totalMs:F3}ms " +
            $"UI={(UiRefreshed ? "是" : "否")} " +
            $"WorldState={(WorldStateWritten ? "是" : "否")} " +
            $"Diagnostics={(DiagnosticsRefreshed ? "是" : "否")} " +
            $"Inspector={(InspectorRefreshed ? "是" : "否")}");
    }

    public void Dispose() => GizmoDragProbe.EndFrame();
}
