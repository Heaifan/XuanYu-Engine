using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace XYUI.Avalonia.Controls;

public static partial class XyuiControlStyles
{
    static void Slider(Styles styles)
    {
        var root = new Style(x => x.OfType<XYSlider>().Class("xyui-slider"));
        root.Setters.Add(new Setter(TemplatedControl.TemplateProperty, XYSlider.CreateTemplate())); Set(root, Control.MinHeightProperty, "XY.Slider.TouchTarget.MinHeight"); styles.Add(root);
        var track = new Style(x => x.OfType<XYSlider>().Template().Name("PART_Track")); Set(track, XYSliderTrack.RailInactiveProperty, "XY.Brush.Divider.Default"); Set(track, XYSliderTrack.RailActiveProperty, "XY.Brush.Accent.Default"); Set(track, XYSliderTrack.ThumbBackgroundProperty, "XY.Brush.Surface.Raised"); Set(track, XYSliderTrack.ThumbBorderProperty, "XY.Brush.Accent.Strong"); styles.Add(track);
        var gap = new Style(x => x.OfType<XYSlider>().Template().Name("PART_Gap")); gap.Setters.Add(new Setter(Control.WidthProperty, new DynamicResourceExtension("XY.Space.2"))); styles.Add(gap);
        var slider = new Style(x => x.OfType<XYSlider>().Template().Name("PART_Slider")); slider.Setters.Add(new Setter(Control.FocusableProperty, true)); styles.Add(slider);
    }
}
