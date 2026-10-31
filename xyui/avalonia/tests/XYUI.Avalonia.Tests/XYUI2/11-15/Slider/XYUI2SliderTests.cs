using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2SliderTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2SliderTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Slider_exposes_one_value_to_real_parts() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var slider = new XYSlider { Value = 50, Step = 2, LargeStep = 20, SmallStep = .5, Suffix = "%" }; var window = XyuiBatchTestHost.Show(slider);
        Assert.IsType<XYNumberField>(slider.NumberFieldPart); Assert.IsType<Slider>(slider.SliderPart); Assert.Equal(50, slider.SliderPart!.Value); Assert.Equal(50, slider.NumberFieldPart!.Value); Assert.Equal(2, slider.NumberFieldPart.Step); Assert.Equal(20, slider.NumberFieldPart.LargeStep); Assert.Equal(.5, slider.NumberFieldPart.SmallStep); Assert.Equal("%", slider.NumberFieldPart.Suffix); window.Close();
    });

    [Fact]
    public void Slider_value_changes_follow_both_editors() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var slider = new XYSlider { Value = 50 }; var window = XyuiBatchTestHost.Show(slider);
        slider.SliderPart!.Value = 75; Assert.Equal(75, slider.Value); Assert.Equal(75, slider.NumberFieldPart!.Value);
        slider.NumberFieldPart.Value = 30; Assert.Equal(30, slider.Value); Assert.Equal(30, slider.SliderPart.Value);
        slider.Value = 90; Assert.Equal(90, slider.SliderPart.Value); Assert.Equal(90, slider.NumberFieldPart.Value); window.Close();
    });

    [Fact]
    public void Slider_template_has_touch_host_and_compact_number_field() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var slider = new XYSlider { Width = 360 }; var window = XyuiBatchTestHost.Show(slider);
        Assert.True(slider.Bounds.Height >= 44); Assert.Equal(30, slider.NumberFieldPart!.Height); Assert.Equal(4, XyuiComponentTokens.SliderRailHeight); Assert.Equal(14, XyuiComponentTokens.SliderThumbSize); Assert.Equal(16, XyuiComponentTokens.SliderThumbActiveSize); window.Close();
    });

    [Fact]
    public void Slider_gallery_contains_integrated_number_fields() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var preview = XYUI2GalleryCatalog.CreatePreview("XYUI-2-11"); var window = XyuiBatchTestHost.Show(preview);
        Assert.True(preview.GetVisualDescendants().OfType<XYSlider>().Count() >= 4); Assert.True(preview.GetVisualDescendants().OfType<XYNumberField>().Count() >= 4); window.Close();
    });

    [Fact]
    public void Slider_track_value_change_invalidates_visual_contract() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var slider = new XYSlider { Width = 360, Value = 30 }; var window = XyuiBatchTestHost.Show(slider); var track = slider.TrackPart!;
        track.Value = 80; Assert.Equal(80, track.Value); track.InvalidateVisual(); Assert.True(track.IsVisible); window.Close();
    });

    [Fact]
    public void Slider_number_field_reserves_suffix_space_and_clips_value_host() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var slider = new XYSlider { Width = 360, Value = 83.4, Suffix = "%" }; var window = XyuiBatchTestHost.Show(slider);
        var numberField = slider.NumberFieldPart!; var host = numberField.GetVisualDescendants().OfType<Border>().Single(x => x.Name == "PART_ValueHost"); var suffix = numberField.GetVisualDescendants().OfType<Border>().Single(x => x.Name == "PART_SuffixHost");
        Assert.True(numberField.Bounds.Width >= 104); Assert.True(host.ClipToBounds); Assert.True(suffix.Bounds.Width >= 24); Assert.Equal(HorizontalAlignment.Stretch, suffix.HorizontalAlignment); window.Close();
    });

    [Fact]
    public void Slider_clamps_value_when_range_changes() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var slider = new XYSlider { Value = 50 }; var window = XyuiBatchTestHost.Show(slider);
        slider.Minimum = 60; Assert.Equal(60, slider.Value); slider.Maximum = 70; slider.Value = 100; Assert.Equal(70, slider.Value); window.Close();
    });
}
