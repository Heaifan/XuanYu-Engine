using Avalonia.Threading;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;
using FluidWarfare.Render.Camera;
using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Selection.Presented;
using FluidWarfare.Render.Vulkan.Scene3D.Session;

namespace FluidWarfare.Editor.Windows.Viewport.Scene3D.Frame;

/// <summary>
/// Scene3D 帧路径路由。单一职责：请求、合并、执行帧。
/// 依赖注入：Session + CameraRevision 提供者 + Snapshot 构建。
/// 不接收 EditorShell 引用，不持有 Func 参数。
/// </summary>
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

    /// <summary>
    /// 请求一帧。framePending 闸门合并连续请求。
    /// cameraRevision 由调用者提供（仅相机真实变化时递增），此方法不修改它。
    /// </summary>
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

        Dispatcher.UIThread.Post(() =>
        {
            _state.Release();

            if (_state.IsDisposed || generation != _state.SessionGeneration)
                return;

            var unitDraws = Scene3dDrawListBuilder.Build(renderScene);
            var sessionPose = SceneCameraPose.FromOrbitState(cameraState, cameraRevision);

            // Pending Snapshot 来自调用者（不在此处构建）
            _snapshots.SetPendingGizmo(gizmoSnapshot);
            _snapshots.SetPendingPick(pickSnapshot);

            var result = _session.RenderFrame(reason, sessionPose, [.. unitDraws]);

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
