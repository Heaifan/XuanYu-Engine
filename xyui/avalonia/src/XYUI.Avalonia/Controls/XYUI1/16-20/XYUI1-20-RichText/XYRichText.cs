using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using XYUI.Avalonia.Typography;

namespace XYUI.Avalonia.Controls;

public sealed class XYRichText : XyuiTextComponent
{
    string _plainText = "";
    bool _rebuilding;
    public static readonly StyledProperty<string> StrongTextProperty = AvaloniaProperty.Register<XYRichText, string>(nameof(StrongText), "");
    public static readonly StyledProperty<string> MonoTextProperty = AvaloniaProperty.Register<XYRichText, string>(nameof(MonoText), "");
    public XYRichText() : base("xyui-rich-text") { TextWrapping = TextWrapping.Wrap; }
    public override string CanonicalId => "XYUI-1-20";
    public string StrongText { get => GetValue(StrongTextProperty); set => SetValue(StrongTextProperty, value); }
    public string MonoText { get => GetValue(MonoTextProperty); set => SetValue(MonoTextProperty, value); }
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextProperty && !_rebuilding) _plainText = change.GetNewValue<string>();
        if (change.Property == TextProperty || change.Property == StrongTextProperty || change.Property == MonoTextProperty) RebuildInlines();
    }
    void RebuildInlines()
    {
        _rebuilding = true;
        Inlines ??= new InlineCollection();
        Inlines.Clear();
        if (!string.IsNullOrEmpty(_plainText)) Inlines.Add(new Run(_plainText));
        if (!string.IsNullOrEmpty(StrongText)) Inlines.Add(new Run($"  {StrongText}") { FontWeight = FontWeight.SemiBold });
        if (!string.IsNullOrEmpty(MonoText)) Inlines.Add(new Run($"  {MonoText}") { FontFamily = new FontFamily(XyuiTypographyTokens.FontMono), FontSize = XyuiTypographyTokens.FontSizeMono });
        _rebuilding = false;
    }
}
