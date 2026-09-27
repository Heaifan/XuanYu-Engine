using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3ContextToolbarLifecycleTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3ContextToolbarLifecycleTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Minimized_owner_closes_context_popup() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var board = new XYContextDropdownBoard("绘制", [new("point", "点")], new Dictionary<string, IReadOnlyList<XYContextAction>> { ["point"] = [new("marker", "点标记")] });
        var anchor = new XYSplitButton();
        var window = XyuiBatchTestHost.Show(new StackPanel { Children = { anchor, board } });
        board.Open(anchor);
        window.WindowState = WindowState.Minimized;
        Dispatcher.UIThread.RunJobs();
        Assert.False(board.IsOpen);
        Assert.False(board.Popup.IsOpen);
        window.Close();
    });
}
