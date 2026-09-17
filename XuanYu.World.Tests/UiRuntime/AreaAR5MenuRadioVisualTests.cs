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
            var switcher = selector.GetVisualDescendants().OfType<XYWorkspaceSwitcher>().Single(); switcher.Open(); Dispatcher.UIThread.RunJobs();
            var feature = switcher.WorkspaceMenu.Items.OfType<XYMenuItem>().Single(x => x.Label == "要素编辑");
            AssertRadio(feature, false); Assert.True(vm.IsManageMode); Assert.False(vm.IsEditMode);
            Assert.True(feature.Activate()); switcher.Open(); Dispatcher.UIThread.RunJobs();
            feature = switcher.WorkspaceMenu.Items.OfType<XYMenuItem>().Single(x => x.Label == "要素编辑");
            AssertRadio(feature, true); Assert.True(vm.IsEditMode); Assert.True(vm.IsRegionWorkspace);
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
