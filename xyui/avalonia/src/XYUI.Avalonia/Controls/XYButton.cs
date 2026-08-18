using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public class XYButton : Button
{
    public static readonly StyledProperty<XyuiButtonVariant> VariantProperty =
        AvaloniaProperty.Register<XYButton, XyuiButtonVariant>(nameof(Variant), XyuiButtonVariant.Primary);

    public XyuiButtonVariant Variant
    {
        get => GetValue(VariantProperty);
        set => SetValue(VariantProperty, value);
    }

    public XYButton() => Classes.Add("xyui-button");
}
