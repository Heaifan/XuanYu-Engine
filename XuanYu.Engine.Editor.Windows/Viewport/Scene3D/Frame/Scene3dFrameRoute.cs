using Avalonia.Threading;
using XuanYu.Engine.Editor.Windows.Shell.Diagnostics;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo;
using XuanYu.Engine.Render.Camera;
using XuanYu.Engine.Render.Scene;
using XuanYu.Engine.Render.Selection.Presented;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;

namespace XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Frame;

/// <summary>Scene3D 帧路径路由。负责请求、合并、执行帧。</summary>
public sealed class Scene3dFrameRoute
{
    private readonly VulkanScene3dSession _session;
    private readonly Scene3dFrameState _state = new();
    private readonly Scene3dPresentedState _snapshots = new();

    public Scene3dFrameRoute(VulkanScene3dSession session)
    {
        _session = session;
        _state.NextGeneration();
    }

    public Scene3dPresentedState Snapshots => _snapshots;
    public int SessionGeneration => _state.SessionGeneration;
    public bool IsDisposed => _state.IsDisposed;
    public int RenderSeq => _state.RenderSeq;

    /// <summary>请求一帧。framePending 闸门合并连续请求。</summary>
    public void Request(
        VulkanScene3dFrameReason reason,
        SceneOrbitCameraState cameraState,
        int cameraRevision,
        RenderScene renderScene,
        PresentedMoveGizmoSnapshot gizmoSnapshot,
        PresentedScenePickSnapshot pickSnapshot,
        Action? onCompleted = null)
    {
        if (!_state.TryAcquire()) return;
        var generation = _state.SessionGeneration;

        GizmoDragProbe.Log("Dispatcher.UIThread.Post 入队");
        Dispatcher.UIThread.Post(() =>
        {
            GizmoDragProbe.Log("Dispatcher.UIThread.Post 执行");
            _state.Release();

            if (_state.IsDisposed || generation != _state.SessionGeneration)
                return;

            var unitDraws = Scene3dDrawListBuilder.Build(renderScene);
            var sessionPose = SceneCameraPose.FromOrbitState(cameraState, cameraRevision);

            // Pending Snapshot 来自调用者（不在此处构建）
            _snapshots.SetPendingGizmo(gizmoSnapshot);
            _snapshots.SetPendingPick(pickSnapshot);

            var result = _session.RenderFrame(reason, sessionPose, [.. unitDraws]);
            GizmoDragProbe.Log($"Preview Render 完成 success={result.Success}");

            if (result.Success)
            {
                _snapshots.OnPresentSuccess();
                _state.OnFrameRendered();
            }
            else
                _snapshots.OnPresentFailed();

            onCompleted?.Invoke();
        });
    }

    /// <summary>
    /// 清除 Gizmo Pending（不修改 Presented）。
    /// Presented 只在 Present 成功后清除。
    /// </summary>
    public void ClearGizmo()
    {
        _session.SetMoveGizmoVertices(null);
        _snapshots.ClearPendingGizmo();
    }

    /// <summary>释放路由。已排队的旧帧回调将被跳过。</summary>
    public void Dispose()
    {
        _state.Dispose();
    }

    /// <summary>路由重新上线（Session 重启后调用）。</summary>
    public void Reinitialize()
    {
        _state.NextGeneration();
        _snapshots.Reset();
    }
}
