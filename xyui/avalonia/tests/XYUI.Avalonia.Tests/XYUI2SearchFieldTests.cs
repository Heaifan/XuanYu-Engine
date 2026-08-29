using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2SearchFieldTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2SearchFieldTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void SearchField_has_real_editor_clear_filter_and_chinese_gallery() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var preview = XYUI2GalleryCatalog.CreatePreview("XYUI-2-15"); var fields = preview.GetVisualDescendants().OfType<XYSearchField>().ToArray(); Assert.True(fields.Length >= 6);
        var field = new XYSearchField { Width = 360, Text = "区域" }; var window = XyuiBatchTestHost.Show(field);
        Assert.IsType<Button>(field.ClearActionPart); Assert.IsType<Button>(field.FilterPart); Assert.Equal(35, field.FilterPart!.Bounds.Width); field.Text = "区域"; Dispatcher.UIThread.RunJobs(); Assert.True(field.ClearActionPart!.IsVisible); window.Close();
    });

    [Fact]
    public void SearchField_first_focus_selects_all_and_typing_replaces_text() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYSearchField { Width = 280, Text = "北部区域" }; var window = XyuiBatchTestHost.Show(field); field.Focus(); Assert.Equal(0, field.SelectionStart); Assert.Equal(field.Text?.Length, field.SelectionEnd); window.KeyTextInput("南部"); Dispatcher.UIThread.RunJobs(); Assert.Equal("南部", field.Text); window.Close();
    });

    [Fact]
    public void SearchField_enter_search_escape_clear_and_filter_raise_events() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYSearchField { Width = 280, Text = "区域" }; var window = XyuiBatchTestHost.Show(field); var searches = 0; var filters = 0; field.SearchRequested += (_, _) => searches++; field.FilterRequested += (_, _) => filters++;
        Raise(field, Key.Enter); Assert.Equal(1, searches); Raise(field, Key.Escape); Assert.Equal("", field.Text); field.FilterPart!.RaiseEvent(new RoutedEventArgs { RoutedEvent = Button.ClickEvent }); Assert.Equal(1, filters); Assert.True(field.FilterActive); window.Close();
    });

    [Fact]
    public void SearchField_disabled_blocks_clear_and_filter() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYSearchField { Width = 280, Text = "区域", IsEnabled = false }; var window = XyuiBatchTestHost.Show(field); var filters = 0; field.FilterRequested += (_, _) => filters++; Assert.False(field.ClearActionPart!.IsVisible); Assert.False(field.FilterPart!.IsEnabled); field.ClearSearch(); field.RequestFilter(); Assert.Equal("区域", field.Text); Assert.Equal(0, filters); window.Close();
    });

    static void Raise(XYSearchField field, Key key) => field.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = key });
}
