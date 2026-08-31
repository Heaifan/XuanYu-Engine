using Avalonia.Controls;
using Avalonia.Interactivity;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3CompactNavigationInteractionTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3CompactNavigationInteractionTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void TabBar_new_button_adds_and_selects_demo_tab() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var bar = Assert.IsType<XYTabBar>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.09"));
        var count = bar.Tabs.Items.Count; bar.NewButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Assert.Equal(count + 1, bar.Tabs.Items.Count); Assert.StartsWith("新页签-", bar.Tabs.SelectedItem!.Label);
    });

    [Fact] public void Breadcrumb_popup_is_real_menu_and_selection_navigates() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var breadcrumb = Assert.IsType<XYBreadcrumb>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.11"));
        var collapsed = breadcrumb.Items.Single(x => x.IsCollapsed); collapsed.Invoke();
        Assert.True(breadcrumb.DropdownPopup.IsOpen); var menu = Assert.IsType<XYMenu>(breadcrumb.DropdownPopup.Child); Assert.Equal(2, menu.Items.Count);
        Assert.True(Assert.IsType<XYMenuItem>(menu.Items[0]).Activate()); Assert.False(breadcrumb.DropdownPopup.IsOpen);
    });

    [Fact] public void Tree_focus_and_selection_are_independent() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var tree = Assert.IsType<XYTreeNavigation>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.12"));
        var selected = tree.SelectedNode!; var focused = tree.Items[1]; tree.Focus(focused);
        Assert.Same(focused, tree.FocusedNode); Assert.Same(selected, tree.SelectedNode); tree.Select(focused); Assert.Same(focused, tree.SelectedNode);
    });
}
