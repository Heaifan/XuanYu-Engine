using XuanYu.Engine.Core.Math;

namespace FluidWarfare.Editor.Transform.Translation.Constraint;

public enum TranslationAxis
{
    X,
    Y,
    Z,
}

public static class TranslationAxisExtensions
{
    public static Vector3d ToWorldVector(this TranslationAxis axis, TransformOrientation orientation, Vector3d localRotation)
    {
        var world = axis switch
        {
            TranslationAxis.X => new Vector3d(1, 0, 0),
            TranslationAxis.Y => new Vector3d(0, 1, 0),
            TranslationAxis.Z => new Vector3d(0, 0, 1),
            _ => Vector3d.Zero,
        };
        return orientation == TransformOrientation.Local
            ? RotateByEuler(world, localRotation)
            : world;
    }

    internal static Vector3d RotateByEuler(Vector3d v, Vector3d euler)
    {
        var rad = euler * (System.Math.PI / 180.0);
        var cosX = System.Math.Cos(rad.X); var sinX = System.Math.Sin(rad.X);
        var cosY = System.Math.Cos(rad.Y); var sinY = System.Math.Sin(rad.Y);
        var cosZ = System.Math.Cos(rad.Z); var sinZ = System.Math.Sin(rad.Z);
        var x = v.X * (cosY * cosZ) + v.Y * (-cosX * sinZ + sinX * sinY * cosZ) + v.Z * (sinX * sinZ + cosX * sinY * cosZ);
        var y = v.X * (cosY * sinZ) + v.Y * (cosX * cosZ + sinX * sinY * sinZ) + v.Z * (-sinX * cosZ + cosX * sinY * sinZ);
        var z = v.X * (-sinY) + v.Y * (sinX * cosY) + v.Z * (cosX * cosY);
        return new Vector3d(x, y, z);
    }
}
