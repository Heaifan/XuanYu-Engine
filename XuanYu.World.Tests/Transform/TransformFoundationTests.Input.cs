using XuanYu.Core.Gizmo;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Editor.Transform;
using XuanYu.Editor.UI;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

public sealed partial class TransformFoundationTests
{
    [Fact]
    public void Full_transform_commit_updates_rotation_without_moving_or_scaling()
    {
        var scene = new SceneStateOwner();
        var key = scene.RenderSnapshot.Entity.EntityKey;
        var original = new CommittedTransform(
            new(1, 2, 3), Vector3d.Zero, new Vector3d(2, 3, 4));

        Assert.True(scene.RestoreTransform(key, original));
        var commit = scene.CommitTransformWithResult(key,
            original.WithRotation(new Vector3d(0, 0, 90)));

        Assert.True(commit.Changed);
        Assert.Equal(original.Position, commit.After.Position);
        Assert.Equal(new Vector3d(0, 0, 90), commit.After.Rotation);
        Assert.Equal(original.Scale, commit.After.Scale);
    }

    [Fact]
    public void Move_session_preview_preserves_rotation_and_scale()
    {
        var scene = new SceneStateOwner();
        var key = scene.RenderSnapshot.Entity.EntityKey;
        var original = new CommittedTransform(
            Vector3d.Zero, new Vector3d(0, 0, 45), new Vector3d(2, 3, 4));
        Assert.True(scene.RestoreTransform(key, original));
        var session = new TransformSession();
        Assert.True(session.Begin(17, scene.RenderSnapshot.Entity, MoveGizmoAxis.X));
        Assert.True(session.TryPreview(17, new Vector3d(5, 0, 0)));
        Assert.True(session.TryCommit(17, scene, out var commit));

        Assert.Equal(new Vector3d(5, 0, 0), commit.After.Position);
        Assert.Equal(original.Rotation, commit.After.Rotation);
        Assert.Equal(original.Scale, commit.After.Scale);
    }

    [Fact]
    public void Transform_session_commits_rotation_and_scale_through_same_history_candidate()
    {
        var scene = new SceneStateOwner();
        var session = new TransformSession();
        Assert.True(session.Begin(17, scene.RenderSnapshot.Entity, MoveGizmoAxis.X));
        var next = scene.RenderSnapshot.Entity.Transform
            .WithRotation(new Vector3d(10, 0, 0))
            .WithScale(new Vector3d(2, 2, 2));
        Assert.True(session.TryPreviewTransform(17, next));
        Assert.True(session.TryCommit(17, scene, out var commit));
        Assert.True(commit.Changed);
        Assert.Equal(Vector3d.Zero, commit.After.Position);
        Assert.Equal(new Vector3d(10, 0, 0), commit.After.Rotation);
        Assert.Equal(new Vector3d(2, 2, 2), commit.After.Scale);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(0.00001)]
    public void Unsafe_scale_is_rejected_by_committed_transform(double scale)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new CommittedTransform(Vector3d.Zero, Vector3d.Zero, new Vector3d(scale, 1, 1)));
    }

    [Fact]
    public void Inspector_position_rotation_and_scale_inputs_commit_full_transform_history()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(i => i.Key == "EntityId(1)");

        Assert.True(vm.TryCommitInspectorTransformValue("位置", "X", "3.114397031608849"));
        Assert.True(vm.TryCommitInspectorTransformValue("旋转", "Z", "90"));
        Assert.True(vm.TryCommitInspectorTransformValue("缩放", "Y", "2"));

        var transform = SceneOf(vm).RenderSnapshot.Entity.Transform;
        Assert.Equal(3.114397031608849, transform.Position.X);
        Assert.Equal(90, transform.Rotation.Z);
        Assert.Equal(2, transform.Scale.Y);
        Assert.Equal(3, vm.TransformHistoryCount);
        vm.RunCommand.Execute("撤销");
        Assert.Equal(1, SceneOf(vm).RenderSnapshot.Entity.Transform.Scale.Y);
        vm.RunCommand.Execute("重做");
        Assert.Equal(2, SceneOf(vm).RenderSnapshot.Entity.Transform.Scale.Y);
    }
}
