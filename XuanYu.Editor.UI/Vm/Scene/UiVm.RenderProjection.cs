using XuanYu.Core.Map;
using XuanYu.Core.Scene;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;
using XuanYu.World.Terrain;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    RenderProjectionResult CreateRenderProjection(SceneRenderSnapshot snapshot)
    {
        if (_lastViewport is { } viewport && !ViewProjectionState.TryCreate(_camera, viewport, out _))
            return RenderProjectionResult.Fail("相机投影超出当前单精度渲染表示范围。");
        var transform = snapshot.RenderTransform;
        var vectorOverlay = MapRegionRenderProjection.Build(RegionFillColorPreviewMap(), _regionDrawing,
            _roadDrawing, MapGeometryPreview, _viewportDpiScale, _mapLabelBitmapCache);
        IReadOnlyList<RenderVectorOverlayResource> overlays =
            vectorOverlay.Primitives.Count == 0 ? [] : [vectorOverlay];
        var terrains = _terrainTiles?.Tiles.Select(tile =>
            TerrainWorldPlacement.ToRenderResource(tile, _terrainTiles.Bounds)).ToArray();
        var terrain = terrains?.FirstOrDefault() ?? TerrainWorld?.ToRenderSnapshot("terrain", 1);
        var map = IsTerrainContext && terrains is { Length: > 0 }
            ? MapRenderSnapshot.Empty : _mapRenderSnapshot;
        return SceneRenderProjectionAdapter.TryCreate(
            snapshot,
            ComputeRotateGizmoWorldRadius(transform.Position),
            ComputeScaleGizmoWorldAxisLength(transform.Position),
            snapshot.ShowScaleGizmo ? default : transform.Rotation,
            ViewportAssistState,
            ComputeMoveGizmoWorldAxisLength(transform.Position),
            _staticModelCatalog,
            _staticModelResources,
            map,
            _viewportDpiScale,
            overlays,
            new ScaleIndicatorOverlayProjection(
                IsScaleIndicatorVisible, ScaleIndicatorText, ScaleIndicatorWidthDip),
            terrain,
            new TerrainRenderTransform(VerticalExaggeration),
            terrains);
    }
}
