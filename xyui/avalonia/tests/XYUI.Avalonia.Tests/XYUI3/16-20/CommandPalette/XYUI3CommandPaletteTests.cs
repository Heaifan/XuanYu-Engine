using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3CommandPaletteTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3CommandPaletteTests(XyuiHeadlessFixture fx) => _fx = fx;

    static XYPaletteCommand Command(string id, string label, XYPaletteCommandType type = XYPaletteCommandType.Command, string category = "地图", string shortcut = "Ctrl+D") => new(id, label, type, category, $"详情：{label}", shortcut, [label]);

    [Fact] public void Result_rows_are_full_width_without_button_chrome() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var palette = new XYCommandPalette(Command("a", "创建道路")); var window = XyuiBatchTestHost.Show(palette); var row = palette.GetVisualDescendants().OfType<XYCommandPaletteItem>().Single();
        Assert.Equal(30, row.Height); Assert.Equal(HorizontalAlignment.Stretch, row.HorizontalAlignment); Assert.Equal(new Thickness(0), row.BorderThickness); Assert.DoesNotContain(row.GetVisualDescendants(), x => x is Button); window.Close();
    });

    [Fact] public void Detail_tracks_model_and_scope_prefix() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var commands = new[] { Command("a", "创建道路", category: "编辑", shortcut: "Ctrl+N"), Command("b", "打开设置", XYPaletteCommandType.Setting, "设置", "Ctrl+, ") }; var palette = new XYCommandPalette(commands, [commands[1]]);
        palette.SearchBox.Text = ": 设置"; Assert.Single(palette.FilteredCommands); Assert.Equal(XYPaletteCommandType.Setting, palette.SelectedCommand!.Type); Assert.Equal("设置", palette.SelectedCommand.Category); Assert.Equal("Ctrl+, ", palette.SelectedCommand.Shortcut);
    });

    [Fact] public void Empty_query_uses_recent_items_and_enter_closes_popup() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var all = new[] { Command("a", "创建道路"), Command("b", "验证道路") }; var palette = new XYCommandPalette(all, [all[1]]); var executed = ""; palette.ExecuteRequested += (_, item) => executed = item.Id; var window = XyuiBatchTestHost.Show(palette);
        Assert.Single(palette.FilteredCommands); palette.Open(window); palette.SearchBox.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Enter }); Assert.Equal("b", executed); Assert.False(palette.IsOpen); window.Close();
    });

    [Fact] public void Arrow_navigation_updates_selection_and_consumes_key() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var palette = new XYCommandPalette(Command("a", "创建道路"), Command("b", "验证道路")); palette.SearchBox.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Down }); Assert.Equal("b", palette.SelectedCommand!.Id);
    });

    [Fact] public void Filter_cell_uses_real_scope_menu() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var palette = new XYCommandPalette(Command("a", "创建道路")); var window = XyuiBatchTestHost.Show(palette); var filter = palette.SearchBox.GetVisualDescendants().OfType<Button>().Last();
        filter.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Assert.Same(palette.ScopeMenu, palette.SearchBox.FilterContent); Assert.Equal(5, palette.ScopeMenu.Items.Count); Assert.True(palette.SearchBox.IsFilterOpen); window.Close();
    });
}
