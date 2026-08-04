using System.Reflection;
using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;
using XuanYu.Editor.UI;
using XuanYu.World.Scene;

namespace XuanYu.World.Tests.World;

public sealed partial class TransformFoundationTests
{
    [Fact]
    public void Position_constructor_defaults_rotation_and_scale()
    {
        var transform = new CommittedTransform(new Vector3d(1, 2, 3));

        Assert.Equal(new Vector3d(1, 2, 3), transform.Position);
        Assert.Equal(Vector3d.Zero, transform.Rotation);
        Assert.Equal(new Vector3d(1, 1, 1), transform.Scale);
    }

    [Fact]
    public void Move_position_commit_preserves_rotation_and_scale()
    {
        var scene = new SceneStateOwner();
        var key = scene.RenderSnapshot.Entity.EntityKey;
        var original = new CommittedTransform(
            Vector3d.Zero,
            new Vector3d(0, 0, 45),
            new Vector3d(2, 3, 4));

        Assert.True(scene.RestoreTransform(key, original));
        var commit = scene.CommitPositionWithResult(key, new Vector3d(5, 0, 0));

        Assert.True(commit.Changed);
        Assert.Equal(new Vector3d(5, 0, 0), commit.After.Position);
        Assert.Equal(original.Rotation, commit.After.Rotation);
        Assert.Equal(original.Scale, commit.After.Scale);
    }

    [Fact]
    public void Inspector_projects_real_rotation_and_scale_fields()
    {
        var vm = new UiVm(null, () => true);
        var key = EntityId.FromInt(1);
        var transform = new CommittedTransform(
            new Vector3d(1, 2, 3),
            new Vector3d(10, 20, 30),
            new Vector3d(2, 2, 2));
        SceneOf(vm).RestoreTransform(key, transform);
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(i => i.Key == key.ToString());

        Assert.Contains("变换", vm.InspectorFields);
        Assert.Contains("位置    X 1    Y 2    Z 3", vm.InspectorFields);
        Assert.Contains("旋转    X 10°    Y 20°    Z 30°", vm.InspectorFields);
        Assert.Contains("缩放    X 2    Y 2    Z 2", vm.InspectorFields);
    }

    static SceneStateOwner SceneOf(UiVm vm)
    {
        var field = typeof(UiVm).GetField(
            "_sceneState",
            BindingFlags.Instance | BindingFlags.NonPublic);
        return (SceneStateOwner)field!.GetValue(vm)!;
    }
}
