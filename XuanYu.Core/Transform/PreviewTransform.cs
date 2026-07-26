using XuanYu.Core.Math;
using XuanYu.Core.Scene;

namespace XuanYu.Core.Transform;

public readonly record struct PreviewTransform
{
    public PreviewTransform(Vector3d position) : this(new CommittedTransform(position))
    {
    }

    public PreviewTransform(CommittedTransform transform)
    {
        Transform = transform;
    }

    public CommittedTransform Transform { get; }
    public Vector3d Position => Transform.Position;
}
