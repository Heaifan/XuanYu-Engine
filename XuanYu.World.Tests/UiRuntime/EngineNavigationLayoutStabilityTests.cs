using Avalonia.Controls;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class EngineNavigationLayoutStabilityTests
{
    readonly UiHeadlessFixture _fixture;
    public EngineNavigationLayoutStabilityTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Project_tree_uses_one_explicit_scroll_host()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() => AssertTree(host, new ProjectWorkspace(), "ProjectScrollHost", "ProjectList"));
    }

    [Fact]
    public void Hierarchy_tree_uses_one_explicit_scroll_host()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() => AssertTree(host, new HierarchyWorkspace(), "HierarchyScrollHost", "HierarchyList"));
    }

    void AssertTree(UiRuntimeTestHost host, Control tree, string hostName, string listName)
    {
        var vm = new UiVm(null, seedInitialScene: false);
        tree.DataContext = vm;
        host.Show(tree);
        var scrollHosts = tree.GetVisualDescendants().OfType<ScrollViewer>().ToArray();
        var list = tree.FindControl<ListBox>(listName)!;
        Assert.Empty(list.GetVisualDescendants().OfType<ScrollViewer>());
        Assert.NotNull(tree.FindControl<ScrollViewer>(hostName));
        Assert.Single(scrollHosts);
    }
}
