using Avalonia.Controls.Shapes;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;
using System.Linq;
using Xunit;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class AreaAR4MenuRuntimeTests
{
    [Fact]
    public void Workspace_selector_radio_visuals_follow_vm_after_open_and_switch()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); var selector = new WorkspaceSelector { DataContext = vm }; host.Show(selector, 640, 80); selector.UpdateLayout();
            var bar = selector.GetVisualDescendants().OfType<XYMenuBar>().Single(); var barItem = bar.Items.Single(); bar.Open(barItem); Dispatcher.UIThread.RunJobs();
            var map = bar.OpenMenu!.Items.OfType<XYMenuItem>().Single(x => x.Label == "地图编辑"); var region = bar.OpenMenu.Items.OfType<XYMenuItem>().Single(x => x.Label == "要素编辑");
            AssertRadio(map, true); AssertRadio(region, false); Assert.True(vm.IsMapWorkspace); Assert.False(vm.IsRegionWorkspace);
            Assert.True(region.Activate()); bar.Open(barItem); Dispatcher.UIThread.RunJobs(); AssertRadio(map, false); AssertRadio(region, true); Assert.False(vm.IsMapWorkspace); Assert.True(vm.IsRegionWorkspace);
        });
    }

    static void AssertRadio(XYMenuItem item, bool checkedState)
    {
        Assert.Equal(XyuiMenuCheckKind.Radio, item.CheckKind); Assert.Equal(checkedState, item.IsChecked);
        Assert.True(item.GetVisualDescendants().OfType<Ellipse>().Single(x => x.Classes.Contains("xyui-menu-radio-ring")).IsVisible);
        Assert.Equal(checkedState, item.GetVisualDescendants().OfType<Ellipse>().Single(x => x.Classes.Contains("xyui-menu-radio-dot")).IsVisible);
        Assert.DoesNotContain(item.GetVisualDescendants(), x => x.Classes.Contains("xyui-menu-check"));
    }
}
