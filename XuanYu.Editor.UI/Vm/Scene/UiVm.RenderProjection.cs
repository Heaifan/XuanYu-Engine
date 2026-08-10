using XuanYu.Core.Map;
using XuanYu.Core.Scene;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    RenderProjectionResult CreateRenderProjection(SceneRenderSnapshot snapshot)
    {
        var transform = snapshot.RenderTransform;
        var vectorOverlay = MapRegionRenderProjection.Build(MapSession.CurrentMap, _regionDrawing);
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
                IsScaleIndicatorVisible, ScaleIndicatorText, ScaleIndicatorWidthDip),
            _observationCenter);
    }
}
