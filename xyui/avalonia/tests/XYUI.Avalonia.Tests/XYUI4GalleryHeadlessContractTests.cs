using Avalonia.Controls;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

public sealed partial class XYUI4GalleryTests
{
    [Fact]
    public void Focus_gallery_input_selects_prompt_text_on_focus() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var preview = XYUI4GalleryCatalog.CreatePreview("XYUI-4-4.04");
        var input = preview.GetVisualDescendants().OfType<TextBox>().Single();
        var window = new Window { Content = preview };
        window.Show();
        input.Focus();
        Assert.Equal(0, input.SelectionStart);
        Assert.Equal(input.Text?.Length, input.SelectionEnd);
        window.Close();
    });

    [Fact]
    public void Selection_gesture_gallery_exposes_marquee_and_lasso_runtime_controls() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var marquee = XYUI4GalleryCatalog.CreatePreview("XYUI-4-4.07");
        var boxes = marquee.GetVisualDescendants().OfType<XYMarqueeSelection>().ToArray();
        Assert.Equal(2, boxes.Length);
        Assert.Contains(boxes, x => x.IsCrossing);
        Assert.Contains(boxes, x => !x.IsCrossing);
        var lasso = XYUI4GalleryCatalog.CreatePreview("XYUI-4-4.08");
        var path = lasso.GetVisualDescendants().OfType<XYLassoSelection>().Single();
        Assert.True(path.Points.Count >= 6);
        Assert.NotNull(XYUI4GalleryCatalog.CreateLiveExamples("XYUI-4-4.07"));
        Assert.NotNull(XYUI4GalleryCatalog.CreateLiveExamples("XYUI-4-4.08"));
    });

    [Fact]
    public void ProgressBar_gallery_exposes_four_visual_forms() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var preview = XYUI4GalleryCatalog.CreatePreview("XYUI-4-4.16");
        var bars = preview.GetVisualDescendants().OfType<XYProgressBar>().ToArray();
        Assert.True(bars.Length >= 7);
        Assert.Contains(bars, x => x.Variant == XyuiProgressBarVariant.Labeled);
        Assert.Contains(bars, x => x.Variant == XyuiProgressBarVariant.SegmentedStage);
        Assert.Contains(bars, x => x.Variant == XyuiProgressBarVariant.InlineCompact);
        Assert.IsType<XYProgressBar>(XYUI4GalleryCatalog.CreateLiveExamples("XYUI-4-4.16")
            .GetVisualDescendants().OfType<XYProgressBar>().First());
    });

    [Fact]
    public void Segmented_stage_gallery_maps_completed_current_and_pending() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var preview = XYUI4GalleryCatalog.CreatePreview("XYUI-4-4.16");
        var stages = preview.GetVisualDescendants().OfType<XYProgressBar>()
            .Where(x => x.Variant == XyuiProgressBarVariant.SegmentedStage).ToArray();
        Assert.Equal(5, stages.Length);
        Assert.Equal(new[] { 100d, 100d, 40d, 0d, 0d }, stages.Select(x => x.Value));
        Assert.Contains(preview.GetVisualDescendants().OfType<TextBlock>(), x => x.Text?.Contains("解析") == true);
        Assert.Contains(preview.GetVisualDescendants().OfType<TextBlock>(), x => x.Text?.Contains("当前") == true);
    });
}
