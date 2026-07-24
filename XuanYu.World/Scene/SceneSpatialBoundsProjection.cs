using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Spatial;
using XuanYu.World.Spatial;

namespace XuanYu.World.Scene;

public static class SceneSpatialBoundsProjection
{
    public static SpatialBounds ToSpatialBounds(SceneEntitySnapshot entity)
    {
        var p = entity.Transform.Position;
        var min = new Vector3d(p.X - 0.5, p.Y - 0.5, p.Z - 0.5);
        var max = new Vector3d(p.X + 0.5, p.Y + 0.5, p.Z + 0.5);
        return new SpatialBounds(
            entity.EntityKey,
            new SpatialAabb(min, max),
            SpatialQueryCategory.SceneEntity);
    }
}
