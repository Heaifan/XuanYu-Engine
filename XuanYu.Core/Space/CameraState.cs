using XuanYu.Core.Math;

namespace XuanYu.Core.Space;

public readonly record struct CameraState
{
    const double MinFov = 1.0;
    const double MaxFov = 179.0;

    public CameraState(
        Vector3d position,
        Vector3d forward,
        Vector3d up,
        double verticalFovDegrees,
        double nearPlane,
        double farPlane,
        long revision)
    {
        ValidateFinite(position, nameof(position));
        ValidateFinite(forward, nameof(forward));
        ValidateFinite(up, nameof(up));
        if (forward.IsZero) throw new ArgumentOutOfRangeException(nameof(forward));
        if (up.IsZero) throw new ArgumentOutOfRangeException(nameof(up));
        if (Cross(forward, up).Length < 0.000001) throw new ArgumentOutOfRangeException(nameof(up));
        if (!double.IsFinite(verticalFovDegrees) || verticalFovDegrees <= MinFov || verticalFovDegrees >= MaxFov)
        {
            throw new ArgumentOutOfRangeException(nameof(verticalFovDegrees));
        }

        if (!double.IsFinite(nearPlane) || nearPlane <= 0.0) throw new ArgumentOutOfRangeException(nameof(nearPlane));
        if (!double.IsFinite(farPlane) || farPlane <= nearPlane) throw new ArgumentOutOfRangeException(nameof(farPlane));
        if (revision < 0) throw new ArgumentOutOfRangeException(nameof(revision));

        Position = position;
        Forward = forward.Normalize();
        Up = up.Normalize();
        VerticalFovDegrees = verticalFovDegrees;
        NearPlane = nearPlane;
        FarPlane = farPlane;
        Revision = revision;
    }

    public Vector3d Position { get; }

    public Vector3d Forward { get; }

    public Vector3d Up { get; }

    public double VerticalFovDegrees { get; }

    public double NearPlane { get; }

    public double FarPlane { get; }

    public long Revision { get; }

    static Vector3d Cross(Vector3d left, Vector3d right)
    {
        return new Vector3d(
            (left.Y * right.Z) - (left.Z * right.Y),
            (left.Z * right.X) - (left.X * right.Z),
            (left.X * right.Y) - (left.Y * right.X));
    }

    static void ValidateFinite(Vector3d vector, string name)
    {
        if (!double.IsFinite(vector.X) || !double.IsFinite(vector.Y) || !double.IsFinite(vector.Z))
        {
            throw new ArgumentOutOfRangeException(name);
        }
    }
}
