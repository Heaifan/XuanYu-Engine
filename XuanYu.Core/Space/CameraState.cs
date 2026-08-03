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
        long revision,
        ProjectionMode mode = ProjectionMode.Perspective,
        double orthographicScale = 0.0)
    {
        ValidateFinite(position, nameof(position));
        ValidateFinite(forward, nameof(forward));
        ValidateFinite(up, nameof(up));
        if (forward.IsZero) throw new ArgumentOutOfRangeException(nameof(forward));
        if (up.IsZero) throw new ArgumentOutOfRangeException(nameof(up));
        if (forward.Cross(up).Length < 0.000001) throw new ArgumentOutOfRangeException(nameof(up));
        if (!double.IsFinite(verticalFovDegrees) || verticalFovDegrees <= MinFov || verticalFovDegrees >= MaxFov)
        {
            throw new ArgumentOutOfRangeException(nameof(verticalFovDegrees));
        }

        if (!double.IsFinite(nearPlane) || nearPlane <= 0.0) throw new ArgumentOutOfRangeException(nameof(nearPlane));
        if (!double.IsFinite(farPlane) || farPlane <= nearPlane) throw new ArgumentOutOfRangeException(nameof(farPlane));
        if (revision < 0) throw new ArgumentOutOfRangeException(nameof(revision));
        if (mode == ProjectionMode.Orthographic &&
            (!double.IsFinite(orthographicScale) || orthographicScale <= 0.0))
        {
            throw new ArgumentOutOfRangeException(nameof(orthographicScale));
        }

        Position = position;
        Forward = forward.Normalize();
        Right = Forward.Cross(up).Normalize();
        Up = Right.Cross(Forward).Normalize();
        VerticalFovDegrees = verticalFovDegrees;
        NearPlane = nearPlane;
        FarPlane = farPlane;
        Revision = revision;
        Mode = mode;
        OrthographicScale = mode == ProjectionMode.Orthographic ? orthographicScale : 0.0;
    }

    public Vector3d Position { get; }

    public Vector3d Forward { get; }

    public Vector3d Right { get; }

    public Vector3d Up { get; }

    public double VerticalFovDegrees { get; }

    public double NearPlane { get; }

    public double FarPlane { get; }

    public long Revision { get; }

    public ProjectionMode Mode { get; }

    // 正交视图高度（米）；透视模式下恒为 0。
    public double OrthographicScale { get; }

    static void ValidateFinite(Vector3d vector, string name)
    {
        if (!double.IsFinite(vector.X) || !double.IsFinite(vector.Y) || !double.IsFinite(vector.Z))
        {
            throw new ArgumentOutOfRangeException(name);
        }
    }
}
