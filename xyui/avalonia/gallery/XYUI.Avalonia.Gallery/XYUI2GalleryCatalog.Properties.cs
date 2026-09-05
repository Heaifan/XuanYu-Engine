using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2GalleryCatalog
{
    static Control[] NumberProperties() =>
    [
        new XYNumberProperty { Width = 520, Label = "Mass", Value = 12.50, Minimum = 0, Maximum = 1000, Step = .5, DecimalPlaces = 2, Suffix = "kg" },
        new XYNumberProperty { Width = 520, Label = "Opacity", Value = 0.85, Minimum = 0, Maximum = 1, Step = .05, DecimalPlaces = 2 },
    ];

    static Control[] VectorProperties() =>
    [
        new XYVectorProperty { Width = 620, Label = "Position", Dimension = XYVectorDimension.Vector3, X = 12.0, Y = 4.0, Z = -8.0, Step = .1, DecimalPlaces = 1 },
        new XYVectorProperty { Width = 620, Label = "UV", Dimension = XYVectorDimension.Vector2, X = 0.5, Y = 0.5, Step = .1, DecimalPlaces = 1 },
    ];

    static Control[] EnumProperties() =>
    [
        new XYEnumProperty { Width = 520, Label = "Blend Mode", ItemsSource = new[] { "Opaque", "Masked", "Translucent", "Additive" }, SelectedIndex = 0 },
        new XYEnumProperty { Width = 520, Label = "Quality", ItemsSource = new[] { "Low", "Medium", "High", "Ultra" }, SelectedIndex = 2 },
    ];

    static Control[] ReferenceProperties() =>
    [
        ReferenceSample("Material", new("Steel_Mat", "Material", "M-001"), "Material"),
        ReferenceSample("Texture", new("Terrain_Albedo", "Texture", "T-104"), "Texture"),
    ];

    static Control ReferenceSample(string caption, XYReferenceValue? value, string expected)
    {
        var list = new ListBox { ItemsSource = new[] { new XYReferenceValue("Steel_Mat", "Material", "M-001"), new XYReferenceValue("Terrain_Albedo", "Texture", "T-104") }, MinWidth = 220 };
        return new XYReferenceProperty { Width = 520, Label = caption, Reference = value, ExpectedType = expected, ReferenceState = XYReferenceState.Resolved, ReferencePickerContent = list };
    }

    static Control Sample(string caption, Control control) => new StackPanel { Spacing = 3, Children = { new XYCaption { Text = caption }, control } };
    static Control Hint(string title, string text) => new StackPanel { Spacing = 4, Children = { new XYCaption { Text = title }, new TextBlock { Text = text } } };
}
