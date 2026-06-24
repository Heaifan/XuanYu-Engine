using Avalonia.Threading;

namespace FluidWarfare.Editor.Windows.Shell.Lifecycle;

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

        Dispatcher.UIThread.Post(() =>
        {
            request.NativeHostReportAction();
            request.InputPipelineInitAction();
        });

        return new(true);
    }

    /// <summary>重置已分发标志，供分离恢复后重新附加时使用。</summary>
    public void Reset() => _dispatched = false;
}
