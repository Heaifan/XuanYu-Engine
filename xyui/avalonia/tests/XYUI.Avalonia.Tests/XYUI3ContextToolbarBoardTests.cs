using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
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
    [Fact] public void Dropdown_board_does_not_use_cascading_popup() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var board = TestData.Board(); Assert.NotNull(board.Popup); Assert.Empty(board.GetVisualDescendants().OfType<XYMenu>()); Assert.Empty(board.GetVisualDescendants().OfType<XYSubMenu>()); });
    [Fact] public void Gallery_context_toolbar_section_uses_real_components() => _fx.Run(() => { XyuiBatchTestHost.Prepare(); var preview = XYUI3GalleryCatalog.CreatePreview(XYUI3GalleryCatalog.ContextToolbarId); var document = XYUI3DocumentationCatalog.ContextToolbarDocument(); Assert.Equal(3, preview.GetVisualDescendants().OfType<XYContextToolbar>().Count()); Assert.True(document.HasLiveExamples); Assert.Contains(preview.GetVisualDescendants().OfType<XYContextDropdownBoard>(), board => board.Popup.IsLightDismissEnabled); });
}

static class TestData
{
    internal static XYContextDropdownBoard Board() => new("绘制", [new("point", "点"), new("line", "线"), new("area", "面")], new Dictionary<string, IReadOnlyList<XYContextAction>> { ["point"] = [new("marker", "点标记"), new("poi", "兴趣点")], ["line"] = [new("road", "道路"), new("boundary", "边界线"), new("river", "河流")], ["area"] = [new("region", "区域"), new("blocked", "禁行区"), new("parcel", "地块")] });
    internal static KeyEventArgs Key(Key key) => new() { RoutedEvent = InputElement.KeyDownEvent, Key = key };
    internal sealed class Command(Action<object?> execute) : System.Windows.Input.ICommand { public event EventHandler? CanExecuteChanged { add { } remove { } } public bool CanExecute(object? p) => true; public void Execute(object? p) => execute(p); }
}
