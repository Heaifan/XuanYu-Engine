using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Spatial;
using XuanYu.Editor.Assets;
using XuanYu.Render.Abstractions;
using XuanYu.World;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    SpatialAabb[] BuildSceneFarBounds()
    {
        var snapshot = RenderSnapshot;
        var result = new List<SpatialAabb>();
        foreach (var entity in snapshot.Entities)
        {
            if (entity.Type == WorldEntityTypes.StaticModel &&
                _staticModelCatalog.TryGetByEntity(entity.EntityKey, out var binding) &&
                _staticModelResources.TryGetValue(binding.AssetId, out var model))
                result.Add(Transform(model.LocalBounds, snapshot.TransformFor(entity)));
            else if (entity.Type != WorldEntityTypes.StaticModel)
                result.Add(Transform(new(new(-0.5, -0.5, -0.5), new(0.5, 0.5, 0.5)),
                    snapshot.TransformFor(entity)));
        }
        return result.ToArray();
    }

    static SpatialAabb Transform(SpatialAabb local, CommittedTransform transform)
    {
        var min = new Vector3d(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
        var max = new Vector3d(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
        for (var bits = 0; bits < 8; bits++)
        {
            var point = new Vector3d((bits & 1) == 0 ? local.Min.X : local.Max.X,
                (bits & 2) == 0 ? local.Min.Y : local.Max.Y,
                (bits & 4) == 0 ? local.Min.Z : local.Max.Z);
            var world = transform.Position + Rotate(new(point.X * transform.Scale.X,
                point.Y * transform.Scale.Y, point.Z * transform.Scale.Z), transform.Rotation);
            min = new(Math.Min(min.X, world.X), Math.Min(min.Y, world.Y), Math.Min(min.Z, world.Z));
            max = new(Math.Max(max.X, world.X), Math.Max(max.Y, world.Y), Math.Max(max.Z, world.Z));
        }
        return new(min, max);
    }

    static Vector3d Rotate(Vector3d value, Vector3d degrees)
    {
        var rx = Math.PI * degrees.X / 180; var ry = Math.PI * degrees.Y / 180;
        var rz = Math.PI * degrees.Z / 180;
        var y = value.Y * Math.Cos(rx) - value.Z * Math.Sin(rx);
        var z = value.Y * Math.Sin(rx) + value.Z * Math.Cos(rx);
        var x = value.X * Math.Cos(ry) + z * Math.Sin(ry);
        z = -value.X * Math.Sin(ry) + z * Math.Cos(ry);
        return new(x * Math.Cos(rz) - y * Math.Sin(rz), x * Math.Sin(rz) + y * Math.Cos(rz), z);
    }
}
