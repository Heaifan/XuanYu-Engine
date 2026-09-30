using System.Windows.Input;
using Avalonia.Data;
using Avalonia.Input;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed partial class AreaAR4MenuCompositionTests
{
    readonly UiHeadlessFixture _fixture;
    public AreaAR4MenuCompositionTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void File_item_routes_one_real_file_command()
    {
        _fixture.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false); var requests = new List<string>();
            vm.FileCommandRequested += requests.Add;
            var item = new XYMenuItem { Command = vm.RunCommand, CommandParameter = "新建" };
            Assert.True(item.Activate()); Assert.Equal(["新建"], requests);
        });
    }

    [Fact]
    public void Workspace_radio_transitions_follow_vm_truth()
    {
        _fixture.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var map = Item(vm.SwitchWorkspaceCommand, "MapEditor", XyuiMenuCheckKind.Radio);
            var region = Item(vm.SwitchWorkspaceCommand, "RegionEditor", XyuiMenuCheckKind.Radio);
            _ = new XYMenu(map, region); Assert.True(region.Activate());
            Assert.True(region.IsChecked); Assert.True(vm.IsRegionWorkspace);
            Assert.True(map.Activate()); Assert.True(map.IsChecked); Assert.False(region.IsChecked);
            Assert.True(vm.IsMapWorkspace);
        });
    }

    [Fact]
    public void Environment_check_is_vm_bound_and_escape_is_inert()
    {
        _fixture.Run(() =>
        {
            var vm = new UiVm(null, seedInitialScene: false);
            var item = Item(vm.RunCommand, "显示世界坐标轴", XyuiMenuCheckKind.Check);
            item.DataContext = vm;
            item.Bind(XYMenuItem.IsCheckedProperty, new Binding(nameof(UiVm.ShowWorldAxes)));
            var menu = new XYMenu(item); Assert.True(item.Activate());
            Assert.True(vm.ShowWorldAxes); Assert.True(item.IsChecked); menu.Open();
            menu.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape });
            Assert.False(menu.IsOpen); Assert.True(vm.ShowWorldAxes);
        });
    }

    [Fact]
    public void Disabled_and_can_execute_false_items_never_run()
    {
        _fixture.Run(() =>
        {
            var blocked = new ProbeCommand(false);
            var blockedItem = new XYMenuItem { Command = blocked };
            Assert.False(blockedItem.Activate()); Assert.Equal(0, blocked.Count);
            var enabled = new ProbeCommand(true);
            var disabledItem = new XYMenuItem { Command = enabled, IsEnabled = false };
            Assert.False(disabledItem.Activate()); Assert.Equal(0, enabled.Count);
        });
    }

    [Fact]
    public void Keyboard_activation_executes_once()
    {
        _fixture.Run(() =>
        {
            var command = new ProbeCommand(true); var item = new XYMenuItem { Command = command };
            item.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Enter });
            Assert.Equal(1, command.Count);
        });
    }

    static XYMenuItem Item(ICommand command, string parameter, XyuiMenuCheckKind kind) =>
        new() { Command = command, CommandParameter = parameter, CheckKind = kind };

    sealed class ProbeCommand(bool allowed) : ICommand
    {
        bool _allowed = allowed;
        public int Count { get; private set; }
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => _allowed;
        public void Execute(object? parameter) => Count++;
        public void SetAllowed(bool value) { _allowed = value; CanExecuteChanged?.Invoke(this, EventArgs.Empty); }
    }
}
