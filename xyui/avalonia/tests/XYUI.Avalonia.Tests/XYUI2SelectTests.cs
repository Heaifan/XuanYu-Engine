using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2SelectTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    static readonly string[] Items = ["English", "简体中文", "日本語", "繁體中文"];
    public XYUI2SelectTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Select_is_fixed_and_uses_its_own_split_template() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var select = new XYSelect { Width = 220, ItemsSource = Items, SelectedIndex = 0 }; var window = XyuiBatchTestHost.Show(select);
        Assert.False(select.IsEditable); Assert.Equal(30, select.Bounds.Height); Assert.Equal(36, select.ChevronCellPart!.Bounds.Width); Assert.NotNull(select.ValuePart); Assert.IsType<XYIcon>(select.ChevronPart); Assert.DoesNotContain(select.GetVisualDescendants(), x => x is XYTextField); window.Close();
    });

    [Fact]
    public void Select_opens_from_value_and_chevron_surfaces() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var select = new XYSelect { Width = 220, ItemsSource = Items }; var window = XyuiBatchTestHost.Show(select);
        Click(window, select, 10); Assert.True(select.IsDropDownOpen); select.IsDropDownOpen = false; Click(window, select, select.Bounds.Width - 8); Assert.True(select.IsDropDownOpen); window.Close();
    });

    [Fact]
    public void Select_selection_updates_value_and_closes_popup() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var select = new XYSelect { ItemsSource = Items }; var window = XyuiBatchTestHost.Show(select); select.IsDropDownOpen = true;
        select.ListPart!.SelectedIndex = 2; Assert.Equal(2, select.SelectedIndex); Assert.Equal("日本語", select.SelectedItem); Assert.Equal("日本語", select.ValuePart!.Text); Assert.False(select.IsDropDownOpen); Assert.False(select.PopupPart!.IsOpen); window.Close();
    });

    [Fact]
    public void Select_placeholder_and_disabled_contracts_hold() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var select = new XYSelect { ItemsSource = Items, Placeholder = "Select language" }; var window = XyuiBatchTestHost.Show(select);
        Assert.Equal("Select language", select.ValuePart!.Text); Assert.Contains("xyui-select-placeholder", select.ValuePart.Classes); select.IsEnabled = false; select.IsDropDownOpen = true; Assert.False(select.IsDropDownOpen); Click(window, select, 10); Assert.False(select.IsDropDownOpen); window.Close();
    });

    [Fact]
    public void Select_keyboard_open_navigate_commit_and_escape() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var select = new XYSelect { ItemsSource = Items }; var window = XyuiBatchTestHost.Show(select); select.Focus(); Raise(select, Key.Enter); Assert.True(select.IsDropDownOpen); Assert.IsType<RotateTransform>(select.ChevronPart!.RenderTransform); Assert.Equal(4, select.ListPart!.ItemCount);
        Raise(select, Key.Down); Assert.Equal(0, select.ListPart.SelectedIndex); Raise(select, Key.Enter); Assert.Equal("English", select.SelectedItem); Assert.False(select.IsDropDownOpen); Raise(select, Key.Space); Assert.True(select.IsDropDownOpen); Raise(select, Key.Escape); Assert.False(select.IsDropDownOpen); Assert.Null(select.ChevronPart.RenderTransform); window.Close();
    });

    [Fact]
    public void Select_light_dismiss_and_detach_reset_open_state() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var select = new XYSelect { ItemsSource = Items }; var window = XyuiBatchTestHost.Show(select); select.IsDropDownOpen = true; window.MouseDown(new Point(430, 180), MouseButton.Left); Dispatcher.UIThread.RunJobs();
        Assert.False(select.IsDropDownOpen); Assert.False(select.PopupPart!.IsOpen); select.IsDropDownOpen = true; window.Content = null; Assert.False(select.IsDropDownOpen); Assert.False(select.PopupPart.IsOpen); window.Close();
    });

    [Fact]
    public void Select_gallery_has_real_fixed_candidate_examples() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var preview = XYUI2GalleryCatalog.CreatePreview("XYUI-2-13");
        Assert.Equal(5, preview.GetVisualDescendants().OfType<XYSelect>().Count()); Assert.Contains(preview.GetVisualDescendants().OfType<XYSelect>(), x => x.Placeholder == "Select status");
    });

    static void Click(Window window, XYSelect select, double x) { var point = select.TranslatePoint(new Point(x, select.Bounds.Height / 2), window)!.Value; window.MouseDown(point, MouseButton.Left); window.MouseUp(point, MouseButton.Left); Dispatcher.UIThread.RunJobs(); }
    static void Raise(XYSelect select, Key key) { select.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = key }); Dispatcher.UIThread.RunJobs(); }
}
