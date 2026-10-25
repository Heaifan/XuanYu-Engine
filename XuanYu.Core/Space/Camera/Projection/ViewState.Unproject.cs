using System.Numerics;
using XuanYu.Core.Math;

namespace XuanYu.Core.Space;
public sealed partial class ViewProjectionState
{
    public Vector3d ToWorld(double x, double y, double z) => TransformPointToWorld(x, y, z);
    public Vector3d TransformPointToWorld(double ndcX, double ndcY, double ndcZ)
    {
        if (!double.IsFinite(ndcX) || !double.IsFinite(ndcY) || !double.IsFinite(ndcZ))
            throw new ArgumentOutOfRangeException(nameof(ndcX));
        var clip = new Vector4((float)ndcX, (float)ndcY, (float)ndcZ, 1.0f);
        var world = Vector4.Transform(clip, InverseViewProjection);
        if (!float.IsFinite(world.W) || world.W == 0 ||
            !float.IsFinite(world.X) || !float.IsFinite(world.Y) || !float.IsFinite(world.Z))
            throw new InvalidOperationException("反投影坐标无效。");
        var p = RenderOrigin + new Vector3d((double)world.X / world.W,
            (double)world.Y / world.W, (double)world.Z / world.W);
        if (!double.IsFinite(p.X) || !double.IsFinite(p.Y) || !double.IsFinite(p.Z))
            throw new InvalidOperationException("反投影坐标非有限值。");
        return p;
    }
}
