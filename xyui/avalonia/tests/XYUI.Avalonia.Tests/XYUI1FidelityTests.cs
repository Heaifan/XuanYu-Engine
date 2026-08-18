using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI1FidelityTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI1FidelityTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Inline_components_expose_their_semantic_marks() => _fx.Run(() =>
    {
        Assert.Contains("</>", ChildText(new XYCodeText { Text = "region-id" }));
        Assert.Contains("▌", ChildText(new XYSectionTitle { Text = "属性分组" }));
        Assert.Contains("◀", ChildText(new XYBadge { Text = "草稿" }));
        Assert.Contains("●", ChildText(new XYStatusBadge { Text = "已保存" }));
        Assert.Contains("ⓘ", ChildText(new XYHelpText { Text = "填写说明" }));
        Assert.Contains("✕", ChildText(new XYErrorText { Text = "名称无效" }));
        Assert.Contains("△", ChildText(new XYWarningText { Text = "尚未保存" }));
        Assert.Contains("—", new XYEmptyText { Text = "暂无数据" }.Text);
        Assert.Contains("⌕", new XYSearchHighlight { Text = "命中内容" }.Text);
    });

    [Fact]
    public void Icon_stroke_width_follows_size_variant() => _fx.Run(() =>
    {
        var icon = new XYIcon { Size = XyuiIconSize.Tiny };
        Assert.Equal(1d, icon.StrokeWidth);
        icon.Size = XyuiIconSize.Small; Assert.Equal(1.25d, icon.StrokeWidth);
        icon.Size = XyuiIconSize.Default; Assert.Equal(1.5d, icon.StrokeWidth);
        icon.Size = XyuiIconSize.Large; Assert.Equal(1.75d, icon.StrokeWidth);
    });

    [Fact]
    public void Rich_selectable_mono_and_truncation_contracts_are_present() => _fx.Run(() =>
    {
        var rich = new XYRichText { Text = "普通", StrongText = "重点", MonoText = "region-id" };
        Assert.True(rich.Inlines?.Count >= 3);
        Assert.Null(new XYMonoText { Text = "X=1" }.Background);
        var selectable = new XYSelectableText { Text = "可复制" };
        Assert.Contains("xyui-selectable-text", selectable.Classes);
        Assert.Equal("⧉", selectable.CopyGlyph);
        var middle = new XYTruncatedText { Text = "region-7ad21c", Mode = XyuiTruncatedTextMode.Middle };
        Assert.Contains("xyui-truncated-middle", middle.Classes);
    });

    static string ChildText(Border control) => ((TextBlock)control.Child!).Text ?? "";
}
