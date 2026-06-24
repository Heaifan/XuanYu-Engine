using Avalonia.Threading;
using FluidWarfare.Editor.Windows.Viewport.Scene3D.Lifecycle;

namespace FluidWarfare.Editor.Windows.Shell.Lifecycle;

/// <summary>
/// 管理 EditorShell 从 VisualTree 分离后的清理时序。
/// 停止 Scene3D 会话、清理 Resize 计时器、通知 Shell 恢复状态标志。
/// </summary>
public sealed class EditorShellDetachRoute
{
    public EditorShellDetachResult Detach(EditorShellDetachRequest request)
    {
        var sessionStopped = TryStopSession(request.Lifecycle);
        var timerCleanedUp = TryStopTimer(request.ResizeRenderTimer, request.ResizeRenderTimerTickHandler);
        return new(sessionStopped, timerCleanedUp);
    }

    private static bool TryStopSession(Scene3dSessionLifecycle lifecycle)
    {
        if (lifecycle is null) return false;
        lifecycle.Stop();
        return true;
    }

    private static bool TryStopTimer(
        DispatcherTimer? timer,
        EventHandler? tickHandler)
    {
        if (timer is null || tickHandler is null) return false;
        timer.Stop();
        timer.Tick -= tickHandler;
        return true;
    }
}

/// <summary>Shell → DetachRoute 的请求。</summary>
public sealed record EditorShellDetachRequest(
    Scene3dSessionLifecycle Lifecycle,
    DispatcherTimer? ResizeRenderTimer,
    EventHandler? ResizeRenderTimerTickHandler);
