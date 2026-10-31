using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;
namespace XYUI.Avalonia.Tests;
[Collection("XyuiHeadless")]
public sealed class XYUI2InputControlsTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2InputControlsTests(XyuiHeadlessFixture fx) => _fx = fx;
    [Fact]
    public void TextField_exposes_editing_contract() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYTextField { Placeholder = "名称", Text = "对象" };
        var window = XyuiBatchTestHost.Show(field); Assert.Equal("对象", field.Text); Assert.Equal("名称", field.Placeholder); Assert.Equal(32, field.Bounds.Height);
        Assert.Contains(field.GetVisualDescendants(), x => x is TextPresenter p && p.Name == "PART_TextPresenter");
        Assert.Contains(field.GetVisualDescendants(), x => x is Border b && b.Name == "PART_FocusEdge"); window.Close();
    });
    [Fact]
    public void TextField_focus_edge_is_an_overlay() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYTextField { Width = 200, Text = "Northern Region" };
        var window = XyuiBatchTestHost.Show(field); var edge = field.GetVisualDescendants().OfType<Border>().Single(x => x.Name == "PART_FocusEdge");
        var text = field.GetVisualDescendants().OfType<TextPresenter>().Single(); var before = text.Bounds;
        Assert.Equal(3, edge.Height); Assert.Equal(0, edge.Opacity); field.Focus();
        Assert.Equal(1, edge.Opacity); Assert.Equal(before.Y, text.Bounds.Y, 0.25); window.Close();
    });
    [Fact]
    public void TextField_text_and_placeholder_share_centered_left_layout() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYTextField { Width = 220, Placeholder = "输入名称" };
        var window = XyuiBatchTestHost.Show(field); var text = field.GetVisualDescendants().OfType<TextPresenter>().Single();
        var placeholder = field.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Name == "PART_Placeholder");
        Assert.Equal(TextAlignment.Left, field.TextAlignment); Assert.Equal(TextAlignment.Left, text.TextAlignment); Assert.Equal(TextAlignment.Left, placeholder.TextAlignment);
        var textCenter = text.TranslatePoint(new Point(text.Bounds.Width / 2, text.Bounds.Height / 2), field)!.Value;
        var placeholderCenter = placeholder.TranslatePoint(new Point(placeholder.Bounds.Width / 2, placeholder.Bounds.Height / 2), field)!.Value;
        Assert.Equal(field.Bounds.Height / 2, textCenter.Y, 0.25); Assert.Equal(textCenter.Y, placeholderCenter.Y, 0.25); Assert.Equal(textCenter.X - text.Bounds.Width / 2, placeholderCenter.X - placeholder.Bounds.Width / 2, 0.25); window.Close();
    });
    [Fact]
    public void TextField_first_focus_selects_editable_text_but_not_readonly() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYTextField { Text = "Northern Region" }; var window = XyuiBatchTestHost.Show(field);
        Assert.Equal(field.SelectionStart, field.SelectionEnd); field.Focus(); Assert.Equal(0, field.SelectionStart); Assert.Equal(field.Text?.Length, field.SelectionEnd);
        var readOnly = new XYTextField { Text = "只读属性", IsReadOnly = true }; var second = XyuiBatchTestHost.Show(readOnly);
        readOnly.Focus(); Assert.Equal(readOnly.SelectionStart, readOnly.SelectionEnd); window.Close(); second.Close();
    });
    [Fact]
    public void TextField_first_mouse_click_selects_all_after_native_pointer_release() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYTextField { Width = 220, Text = "Northern Region" }; var window = XyuiBatchTestHost.Show(field);
        var point = field.TranslatePoint(new Point(field.Bounds.Width / 2, field.Bounds.Height / 2), window)!.Value;
        window.MouseDown(point, MouseButton.Left); Dispatcher.UIThread.RunJobs(); window.MouseUp(point, MouseButton.Left); Dispatcher.UIThread.RunJobs();
        Assert.Equal(0, field.SelectionStart); Assert.Equal(field.Text?.Length, field.SelectionEnd); window.Close();
    });
    [Fact]
    public void TextField_selection_has_visible_theme_contrast() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYTextField { Text = "Northern Region" }; var window = XyuiBatchTestHost.Show(field);
        Assert.NotNull(field.SelectionBrush); Assert.NotNull(field.Background); Assert.NotEqual(XyuiBatchTestHost.ColorOf(field.Background), XyuiBatchTestHost.ColorOf(field.SelectionBrush));
        Assert.NotNull(field.SelectionForegroundBrush); window.Close();
    });
    [Fact]
    public void NumberField_clamps_one_shared_value() => _fx.Run(() =>
    {
        var field = new XYNumberField { Minimum = 0, Maximum = 10, Value = 20, Step = 2 };
        Assert.Equal(10, field.Value); field.Adjust(-2); Assert.Equal(8, field.Value);
    });
    [Fact]
    public void Slider_uses_real_number_field_and_shared_value() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var slider = new XYSlider { Value = 20 }; var window = XyuiBatchTestHost.Show(slider);
        slider.Value = 60; Assert.Equal(60, slider.Value); Assert.Contains(slider.GetVisualDescendants(), x => x is XYNumberField); window.Close();
    });
    [Fact]
    public void Combo_is_editable_but_select_is_fixed() => _fx.Run(() =>
    {
        var combo = new XYComboBox(); var select = new XYSelect(); Assert.True(combo.IsEditable); Assert.False(select.IsEditable);
        Assert.IsType<XYComboBox>(XYUI2GalleryCatalog.CreatePreview("XYUI-2-12").GetVisualDescendants().OfType<XYComboBox>().First());
    });
    [Fact]
    public void TextArea_is_multiline_and_counts_real_text() => _fx.Run(() =>
    {
        var area = new XYTextArea { Text = "第一行\n第二行", Mode = XYTextAreaMode.Editor }; Assert.True(area.AcceptsReturn); Assert.Equal(2, area.LineCount); Assert.Equal(7, area.CharacterCount);
        Assert.IsType<XYTextArea>(XYUI2GalleryCatalog.CreatePreview("XYUI-2-14").GetVisualDescendants().OfType<XYTextArea>().First());
    });
}
