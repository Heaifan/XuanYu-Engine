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

    public XYButton()
    {
        Classes.Add("xyui-button");
        SyncVariantClass();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == VariantProperty) SyncVariantClass();
    }

    void SyncVariantClass()
    {
        foreach (var variant in new[] { "primary", "secondary", "danger" })
            Classes.Remove($"xyui-button-{variant}");
        Classes.Add($"xyui-button-{Variant.ToString().ToLowerInvariant()}");
    }
}
