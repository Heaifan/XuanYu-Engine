using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Spatial;
using XuanYu.Core.Transform;
using XuanYu.Render.Abstractions;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    IReadOnlyList<TerrainRenderResource>? _farBoundsResources;
    double _farBoundsExaggeration;
    SpatialAabb[] _farBounds = [];
    long _farPolicySceneRevision = -1;
    long _farPolicyModelRevision = -1;
    PreviewTransform? _farPolicyPreview;
    IReadOnlyList<TerrainRenderResource>? _farPolicyResources;
    double _farPolicyExaggeration = double.NaN;

    CameraState RefreshFarPlanePolicyForGeometryChange(CameraState camera)
    {
        var resources = CachedTerrainRenderResources();
        var preview = RenderSnapshot.PreviewTransform;
        if (_farPolicySceneRevision == _sceneState.SpatialRevision &&
            _farPolicyModelRevision == _staticModelCatalog.Revision &&
            _farPolicyPreview == preview && ReferenceEquals(_farPolicyResources, resources) &&
            _farPolicyExaggeration == VerticalExaggeration) return camera;
        return ApplyFarPlanePolicy(camera);
    }

    CameraState ApplyFarPlanePolicy(CameraState camera)
    {
        var resources = CachedTerrainRenderResources();
        if (resources is not { Count: > 0 })
        {
            _farBounds = [];
            _farBoundsResources = null;
        }
        else if (!ReferenceEquals(resources, _farBoundsResources) ||
                 _farBoundsExaggeration != VerticalExaggeration)
        {
            _farBounds = BuildTerrainFarBounds(resources);
            _farBoundsResources = resources;
            _farBoundsExaggeration = VerticalExaggeration;
        }
        var sceneBounds = BuildSceneFarBounds();
        var far = TerrainFarPlanePolicy.Resolve(camera, CurrentViewport, _farBounds, sceneBounds);
        _farPolicySceneRevision = _sceneState.SpatialRevision;
        _farPolicyModelRevision = _staticModelCatalog.Revision;
        _farPolicyPreview = RenderSnapshot.PreviewTransform;
        _farPolicyResources = resources;
        _farPolicyExaggeration = VerticalExaggeration;
        return camera.WithFarPlane(far);
    }

    SpatialAabb[] BuildTerrainFarBounds(IReadOnlyList<TerrainRenderResource> resources)
    {
        var result = new List<SpatialAabb>();
        foreach (var resource in resources)
        {
            if (!resource.IsAvailable) continue;
            foreach (var chunk in TerrainChunkPartitioner.Partition(resource.Heightfield,
                         resource.TerrainId, resource.Revision))
            {
                var transform = new TerrainRenderTransform(VerticalExaggeration);
                var skirt = Math.Max(1, Math.Max(resource.Heightfield.CellSizeMeters,
                    resource.Heightfield.CellSizeYMeters) * VerticalExaggeration);
                var b = chunk.WorldBounds.WithVerticalExaggeration(transform, skirt);
                var origin = resource.WorldOrigin;
                result.Add(new(new(origin.X + b.MinX, origin.Y + b.MinY, b.MinZ),
                    new(origin.X + b.MaxX, origin.Y + b.MaxY, b.MaxZ)));
            }
        }
        return result.ToArray();
    }
}
