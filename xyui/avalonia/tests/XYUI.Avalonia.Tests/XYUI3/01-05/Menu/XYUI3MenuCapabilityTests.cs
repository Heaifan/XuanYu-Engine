using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3MenuCapabilityTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3MenuCapabilityTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void MenuItem_executes_ICommand_with_parameter() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        object? executedParam = null;
        var cmd = new TestRelayCommand(p => executedParam = p);
        var item = new XYMenuItem { Label = "新建", Command = cmd, CommandParameter = "scene.json" };
        Assert.True(item.Activate());
        Assert.Equal("scene.json", executedParam);
    });

    [Fact]
    public void MenuItem_respects_CanExecute_and_updates_IsEnabled() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var canRun = false;
        var cmd = new TestRelayCommand(_ => { }, _ => canRun);
        var item = new XYMenuItem { Label = "保存", Command = cmd };
        Assert.False(item.IsEnabled);
        Assert.False(item.Activate());
        canRun = true;
        cmd.RaiseCanExecuteChanged();
        Assert.True(item.IsEnabled);
        Assert.True(item.Activate());
    });

    [Fact]
    public void MenuItem_CheckKind_Check_toggles_IsChecked() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var item = new XYMenuItem { Label = "构造网格", CheckKind = XyuiMenuCheckKind.Check, IsChecked = false };
        item.Activate();
        Assert.True(item.IsChecked);
        item.Activate();
        Assert.False(item.IsChecked);
    });

    [Fact]
    public void MenuItem_CheckKind_Radio_exclusively_selects_within_menu() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var item1 = new XYMenuItem { Label = "地图编辑", CheckKind = XyuiMenuCheckKind.Radio, IsChecked = true };
        var item2 = new XYMenuItem { Label = "区域编辑", CheckKind = XyuiMenuCheckKind.Radio, IsChecked = false };
        var menu = new XYMenu(item1, item2);
        item2.Activate();
        Assert.False(item1.IsChecked);
        Assert.True(item2.IsChecked);
    });

    [Fact]
    public void MenuBar_declarative_items_and_compact_mode() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var bar = new XYMenuBar { ShowDivider = false, Classes = { "compact" } };
        var barItem = new XYMenuBarItem { Label = "工作区", ShowChevron = true };
        bar.Items.Add(barItem);
        Assert.Single(bar.Items);
        Assert.Empty(bar.GetVisualDescendants().OfType<XYSeparator>());
        Assert.Contains(barItem.GetVisualDescendants().OfType<XYIcon>(), x => x.Classes.Contains("xyui-menu-chevron"));
    });

    sealed class TestRelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null) : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => execute(parameter);
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
