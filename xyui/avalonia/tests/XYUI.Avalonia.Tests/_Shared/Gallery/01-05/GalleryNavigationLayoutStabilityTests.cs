using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Gallery.Views;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class GalleryNavigationLayoutStabilityTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public GalleryNavigationLayoutStabilityTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Navigation_lists_are_non_scrolling_selection_hosts() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var view = new XYUI1DocumentationView();
        var window = XyuiBatchTestHost.Show(view);
        var lists = view.GetVisualDescendants().OfType<ListBox>()
            .Where(x => x.Classes.Contains("nav-foundation") || x.Classes.Contains("nav-tree"))
            .ToArray();

        Assert.Equal(5, lists.Length);
        Assert.All(lists, list => Assert.Empty(list.GetVisualDescendants().OfType<ScrollViewer>()));
        Assert.All(lists.Where(list => list.IsVisible), list =>
            Assert.NotEmpty(list.GetVisualDescendants().OfType<ItemsPresenter>()));
        window.Close();
    });

    [Fact]
    public void Outer_extent_is_stable_after_navigation_scroll() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var view = new XYUI1DocumentationView();
        var window = XyuiBatchTestHost.Show(view);
        var host = Assert.IsType<ScrollViewer>(view.FindControl<ScrollViewer>("NavigationScrollHost"));
        var before = host.Extent.Height;
        host.Offset = new global::Avalonia.Vector(0, Math.Min(1700, Math.Max(0, before - host.Viewport.Height)));
        Dispatcher.UIThread.RunJobs();

        Assert.InRange(Math.Abs(host.Extent.Height - before), 0, 0.5);
        window.Close();
    });
}
