using Avalonia.Controls;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class EngineNavigationScrollAuthorityTests : IClassFixture<UiHeadlessFixture>
{
    readonly UiHeadlessFixture _fixture;

    public EngineNavigationScrollAuthorityTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Left_project_tree_disables_implicit_selected_item_scroll()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var value = host.Run(() =>
        {
            var left = new Left { DataContext = new UiVm(null, seedInitialScene: true) };
            host.Show(left, 216, 420);
            return left.FindControl<ProjectWorkspace>("ProjectWorkspace")!
                .GetVisualDescendants().OfType<ListBox>().Single().AutoScrollToSelectedItem;
        });
        Assert.False(value);
    }

    [Fact]
    public void Right_hierarchy_tree_disables_implicit_selected_item_scroll()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var value = host.Run(() =>
        {
            var hierarchy = new HierarchyWorkspace { DataContext = new UiVm(null, seedInitialScene: true) };
            host.Show(hierarchy, 216, 420);
            return hierarchy.GetVisualDescendants().OfType<ListBox>().Single().AutoScrollToSelectedItem;
        });
        Assert.False(value);
    }
}
