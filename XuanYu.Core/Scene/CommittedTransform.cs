using XuanYu.Core.Math;

namespace XuanYu.Core.Scene;

public readonly record struct CommittedTransform
{
    public CommittedTransform(Vector3d position)
    {
        if (!double.IsFinite(position.X) || !double.IsFinite(position.Y) || !double.IsFinite(position.Z))
        {
            throw new ArgumentOutOfRangeException(nameof(position), position, "Position 必须是有限数值。");
        }

        Position = position;
    }

    public Vector3d Position { get; }

    public static CommittedTransform Identity { get; } = new(Vector3d.Zero);
}
