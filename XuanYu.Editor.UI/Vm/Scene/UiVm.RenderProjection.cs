using XuanYu.Core.Map;
using XuanYu.Core.Scene;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    RenderProjectionResult CreateRenderProjection(SceneRenderSnapshot snapshot)
    {
        var transform = snapshot.RenderTransform;
        var regionModels = MapRegionRenderProjection.Build(MapSession.CurrentMap, _regionDrawing);
        if (F1ForensicTrace.IsNativeClick)
        {
            var draft = regionModels.FirstOrDefault(x => x.Key.Value == "map-region-draft");
            F1ForensicTrace.Projection(this, _regionDrawing.Draft?.Vertices.Length ?? 0,
                draft?.Indices.Count / 6 ?? 0, draft?.Primitives.Count ?? 0,
                MapSession.CurrentMap.Regions.Length, draft?.Key.Value ?? "none");
        }
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
            regionModels);
    }
}
