using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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
            
            var split = toolbar.FindControl<XYSplitButton>("DrawSplitButton");
            Assert.NotNull(split);
            split!.MenuCommand!.Execute(null);
            Dispatcher.UIThread.RunJobs();
            var popup = toolbar.FindControl<Popup>("DrawMenuPopup");
            var labels = (popup!.Child as XYMenu)!.Items.OfType<XYMenuItem>().Select(x => x.Label).ToArray();
            Assert.Equal(["点", "线", "面"], labels);
            Assert.All((popup.Child as XYMenu)!.Items.OfType<XYMenuItem>(), item => Assert.True(item.HasSubMenu));
        });
    }
}
