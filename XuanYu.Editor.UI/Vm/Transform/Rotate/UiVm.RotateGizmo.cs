using XuanYu.Core.Gizmo;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Core.Math;
using XuanYu.Editor.Transform;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    RotateGizmoDrag? _rotateDrag;

    public bool TryBeginRotateGizmoCapture(
        long pointerId,
        double x,
        double y,
        ViewportState viewport,
        bool hostValid)
    {
        if (_cameraSession is not null) return false;
        var entity = _sceneState.RenderSnapshot.Entity;
        var sessionTool = ActiveTool;
        if (!entity.IsValid) return false;
        if (!hostValid || !HasSelection ||
            SelectionKey != entity.EntityKey.ToString()) return false;
        if (!EditorTransformCapturePolicy.CanBeginRotateGizmo(_editorState.ToolSnapshot))
        {
            LogTransformCaptureRejected(sessionTool, EditorDisplayText.Entity(entity.EntityKey));
            return false;
        }

        var state = ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        _lastViewport = viewport;
        var layout = RotateGizmoLayout.Project(
            state, entity.Transform.Position, ComputeRotateGizmoWorldRadius(entity.Transform.Position));
        var axis = layout.HitTest(x, y);
        if (axis is null) return false;

        var pointer = new EditorInteractionPointerSnapshot(pointerId, x, y, x, y, 0);
        var start = $"实体={EditorDisplayText.Entity(entity.EntityKey)}；轴={axis}";
        var result = _editorState.Begin(new BeginInteractionCommand(sessionTool, start, pointer));
        if (result is null) return false;
        if (!_transformSession.BeginRotate(result.Snapshot.SessionId, entity, axis.Value))
        {
            _editorState.Cancel(new CancelInteractionCommand(
                result.Snapshot.SessionId, sessionTool, "Transform Session 启动失败"));
            return false;
        }
        _rotateDrag = new RotateGizmoDrag(
            state, entity.Transform.Position, axis.Value, entity.Transform.Rotation, x, y);
        if (!_rotateDrag.TryInitialize(x, y))
        {
            _transformSession.TryCancel(result.Snapshot.SessionId);
            _editorState.Cancel(new CancelInteractionCommand(
                result.Snapshot.SessionId, sessionTool, "旋转射线与环无有效交点"));
            _rotateDrag = null;
            return false;
        }
        FooterState = "状态：捕获中";
        FooterMessage = $"旋转轴 {axis} 已捕获";
        var detail = $"实体={EditorDisplayText.Entity(entity.EntityKey)}；轴={axis}";
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "旋转工具会话开始",
            $"{detail}；会话={result.Snapshot.SessionId}");
        LogTransformCaptureBegin(
            sessionTool,
            result.Snapshot.OwnerTool,
            EditorDisplayText.Entity(entity.EntityKey),
            axis.Value);
        RefreshLogBindings();
        RaiseInteractionChanged();
        return true;
    }

    bool PreviewRotateGizmo(long sessionId, double x, double y)
    {
        if (!_transformSession.IsActive || _rotateDrag is not { } drag) return false;
        var rotation = drag.Solve(x, y);
        if (rotation is null) return false;
        if (!_transformSession.TryPreviewRotation(sessionId, rotation.Value)) return false;
        _rotateDrag = drag;
        PublishSceneRenderSnapshot();
        return true;
    }

    // 旋转环屏幕空间恒定尺寸：按当前相机深度与视口逻辑高度，将目标 DIP 半径换算为世界半径。
    // CPU 命中（RotateGizmoLayout）与 Shader 绘制（RenderProjection.RotateGizmoWorldRadius）共用同一值。
    double ComputeRotateGizmoWorldRadius(Vector3d origin)
    {
        if (_lastViewport is not { } vp) return RotateGizmoLayout.RingRadius;
        return RotateGizmoScreenRadius.ComputeWorldRadius(_camera, vp, origin);
    }
}
