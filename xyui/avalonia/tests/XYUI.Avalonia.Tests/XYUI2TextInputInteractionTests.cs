using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2TextInputInteractionTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2TextInputInteractionTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Editable_text_hosts_select_all_on_focus_and_pointer_activation() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var field = new XYTextField { Width = 220, Text = "Northern Region", Placeholder = "输入名称" };
        var window = XyuiBatchTestHost.Show(field);
        field.Focus();
        Assert.Equal(0, field.SelectionStart); Assert.Equal(field.Text?.Length, field.SelectionEnd);
        var placeholder = field.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Name == "PART_Placeholder");
        Assert.False(placeholder.IsVisible);
        var point = field.TranslatePoint(new Point(90, 16), window)!.Value; window.MouseDown(point, MouseButton.Left); Dispatcher.UIThread.RunJobs(); window.MouseUp(point, MouseButton.Left); Dispatcher.UIThread.RunJobs();
        Assert.Equal(field.SelectionStart, field.SelectionEnd); Assert.NotEqual(field.Text?.Length, field.SelectionEnd); window.Close();
    });

    [Fact]
    public void TextArea_uses_the_same_edit_activation_contract() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var area = new XYTextArea { Width = 240, Text = "第一行\n第二行", Placeholder = "输入内容" };
        var window = XyuiBatchTestHost.Show(area); area.Focus();
        Assert.Equal(0, area.SelectionStart); Assert.Equal(area.Text?.Length, area.SelectionEnd); window.Close();
    });

    [Fact]
    public void ComboBox_text_host_selects_all_on_click() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var combo = new XYComboBox { Width = 240, Text = "Northern Region" };
        var window = XyuiBatchTestHost.Show(combo); var field = combo.TextFieldPart!;
        var point = field.TranslatePoint(new Point(field.Bounds.Width / 2, field.Bounds.Height / 2), window)!.Value;
        window.MouseDown(point, MouseButton.Left); Dispatcher.UIThread.RunJobs(); window.MouseUp(point, MouseButton.Left); Dispatcher.UIThread.RunJobs();
        Assert.Equal(0, field.SelectionStart); Assert.Equal(field.Text?.Length, field.SelectionEnd); window.Close();
    });
}
