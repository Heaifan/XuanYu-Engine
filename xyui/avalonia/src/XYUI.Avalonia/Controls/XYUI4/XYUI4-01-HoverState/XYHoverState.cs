using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public enum XyuiHoverStateVariant { Surface, Border, Outline, Handle }

public sealed class XYHoverState : Border
{
    public static readonly StyledProperty<XyuiHoverStateVariant> VariantProperty =
        AvaloniaProperty.Register<XYHoverState, XyuiHoverStateVariant>(nameof(Variant));
    public static readonly StyledProperty<bool> IsHoveredProperty =
        AvaloniaProperty.Register<XYHoverState, bool>(nameof(IsHovered));
    public static readonly StyledProperty<bool> IsSelectedProperty =
        AvaloniaProperty.Register<XYHoverState, bool>(nameof(IsSelected));

    public XYHoverState()
    {
        Classes.Add("xyui-4-component"); Classes.Add("xyui-hover-state");
        PointerEntered += (_, _) => IsHovered = IsEnabled;
        PointerExited += (_, _) => IsHovered = false;
        Apply();
    }

    public string CanonicalId => "XYUI-4-4.01";
    public XyuiHoverStateVariant Variant { get => GetValue(VariantProperty); set => SetValue(VariantProperty, value); }
    public bool IsHovered { get => GetValue(IsHoveredProperty); set => SetValue(IsHoveredProperty, value); }
    public bool IsSelected { get => GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == VariantProperty || change.Property == IsHoveredProperty ||
            change.Property == IsSelectedProperty || change.Property == IsEnabledProperty) Apply();
    }

    void Apply()
    {
        foreach (var variant in Enum.GetValues<XyuiHoverStateVariant>())
            Classes.Set($"xyui-hover-state-{variant.ToString().ToLowerInvariant()}", Variant == variant);
        Classes.Set("xyui-hover-state-hovered", IsHovered && IsEnabled);
        Classes.Set("xyui-hover-state-selected", IsSelected);
    }
}
