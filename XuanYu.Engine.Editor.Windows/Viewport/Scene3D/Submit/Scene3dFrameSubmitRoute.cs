using XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Frame;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Application;
using XuanYu.Engine.Editor.Windows.Viewport.Transform.Presentation;
using XuanYu.Engine.Render.Selection.Presented;
using XuanYu.Engine.Render.Vulkan.Scene3D.Session;

namespace XuanYu.Engine.Editor.Windows.Viewport.Scene3D.Submit;

/// <summary>
/// Scene3D 帧提交流程编排。组装 Gizmo + PickSnapshot → 调用 Scene3dFrameRoute.Request。
/// 唯一被允许调用 SetMoveGizmoVertices 并组合 PresentedScenePickSnapshotBuilder 的模块。
/// </summary>
public sealed class Scene3dFrameSubmitRoute
{
    readonly Scene3dFrameRoute _route;
    readonly VulkanScene3dSession _session;
    readonly ViewportRenderSceneStore _renderSceneStore;
    int _renderSeq;

    public int RenderSeq => _renderSeq;

    public Scene3dFrameSubmitRoute(
        Scene3dFrameRoute route,
        VulkanScene3dSession session,
        ViewportRenderSceneStore renderSceneStore)
    {
        _route = route; _session = session; _renderSceneStore = renderSceneStore;
    }

    public void Request(Scene3dFrameSubmitInput input, Action? onCompleted = null)
    {
        // Gizmo 顶点
        var gizmoInput = new MoveGizmoFrameInput(
            input.MoveToolActive, input.SelectedEntityId, input.EntityPosition,
            _session.LastPresentedSnapshot, input.HoveredElement, input.SelectionRevision);
        var gizmoResult = Scene3dGizmoSubmitSource.Build(gizmoInput, _session);

        // Pick Snapshot
        // Preview 帧不重建 PickSnapshot（拖动中不需要重新 Pick，复用上一次快照）
        var presented = _route.Snapshots.PresentedGizmo;
        var pick = input.Reason == VulkanScene3dFrameReason.TransformPreview
            ? PresentedScenePickSnapshot.None
            : Scene3dPickSnapshotSource.Build(
                _renderSceneStore.Current, input.RenderSeq, input.CameraRevision, presented);

        // Frame 请求
        _route.Request(input.Reason, input.CameraState, input.CameraRevision,
            _renderSceneStore.Current, gizmoResult.PendingSnapshot, pick, () =>
        {
            _renderSeq = _route.RenderSeq;
            onCompleted?.Invoke();
        });
    }
}
