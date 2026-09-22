using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3ContextToolbarBoardTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3ContextToolbarBoardTests(XyuiHeadlessFixture fx) => _fx = fx;
    [Fact] public void Enter_executes_action() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var board = TestData.Board(); var executed = ""; board.ActionExecuted += (_, action) => executed = action.Label; board.RaiseEvent(TestData.Key(Key.Enter)); Assert.Equal("点标记", executed); });
    [Fact] public void Escape_closes_board() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var board = TestData.Board(); board.Open(); board.RaiseEvent(TestData.Key(Key.Escape)); Assert.False(board.IsOpen); });
    [Fact] public void Outside_click_closes_board() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var board = TestData.Board(); board.Open(); board.DismissOutside(); Assert.False(board.IsOpen); });
    [Fact] public void Repeated_open_close_has_no_visual_residue() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var board = TestData.Board(); var child = board.Popup.Child; board.Open(); board.Close(); board.Open(); Assert.Same(child, board.Popup.Child); Assert.Same(board, board.Popup.PlacementTarget); });
    [Fact] public void Dropdown_board_uses_real_menu_and_cascade_items() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var board = TestData.Board(); Assert.Equal(3, board.Menu.Items.Count); Assert.Equal(3, board.SubMenus.Count); Assert.All(board.Menu.Items.OfType<XYMenuItem>(), item => Assert.True(item.HasSubMenu)); Assert.All(board.SubMenus, submenu => Assert.NotEmpty(submenu.ChildMenu.Items)); });
    [Fact] public void Dropdown_board_opens_hovered_submenu() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var board = TestData.Board(); board.Open(); board.Menu.Items.OfType<XYMenuItem>().ElementAt(1).OpenSubMenu(); Assert.True(board.SubMenus[1].EffectiveVisible); Assert.Equal("道路", board.SubMenus[1].ChildMenu.Items.OfType<XYMenuItem>().First().Label); board.Close(); });
    [Fact] public void Dropdown_popup_host_does_not_change_gallery_flow() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var preview = XYUI3GalleryCatalog.CreatePreview(XYUI3GalleryCatalog.ContextToolbarId); var window = XyuiBatchTestHost.Show(preview); Dispatcher.UIThread.RunJobs(); var bars = preview.GetVisualDescendants().OfType<XYContextToolbar>().ToArray(); var before = bars[2].Bounds.Top; var board = preview.GetVisualDescendants().OfType<XYContextDropdownBoard>().ElementAt(1); Assert.True(board.IsOpen); Assert.True(board.Bounds.Height <= 2); board.Close(); Dispatcher.UIThread.RunJobs(); board.Open(bars[1].GetVisualDescendants().OfType<XYSplitButton>().Single()); Dispatcher.UIThread.RunJobs(); Assert.Equal(before, bars[2].Bounds.Top); Assert.True(board.Bounds.Height <= 2); window.Close(); });
    [Fact] public void Gallery_context_toolbar_section_uses_real_components() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var preview = XYUI3GalleryCatalog.CreatePreview(XYUI3GalleryCatalog.ContextToolbarId); var document = XYUI3DocumentationCatalog.ContextToolbarDocument(); Assert.Equal(3, preview.GetVisualDescendants().OfType<XYContextToolbar>().Count()); Assert.True(document.HasLiveExamples); Assert.Contains(preview.GetVisualDescendants().OfType<XYContextDropdownBoard>(), board => board.Popup.IsLightDismissEnabled); });
    [Fact] public void Gallery_live_and_composition_do_not_duplicate_demos() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var document = XYUI3DocumentationCatalog.ContextToolbarDocument(); var live = document.LiveExamplesFactory!(); var composition = document.CompositionFactory!(); Assert.Equal(3, live.GetVisualDescendants().OfType<XYContextToolbar>().Count()); Assert.Single(composition.GetVisualDescendants().OfType<XYContextToolbar>()); });
    [Fact] public void Gallery_open_demo_shows_board_after_layout() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var preview = XYUI3GalleryCatalog.CreatePreview(XYUI3GalleryCatalog.ContextToolbarId); var window = XyuiBatchTestHost.Show(preview); Dispatcher.UIThread.RunJobs(); var boards = preview.GetVisualDescendants().OfType<XYContextDropdownBoard>().ToArray(); Assert.True(boards[1].IsOpen); window.Close(); });
    [Fact] public void Toolbar_groups_fit_inside_toolbar_bounds() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var toolbar = new XYContextToolbar(new XYContextGroup("编辑", new XYButton { Content = "选择", Height = 32 })); var window = XyuiBatchTestHost.Show(toolbar); Dispatcher.UIThread.RunJobs(); var group = toolbar.GetVisualDescendants().OfType<XYContextGroup>().Single(); var button = group.GetVisualDescendants().OfType<XYButton>().Single(); Assert.True(group.Bounds.Bottom <= toolbar.Bounds.Height + 0.1); Assert.True(button.Bounds.Bottom <= group.Bounds.Height + 0.1); window.Close(); });
}

static class TestData
{
    internal static XYContextDropdownBoard Board() => new("绘制", [new("point", "点"), new("line", "线"), new("area", "面")], new Dictionary<string, IReadOnlyList<XYContextAction>> { ["point"] = [new("marker", "点标记"), new("poi", "兴趣点")], ["line"] = [new("road", "道路"), new("boundary", "边界线"), new("river", "河流")], ["area"] = [new("region", "区域"), new("blocked", "禁行区"), new("parcel", "地块")] });
    internal static KeyEventArgs Key(Key key) => new() { RoutedEvent = InputElement.KeyDownEvent, Key = key };
    internal sealed class Command(Action<object?> execute) : System.Windows.Input.ICommand { public event EventHandler? CanExecuteChanged { add { } remove { } } public bool CanExecute(object? p) => true; public void Execute(object? p) => execute(p); }
}
