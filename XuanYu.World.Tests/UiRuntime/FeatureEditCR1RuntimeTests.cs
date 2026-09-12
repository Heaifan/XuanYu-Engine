using System.Linq;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Xunit;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class FeatureEditCR1RuntimeTests
{
    readonly UiHeadlessFixture _fixture;
    public FeatureEditCR1RuntimeTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Non_feature_edit_mode_hides_feature_tools()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var isVisible = host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var toolbar = new ContextToolBar { DataContext = vm };
            host.Show(toolbar, 800, 100); toolbar.UpdateLayout();
            
            var root = toolbar.FindControl<Border>("ContextRoot");
            return root?.IsVisible ?? false;
        });
        
        Assert.False(isVisible);
    }

    [Fact]
    public void Feature_edit_mode_shows_tools_and_breadcrumb()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        host.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            vm.ToggleFeatureEditingCommand.Execute(null);
            var toolbar = new ContextToolBar { DataContext = vm };
            host.Show(toolbar, 800, 100); toolbar.UpdateLayout();
            
            var root = toolbar.FindControl<Border>("ContextRoot");
            Assert.True(root?.IsVisible ?? false);
            
            var logicals = toolbar.GetLogicalDescendants().ToList();
            var breadcrumbs = logicals.OfType<XYBreadcrumbItem>().Select(x => x.Label).ToArray();
            Assert.Contains("地图编辑", breadcrumbs);
            Assert.Contains("要素编辑", breadcrumbs);
            
            var menuBar = UiRuntimeTestHost.Descendants<XYMenuBar>(toolbar).Single();
            var menuBars = menuBar.Items.Cast<XYMenuBarItem>().Select(x => x.Label).ToArray();
            Assert.Contains("点", menuBars);
            Assert.Contains("线", menuBars);
            Assert.Contains("面", menuBars);

            // Open "点"
            menuBar.Open(menuBar.Items.Cast<XYMenuBarItem>().Single(x => x.Label == "点"));
            Dispatcher.UIThread.RunJobs();
            var pointItems = menuBar.OpenMenu!.Items.OfType<XYMenuItem>().Select(x => x.Label).ToArray();
            Assert.Contains("点标记", pointItems);

            // Open "线"
            menuBar.Open(menuBar.Items.Cast<XYMenuBarItem>().Single(x => x.Label == "线"));
            Dispatcher.UIThread.RunJobs();
            var lineItems = menuBar.OpenMenu!.Items.OfType<XYMenuItem>().Select(x => x.Label).ToArray();
            Assert.Contains("道路", lineItems);

            // Open "面"
            menuBar.Open(menuBar.Items.Cast<XYMenuBarItem>().Single(x => x.Label == "面"));
            Dispatcher.UIThread.RunJobs();
            var areaItems = menuBar.OpenMenu!.Items.OfType<XYMenuItem>().Select(x => x.Label).ToArray();
            Assert.Contains("区域", areaItems);
        });
    }
}
