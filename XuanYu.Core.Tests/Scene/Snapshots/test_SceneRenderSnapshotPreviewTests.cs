using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Core.Transform;

namespace XuanYu.Core.Tests.Scene;

public sealed class SceneRenderSnapshotPreviewTests
{
    [Fact]
    public void Empty_scene_cannot_apply_preview_to_missing_entity()
    {
        var missing = new SceneEntitySnapshot(
            EntityId.None, "", "", CommittedTransform.Identity);
        var snapshot = SceneRenderSnapshot.Empty with
        {
            PreviewTransform = new PreviewTransform(new Vector3d(10, 0, 0))
        };

        Assert.False(snapshot.HasEntity);
        Assert.Equal(CommittedTransform.Identity, snapshot.RenderTransform);
        Assert.Equal(Vector3d.Zero, snapshot.RenderPosition);
        Assert.Equal(CommittedTransform.Identity, snapshot.TransformFor(missing));
        Assert.Equal(Vector3d.Zero, snapshot.PositionFor(missing));
    }

    [Fact]
    public void Preview_affects_only_active_valid_entity()
    {
        var active = new SceneEntitySnapshot(
            EntityId.FromInt(1), "当前实体", "Test", CommittedTransform.Identity);
        var other = new SceneEntitySnapshot(
            EntityId.FromInt(2), "其他实体", "Test", CommittedTransform.Identity);
        var snapshot = new SceneRenderSnapshot(active, PreviewTransform:
            new PreviewTransform(new Vector3d(10, 0, 0)));

        Assert.True(snapshot.HasEntity);
        Assert.Equal(new Vector3d(10, 0, 0), snapshot.RenderPosition);
        Assert.Equal(new Vector3d(10, 0, 0), snapshot.PositionFor(active));
        Assert.Equal(Vector3d.Zero, snapshot.PositionFor(other));
        Assert.Equal(CommittedTransform.Identity, snapshot.TransformFor(other));
    }
}
