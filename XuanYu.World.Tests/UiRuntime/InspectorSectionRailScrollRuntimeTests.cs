using Avalonia;
using Avalonia.Controls;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class InspectorSectionRailScrollRuntimeTests
{
    readonly UiHeadlessFixture _fixture;

    public InspectorSectionRailScrollRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Theory]
    [InlineData(300)]
    [InlineData(360)]
    [InlineData(480)]
    public void Inspector_keeps_fixed_shell_with_one_content_scroll_host(double width)
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var result = host.Run<(int Count, Rect Before, Rect Bounds)>(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            vm.AddCubeEntity();
            var tabs = new EditorRightTabs { DataContext = vm };
            host.Show(tabs, width, 260); tabs.UpdateLayout();
            var inspector = tabs.FindControl<InspectorPanel>("InspectorWorkspace")!;
            var header = tabs.FindControl<XYTabs>("SideTabs")!.SelectedItem!;
            var before = header.Bounds;
            tabs.UpdateLayout();
            return (UiRuntimeTestHost.Descendants<ScrollViewer>(inspector).Count(), before, header.Bounds);
        });

        Assert.Equal(1, result.Count);
        Assert.Equal(result.Before.Y, result.Bounds.Y);
    }

    [Fact]
    public void Empty_inspector_keeps_one_content_scroll_host()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var sizes = host.Run<int>(() =>
        {
            var tabs = new EditorRightTabs { DataContext = new UiVm(null, seedInitialScene: false) };
            host.Show(tabs, 300, 420); tabs.UpdateLayout();
            var inspector = tabs.FindControl<InspectorPanel>("InspectorWorkspace")!;
            return UiRuntimeTestHost.Descendants<ScrollViewer>(inspector).Count();
        });

        Assert.Equal(1, sizes);
    }
}
