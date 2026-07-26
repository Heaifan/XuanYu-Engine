using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed partial class WorldR4TransformFoundationTests
{
    [Fact]
    public void Inspector_input_formats_visible_transform_without_losing_precision()
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(i => i.Key == "EntityId(1)");

        Assert.True(vm.TryCommitInspectorTransformValue("位置", "X", "3.114397031608849"));
        Assert.True(vm.TryCommitInspectorTransformValue("旋转", "Z", "90"));

        Assert.Contains("位置    X 3.114397    Y 0    Z 0", vm.InspectorFields);
        Assert.Contains("旋转    X 0°    Y 0°    Z 90°", vm.InspectorFields);
        Assert.Equal(3.114397031608849, SceneOf(vm).RenderSnapshot.Entity.Transform.Position.X);
    }

    [Theory]
    [InlineData("缩放", "X", "0")]
    [InlineData("缩放", "X", "-1")]
    [InlineData("旋转", "Z", "NaN")]
    [InlineData("位置", "X", "abc")]
    public void Inspector_invalid_input_does_not_pollute_transform_or_history(
        string group,
        string axis,
        string text)
    {
        var vm = new UiVm(null, () => true);
        vm.SelectedHierarchyItem = vm.HierarchyItems.Single(i => i.Key == "EntityId(1)");
        var before = SceneOf(vm).RenderSnapshot.Entity.Transform;

        Assert.False(vm.TryCommitInspectorTransformValue(group, axis, text));

        Assert.Equal(before, SceneOf(vm).RenderSnapshot.Entity.Transform);
        Assert.Equal(0, vm.TransformHistoryCount);
    }
}
