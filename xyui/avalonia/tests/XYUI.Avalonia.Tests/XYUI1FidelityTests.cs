using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI1FidelityTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI1FidelityTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void XYUI1_uses_registered_vector_marks_instead_of_text_glyphs() => _fx.Run(() =>
    {
        Assert.Equal(10, XyuiVectorIcons.PathData.Count);
        Assert.Equal(10, XyuiVectorIcons.CreateResources().Count);
        Assert.All(XyuiVectorIcons.PathData.Keys, icon => Assert.NotNull(XyuiVectorIcons.Create(icon)));
        Assert.NotNull(Mark(new XYCodeText { Text = "region-id" }, "xyui-code-text-mark").Data);
        Assert.NotNull(Mark(new XYSectionTitle { Text = "属性分组" }, "xyui-section-title-mark").Data);
        Assert.NotNull(Mark(new XYBadge { Text = "草稿" }, "xyui-badge-mark-default").Data);
        Assert.NotNull(Mark(new XYStatusBadge { Text = "已保存", State = XyuiStatusState.Success }, "xyui-status-mark-success").Data);
        Assert.NotNull(Mark(new XYHelpText { Text = "填写说明" }, "xyui-help-text-mark").Data);
        Assert.NotNull(Mark(new XYErrorText { Text = "名称无效" }, "xyui-error-text-mark").Data);
        Assert.NotNull(Mark(new XYWarningText { Text = "尚未保存" }, "xyui-warning-text-mark").Data);
        Assert.NotNull(Mark(new XYEmptyText { Text = "暂无数据" }, "xyui-empty-text-mark").Data);
        Assert.NotNull(Mark(new XYSearchHighlight { Text = "命中内容" }, "xyui-search-highlight-mark").Data);
        Assert.NotNull(new XYIcon { Icon = XyuiVectorIcon.Code }.Data);
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
        Assert.Equal(XyuiVectorIcon.Copy, selectable.CopyIcon);
        Assert.NotNull(Mark(selectable, "xyui-selectable-copy-mark").Data);
        var middle = new XYTruncatedText { Text = "region-7ad21c", Mode = XyuiTruncatedTextMode.Middle };
        Assert.Contains("xyui-truncated-middle", middle.Classes);
    });

    static VectorPath Mark(Control control, string className) => control.GetVisualDescendants().OfType<VectorPath>().Single(x => x.Classes.Contains(className));
}
