using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2TextAreaTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2TextAreaTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void TextArea_accepts_multiline_and_counts_newlines() => _fx.Run(() =>
    {
        var area = new XYTextArea { Text = "第一行\n第二行", Mode = XYTextAreaMode.Editor }; Assert.True(area.AcceptsReturn); Assert.Equal(2, area.LineCount); Assert.Equal(7, area.CharacterCount); area.Text += "\n"; Assert.Equal(3, area.LineCount);
    });

    [Fact]
    public void TextArea_first_focus_selects_all_but_second_click_places_caret() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var area = new XYTextArea { Width = 240, Text = "First line\nSecond line" }; var window = XyuiBatchTestHost.Show(area); area.Focus(); Assert.Equal(0, area.SelectionStart); Assert.Equal(area.Text?.Length, area.SelectionEnd);
        var point = area.TextPresenterPart!.TranslatePoint(new Point(10, 10), window)!.Value; window.MouseMove(point); window.MouseDown(point, MouseButton.Left); Dispatcher.UIThread.RunJobs(); window.MouseUp(point, MouseButton.Left); Dispatcher.UIThread.RunJobs(); Assert.Equal(area.SelectionStart, area.SelectionEnd); Assert.False(area.SelectionStart == 0 && area.SelectionEnd == area.Text?.Length); window.Close();
    });

    [Fact]
    public void TextArea_auto_grows_from_minimum_using_real_layout() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var area = new XYTextArea { Width = 240, Text = "One line" }; var window = XyuiBatchTestHost.Show(area); var minimum = area.Bounds.Height; area.Text = "One\nTwo\nThree\nFour"; Dispatcher.UIThread.RunJobs(); Assert.Equal(54, minimum); Assert.True(area.Bounds.Height > minimum); window.Close();
    });

    [Fact]
    public void TextArea_stops_at_max_height_and_scrolls_content() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var area = new XYTextArea { Width = 240, MaxHeight = 100, Text = string.Join("\n", Enumerable.Repeat("Long diagnostic line", 12)) }; var window = XyuiBatchTestHost.Show(area); Dispatcher.UIThread.RunJobs(); Assert.True(area.Bounds.Height <= 100); Assert.True(area.ScrollViewerPart!.Extent.Height > area.ScrollViewerPart.Viewport.Height); window.Close();
    });

    [Fact]
    public void TextArea_placeholder_readonly_disabled_and_error_are_distinct() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var area = new XYTextArea { Width = 240, Placeholder = "Describe the issue...", IsReadOnly = true, IsError = true }; var window = XyuiBatchTestHost.Show(area); Assert.Equal("Describe the issue...", area.GetVisualDescendants().OfType<TextBlock>().Single(x => x.Name == "PART_Placeholder").Text); Assert.True(area.IsReadOnly); area.IsEnabled = false; Assert.False(area.IsEnabled); Assert.Contains(":error", area.Classes); window.Close();
    });

    [Fact]
    public void TextArea_editor_bar_exposes_type_and_live_counts() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var area = new XYTextArea { Width = 280, Mode = XYTextAreaMode.Editor, EditorType = "JSON", Text = "{\n  \"mode\": \"balanced\"\n}" }; var window = XyuiBatchTestHost.Show(area); Assert.True(area.EditorBarPart!.IsVisible); Assert.Equal("JSON", area.GetVisualDescendants().Single(x => x.Name == "PART_EditorType").GetValue(TextBlock.TextProperty)); Assert.Equal($"3 行 · {area.CharacterCount} 字符", area.EditorMetadataPart!.Text); window.Close();
    });

    [Fact]
    public void TextArea_gallery_has_real_standard_and_editor_examples() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var preview = XYUI2GalleryCatalog.CreatePreview("XYUI-2-14"); Assert.True(preview.GetVisualDescendants().OfType<XYTextArea>().Count() >= 8); Assert.Contains(preview.GetVisualDescendants().OfType<XYTextArea>(), x => x.Mode == XYTextAreaMode.Editor && x.EditorType == "JSON");
    });
}
