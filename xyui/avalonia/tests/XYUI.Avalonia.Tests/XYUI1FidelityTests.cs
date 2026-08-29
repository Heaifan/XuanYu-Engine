using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;
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
        Assert.Equal(21, XyuiVectorIcons.PathData.Count);
        Assert.Equal(21, XyuiVectorIcons.CreateResources().Count);
        Assert.All(XyuiVectorIcons.PathData.Keys, icon => Assert.NotNull(XyuiVectorIcons.Create(icon)));
        Assert.NotNull(Mark(new XYCodeText { Text = "region-id" }, "xyui-code-text-mark").Data);
        Assert.NotNull(Mark(new XYBadge { Text = "草稿" }, "xyui-badge-mark-default").Data);
        Assert.NotNull(Mark(new XYStatusBadge { Text = "已保存", State = XyuiStatusState.Success }, "xyui-status-mark-success").Data);
        Assert.NotNull(Mark(new XYHelpText { Text = "填写说明" }, "xyui-help-text-mark").Data);
        Assert.NotNull(Mark(new XYErrorText { Text = "名称无效" }, "xyui-error-text-mark").Data);
        Assert.NotNull(Mark(new XYWarningText { Text = "尚未保存" }, "xyui-warning-text-mark").Data);
        Assert.NotNull(Mark(new XYSearchHighlight { Text = "命中内容" }, "xyui-search-highlight-mark").Data);
        Assert.NotNull(new XYIcon { Icon = XyuiVectorIcon.Code }.IconGeometry);
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
    public void Corner_marks_and_badge_shape_follow_precision_contract() => _fx.Run(() =>
    {
        var code = Mark(new XYCodeText { Text = "terrain/main-heightfield" }, "xyui-code-text-mark");
        Assert.Equal(8, code.Width); Assert.Equal(8, code.Height); Assert.Equal(1.25, code.StrokeThickness); Assert.False(code.IsHitTestVisible);
        Assert.Equal(6, Canvas.GetRight(code)); Assert.Equal(5, Canvas.GetBottom(code));
        var search = Mark(new XYSearchHighlight { Text = "Main Terrain Layer" }, "xyui-search-highlight-mark");
        Assert.Equal(8, search.Width); Assert.Equal(8, search.Height); Assert.Equal(1, search.StrokeThickness); Assert.False(search.IsHitTestVisible);
        Assert.Equal(6, Canvas.GetRight(search)); Assert.Equal(5, Canvas.GetTop(search));
        var badge = new XYBadge { Text = "草稿" }; var shape = Mark(badge, "xyui-badge-background-shape");
        Assert.Equal(22, badge.Height); Assert.NotNull(shape.Data); Assert.Contains("xyui-badge-background-shape", shape.Classes);
    });

    [Fact]
    public void Mono_preview_uses_shared_columns_and_row_rhythm() => _fx.Run(() =>
    {
        var grid = Assert.IsType<XYMonoText>(XYUI1GalleryCatalog.CreatePreview("XYUI-1-08"));
        Assert.Equal(5, grid.ColumnDefinitions.Count); Assert.Equal(6, grid.RowDefinitions.Count);
        Assert.Equal(new GridLength(XYMonoText.LabelValueGap), grid.ColumnDefinitions[1].Width);
        Assert.Equal(new GridLength(XYMonoText.ValueUnitGap), grid.ColumnDefinitions[3].Width);
        Assert.All(grid.RowDefinitions, row => Assert.Equal(GridLength.Auto, row.Height));
        Assert.Equal(6, grid.Children.OfType<TextBlock>().Count(x => x.Classes.Contains("xyui-mono-data-label")));
        Assert.Equal(6, grid.Children.OfType<TextBlock>().Count(x => x.Classes.Contains("xyui-mono-data-value")));
        Assert.Equal(6, grid.Children.OfType<TextBlock>().Count(x => x.Classes.Contains("xyui-mono-data-unit")));
    });

    [Fact]
    public void Rich_selectable_mono_and_truncation_contracts_are_present() => _fx.Run(() =>
    {
        var rich = new XYRichText { Text = "普通", StrongText = "重点", MonoText = "region-id" };
        Assert.True(rich.Inlines?.Count >= 3);
        Assert.Null(new XYMonoText().Background);
        var selectable = new XYSelectableText { Text = "可复制" };
        Assert.Contains("xyui-selectable-text", selectable.Classes);
        Assert.Equal(XyuiVectorIcon.Copy, selectable.CopyIcon);
        Assert.NotNull(Mark(selectable, "xyui-selectable-copy-mark").Data);
        var middle = new XYTruncatedText { Text = "region-7ad21c", Mode = XyuiTruncatedTextMode.Middle };
        Assert.Contains("xyui-truncated-middle", middle.Classes);
    });

    static VectorPath Mark(Control control, string className) => control.GetVisualDescendants().OfType<VectorPath>().Single(x => x.Classes.Contains(className));
}
