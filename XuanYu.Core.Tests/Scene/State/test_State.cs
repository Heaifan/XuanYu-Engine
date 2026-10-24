using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;

namespace XuanYu.Core.Tests.Scene.State;

public sealed class StateTests
{
    [Fact]
    public void ValidatesEntityId()
    {
        var invalid = new SceneEntitySnapshot(EntityId.None,
            "Unassigned", "Test", CommittedTransform.Identity);
        var valid = invalid with { EntityKey = EntityId.FromInt(7) };

        Assert.False(invalid.IsValid);
        Assert.True(valid.IsValid);
        Assert.Equal("Unassigned", valid.Name);
        Assert.Equal("Test", valid.Type);
        Assert.Equal(CommittedTransform.Identity, valid.Transform);
    }

    [Fact]
    public void RetainsCommitData()
    {
        var original = CommittedTransform.Identity;
        var next = original.WithPosition(Vector3d.UnitX);
        var result = new SceneTransformCommitResult(
            EntityId.FromInt(4), original, next, true);

        Assert.Equal(4, result.EntityKey.Value);
        Assert.Equal(original, result.Before);
        Assert.Equal(next, result.After);
        Assert.True(result.Changed);
    }

    [Fact]
    public void ChangedIsDeclared()
    {
        var result = new SceneTransformCommitResult(EntityId.FromInt(4),
            CommittedTransform.Identity, CommittedTransform.Identity, true);

        Assert.True(result.Changed);
        Assert.Equal(result.Before, result.After);
    }
}
