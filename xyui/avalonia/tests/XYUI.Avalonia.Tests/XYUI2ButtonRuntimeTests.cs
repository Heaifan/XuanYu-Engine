using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

// XYUI-2-01 Button 运行时合同：Variant→class、Action Edge 存在性与状态填色（弱化/语义/衰减）。
[Collection("XyuiHeadless")]
public sealed class XYUI2ButtonRuntimeTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2ButtonRuntimeTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Variant_drives_class_and_default_is_primary() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var button = new XYButton { Content = "新建" };
        Assert.Contains("xyui-button-primary", button.Classes);
        button.Variant = XyuiButtonVariant.Danger;
        Assert.Contains("xyui-button-danger", button.Classes);
        Assert.DoesNotContain("xyui-button-primary", button.Classes);
        button.Variant = XyuiButtonVariant.Secondary;
        Assert.Contains("xyui-button-secondary", button.Classes);
    });

    [Fact]
    public void Primary_edge_exists_with_3dip_accent_and_hover_4dip() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var button = new XYButton { Content = "新建" };
        var window = XyuiBatchTestHost.Show(button);
        var edge = XyuiBatchTestHost.Edge(button);
        Assert.True(edge.IsVisible);
        Assert.Equal(XyuiActionEdge.DefaultHeight, edge.Height);
        Assert.Equal(XyuiBatchTestHost.Token("XY.Accent.Strong"), XyuiBatchTestHost.ColorOf(edge.Background));
        XyuiBatchTestHost.Hover(window, button);
        Assert.Equal(XyuiActionEdge.HoverHeight, edge.Height);
        window.Close();
    });

    [Fact]
    public void Secondary_keeps_weakened_edge_using_divider_token() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var button = new XYButton { Content = "取消", Variant = XyuiButtonVariant.Secondary };
        var window = XyuiBatchTestHost.Show(button);
        var edge = XyuiBatchTestHost.Edge(button);
        Assert.True(edge.IsVisible, "Secondary 必须保留弱化 Action Edge，不得退回取消方案");
        Assert.Equal(XyuiBatchTestHost.Token("XY.Divider.Default"), XyuiBatchTestHost.ColorOf(edge.Background));
        window.Close();
    });

    [Fact]
    public void Danger_uses_error_border_and_error_edge() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var button = new XYButton { Content = "删除", Variant = XyuiButtonVariant.Danger };
        var window = XyuiBatchTestHost.Show(button);
        var edge = XyuiBatchTestHost.Edge(button);
        Assert.Equal(XyuiBatchTestHost.Token("XY.Semantic.Error.Border"), XyuiBatchTestHost.ColorOf(button.BorderBrush));
        Assert.Equal(XyuiBatchTestHost.Token("XY.Semantic.Error.Text"), XyuiBatchTestHost.ColorOf(edge.Background));
        window.Close();
    });

    [Fact]
    public void Disabled_attenuates_chrome_and_edge() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var button = new XYButton { Content = "保存", IsEnabled = false };
        var window = XyuiBatchTestHost.Show(button);
        var edge = XyuiBatchTestHost.Edge(button);
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Background"), XyuiBatchTestHost.ColorOf(button.Background));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Text"), XyuiBatchTestHost.ColorOf(button.Foreground));
        Assert.Equal(XyuiBatchTestHost.Token("XY.State.Disabled.Border"), XyuiBatchTestHost.ColorOf(edge.Background));
        window.Close();
    });
}




