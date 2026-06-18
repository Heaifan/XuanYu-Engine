using Avalonia.Threading;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo;
using FluidWarfare.Editor.Windows.Viewport.Transform.Gizmo.Visual;
using FluidWarfare.Editor.Windows.Viewport.Transform.Input;
using FluidWarfare.Render.Camera;
using FluidWarfare.Render.Scene;
using FluidWarfare.Render.Scene.Position;
using FluidWarfare.Render.Selection;
using FluidWarfare.Render.Selection.Presented;
using FluidWarfare.Render.Vulkan.Scene3D;
using FluidWarfare.Render.Vulkan.Scene3D.Overlay;
using FluidWarfare.Render.Vulkan.Scene3D.Session;

namespace FluidWarfare.Editor.Windows.Viewport.Scene3D;

/// <summary>
/// Scene3D 帧表现层路由。
/// 管理帧调度、UnitDrawList 构建、Gizmo 顶点提交和 Presented Snapshot 发布。
/// EditorShell 只调用 ScheduleFrame()，不再直接操作帧队列。
/// </summary>
public sealed class Scene3dPresentation
{
    private readonly VulkanScene3dSession _session;
    private readonly Scene3dPresentationState _state = new();
    private PresentedMoveGizmoSnapshot _pendingGizmo = PresentedMoveGizmoSnapshot.None;
    private PresentedScenePickSnapshot _pendingPickSnapshot = PresentedScenePickSnapshot.None;

    public PresentedMoveGizmoSnapshot PresentedGizmo { get; private set; } = PresentedMoveGizmoSnapshot.None;
    public PresentedScenePickSnapshot PresentedPickSnapshot { get; private set; } = PresentedScenePickSnapshot.None;
    public int RenderSeq => _state.RenderSeq;
    public string RenderLastMode => _state.RenderLastMode;

    public Scene3dPresentation(VulkanScene3dSession session) => _session = session;

    /// <summary>请求一帧。通过 _framePending 合并连续请求。</summary>
    public void ScheduleFrame(
        VulkanScene3dFrameReason reason,
        SceneOrbitCameraState cameraState,
        RenderScene renderScene,
        Func<RenderScene> buildUnitDrawScene,
        Func<PresentedMoveGizmoSnapshot> buildGizmo,
        Func<PresentedScenePickSnapshot> buildPickSnapshot)
    {
        if (_state.FramePending) return;
        _state.FramePending = true;

        Dispatcher.UIThread.Post(() =>
        {
            _state.FramePending = false;

            var unitDraws = BuildUnitDrawList(renderScene);
            var rev = _state.NextCameraRevision();
            var sessionPose = SceneCameraPose.FromOrbitState(cameraState, rev);

            // 构建 Gizmo + Pick Snapshot
            _pendingGizmo = buildGizmo();
            _pendingPickSnapshot = buildPickSnapshot();

            var result = _session.RenderFrame(reason, sessionPose, [.. unitDraws]);

            if (result.Success)
            {
                // Present 成功 → 提升快照
                if (_pendingGizmo.IsAvailable)
                    PresentedGizmo = _pendingGizmo;
                if (_pendingPickSnapshot.IsValid)
                    PresentedPickSnapshot = _pendingPickSnapshot;

                _state.OnFrameRendered();
            }
        });
    }

    /// <summary>清除 Gizmo 顶点和 Pending 快照。</summary>
    public void ClearGizmo()
    {
        _session.SetMoveGizmoVertices(null);
        _pendingGizmo = PresentedMoveGizmoSnapshot.None;
        PresentedGizmo = PresentedMoveGizmoSnapshot.None;
    }

    /// <summary>从 RenderScene 构建单位绘制列表。</summary>
    public static List<VulkanScene3dUnitDrawInfo> BuildUnitDrawList(RenderScene scene)
    {
        var list = new List<VulkanScene3dUnitDrawInfo>();
        foreach (var obj in scene.Objects)
        {
            if (obj.VisualKind != RenderObjectVisualKind.UnitMarker) continue;
            if (obj.Placement is not { } p) continue;
            list.Add(new VulkanScene3dUnitDrawInfo(
                obj.EntityId.Value.ToString(),
                (float)p.VisualCenter.X,
                (float)p.VisualCenter.Y,
                (float)p.VisualCenter.Z,
                (float)RenderUnitPlacement.Scale));
        }
        return list;
    }
}

/// <summary>Scene3dPresentation 的帧调度状态。</summary>
internal sealed class Scene3dPresentationState
{
    public bool FramePending;
    public int RenderSeq;
    public int CameraRevision;
    public string RenderLastMode = "无";

    public int NextCameraRevision() => ++CameraRevision;
    public void OnFrameRendered() { RenderSeq++; RenderLastMode = "Scene3D"; }
}
