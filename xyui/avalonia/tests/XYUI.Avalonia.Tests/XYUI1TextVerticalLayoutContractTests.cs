using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI1TextVerticalLayoutContractTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI1TextVerticalLayoutContractTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Text_primitives_share_single_line_visual_centering() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var controls = new Control[]
        {
            new XYText { Text = "中英 ABC" }, new XYLabel { Text = "字段 ABC" },
            new XYCaption { Text = "说明 ABC" }, new XYHeading { Text = "标题 ABC" }
        };
        var host = new StackPanel { Children = { controls[0], controls[1], controls[2], controls[3] } };
        foreach (var control in controls) { control.Width = 220; control.Height = 34; }
        var window = XyuiBatchTestHost.Show(host);
        Assert.All(controls, control => Assert.IsType<TranslateTransform>(control.RenderTransform));
        window.Close();
    });

    [Fact]
    public void Multiline_text_does_not_receive_single_line_transform() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var caption = new XYCaption { Text = "第一行\n第二行", Width = 220, Height = 40 };
        var window = XyuiBatchTestHost.Show(caption);
        Assert.Null(caption.RenderTransform);
        window.Close();
    });
}
