using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Spatial;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public class XYButton : Button
{
    bool _syncing;
    object? _consumerContent;
    public static readonly StyledProperty<XyuiVectorIcon?> IconProperty =
        AvaloniaProperty.Register<XYButton, XyuiVectorIcon?>(nameof(Icon));
    public static readonly StyledProperty<XyuiButtonVariant> VariantProperty =
        AvaloniaProperty.Register<XYButton, XyuiButtonVariant>(nameof(Variant), XyuiButtonVariant.Primary);

    public XyuiVectorIcon? Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
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
        if (e.Property == IconProperty || e.Property == ContentControl.ContentProperty) SyncIconContent(e);
    }

    void SyncIconContent(AvaloniaPropertyChangedEventArgs e)
    {
        if (_syncing) return;
        if (e.Property == ContentControl.ContentProperty) _consumerContent = e.GetNewValue<object?>();
        _syncing = true;
        try { base.SetValue(ContentControl.ContentProperty, Icon is { } icon ? new StackPanel { Orientation = Orientation.Horizontal, Spacing = XyuiSpatialTokens.IndentIconTextGap, Children = { new XYIcon { Icon = icon }, new TextBlock { Text = _consumerContent?.ToString() ?? "" } } } : _consumerContent); }
        finally { _syncing = false; }
    }

    void SyncVariantClass()
    {
        foreach (var variant in new[] { "primary", "secondary", "danger" })
            Classes.Remove($"xyui-button-{variant}");
        Classes.Add($"xyui-button-{Variant.ToString().ToLowerInvariant()}");
    }
}
