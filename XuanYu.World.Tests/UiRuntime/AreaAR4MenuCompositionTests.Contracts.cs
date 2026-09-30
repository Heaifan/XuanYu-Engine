using Avalonia.Controls;
using Avalonia.Input;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

public sealed partial class AreaAR4MenuCompositionTests
{
    [Fact]
    public void File_parameters_remain_distinct_through_one_command()
    {
        _fixture.Run(() =>
        {
            var vm = new XuanYu.Editor.UI.UiVm(null, seedInitialScene: false);
            var requests = new List<string>(); vm.FileCommandRequested += requests.Add;
            Assert.True(Item(vm.RunCommand, "新建", XyuiMenuCheckKind.None).Activate());
            Assert.True(Item(vm.RunCommand, "保存", XyuiMenuCheckKind.None).Activate());
            Assert.Equal(["新建", "保存"], requests);
        });
    }

    [Fact]
    public void Can_execute_changed_updates_menu_availability()
    {
        _fixture.Run(() =>
        {
            var command = new ProbeCommand(false); var item = new XYMenuItem { Command = command };
            Assert.False(item.IsEnabled); command.SetAllowed(true);
            Assert.True(item.IsEnabled); Assert.True(item.Activate()); Assert.Equal(1, command.Count);
        });
    }

    [Fact]
    public void ICommand_is_the_only_path_when_legacy_action_is_also_set()
    {
        _fixture.Run(() =>
        {
            var command = new ProbeCommand(true); var actionCalls = 0;
            var item = new XYMenuItem { Command = command, Action = () => actionCalls++ };
            Assert.True(item.Activate()); Assert.Equal(1, command.Count); Assert.Equal(0, actionCalls);
        });
    }

    [Fact]
    public void Keyboard_navigation_activation_and_escape_preserve_contract()
    {
        _fixture.Run(() =>
        {
            var command = new ProbeCommand(true); var first = new XYMenuItem { Command = command };
            var second = new XYMenuItem { IsEnabled = false }; var menu = new XYMenu(first, second);
            menu.Open(); menu.RaiseEvent(Args(Key.Down)); Assert.Equal(0, menu.FocusedIndex);
            menu.RaiseEvent(Args(Key.Up)); Assert.Equal(0, menu.FocusedIndex);
            menu.RaiseEvent(Args(Key.Enter)); Assert.Equal(1, command.Count);
            first.RaiseEvent(Args(Key.Space)); Assert.Equal(2, command.Count);
            menu.Open(); menu.RaiseEvent(Args(Key.Escape)); Assert.False(menu.IsOpen); Assert.Equal(2, command.Count);
        });
    }

    [Fact]
    public void Closing_menu_restores_focus_to_its_trigger()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var restored = host.Run(() =>
        {
            var trigger = new XYButton { Content = "文件" }; host.Show(trigger, 160, 48);
            trigger.Focus(); var menu = new XYMenu { FocusRestoreTarget = trigger }; menu.Open(); menu.Close();
            return trigger.IsFocused;
        });
        Assert.True(restored);
    }

    static KeyEventArgs Args(Key key) => new() { RoutedEvent = InputElement.KeyDownEvent, Key = key };
}
