using XuanYu.Core.Math;
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
        frame = GroundTerrainFrame(frame, resources.ToArray());
        ApplyCameraFrame(frame, "FrameTerrain");
        _viewportCameraFramed = true;
        PublishSceneRenderSnapshot();
    }

    CameraFrameResult GroundTerrainFrame(CameraFrameResult frame,
        IReadOnlyList<TerrainRenderResource> resources)
    {
        var surface = new TerrainRenderGroundSurface(resources);
        var ground = surface.QuerySurface(new(frame.ObservationCenter.X, frame.ObservationCenter.Y));
        if (!ground.IsValid) return frame;
        var center = new Vector3d(frame.ObservationCenter.X, frame.ObservationCenter.Y,
            ground.SurfaceZ * VerticalExaggeration);
        var delta = center.Z - frame.ObservationCenter.Z;
        var camera = frame.Camera;
        var position = new Vector3d(camera.Position.X, camera.Position.Y,
            camera.Position.Z + delta);
        return new(new(position, camera.Forward, camera.Up, camera.VerticalFovDegrees,
            camera.NearPlane, camera.FarPlane, camera.Revision, camera.Mode,
            camera.OrthographicScale), center);
    }
}
