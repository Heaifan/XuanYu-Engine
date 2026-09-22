using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3ContextToolbarCoreTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3ContextToolbarCoreTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void ContextToolbar_renders_groups() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var toolbar = new XYContextToolbar(new XYContextGroup("编辑", new XYButton { Content = "选择" }), new XYContextGroup("视图", new XYButton { Content = "聚焦" }));
        Assert.Equal(2, toolbar.Items.Count);
        Assert.Equal(2, toolbar.GetVisualDescendants().OfType<XYContextGroup>().Count());
    });

    [Fact] public void ContextGroup_hides_without_leaving_layout_gap() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var hidden = new XYContextGroup("绘制", new XYButton { Content = "道路" }) { IsVisible = false };
        var toolbar = new XYContextToolbar(new XYContextGroup("编辑", new XYButton { Content = "选择" }), hidden);
        Assert.False(hidden.IsVisible);
        Assert.Equal(1, toolbar.Items.Count(x => x.IsVisible));
        Assert.Contains(toolbar.GetVisualDescendants().OfType<XYContextGroup>(), x => ReferenceEquals(x, hidden) && !x.IsVisible);
    });

    [Fact] public void DropdownBoard_opens_from_split_button() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var board = TestData.Board();
        var split = new XYSplitButton { Content = "道路" }; board.AttachTrigger(split); split.MenuCommand = new TestData.Command(_ => board.Toggle());
        split.MenuCommand!.Execute(null);
        Assert.True(board.IsOpen); Assert.True(board.Popup.IsLightDismissEnabled);
    });

    [Fact] public void Category_switch_updates_actions() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var board = TestData.Board(); board.SelectCategory("line");
        Assert.Equal("道路", board.SelectedAction!.Label); Assert.Contains("边界线", board.ActionPane.Actions.Select(x => x.Label));
    });

    [Fact] public void Keyboard_arrows_change_selection() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var board = TestData.Board(); board.RaiseEvent(TestData.Key(Key.Down));
        Assert.Equal("poi", board.SelectedAction!.Id); board.RaiseEvent(TestData.Key(Key.Right)); Assert.Equal("line", board.SelectedCategory!.Id);
    });
}
