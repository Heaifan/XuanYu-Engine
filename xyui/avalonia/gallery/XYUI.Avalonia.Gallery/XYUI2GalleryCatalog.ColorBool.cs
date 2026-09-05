using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2GalleryCatalog
{
    static Control[] ColorPickers() =>
    [
        ColorSample("Default", new XYColorPicker { Width = 300, Color = Color.FromRgb(74, 120, 148), Mode = XYColorPickerMode.RGB }),
        ColorSample("Open", OpenColor(Color.FromArgb(180, 74, 120, 148))),
    ];

    static Control[] BoolProperties() =>
    [
        BoolSample("Visible", true),
        BoolSample("Cast Shadow", false),
    ];

    static Control ColorSample(string caption, XYColorPicker picker) =>
        new StackPanel { Spacing = 8, Margin = new Thickness(0, 0, 0, 12), Children = { new XYCaption { Text = caption }, picker } };

    static Control BoolSample(string label, bool value) =>
        new XYBoolProperty { Width = 420, Label = label, Value = value };

    static XYColorPicker OpenColor(Color color)
    {
        var picker = new XYColorPicker { Width = 300, Color = color, Mode = XYColorPickerMode.RGBA };
        picker.AttachedToVisualTree += (_, _) => picker.IsOpen = true;
        return picker;
    }
}
