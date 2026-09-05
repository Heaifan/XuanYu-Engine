using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

// Phase 2B 关键契约测试：Radio互斥/Switch切换/TextField首焦全选与二次Caret/NumberField边界回退/Slider边界/ComboBox过滤与Popup。
[Collection("XyuiHeadless")]
public sealed class XYUI2Phase2BContractTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2Phase2BContractTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void RadioButton_group_exclusivity_and_switch_toggle() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var r1 = new XYRadioButton { GroupName = "g1", Content = "A", IsChecked = true };
        var r2 = new XYRadioButton { GroupName = "g1", Content = "B" };
        var sw = new XYSwitch { Content = "自动保存", IsChecked = false };
        var panel = new StackPanel { Children = { r1, r2, sw } };
        var window = XyuiBatchTestHost.Show(panel);
        Assert.True(r1.IsChecked); Assert.False(r2.IsChecked);
        r2.IsChecked = true;
        Assert.False(r1.IsChecked, "切换后 r1 应自动取消勾选");
        Assert.True(r2.IsChecked);
        sw.IsChecked = true;
        Assert.True(sw.IsChecked);
        window.Close();
    });

    [Fact]
    public void TextField_first_focus_selects_all_and_second_click_places_caret() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var field = new XYTextField { Width = 200, Text = "EngineScene", Placeholder = "请输入" };
        var window = XyuiBatchTestHost.Show(field);
        field.Focus();
        Assert.Equal(0, field.SelectionStart);
        Assert.Equal(field.Text!.Length, field.SelectionEnd);
        var pt = field.TranslatePoint(new Point(60, 16), window)!.Value;
        window.MouseDown(pt, MouseButton.Left); Dispatcher.UIThread.RunJobs();
        window.MouseUp(pt, MouseButton.Left); Dispatcher.UIThread.RunJobs();
        Assert.Equal(field.SelectionStart, field.SelectionEnd);
        window.Close();
    });

    [Fact]
    public void NumberField_clamps_min_max_and_escape_reverts() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var nf = new XYNumberField { Minimum = 0, Maximum = 100, Value = 50 };
        var window = XyuiBatchTestHost.Show(nf);
        nf.Value = 150; Assert.Equal(100, nf.Value);
        nf.Value = -50; Assert.Equal(0, nf.Value);
        nf.Value = 25; nf.Focus();
        nf.Text = "88";
        nf.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape });
        Assert.Equal("25.00", nf.Text);
        window.Close();
    });

    [Fact]
    public void Slider_bounds_0_50_100_and_combobox_filter_escape_popup() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var slider = new XYSlider { Minimum = 0, Maximum = 100, Value = 0 };
        var combo = new XYComboBox { ItemsSource = new[] { "Vulkan", "Direct3D12", "Metal", "OpenGL" } };
        var window = XyuiBatchTestHost.Show(new StackPanel { Children = { slider, combo } });
        slider.Value = 50; Assert.Equal(50, slider.NumberFieldPart!.Value);
        slider.Value = 100; Assert.Equal(100, slider.NumberFieldPart!.Value);
        combo.TextFieldPart!.Text = "dir";
        Assert.Single(combo.FilteredItems);
        combo.IsDropDownOpen = true;
        combo.TextFieldPart.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape });
        Assert.False(combo.IsDropDownOpen, "Escape 应关闭 Popup");
        window.Close();
    });
}
