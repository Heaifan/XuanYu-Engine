using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3MenuRadioStateContractTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3MenuRadioStateContractTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void Checked_radio_has_visible_ring_and_dot() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var item = Item(true, "Radio"); var window = XyuiBatchTestHost.Show(item);
        Assert.True(Part<Ellipse>(item, "xyui-menu-radio-ring").IsVisible); Assert.True(Part<Ellipse>(item, "xyui-menu-radio-dot").IsVisible); window.Close();
    });

    [Fact] public void Unchecked_radio_has_ring_without_dot() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var item = Item(false, "Radio"); var window = XyuiBatchTestHost.Show(item);
        Assert.True(Part<Ellipse>(item, "xyui-menu-radio-ring").IsVisible); Assert.False(Part<Ellipse>(item, "xyui-menu-radio-dot").IsVisible); window.Close();
    });

    [Fact] public void ToggleType_none_to_radio_refreshes_visual_kind() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var item = Item(true, "None"); var window = XyuiBatchTestHost.Show(item); Assert.Empty(Parts(item, "xyui-menu-radio-ring"));
        item.ToggleType = "Radio"; Dispatcher.UIThread.RunJobs(); Assert.Single(Parts(item, "xyui-menu-radio-ring")); window.Close();
    });

    [Fact] public void ToggleType_switches_between_check_and_radio_visuals() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var item = Item(true, "Check"); var window = XyuiBatchTestHost.Show(item); Assert.Single(Parts(item, "xyui-menu-check"));
        item.ToggleType = "Radio"; Dispatcher.UIThread.RunJobs(); Assert.Single(Parts(item, "xyui-menu-radio-ring")); Assert.Empty(Parts(item, "xyui-menu-check"));
        item.ToggleType = "CheckBox"; Dispatcher.UIThread.RunJobs(); Assert.Single(Parts(item, "xyui-menu-check")); Assert.Empty(Parts(item, "xyui-menu-radio-ring")); window.Close();
    });

    [Fact] public void IsChecked_refreshes_dot_without_rebuilding_item_visual() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var item = Item(false, "Radio"); var window = XyuiBatchTestHost.Show(item); var visual = item.Child;
        item.IsChecked = true; Dispatcher.UIThread.RunJobs(); Assert.Same(visual, item.Child); Assert.True(Part<Ellipse>(item, "xyui-menu-radio-dot").IsVisible);
        item.IsChecked = false; Dispatcher.UIThread.RunJobs(); Assert.False(Part<Ellipse>(item, "xyui-menu-radio-dot").IsVisible); window.Close();
    });

    [Fact] public void Check_and_radio_use_distinct_visual_kinds() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var check = Item(true, "Check"); var radio = Item(true, "Radio"); var first = XyuiBatchTestHost.Show(check); var second = XyuiBatchTestHost.Show(radio);
        Assert.Single(Parts(check, "xyui-menu-check")); Assert.Empty(Parts(check, "xyui-menu-radio-ring")); Assert.Single(Parts(radio, "xyui-menu-radio-ring")); Assert.Empty(Parts(radio, "xyui-menu-check")); first.Close(); second.Close();
    });

    [Fact] public void MenuBar_radio_items_keep_runtime_visual_state() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var map = Item(true, "Radio"); var region = Item(false, "Radio"); var menu = new XYMenu(map, region); var barItem = new XYMenuBarItem { Label = "工作区", Menu = menu }; var bar = new XYMenuBar(barItem) { ShowDivider = false }; var window = XyuiBatchTestHost.Show(bar);
        bar.Open(barItem); Dispatcher.UIThread.RunJobs(); Assert.True(Part<Ellipse>(map, "xyui-menu-radio-dot").IsVisible); Assert.False(Part<Ellipse>(region, "xyui-menu-radio-dot").IsVisible); window.Close();
    });

    static XYMenuItem Item(bool checkedState, string toggle) => new() { Label = toggle, ToggleType = toggle == "Check" ? "CheckBox" : toggle, IsChecked = checkedState };
    static T Part<T>(XYMenuItem item, string className) where T : Visual => Parts(item, className).OfType<T>().Single();
    static IEnumerable<Visual> Parts(XYMenuItem item, string className) => item.GetVisualDescendants().Where(x => x.Classes.Contains(className));
}
