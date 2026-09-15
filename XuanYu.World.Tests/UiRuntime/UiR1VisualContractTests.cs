using Avalonia.Controls;
using Avalonia.Media;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class UiR1VisualContractTests
{
    readonly UiHeadlessFixture _fixture;
    public UiR1VisualContractTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Editor_right_uses_semantic_inspector_navigation_items()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            vm.MapSession.SelectMap();
            var tabs = new EditorRightTabs { DataContext = vm };
            host.Show(tabs, 640, 700); tabs.UpdateLayout();
            return (Count: UiRuntimeTestHost.Descendants<XYToggleButton>(tabs)
                    .Count(x => x.Classes.Contains("inspectorNavigationItem")), Text: vm.InspectorCategory);
        });

        Assert.True(state.Count > 0); Assert.Equal("最近", state.Text);
    }

    [Fact]
    public void Engine_app_keeps_representative_xyui1_visual_contracts()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var state = host.Run(() =>
        {
            var panel = new StackPanel
            {
                Children =
                {
                    new XYHeading { Text = "检查器" }, new XYLabel { Text = "字段" },
                    new XYBadge { Text = "Accent", Variant = XyuiBadgeVariant.Accent },
                    new XYStatusBadge { Text = "完成", State = XyuiStatusState.Success },
                    new XYErrorText { Text = "错误" },
                },
            };
            host.Show(panel, 420, 180); panel.UpdateLayout();
            return (Heading: panel.Children.OfType<XYHeading>().Single().FontSize > 0,
                Label: panel.Children.OfType<XYLabel>().Single().FontSize > 0,
                Badge: panel.Children.OfType<XYBadge>().Single().Height == XYBadge.BadgeHeight,
                Status: panel.Children.OfType<XYStatusBadge>().Single().Classes.Contains("xyui-status-success"),
                Error: panel.Children.OfType<XYErrorText>().Any());
        });

        Assert.True(state.Heading); Assert.True(state.Label); Assert.True(state.Badge);
        Assert.True(state.Status); Assert.True(state.Error);
    }

    static Color ColorOf(IBrush? brush) => Assert.IsType<SolidColorBrush>(brush).Color;
}
