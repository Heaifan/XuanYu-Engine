using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Transform;

namespace XuanYu.Core.Tests.Scene.Transform;

public sealed class PreviewTests
{
    [Fact]
    public void PositionUsesDefaults()
    {
        var preview = new PreviewTransform(new Vector3d(2, 3, 4));

        Assert.Equal(new Vector3d(2, 3, 4), preview.Position);
        Assert.Equal(Vector3d.Zero, preview.Transform.Rotation);
        Assert.Equal(new Vector3d(1, 1, 1), preview.Transform.Scale);
    }

    [Fact]
    public void KeepsTransform()
    {
        var committed = new CommittedTransform(Vector3d.UnitX,
            Vector3d.UnitY, new Vector3d(2, 3, 4));
        var preview = new PreviewTransform(committed);

        Assert.Equal(committed, preview.Transform);
        Assert.Equal(committed.Position, preview.Position);
    }

    [Fact]
    public void RetainsStartState()
    {
        var id = EntityId.FromInt(9);
        var transform = CommittedTransform.Identity.WithPosition(Vector3d.UnitY);
        var snapshot = new TransformStartSnapshot(id, transform);

        Assert.Equal(id, snapshot.EntityKey);
        Assert.Equal(transform, snapshot.Transform);
    }
}
