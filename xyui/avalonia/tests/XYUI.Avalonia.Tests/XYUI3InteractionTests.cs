using Avalonia;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3InteractionTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3InteractionTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void MenuItem_selects_first_then_invokes_and_clears_on_second_click() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var calls = 0; var item = new XYMenuItem { Command = () => calls++ }; var events = 0;
        item.Invoked += (_, _) => events++; Assert.True(item.Activate()); Assert.True(item.IsSelected); Assert.Equal(0, calls); Assert.Equal(0, events);
        Assert.True(item.Activate()); Assert.False(item.IsSelected); Assert.Equal(1, calls); Assert.Equal(1, events);
        item.IsEnabled = false; Assert.False(item.Activate()); Assert.Equal(1, calls);
    });

    [Fact] public void Menu_selection_is_single_and_close_clears_it() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var first = new XYMenuItem(); var second = new XYMenuItem(); var menu = new XYMenu(first, second);
        first.Activate(); Assert.Same(first, menu.SelectedItem); second.Activate(); Assert.Same(second, menu.SelectedItem); Assert.False(first.IsSelected); menu.Close(); Assert.Null(menu.SelectedItem);
    });

    [Fact] public void Menu_pointer_press_selects_before_pointer_release() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var item = new XYMenuItem { Label = "打开", Width = 160 }; var menu = new XYMenu(item); var window = XyuiBatchTestHost.Show(menu);
        var point = item.TranslatePoint(new Point(20, item.Bounds.Height / 2), window)!.Value;
        window.MouseMove(point); window.MouseDown(point, MouseButton.Left); Dispatcher.UIThread.RunJobs(); Assert.True(item.IsSelected); window.MouseUp(point, MouseButton.Left); window.Close();
    });

    [Fact] public void ContextMenu_selection_executes_on_second_click_and_clears_outside() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var calls = 0; var item = new XYMenuItem { Command = () => calls++ }; var context = new XYContextMenu { Menu = new XYMenu(item) }; var target = new Border { Width = 100, Height = 40, Background = Brushes.Transparent }; var window = XyuiBatchTestHost.Show(target);
        context.AttachTo(target); var point = target.TranslatePoint(new Point(20, 20), window)!.Value; window.MouseMove(point); window.MouseDown(point, MouseButton.Right); Dispatcher.UIThread.RunJobs(); Assert.True(context.IsOpen); window.MouseUp(point, MouseButton.Right); item.Activate(); Assert.True(item.IsSelected); Assert.Equal(0, calls); item.Activate(); Assert.False(item.IsSelected); Assert.Equal(1, calls); Assert.False(context.IsOpen); context.Close(); Assert.False(item.IsSelected); window.Close();
    });

    [Fact] public void Menu_open_focuses_first_enabled_and_escape_closes() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var disabled = new XYMenuItem { IsEnabled = false }; var enabled = new XYMenuItem(); var menu = new XYMenu(disabled, enabled);
        menu.Open(); Assert.True(menu.IsOpen); Assert.Equal(1, menu.FocusedIndex); menu.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape }); Assert.False(menu.IsOpen);
    });

    [Fact] public void MenuBar_activation_tracks_one_open_menu() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var first = new XYMenuBarItem { Label = "文件", Menu = new XYMenu() }; var second = new XYMenuBarItem { Label = "编辑", Menu = new XYMenu() }; var bar = new XYMenuBar(first, second);
        var window = XyuiBatchTestHost.Show(bar); var point = first.TranslatePoint(new Point(20, first.Bounds.Height / 2), window)!.Value; window.MouseMove(point); window.MouseDown(point, MouseButton.Left); Dispatcher.UIThread.RunJobs(); Assert.Same(first.Menu, bar.OpenMenu); Assert.IsType<XYMenu>(bar.OpenMenu); Assert.Equal(new Thickness(1), first.Menu!.BorderThickness); Assert.Equal(new Thickness(5), first.Menu.Padding); window.MouseUp(point, MouseButton.Left); first.Menu.Close(); Assert.Null(bar.OpenMenu); Assert.False(first.IsActive); bar.Open(second); Assert.Same(second.Menu, bar.OpenMenu); Assert.False(first.IsActive); Assert.True(second.IsActive); bar.Close(); Assert.Null(bar.OpenMenu); window.Close();
    });

    [Fact] public void SubMenu_trigger_opens_and_escape_closes() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var trigger = new XYMenuItem { HasSubMenu = true }; var submenu = new XYSubMenu { ParentMenu = new XYMenu(trigger), ChildMenu = new XYMenu() };
        submenu.Close(); trigger.Activate(); Assert.True(trigger.IsSelected); Assert.True(submenu.IsOpen); trigger.Activate(); Assert.False(trigger.IsSelected); Assert.False(submenu.IsOpen);
    });
}
