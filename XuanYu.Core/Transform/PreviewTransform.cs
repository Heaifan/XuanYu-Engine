using XuanYu.Core.Math;

namespace XuanYu.Core.Transform;

public readonly record struct PreviewTransform
{
    public PreviewTransform(Vector3d position)
    {
        if (!double.IsFinite(position.X) || !double.IsFinite(position.Y) || !double.IsFinite(position.Z))
            throw new ArgumentOutOfRangeException(nameof(position), position, "Preview Position 必须是有限数值。");
        Position = position;
    }

    public Vector3d Position { get; }
}
