using XuanYu.Core.Space;
using XuanYu.Editor.Camera;
using XuanYu.Render.Abstractions;
using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Source;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    partial void OnTerrainRuntimeEstablished(TerrainTileSet tiles) =>
        FrameTerrainResources(tiles.Tiles.Select(tile =>
            TerrainWorldPlacement.ToRenderResource(tile, tiles.Bounds)));

    void FrameTerrainResources(IEnumerable<TerrainRenderResource> resources)
    {
        var points = TerrainWorldBounds.Corners(resources,
            new TerrainRenderTransform(VerticalExaggeration));
        if (points.Count == 0) return;
        var frame = _camera.Mode == ProjectionMode.Orthographic
            ? EditorCameraFraming.FrameOrthographicWithCenter(points, _camera.Forward,
                _camera.Up, _viewportAspect, _camera.Position.DistanceTo(_observationCenter),
                ++_cameraRevision)
            : EditorCameraFraming.FrameAllWithCenter(points, _viewportAspect, ++_cameraRevision);
        ApplyCameraFrame(frame, "FrameTerrain");
        _viewportCameraFramed = true;
        PublishSceneRenderSnapshot();
    }
}
