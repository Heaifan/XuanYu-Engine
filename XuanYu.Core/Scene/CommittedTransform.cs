using XuanYu.Core.Math;

namespace XuanYu.Core.Scene;

public readonly record struct CommittedTransform
{
    public const double MinimumScale = 0.0001;

    public CommittedTransform(Vector3d position) : this(
        position,
        Vector3d.Zero,
        new Vector3d(1, 1, 1))
    {
    }

    public CommittedTransform(Vector3d position, Vector3d rotation, Vector3d scale)
    {
        Validate(position, nameof(position), "Position");
        Validate(rotation, nameof(rotation), "Rotation");
        Validate(scale, nameof(scale), "Scale");
        ValidateScale(scale);
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }

    public Vector3d Position { get; }
    public Vector3d Rotation { get; }
    public Vector3d Scale { get; }

    public static CommittedTransform Identity { get; } = new(Vector3d.Zero);

    public CommittedTransform WithPosition(Vector3d position) =>
        new(position, Rotation, Scale);

    public CommittedTransform WithRotation(Vector3d rotation) =>
        new(Position, rotation, Scale);

    public CommittedTransform WithScale(Vector3d scale) =>
        new(Position, Rotation, scale);

    static void Validate(Vector3d value, string name, string label)
    {
        if (double.IsFinite(value.X) && double.IsFinite(value.Y) && double.IsFinite(value.Z))
            return;
        throw new ArgumentOutOfRangeException(name, value, $"{label} 必须是有限数值。");
    }

    static void ValidateScale(Vector3d scale)
    {
        if (scale.X >= MinimumScale && scale.Y >= MinimumScale && scale.Z >= MinimumScale)
            return;
        throw new ArgumentOutOfRangeException(nameof(scale), scale, $"Scale 必须大于或等于 {MinimumScale}。");
    }
}
