using FluidWarfare.Editor.Windows.Shell.Scene3D.Commands;

namespace FluidWarfare.Editor.Windows.Shell.Scene3D;

/// <summary>Scene3D 命令路由。负责 Session 启动/重启命令薄转发。</summary>
sealed class EditorShellScene3dCommandRoute(
    EditorScene3dCommandRoute scene3dCommandRoute,
    Action stopRedrawTimer,
    Func<EditorScene3dCommandRequest> buildRequest,
    Action<EditorScene3dCommandResult> applyResult)
{
    public void HandleRunRequested()
    {
        stopRedrawTimer();
        applyResult(scene3dCommandRoute.Execute(buildRequest()));
    }

    public void HandleRestart()
    {
        applyResult(scene3dCommandRoute.Execute(buildRequest()));
    }
}
