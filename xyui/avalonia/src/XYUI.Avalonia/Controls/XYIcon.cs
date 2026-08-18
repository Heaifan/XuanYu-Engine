using Avalonia;

namespace XYUI.Avalonia.Controls;

public enum XyuiIconSize { Tiny, Small, Medium, Default = Medium, Large }

public sealed class XYIcon : XyuiTextComponent
{
    public static readonly StyledProperty<string> GlyphProperty = AvaloniaProperty.Register<XYIcon, string>(nameof(Glyph), "•");
    public static readonly StyledProperty<XyuiIconSize> SizeProperty = AvaloniaProperty.Register<XYIcon, XyuiIconSize>(nameof(Size), XyuiIconSize.Medium);
    public static readonly StyledProperty<double> StrokeWidthProperty = AvaloniaProperty.Register<XYIcon, double>(nameof(StrokeWidth), 1.5d);

    public XYIcon() : base("xyui-icon") { ApplySize(Size); }
    public override string CanonicalId => "XYUI-1-12";
    public string Glyph { get => GetValue(GlyphProperty); set => SetValue(GlyphProperty, value); }
    public XyuiIconSize Size { get => GetValue(SizeProperty); set { SetValue(SizeProperty, value); ApplySize(value); } }
    public double StrokeWidth => GetValue(StrokeWidthProperty);
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change); if (change.Property == GlyphProperty) Text = change.GetNewValue<string>();
    }
    void ApplySize(XyuiIconSize value)
    {
        foreach (var name in new[] { "tiny", "small", "medium", "large" }) Classes.Remove($"xyui-icon-{name}");
        var size = value == XyuiIconSize.Tiny ? ("tiny", 1d) : value == XyuiIconSize.Small ? ("small", 1.25d) : value == XyuiIconSize.Large ? ("large", 1.75d) : ("medium", 1.5d);
        Classes.Add($"xyui-icon-{size.Item1}"); SetValue(StrokeWidthProperty, size.Item2);
    }
}
