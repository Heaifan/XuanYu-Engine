using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

// Phase 2C 关键契约测试：Select/TextArea/SearchField/PasswordField/DatePicker/TimePicker
[Collection("XyuiHeadless")]
public sealed class XYUI2Phase2CContractTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2Phase2CContractTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Select_and_textarea_contracts() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var sel = new XYSelect { ItemsSource = new[] { "Vulkan", "D3D12", "Metal" }, SelectedIndex = 0 };
        var area = new XYTextArea { Mode = XYTextAreaMode.Editor, Text = "line1\nline2" };
        var window = XyuiBatchTestHost.Show(new StackPanel { Children = { sel, area } });
        Assert.False(sel.IsEditable);
        Assert.Equal("Vulkan", sel.SelectedItem);
        sel.IsDropDownOpen = true;
        Assert.True(sel.IsDropDownOpen);
        sel.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape });
        Assert.False(sel.IsDropDownOpen);
        Assert.Equal(2, area.LineCount);
        area.Focus();
        Assert.Equal(0, area.SelectionStart);
        Assert.Equal(area.Text!.Length, area.SelectionEnd);
        window.Close();
    });

    [Fact]
    public void Search_and_password_contracts() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var search = new XYSearchField { Text = "Engine" };
        var pwd = new XYPasswordField { Password = "Secret123" };
        var window = XyuiBatchTestHost.Show(new StackPanel { Children = { search, pwd } });
        var searchFired = false;
        search.SearchRequested += (_, _) => searchFired = true;
        search.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Enter });
        Assert.True(searchFired);
        search.ClearSearch();
        Assert.Equal(string.Empty, search.Text);
        Assert.Equal("Secret123", pwd.Password);
        pwd.SetRevealed(true);
        Assert.True(pwd.IsRevealed);
        pwd.ForceHidePassword();
        Assert.False(pwd.IsRevealed);
        window.Close();
    });

    [Fact]
    public void Date_and_time_picker_contracts() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var date = new XYDatePicker { SelectedDate = new DateOnly(2026, 8, 12), MinDate = new DateOnly(2026, 1, 1), MaxDate = new DateOnly(2026, 12, 31) };
        var time = new XYTimePicker { Time = new TimeOnly(14, 30, 25), ShowSeconds = true };
        var window = XyuiBatchTestHost.Show(new StackPanel { Children = { date, time } });
        date.ChangeDays(1);
        Assert.Equal(new DateOnly(2026, 8, 13), date.SelectedDate);
        date.OpenCalendar();
        Assert.True(date.IsCalendarOpen);
        date.CloseCalendarForLifecycle();
        Assert.False(date.IsCalendarOpen);
        Assert.Equal(25, time.Time.Second);
        time.ShowSeconds = false;
        Assert.False(time.ShowSeconds);
        time.SetSegment(XYTimeSegment.Hour, 25);
        Assert.Equal(1, time.Time.Hour);
        window.Close();
    });
}
