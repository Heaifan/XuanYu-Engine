using Avalonia;
using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public enum XyuiTruncatedTextMode { End, Middle }

public sealed class XYTruncatedText : XyuiTextComponent
{
    public static readonly StyledProperty<XyuiTruncatedTextMode> ModeProperty = AvaloniaProperty.Register<XYTruncatedText, XyuiTruncatedTextMode>(nameof(Mode), XyuiTruncatedTextMode.End);
    public XYTruncatedText() : base("xyui-truncated-text") { TextWrapping = TextWrapping.NoWrap; ApplyMode(Mode); }
    public override string CanonicalId => "XYUI-1-24";
    public XyuiTruncatedTextMode Mode { get => GetValue(ModeProperty); set { SetValue(ModeProperty, value); ApplyMode(value); } }
    void ApplyMode(XyuiTruncatedTextMode value) { TextTrimming = TextTrimming.CharacterEllipsis; Classes.Set("xyui-truncated-middle", value == XyuiTruncatedTextMode.Middle); }
}
