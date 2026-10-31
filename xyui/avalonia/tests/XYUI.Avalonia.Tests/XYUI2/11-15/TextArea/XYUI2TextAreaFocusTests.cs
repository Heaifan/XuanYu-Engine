using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2TextAreaFocusTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2TextAreaFocusTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void TextArea_first_pointer_focus_selects_all() => _fx.Run(() =>
    {
        var area = new XYTextArea { Width = 280, Text = "First line\nSecond line" }; var window = XyuiBatchTestHost.Show(area); Click(window, area, 60, 10);
        Assert.Equal(0, area.SelectionStart); Assert.Equal(area.Text!.Length, area.SelectionEnd); window.Close();
    });

    [Fact]
    public void TextArea_first_keyboard_focus_selects_all() => _fx.Run(() =>
    {
        var before = new Button { Content = "Before" }; var area = new XYTextArea { Width = 280, Text = "First line\nSecond line" }; var host = new StackPanel { Children = { before, area } }; var window = XyuiBatchTestHost.Show(host); before.Focus(); window.KeyPress(Key.Tab, RawInputModifiers.None, PhysicalKey.Tab, "Tab"); Dispatcher.UIThread.RunJobs();
        Assert.True(area.IsFocused); Assert.Equal(0, area.SelectionStart); Assert.Equal(area.Text!.Length, area.SelectionEnd); window.Close();
    });

    [Fact]
    public void TextArea_second_click_does_not_select_all() => _fx.Run(() =>
    {
        var area = new XYTextArea { Width = 280, Text = "First line\nSecond line" }; var window = XyuiBatchTestHost.Show(area); area.Focus(); Click(window, area, 60, 10);
        Assert.Equal(area.SelectionStart, area.SelectionEnd); Assert.NotEqual(area.Text!.Length, area.SelectionEnd); window.Close();
    });

    [Fact]
    public void TextArea_second_click_allows_caret() => _fx.Run(() =>
    {
        var area = new XYTextArea { Width = 280, Text = "First line\nSecond line" }; var window = XyuiBatchTestHost.Show(area); area.Focus(); Click(window, area, 70, 10);
        Assert.InRange(area.CaretIndex, 1, area.Text!.Length - 1); Assert.Equal(area.CaretIndex, area.SelectionStart); Assert.Equal(area.CaretIndex, area.SelectionEnd); window.Close();
    });

    [Fact]
    public void TextArea_refocus_selects_all_again() => _fx.Run(() =>
    {
        var area = new XYTextArea { Width = 280, Text = "First line\nSecond line" }; var other = new Button { Content = "Other" }; var host = new StackPanel { Children = { area, other } }; var window = XyuiBatchTestHost.Show(host); area.Focus(); Click(window, area, 70, 10); other.Focus(); area.Focus();
        Assert.Equal(0, area.SelectionStart); Assert.Equal(area.Text!.Length, area.SelectionEnd); window.Close();
    });

    [Fact]
    public void TextArea_typing_after_first_focus_replaces_all() => _fx.Run(() =>
    {
        var area = new XYTextArea { Width = 280, Text = "First line\nSecond line" }; var window = XyuiBatchTestHost.Show(area); area.Focus(); window.KeyTextInput("abc"); Dispatcher.UIThread.RunJobs();
        Assert.Equal("abc", area.Text); window.Close();
    });

    [Fact]
    public void TextArea_multiline_select_all_covers_entire_text() => _fx.Run(() =>
    {
        var area = new XYTextArea { Width = 280, Text = "第一行\n第二行\n" }; var window = XyuiBatchTestHost.Show(area); area.Focus();
        Assert.Equal(0, area.SelectionStart); Assert.Equal(area.Text!.Length, area.SelectionEnd); Assert.Equal(3, area.LineCount); window.Close();
    });

    static void Click(Window window, XYTextArea area, double x, double y) { var point = area.TextPresenterPart!.TranslatePoint(new Point(x, y), window)!.Value; window.MouseMove(point); window.MouseDown(point, MouseButton.Left); Dispatcher.UIThread.RunJobs(); window.MouseUp(point, MouseButton.Left); Dispatcher.UIThread.RunJobs(); }
}
