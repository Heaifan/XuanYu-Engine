using Avalonia.Input;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3InteractionTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3InteractionTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void MenuItem_activate_invokes_command_and_event() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var calls = 0; var item = new XYMenuItem { Command = () => calls++ }; var events = 0;
        item.Invoked += (_, _) => events++; Assert.True(item.Activate()); Assert.Equal(1, calls); Assert.Equal(1, events);
        item.IsEnabled = false; Assert.False(item.Activate()); Assert.Equal(1, calls);
    });

    [Fact] public void Menu_open_focuses_first_enabled_and_escape_closes() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var disabled = new XYMenuItem { IsEnabled = false }; var enabled = new XYMenuItem(); var menu = new XYMenu(disabled, enabled);
        menu.Open(); Assert.True(menu.IsOpen); Assert.Equal(1, menu.FocusedIndex); menu.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape }); Assert.False(menu.IsOpen);
    });

    [Fact] public void MenuBar_activation_tracks_one_open_menu() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var first = new XYMenuBarItem { Label = "文件", Menu = new XYMenu() }; var second = new XYMenuBarItem { Label = "编辑", Menu = new XYMenu() }; var bar = new XYMenuBar(first, second);
        bar.Open(first); Assert.Same(first.Menu, bar.OpenMenu); Assert.Equal("文件", bar.OpenMenuId); bar.Open(second); Assert.Same(second.Menu, bar.OpenMenu); Assert.False(first.IsActive); Assert.True(second.IsActive); bar.Close(); Assert.Null(bar.OpenMenu);
    });

    [Fact] public void SubMenu_trigger_opens_and_escape_closes() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var trigger = new XYMenuItem { HasSubMenu = true }; var submenu = new XYSubMenu { ParentMenu = new XYMenu(trigger), ChildMenu = new XYMenu() };
        submenu.Close(); trigger.Activate(); Assert.True(submenu.IsOpen); submenu.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape }); Assert.False(submenu.IsOpen);
    });
}
