using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;
using XuanYu.Editor.MapEditing;
using XuanYu.World;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    CameraFrameResult ConstrainDollyToTerrain(CameraFrameResult result, double wheelDelta)
    {
        if (TerrainWorld is not { } world || result.Camera.Mode != ProjectionMode.Perspective) return result;
        var resources = CachedTerrainRenderResources();
        IGroundSurface surface = resources is { Count: > 0 }
            ? new TerrainRenderGroundSurface(resources, world.EditDelta.Revision)
            : new TerrainWorldGroundSurface(world);
        return CameraTerrainConstraint.Constrain(_camera, result, wheelDelta,
            VerticalExaggeration, surface, world.Metadata.ResolutionX, world.Metadata.ResolutionY);
    }
}
