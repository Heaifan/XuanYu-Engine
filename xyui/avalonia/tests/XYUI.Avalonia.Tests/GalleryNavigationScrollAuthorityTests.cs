using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Gallery.Views;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class GalleryNavigationScrollAuthorityTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;

    public GalleryNavigationScrollAuthorityTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Navigation_lists_disable_implicit_selected_item_scroll() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var view = new XYUI1DocumentationView();
        var window = XyuiBatchTestHost.Show(view);
        var lists = view.GetVisualDescendants().OfType<ListBox>()
            .Where(x => x.Classes.Contains("nav-foundation") || x.Classes.Contains("nav-tree"))
            .ToArray();

        Assert.Equal(5, lists.Length);
        Assert.All(lists, list => Assert.False(list.AutoScrollToSelectedItem));
        window.Close();
    });

    [Fact]
    public void Selecting_a_document_does_not_rewrite_navigation_offset() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var view = new XYUI1DocumentationView();
        var window = XyuiBatchTestHost.Show(view);
        var host = Assert.IsType<ScrollViewer>(view.FindControl<ScrollViewer>("NavigationScrollHost"));
        var vm = Assert.IsType<XYUI1DocumentationViewModel>(view.DataContext);
        var before = host.Offset.Y;

        vm.Select("XYUI-3-3.10");
        Dispatcher.UIThread.RunJobs();

        Assert.InRange(Math.Abs(host.Offset.Y - before), 0, 0.5);
        Assert.Equal("XYUI-3-3.10", vm.SelectedXYUI3Item?.Id);
        Assert.NotNull(vm.SelectedDocument);
        window.Close();
    });

    [Fact]
    public void Gallery_navigation_has_one_explicit_scroll_host()
    {
        var source = ReadViewAxaml();
        Assert.Single(Regex.Matches(source, "<ScrollViewer\\b"));
        Assert.Contains("x:Name=\"NavigationScrollHost\"", source);
    }

    static string ReadViewAxaml()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var path = Path.Combine(dir.FullName, "xyui", "avalonia", "gallery",
                "XYUI.Avalonia.Gallery", "Views", "XYUI1DocumentationView.axaml");
            if (File.Exists(path)) return File.ReadAllText(path);
            dir = dir.Parent;
        }
        throw new FileNotFoundException("XYUI1DocumentationView.axaml not found");
    }
}
