using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Editor.Transform;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    ScaleGizmoDrag? _scaleDrag;

    public bool TryBeginScaleGizmoCapture(
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
        if (!EditorTransformCapturePolicy.CanBeginScaleGizmo(_editorState.ToolSnapshot))
        {
            LogTransformCaptureRejected(sessionTool, EditorDisplayText.Entity(entity.EntityKey));
            return false;
        }

        var state = ViewProjectionState.Create(CurrentCamera(viewport.Revision), viewport);
        _lastViewport = viewport;
        var worldAxisLength = ComputeScaleGizmoWorldAxisLength(entity.Transform.Position);
        var layout = ScaleGizmoLayout.Project(
            state, entity.Transform.Position, worldAxisLength, entity.Transform.Rotation);
        var handle = ScaleGizmoHitTester.HitTest(layout, x, y);
        if (handle is null) return false;

        var pointer = new EditorInteractionPointerSnapshot(pointerId, x, y, x, y, 0);
        var start = $"实体={EditorDisplayText.Entity(entity.EntityKey)}；手柄={handle}";
        var result = _editorState.Begin(new BeginInteractionCommand(sessionTool, start, pointer));
        if (result is null) return false;
        if (!_transformSession.BeginScale(result.Snapshot.SessionId, entity, handle.Value))
        {
            _editorState.Cancel(new CancelInteractionCommand(
                result.Snapshot.SessionId, sessionTool, "Transform Session 启动失败"));
            return false;
        }

        // 轴向屏幕归一化方向（Uniform 未使用）；CPU 命中层与绘制层共用 worldAxisLength + entityRotation。
        ScreenPoint axisDir = default;
        if (handle.Value != ScaleGizmoHandle.Uniform)
        {
            var i = handle.Value == ScaleGizmoHandle.X ? 0
                : (handle.Value == ScaleGizmoHandle.Y ? 1 : 2);
            var dx = layout.AxisEnd[i].X - layout.Center.X;
            var dy = layout.AxisEnd[i].Y - layout.Center.Y;
            var len = System.Math.Sqrt(dx * dx + dy * dy);
            if (len > 1e-6) axisDir = new ScreenPoint(dx / len, dy / len);
        }
        _scaleDrag = new ScaleGizmoDrag(
            handle.Value, entity.Transform.Scale, x, y, axisDir);
        FooterState = "状态：捕获中";
        FooterMessage = $"缩放手柄 {handle} 已捕获";
        var detail = $"实体={EditorDisplayText.Entity(entity.EntityKey)}；手柄={handle}";
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "缩放开始捕获",
            $"Entity={EditorDisplayText.Entity(entity.EntityKey)} Handle={handle}");
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Capture,
            "缩放工具会话开始",
            $"{detail}；会话={result.Snapshot.SessionId}");
        LogTransformCaptureBegin(
            sessionTool,
            result.Snapshot.OwnerTool,
            EditorDisplayText.Entity(entity.EntityKey),
            handle.Value);
        RefreshLogBindings();
        RaiseInteractionChanged();
        return true;
    }

    bool PreviewScaleGizmo(long sessionId, double x, double y)
    {
        if (!_transformSession.IsActive || _scaleDrag is not { } drag) return false;
        var scale = drag.Solve(x, y);
        if (!_transformSession.TryPreviewScale(sessionId, scale)) return false;
        _scaleDrag = drag;
        PublishSceneRenderSnapshot();
        return true;
    }

    // Scale Gizmo 屏幕空间恒定轴长：与 Rotate 同思路，按相机深度与视口逻辑高度换算世界半径，
    // 使 CPU 命中布局与 Vulkan 绘制使用同一 worldAxisLength。
    double ComputeScaleGizmoWorldAxisLength(Vector3d origin)
    {
        if (_lastViewport is not { } vp) return 1.2;
        return ScaleGizmoScreenSize.ComputeWorldAxisLength(_camera, vp, origin);
    }
}
