using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;
using VectorPath = Avalonia.Controls.Shapes.Path;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2ChoiceControlsTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2ChoiceControlsTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Checkbox_supports_checked_mixed_hover_focus_and_disabled() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var checkbox = new XYCheckbox { Content = "批量属性", IsThreeState = true };
        var window = XyuiBatchTestHost.Show(checkbox);
        var indicator = Part<Grid>(checkbox, "xyui-checkbox-host");
        var box = Part<Border>(checkbox, "xyui-checkbox-box");
        var glyphHost = Part<Grid>(checkbox, "xyui-checkbox-glyph-host");
        var check = Part<VectorPath>(checkbox, "xyui-checkbox-check");
        var mixed = Part<Border>(checkbox, "xyui-checkbox-mixed");
        var content = checkbox.GetVisualDescendants().OfType<ContentPresenter>().Single();
        Assert.Equal(2, box.CornerRadius.TopLeft);
        Assert.Equal(18, indicator.Bounds.Width); Assert.Equal(22, indicator.Bounds.Height);
        Assert.Equal(14, box.Bounds.Width); Assert.Equal(14, box.Bounds.Height);
        Assert.Equal(14, glyphHost.Bounds.Width); Assert.Equal(14, glyphHost.Bounds.Height);
        Assert.Equal(1.25, check.StrokeThickness); Assert.Equal(7, mixed.Width); Assert.Equal(1.25, mixed.Height);
        var indicatorRight = indicator.TranslatePoint(new Point(indicator.Bounds.Width, 0), checkbox)!.Value.X;
        var contentLeft = content.TranslatePoint(new Point(0, 0), checkbox)!.Value.X;
        Assert.Equal(7, contentLeft - indicatorRight, 2);
        var indicatorCenter = indicator.TranslatePoint(new Point(indicator.Bounds.Width / 2, indicator.Bounds.Height / 2), checkbox)!.Value;
        var boxCenter = box.TranslatePoint(new Point(box.Bounds.Width / 2, box.Bounds.Height / 2), checkbox)!.Value;
        var glyphCenter = glyphHost.TranslatePoint(new Point(glyphHost.Bounds.Width / 2, glyphHost.Bounds.Height / 2), checkbox)!.Value;
        var contentCenter = content.TranslatePoint(new Point(content.Bounds.Width / 2, content.Bounds.Height / 2), checkbox)!.Value;
        Assert.Equal(indicatorCenter.Y, boxCenter.Y, 2); Assert.Equal(boxCenter.Y, glyphCenter.Y, 2); Assert.Equal(indicatorCenter.Y, contentCenter.Y, 2);
        var offSize = checkbox.DesiredSize;
        checkbox.IsChecked = true; Dispatcher.UIThread.RunJobs();
        var onSize = checkbox.DesiredSize; Assert.Equal(offSize, onSize);
        Assert.Equal(1, check.Opacity); Assert.Equal(0, mixed.Opacity);
        checkbox.IsChecked = null; Dispatcher.UIThread.RunJobs(); Assert.Null(checkbox.IsChecked);
        Assert.Equal(0, check.Opacity); Assert.Equal(1, mixed.Opacity);
        checkbox.IsChecked = true; checkbox.IsEnabled = false; Dispatcher.UIThread.RunJobs();
        Assert.Equal(XyuiBatchTestHost.Token("XY.Surface.Selected"), XyuiBatchTestHost.ColorOf(box.Background)); window.Close();
    });

    [Fact]
    public void Radio_buttons_are_exclusive_only_inside_their_group() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var a = new XYRadioButton { Content = "实体", GroupName = "render", IsChecked = true };
        var b = new XYRadioButton { Content = "线框", GroupName = "render" };
        var c = new XYRadioButton { Content = "世界", GroupName = "space", IsChecked = true };
        var window = XyuiBatchTestHost.Show(new StackPanel { Children = { a, b, c } });
        var halo = Part<Ellipse>(b, "xyui-radio-halo");
        var host = Part<Grid>(b, "xyui-radio-host");
        var dot = Part<Ellipse>(b, "xyui-radio-dot");
        Assert.IsType<Ellipse>(halo); Assert.Equal(22, host.Bounds.Width); Assert.Equal(22, host.Bounds.Height);
        var offSize = b.DesiredSize;
        b.IsChecked = true; Dispatcher.UIThread.RunJobs();
        var onSize = b.DesiredSize;
        Assert.False(a.IsChecked); Assert.True(b.IsChecked); Assert.True(c.IsChecked);
        Assert.Equal(offSize, onSize); Assert.Equal(1, dot.Opacity);
        Assert.Equal(XyuiBatchTestHost.Token("XY.Accent.Strong"), XyuiBatchTestHost.ColorOf(dot.Fill));
        Assert.Equal(0, Part<Ellipse>(a, "xyui-radio-dot").Opacity);
        window.Close();
    });

    [Fact]
    public void Switch_toggles_thumb_without_layout_shift_and_gallery_has_real_pages() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var sw = new XYSwitch { Content = "自动保存" };
        var window = XyuiBatchTestHost.Show(sw);
        var width = sw.Bounds.Width; sw.IsChecked = true; Dispatcher.UIThread.RunJobs();
        var thumb = Part<Ellipse>(sw, "xyui-switch-thumb");
        var transform = Assert.IsType<TranslateTransform>(thumb.RenderTransform); Assert.Equal(16, transform.X); Assert.Equal(width, sw.Bounds.Width);
        Assert.IsType<XYCheckbox>(XYUI2GalleryCatalog.CreatePreview("XYUI-2-06").GetVisualDescendants().OfType<XYCheckbox>().First());
        Assert.Contains("XYUI-2-08", XYUI2DocumentationCatalog.BatchIds);
        window.Close();
    });

    static T Part<T>(Control host, string cls) where T : Visual => host.GetVisualDescendants().OfType<T>().Single(x => x.Classes.Contains(cls));
}
