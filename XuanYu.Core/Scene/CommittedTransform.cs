using XuanYu.Core.Math;

namespace XuanYu.Core.Scene;

public readonly record struct CommittedTransform
{
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

    static void Validate(Vector3d value, string name, string label)
    {
        if (double.IsFinite(value.X) && double.IsFinite(value.Y) && double.IsFinite(value.Z))
            return;
        throw new ArgumentOutOfRangeException(name, value, $"{label} 必须是有限数值。");
    }
}
