using Avalonia.Threading;

using XuanYu.Engine.Editor.Windows.Shell.Diagnostics;

namespace XuanYu.Engine.Editor.Windows.Shell.Lifecycle;

/// <summary>
/// 管理 EditorShell 首次附加到 VisualTree 后的初始化时序。
/// 通过 Dispatcher.UIThread.Post 延迟执行，确保控件的 VisualTree 已就绪。
/// </summary>
public sealed class EditorShellAttachRoute
{
    private bool _dispatched;

    public EditorShellAttachResult Attach(EditorShellAttachRequest request)
    {
        if (_dispatched) return new(false);
        _dispatched = true;

        GizmoDragProbe.Log("Dispatcher.UIThread.Post 入队(Attach)");
        Dispatcher.UIThread.Post(() =>
        {
            GizmoDragProbe.Log("Dispatcher.UIThread.Post 执行(Attach)");
            request.NativeHostReportAction();
            request.InputPipelineInitAction();
        });

        return new(true);
    }

    /// <summary>重置已分发标志，供分离恢复后重新附加时使用。</summary>
    public void Reset() => _dispatched = false;
}
