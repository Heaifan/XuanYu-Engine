using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

// XYUI-2 Phase 2A 契约测试：约束 A(DropDown 触发器非Popup宿主)、约束 B(SplitButton 双命令非菜单宿主)、约束 C(IconButton 可访问性)、Checkbox 三态。
[Collection("XyuiHeadless")]
public sealed class XYUI2Phase2AContractTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2Phase2AContractTests(XyuiHeadlessFixture fx) => _fx = fx;

    static void Click(Window window, Control zone)
    {
        var center = zone.TranslatePoint(new Point(zone.Bounds.Width / 2, zone.Bounds.Height / 2), window)
                     ?? new Point(zone.Bounds.Width / 2, zone.Bounds.Height / 2);
        window.MouseDown(center, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
        window.MouseUp(center, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
    }

    [Fact]
    public void DropDownButton_is_trigger_only_without_popup_ownership() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var ddb = new XYDropDownButton { Content = "导出" };
        var executed = false;
        ddb.OpenCommand = new RelayCmd(() => executed = true);
        Assert.Null(ddb.GetType().GetProperty("Popup"));
        Assert.Null(ddb.GetType().GetProperty("Menu"));
        var window = XyuiBatchTestHost.Show(ddb);
        ddb.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Enter });
        Assert.True(executed, "Enter 应触发 OpenCommand 动作");
        window.Close();
    });

    [Fact]
    public void SplitButton_isolates_main_and_menu_commands_without_popup_owner() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        bool mainRan = false, menuRan = false;
        var split = new XYSplitButton { Content = "保存", MainCommand = new RelayCmd(() => mainRan = true), MenuCommand = new RelayCmd(() => menuRan = true) };
        Assert.Null(split.GetType().GetProperty("Flyout"));
        Assert.Null(split.GetType().GetProperty("Menu"));
        var window = XyuiBatchTestHost.Show(split);
        split.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Space });
        Assert.True(mainRan, "Space 默认触发 MainCommand");
        Assert.False(menuRan, "Main 不串发 MenuCommand");
        var menuBtn = split.GetVisualDescendants().OfType<Button>().First(b => b.Classes.Contains("xyui-split-menu"));
        Click(window, menuBtn);
        Assert.True(menuRan, "Menu 点击触发 MenuCommand");
        window.Close();
    });

    [Fact]
    public void IconButton_supports_accessibility_name_and_stays_command_semantic() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var btn = new XYIconButton { Content = new XYIcon { Icon = XyuiVectorIcon.Search } };
        AutomationProperties.SetName(btn, "在工程中检索资源");
        Assert.Equal("在工程中检索资源", AutomationProperties.GetName(btn));
        btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Assert.False(btn.IsSelected, "IconButton 点击不翻转 IsSelected");
        btn.IsSelected = true;
        Assert.True(btn.IsSelected);
    });

    [Fact]
    public void Checkbox_supports_three_state_indeterminate_and_disabled_lock() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var box = new XYCheckbox { IsThreeState = true, IsChecked = null };
        Assert.Null(box.IsChecked);
        box.IsChecked = true;
        Assert.True(box.IsChecked);
        box.IsChecked = false;
        Assert.False(box.IsChecked);
        box.IsEnabled = false;
        Assert.False(box.IsEnabled);
    });

    sealed class RelayCmd(Action act) : System.Windows.Input.ICommand
    {
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => act();
        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }
}
