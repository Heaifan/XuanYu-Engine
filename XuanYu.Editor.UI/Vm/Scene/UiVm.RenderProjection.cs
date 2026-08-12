using XuanYu.Core.Map;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    RenderProjectionResult CreateRenderProjection(SceneRenderSnapshot snapshot)
    {
        if (_lastViewport is { } viewport && !ViewProjectionState.TryCreate(_camera, viewport, out _))
            return RenderProjectionResult.Fail("相机投影超出当前单精度渲染表示范围。");
        var transform = snapshot.RenderTransform;
        var vectorOverlay = MapRegionRenderProjection.Build(MapSession.CurrentMap, _regionDrawing, _roadDrawing);
        IReadOnlyList<RenderVectorOverlayResource> overlays =
            vectorOverlay.Primitives.Count == 0 ? [] : [vectorOverlay];
        return SceneRenderProjectionAdapter.TryCreate(
            snapshot,
            ComputeRotateGizmoWorldRadius(transform.Position),
            ComputeScaleGizmoWorldAxisLength(transform.Position),
            snapshot.ShowScaleGizmo ? default : transform.Rotation,
            ViewportAssistState,
            ComputeMoveGizmoWorldAxisLength(transform.Position),
            _staticModelCatalog,
            _staticModelResources,
            _mapRenderSnapshot,
            _viewportDpiScale,
            overlays,
            new ScaleIndicatorOverlayProjection(
                IsScaleIndicatorVisible, ScaleIndicatorText, ScaleIndicatorWidthDip));
    }
}
