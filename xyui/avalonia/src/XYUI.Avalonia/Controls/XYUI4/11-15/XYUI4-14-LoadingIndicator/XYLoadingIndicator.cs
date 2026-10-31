using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public enum XyuiLoadingIndicatorVariant { Inline, Detail }

public sealed class XYLoadingIndicator : Border
{
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<XYLoadingIndicator, string>(nameof(Text), "正在加载…");
    public static readonly StyledProperty<string> SecondaryTextProperty =
        AvaloniaProperty.Register<XYLoadingIndicator, string>(nameof(SecondaryText), "");
    public static readonly StyledProperty<XyuiSpinnerSize> SizeProperty =
        AvaloniaProperty.Register<XYLoadingIndicator, XyuiSpinnerSize>(nameof(Size), XyuiSpinnerSize.Standard);
    public static readonly StyledProperty<XyuiLoadingIndicatorVariant> VariantProperty =
        AvaloniaProperty.Register<XYLoadingIndicator, XyuiLoadingIndicatorVariant>(nameof(Variant));
    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<XYLoadingIndicator, bool>(nameof(IsActive), true);

    readonly TextBlock _label = new() { Classes = { "xyui-loading-indicator-text" } };
    readonly TextBlock _secondary = new() { Classes = { "xyui-loading-indicator-secondary" } };
    public XYSpinner Spinner { get; } = new() { Size = XyuiSpinnerSize.Standard };

    public XYLoadingIndicator()
    {
        Classes.Add("xyui-4-component"); Classes.Add("xyui-loading-indicator");
        IsHitTestVisible = false;
        var copy = new StackPanel { Spacing = 2, VerticalAlignment = VerticalAlignment.Center,
            Children = { _label, _secondary } };
        Child = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8,
            VerticalAlignment = VerticalAlignment.Center, Children = { Spinner, copy } };
        Apply();
    }

    public string CanonicalId => "XYUI-4-4.14";
    public string Text { get => GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public string SecondaryText { get => GetValue(SecondaryTextProperty); set => SetValue(SecondaryTextProperty, value); }
    public XyuiSpinnerSize Size { get => GetValue(SizeProperty); set => SetValue(SizeProperty, value); }
    public XyuiLoadingIndicatorVariant Variant { get => GetValue(VariantProperty); set => SetValue(VariantProperty, value); }
    public bool IsActive { get => GetValue(IsActiveProperty); set => SetValue(IsActiveProperty, value); }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextProperty || change.Property == SecondaryTextProperty ||
            change.Property == SizeProperty || change.Property == VariantProperty || change.Property == IsActiveProperty) Apply();
    }

    void Apply()
    {
        _label.Text = Text; _secondary.Text = SecondaryText;
        _secondary.IsVisible = Variant == XyuiLoadingIndicatorVariant.Detail && !string.IsNullOrWhiteSpace(SecondaryText);
        Spinner.Size = Size; Spinner.IsActive = IsActive;
        Classes.Set("xyui-loading-indicator-detail", Variant == XyuiLoadingIndicatorVariant.Detail);
    }
}
