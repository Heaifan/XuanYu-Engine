using Avalonia.Controls;
using Avalonia.Input;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3Batch05StructureTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3Batch05StructureTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void CommandBar_is_compact_and_executes_once() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var host = XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.17"); var bar = Assert.IsType<XYCommandBar>(host.GetVisualDescendants().First()); var count = 0; bar.CommandExecuted += (_, _) => count++; bar.Items[0].RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent)); bar.MoreButton.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent)); Assert.Equal(1, count); Assert.True(bar.MorePopup.IsOpen); Assert.Equal(34, bar.Height); Assert.Equal(XYCommandRole.Primary, bar.Items[0].Role); Assert.Equal(XyuiVectorIcon.Add, bar.Items[0].Icon);
    });

    [Fact] public void CommandBar_many_commands_do_not_overlap() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var bar = new XYCommandBar(Enumerable.Range(1, 8).Select(i => new XYCommandItem($"命令{i}")).ToArray()); Assert.Equal(8, bar.Child!.GetVisualDescendants().OfType<XYCommandItem>().Count()); Assert.DoesNotContain(bar.Child.GetVisualDescendants().OfType<Canvas>());
    });

    [Fact] public void CommandBar_more_escape_closes_and_context_replaces_commands() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var bar = new XYCommandBar(new XYCommandItem("编辑")) { }; bar.MoreMenu.Items = [new XYMenuItem { Label = "导出" }]; bar.RefreshMore(); bar.MoreButton.RaiseEvent(new Avalonia.Interactivity.RoutedEventArgs(Button.ClickEvent)); Assert.True(bar.MorePopup.IsOpen); bar.MoreButton.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape }); Assert.False(bar.MorePopup.IsOpen); var contextual = new XYCommandBar(XYCommandBarVariant.Contextual, "roads", new XYCommandItem("编辑")); contextual.UpdateContext("cities", new XYCommandItem("复制")); Assert.Equal("cities", contextual.ContextIdentity); Assert.Equal("复制", contextual.Items.Single().Content is TextBlock text ? text.Text : "");
    });

    [Fact] public void CommandPalette_filters_real_commands() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var palette = Assert.IsType<XYCommandPalette>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.18")); palette.SearchBox.Text = "道路"; Assert.NotEmpty(palette.FilteredCommands); Assert.All(palette.FilteredCommands, x => Assert.Contains("道路", x.Label));
    });

    [Fact] public void BackForward_truncates_forward_history() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var nav = new XYBackForwardNavigation(); nav.Navigate("一"); nav.Navigate("二"); nav.Back(); nav.Navigate("三"); Assert.False(nav.CanGoForward); Assert.Equal("三", nav.CurrentLocation);
    });

    [Fact] public void Workspace_switcher_shares_selection_state() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var switcher = Assert.IsType<XYWorkspaceSwitcher>(XYUI3GalleryCatalog.CreatePreview("XYUI-3-3.20")); switcher.SelectWorkspace("数据编辑"); Assert.Equal("数据编辑", switcher.CurrentWorkspace);
    });
}
