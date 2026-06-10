using System.Globalization;

namespace FluidWarfare.Core.Math;

public readonly record struct Vector3d
{
    public Vector3d(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public double X { get; }

    public double Y { get; }

    public double Z { get; }

    public double Length => global::System.Math.Sqrt(LengthSquared);

    public double LengthSquared => (X * X) + (Y * Y) + (Z * Z);

    public bool IsZero => X == 0.0 && Y == 0.0 && Z == 0.0;

    public static Vector3d Zero { get; } = new(0.0, 0.0, 0.0);

    public static Vector3d UnitX { get; } = new(1.0, 0.0, 0.0);

    public static Vector3d UnitY { get; } = new(0.0, 1.0, 0.0);

    public static Vector3d UnitZ { get; } = new(0.0, 0.0, 1.0);

    public double DistanceTo(Vector3d other) => (this - other).Length;

    public double DistanceSquaredTo(Vector3d other) => (this - other).LengthSquared;

    public Vector3d Normalize() => IsZero ? Zero : this / Length;

    public double Dot(Vector3d other) => (X * other.X) + (Y * other.Y) + (Z * other.Z);

    public static Vector3d operator +(Vector3d left, Vector3d right) => new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    public static Vector3d operator -(Vector3d left, Vector3d right) => new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    public static Vector3d operator -(Vector3d value) => new(-value.X, -value.Y, -value.Z);

    public static Vector3d operator *(Vector3d vector, double scalar) => new(vector.X * scalar, vector.Y * scalar, vector.Z * scalar);

    public static Vector3d operator *(double scalar, Vector3d vector) => vector * scalar;

    public static Vector3d operator /(Vector3d vector, double scalar)
    {
        if (scalar == 0.0)
        {
            throw new DivideByZeroException("Vector3d cannot be divided by zero.");
        }

        return new Vector3d(vector.X / scalar, vector.Y / scalar, vector.Z / scalar);
    }

    public override string ToString()
    {
        return string.Create(
            CultureInfo.InvariantCulture,
            $"Vector3d({X:0.###}, {Y:0.###}, {Z:0.###})");
    }
}
