using Avalonia.Controls;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class AreaDR2Fix5TabIntegrationRuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public AreaDR2Fix5TabIntegrationRuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Editor_navigation_tabs_use_content_sizing_and_fixed_tabs()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var result = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var left = new Left { DataContext = vm };
            var leftWindow = host.Show(left, 480, 240); left.UpdateLayout();
            var rightTabs = new EditorRightTabs { DataContext = vm };
            var rightWindow = host.Show(rightTabs, 480, 640); rightTabs.UpdateLayout();
            var map = new MapEditorPanel { DataContext = vm };
            var mapWindow = host.Show(map, 480, 640); map.UpdateLayout();
            var region = new RegionalAuthoringPanel { DataContext = vm };
            var regionWindow = host.Show(region, 480, 640); region.UpdateLayout();
            var leftTabs = left.FindControl<XYTabs>("ContentTabs")!;
            var right = rightTabs.FindControl<XYTabs>("SideTabs")!;
            var mapPager = map.FindControl<XYPager>("MapPager")!;
            var regionPager = region.FindControl<XYPager>("AuthoringPager")!;
            var value = (LeftSizing: leftTabs.SizingMode, RightSizing: right.SizingMode,
                MapPages: mapPager.Pages.Count, RegionPages: regionPager.Pages.Count,
                LeftClosable: leftTabs.Items.Select(tab => tab.IsClosable).ToArray(),
                RightClosable: right.Items.Select(tab => tab.IsClosable).ToArray(),
                MapClosable: Array.Empty<bool>(), RegionClosable: Array.Empty<bool>());
            leftWindow.Close(); rightWindow.Close(); mapWindow.Close(); regionWindow.Close();
            return value;
        });
        Assert.Equal(XyuiTabSizingMode.Content, result.LeftSizing);
        Assert.Equal(XyuiTabSizingMode.Content, result.RightSizing);
        Assert.Equal(5, result.MapPages);
        Assert.Equal(3, result.RegionPages);
        Assert.All(result.LeftClosable, Assert.False);
        Assert.All(result.RightClosable, Assert.False);
        Assert.All(result.MapClosable, Assert.False);
        Assert.All(result.RegionClosable, Assert.False);
    }
}
