using Avalonia;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Controls.Primitives;
using System.Reflection;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3InteractionTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3InteractionTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void MenuItem_invokes_once_and_disabled_items_are_inert() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var calls = 0; var item = new XYMenuItem { Command = () => calls++ }; var events = 0;
        item.Invoked += (_, _) => events++; Assert.True(item.Activate()); Assert.True(item.IsSelected); Assert.Equal(1, calls); Assert.Equal(1, events);
        item.IsEnabled = false; Assert.False(item.Activate()); Assert.Equal(1, calls);
    });

    [Fact] public void Menu_selection_is_single_and_close_clears_it() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var first = new XYMenuItem { HasSubMenu = true }; var second = new XYMenuItem { HasSubMenu = true }; var menu = new XYMenu(first, second); menu.Open();
        first.Activate(); Assert.Same(first, menu.SelectedItem); second.Activate(); Assert.Same(second, menu.SelectedItem); Assert.False(first.IsSelected); menu.Close(); Assert.Null(menu.SelectedItem);
    });

    [Fact] public void Menu_pointer_press_executes_and_closes() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var item = new XYMenuItem { Label = "打开", Width = 160 }; var menu = new XYMenu(item); var window = XyuiBatchTestHost.Show(menu);
        var point = item.TranslatePoint(new Point(20, item.Bounds.Height / 2), window)!.Value;
        window.MouseMove(point); window.MouseDown(point, MouseButton.Left); Dispatcher.UIThread.RunJobs(); Assert.False(menu.IsOpen); window.MouseUp(point, MouseButton.Left); window.Close();
    });

    [Fact] public void ContextMenu_has_real_target_and_closes_after_command() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var calls = 0; var item = new XYMenuItem { Command = () => calls++ }; var context = new XYContextMenu { Menu = new XYMenu(item) }; var target = new Border { Width = 100, Height = 40, Background = Brushes.Transparent }; var window = XyuiBatchTestHost.Show(target);
        context.AttachTo(target); var point = target.TranslatePoint(new Point(20, 20), window)!.Value; window.MouseMove(point); window.MouseDown(point, MouseButton.Right); Dispatcher.UIThread.RunJobs(); Assert.True(context.IsOpen); Assert.Same(target, context.Target); window.MouseUp(point, MouseButton.Right); item.Activate(); Assert.Equal(1, calls); Assert.False(context.IsOpen); context.Close(); Assert.False(item.IsSelected); window.Close();
    });

    [Fact] public void ContextMenu_open_at_uses_xyui_root_and_explicit_pointer_position() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var context = new XYContextMenu { ContextName = "区域1", Menu = new XYMenu(new XYMenuItem { Label = "删除" }) };
        var target = new Border { Width = 100, Height = 40, Background = Brushes.Transparent }; var window = XyuiBatchTestHost.Show(target);
        context.OpenAt(target, new Point(20, 12)); Dispatcher.UIThread.RunJobs();
        Assert.True(context.IsOpen); Assert.Contains("xyui-context-menu", context.Classes); Assert.Contains(context.GetVisualDescendants().OfType<TextBlock>(), x => x.Text == "区域1"); Assert.Equal(new Thickness(1), context.BorderThickness); Assert.Equal(new CornerRadius(6), context.CornerRadius);
        context.Close(); window.Close();
    });

    [Fact] public void ContextMenu_top_level_position_contract_anchors_popup_in_same_space() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var context = new XYContextMenu { Menu = new XYMenu(new XYMenuItem { Label = "删除" }) };
        var target = new Border { Width = 100, Height = 40, Background = Brushes.Transparent }; var window = XyuiBatchTestHost.Show(target);
        context.OpenAtTopLevel(target, new Point(120, 80)); Dispatcher.UIThread.RunJobs();
        var field = typeof(XYContextMenu).GetField("_popup", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var popup = Assert.IsType<Popup>(field.GetValue(context));
        Assert.Same(window, popup.PlacementTarget);
        Assert.Equal(PlacementMode.Custom, popup.Placement);
        Assert.NotNull(popup.CustomPopupPlacementCallback);
        context.Close(); window.Close();
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
        XyuiBatchTestHost.Prepare(); var trigger = new XYMenuItem { HasSubMenu = true }; var submenu = new XYSubMenu { ParentMenu = new XYMenu(trigger), ChildMenu = new XYMenu(), Trigger = trigger }; trigger.SubMenu = submenu;
        submenu.Close(); trigger.Activate(); Assert.True(trigger.IsSelected); Assert.True(submenu.IsOpen); trigger.Activate(); Assert.False(trigger.IsSelected); Assert.False(submenu.IsOpen);
    });
}
