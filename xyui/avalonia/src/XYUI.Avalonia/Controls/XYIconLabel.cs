using Avalonia;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public sealed class XYIconLabel : XyuiTextComponent
{
    public static readonly StyledProperty<string> IconGlyphProperty = AvaloniaProperty.Register<XYIconLabel, string>(nameof(IconGlyph), "•");
    public static readonly StyledProperty<string> LabelProperty = AvaloniaProperty.Register<XYIconLabel, string>(nameof(Label), "");

    public XYIconLabel() : base("xyui-icon-label") { TextWrapping = TextWrapping.NoWrap; UpdateText(); }
    public override string CanonicalId => "XYUI-1-13";
    public string IconGlyph { get => GetValue(IconGlyphProperty); set => SetValue(IconGlyphProperty, value); }
    public string Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change); if (change.Property == IconGlyphProperty || change.Property == LabelProperty) UpdateText();
    }
    void UpdateText() => Text = $"{IconGlyph} {Label}".Trim();
}
