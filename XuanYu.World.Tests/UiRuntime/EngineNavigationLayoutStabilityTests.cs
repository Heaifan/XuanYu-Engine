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

    [Fact]
    public void Project_rows_fill_the_viewport_after_resize_without_horizontal_overflow()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var evidence = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            vm.ProjectItems[0].Update(new string('长', 160), "场景", "长地图文档", 0, "project");
            var tree = new ProjectWorkspace { DataContext = vm };
            host.Show(tree, 300, 420);
            return MeasureProjectRows(tree, 216);
        });

        Assert.Equal((300d, 300d, 216d, 216d, false), evidence);
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

    static (double List, double Row, double ResizedList, double ResizedRow, bool HasHorizontalScroll)
        MeasureProjectRows(ProjectWorkspace tree, double resizedWidth)
    {
        var list = tree.FindControl<ListBox>("ProjectList")!;
        var row = list.ContainerFromIndex(0)!.GetVisualDescendants()
            .OfType<Border>().Single(border => border.Classes.Contains("treeRow"));
        var initialListWidth = list.Bounds.Width;
        var initialRowWidth = row.Bounds.Width;
        var hasHorizontalScroll = list.GetVisualDescendants().OfType<ScrollViewer>().Any();
        tree.Width = resizedWidth;
        tree.UpdateLayout();
        var resizedRow = list.ContainerFromIndex(0)!.GetVisualDescendants()
            .OfType<Border>().Single(border => border.Classes.Contains("treeRow"));
        return (initialListWidth, initialRowWidth, list.Bounds.Width, resizedRow.Bounds.Width,
            hasHorizontalScroll);
    }
}
