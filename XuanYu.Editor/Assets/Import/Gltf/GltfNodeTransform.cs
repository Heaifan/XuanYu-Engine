using System.Numerics;
using System.Text.Json;
using XuanYu.Core.Math;

namespace XuanYu.Editor.Assets;

static class GltfNodeTransform
{
    public static Matrix4x4 WorldMatrix(JsonElement node, Matrix4x4 parent)
    {
        return LocalMatrix(node) * parent;
    }

    public static Vector3d Position(Vector3 value, Matrix4x4 world)
    {
        var v = Vector3.Transform(value, world);
        return GltfCoordinatePolicy.ToXuanYuPosition(new Vector3d(v.X, v.Y, v.Z));
    }

    public static bool Normal(Vector3 value, Matrix4x4 world, out Vector3d normal)
    {
        normal = default;
        if (!Matrix4x4.Invert(world, out var inverse)) return false;
        var n = Vector3.TransformNormal(value, Matrix4x4.Transpose(inverse));
        if (n.LengthSquared() <= 0 || !float.IsFinite(n.X) || !float.IsFinite(n.Y) || !float.IsFinite(n.Z))
            return false;
        normal = GltfCoordinatePolicy.ToXuanYuNormal(new Vector3d(n.X, n.Y, n.Z));
        return true;
    }

    static Matrix4x4 LocalMatrix(JsonElement node)
    {
        var matrix = GltfJsonAccess.FloatArray(node, "matrix");
        if (matrix.Count == 16) return new Matrix4x4(
            matrix[0], matrix[1], matrix[2], matrix[3],
            matrix[4], matrix[5], matrix[6], matrix[7],
            matrix[8], matrix[9], matrix[10], matrix[11],
            matrix[12], matrix[13], matrix[14], matrix[15]);
        var t = GltfJsonAccess.FloatArray(node, "translation");
        var r = GltfJsonAccess.FloatArray(node, "rotation");
        var s = GltfJsonAccess.FloatArray(node, "scale");
        var scale = s.Count == 3 ? Matrix4x4.CreateScale(s[0], s[1], s[2]) : Matrix4x4.Identity;
        var rot = r.Count == 4 ? Matrix4x4.CreateFromQuaternion(new Quaternion(r[0], r[1], r[2], r[3])) : Matrix4x4.Identity;
        var trans = t.Count == 3 ? Matrix4x4.CreateTranslation(t[0], t[1], t[2]) : Matrix4x4.Identity;
        return scale * rot * trans;
    }
}
