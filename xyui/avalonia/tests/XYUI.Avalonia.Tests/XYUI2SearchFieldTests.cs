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
    public void SearchField_has_real_editor_square_filter_and_chinese_gallery() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var preview = XYUI2GalleryCatalog.CreatePreview("XYUI-2-15"); var fields = preview.GetVisualDescendants().OfType<XYSearchField>().ToArray(); Assert.Equal(2, fields.Length);
        var field = new XYSearchField { Width = 360, Text = "区域" }; var window = XyuiBatchTestHost.Show(field);
        Assert.IsType<Button>(field.ClearActionPart); Assert.IsType<Button>(field.FilterPart); Assert.Equal(32, field.FilterPart!.Bounds.Width); Assert.Equal(32, field.FilterPart.Bounds.Height); field.Text = "区域"; Dispatcher.UIThread.RunJobs(); Assert.True(field.ClearActionPart!.IsVisible); window.Close();
    });

    [Fact]
    public void SearchField_first_focus_selects_all_and_typing_replaces_text() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYSearchField { Width = 280, Text = "北部区域" }; var window = XyuiBatchTestHost.Show(field); field.Focus(); Assert.Equal(0, field.SelectionStart); Assert.Equal(field.Text?.Length, field.SelectionEnd); window.KeyTextInput("南部"); Dispatcher.UIThread.RunJobs(); Assert.Equal("南部", field.Text); window.Close();
    });

    [Fact]
    public void SearchField_filter_popup_is_real_open_close_and_independent_from_active_state() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var filterContent = new StackPanel { Children = { new XYCheckbox { Content = "仅显示已启用" } } }; var field = new XYSearchField { Width = 280, Text = "区域", FilterContent = filterContent }; var window = XyuiBatchTestHost.Show(field); var filters = 0; field.FilterRequested += (_, _) => filters++;
        field.FilterPart!.RaiseEvent(new RoutedEventArgs { RoutedEvent = Button.ClickEvent }); Dispatcher.UIThread.RunJobs(); Assert.Equal(1, filters); Assert.True(field.IsFilterOpen); Assert.True(field.FilterPopupPart!.IsOpen); Assert.Single(filterContent.GetVisualDescendants().OfType<XYCheckbox>()); Assert.False(field.FilterActive); field.FilterActive = true; Assert.True(field.IsFilterOpen); Assert.True(field.FilterActive);
        field.FilterPart.RaiseEvent(new RoutedEventArgs { RoutedEvent = Button.ClickEvent }); Assert.False(field.IsFilterOpen); Assert.False(field.FilterPopupPart.IsOpen); field.IsFilterOpen = true; Dispatcher.UIThread.RunJobs(); window.MouseDown(new Point(20, 180), MouseButton.Left); Dispatcher.UIThread.RunJobs(); Assert.False(field.IsFilterOpen); field.IsFilterOpen = true; Raise(field, Key.Escape); Assert.False(field.IsFilterOpen); window.Content = null; Assert.False(field.IsFilterOpen); window.Close();
    });

    [Fact]
    public void SearchField_enter_requests_search_and_escape_clears_query() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYSearchField { Width = 280, Text = "区域" }; var window = XyuiBatchTestHost.Show(field); var searches = 0; field.SearchRequested += (_, _) => searches++; Raise(field, Key.Enter); Assert.Equal(1, searches); Raise(field, Key.Escape); Assert.Equal("", field.Text); window.Close();
    });

    [Fact]
    public void SearchField_clear_clears_text_and_preserves_focus() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYSearchField { Width = 280, Text = "区域" }; var window = XyuiBatchTestHost.Show(field); field.Focus(); field.ClearActionPart!.RaiseEvent(new RoutedEventArgs { RoutedEvent = Button.ClickEvent }); Dispatcher.UIThread.RunJobs(); Assert.Equal("", field.Text); Assert.True(field.IsFocused); window.Close();
    });

    [Fact]
    public void SearchField_disabled_blocks_clear_and_filter() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYSearchField { Width = 280, Text = "区域", IsEnabled = false, FilterContent = new XYCheckbox { Content = "筛选" } }; var window = XyuiBatchTestHost.Show(field); var filters = 0; field.FilterRequested += (_, _) => filters++; Assert.False(field.ClearActionPart!.IsVisible); Assert.False(field.FilterPart!.IsEnabled); field.ClearSearch(); field.RequestFilter(); Assert.Equal("区域", field.Text); Assert.Equal(0, filters); Assert.False(field.IsFilterOpen); window.Close();
    });

    static void Raise(XYSearchField field, Key key) => field.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = key });
}
