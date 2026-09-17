using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class AreaAR4MenuRuntimeTests
{
    [Fact]
    public void R7_real_popup_radio_styles_are_ready_before_first_attach()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var selector = new WorkspaceSelector { DataContext = vm };
            host.Show(selector, 640, 80);
            selector.UpdateLayout();
            var switcher = selector.GetVisualDescendants().OfType<XYWorkspaceSwitcher>().Single();
            switcher.Open();
            Dispatcher.UIThread.RunJobs();
            var menu = switcher.WorkspaceMenu;
            Assert.True(menu.IsOpen);
            Assert.Single(menu.Styles);
            foreach (var item in menu.Items.OfType<XYMenuItem>().Where(x => x.Classes.Contains("xyui-workspace-item")))
            {
                var ring = item.GetVisualDescendants().OfType<Ellipse>().Single(x => x.Classes.Contains("xyui-menu-radio-ring"));
                var dot = item.GetVisualDescendants().OfType<Ellipse>().Single(x => x.Classes.Contains("xyui-menu-radio-dot"));
                Assert.Equal(XyuiMenuCheckKind.Radio, item.CheckKind);
                Assert.True(ring.IsVisible);
                Assert.Equal(item.IsChecked, dot.IsVisible);
            }
        });
    }
}
