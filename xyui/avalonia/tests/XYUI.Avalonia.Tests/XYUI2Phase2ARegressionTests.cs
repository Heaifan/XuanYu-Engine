using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery.Views;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2Phase2ARegressionTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2Phase2ARegressionTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Catalog_live_examples_are_fresh_per_mount()
    {
        var document = XYUI2DocumentationCatalog.Build().Single(x => x.Id == "XYUI-2-01");
        var first = document.LiveExamplesFactory!();
        var second = document.LiveExamplesFactory!();
        Assert.NotSame(first, second);
    }

    [Fact]
    public void DropDown_action_edge_spans_unified_zone() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var dropdown = new XYDropDownButton { Content = "导出" };
        var window = XyuiBatchTestHost.Show(dropdown);
        var edge = dropdown.GetVisualDescendants().OfType<XyuiActionEdge>().Single();
        var zone = dropdown.GetVisualDescendants().OfType<Button>().Single(x => x.Name == "PART_OpenZone");
        Assert.Equal(zone.Bounds.Width, edge.Bounds.Width, 3);
        window.Close();
    });

    [Fact]
    public void Gallery_navigation_rebuilds_phase2a_without_visual_parent_conflicts() => _fx.Run(() =>
    {
        var view = new XYUI1DocumentationView();
        var window = XyuiBatchTestHost.Show(view);
        var model = (XYUI1DocumentationViewModel)view.DataContext!;
        var forward = Enumerable.Range(1, 24).Select(i => $"XYUI-2-{i:00}").ToArray();
        var reverse = forward.Reverse().ToArray();
        Repeat(model, forward, 3);
        Repeat(model, reverse, 3);
        foreach (var id in forward) model.Select(id);
        Dispatcher.UIThread.RunJobs();
        Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;
        Repeat(model, forward, 1);
        Application.Current.RequestedThemeVariant = ThemeVariant.Light;
        Repeat(model, reverse, 1);
        window.Close();
    });

    static void Repeat(XYUI1DocumentationViewModel model, IEnumerable<string> ids, int rounds)
    {
        for (var round = 0; round < rounds; round++)
            foreach (var id in ids)
            {
                model.Select(id);
                Dispatcher.UIThread.RunJobs();
            }
    }
}
