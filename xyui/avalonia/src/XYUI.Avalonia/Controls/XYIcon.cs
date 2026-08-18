using Avalonia;

namespace XYUI.Avalonia.Controls;

public enum XyuiIconSize { Small, Medium, Large }

public sealed class XYIcon : XyuiTextComponent
{
    public static readonly StyledProperty<string> GlyphProperty = AvaloniaProperty.Register<XYIcon, string>(nameof(Glyph), "•");
    public static readonly StyledProperty<XyuiIconSize> SizeProperty = AvaloniaProperty.Register<XYIcon, XyuiIconSize>(nameof(Size), XyuiIconSize.Medium);

    public XYIcon() : base("xyui-icon") { ApplySize(Size); }
    public override string CanonicalId => "XYUI-1-12";
    public string Glyph { get => GetValue(GlyphProperty); set => SetValue(GlyphProperty, value); }
    public XyuiIconSize Size { get => GetValue(SizeProperty); set { SetValue(SizeProperty, value); ApplySize(value); } }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change); if (change.Property == GlyphProperty) Text = change.GetNewValue<string>();
    }
    void ApplySize(XyuiIconSize value) { Classes.Remove("xyui-icon-small"); Classes.Remove("xyui-icon-medium"); Classes.Remove("xyui-icon-large"); Classes.Add($"xyui-icon-{value.ToString().ToLowerInvariant()}"); }
}
