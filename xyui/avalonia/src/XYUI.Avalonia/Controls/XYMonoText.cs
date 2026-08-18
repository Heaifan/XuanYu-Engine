namespace XYUI.Avalonia.Controls;

using TextWrapping = global::Avalonia.Media.TextWrapping;

public sealed class XYMonoText : XyuiTextComponent
{
    public XYMonoText() : base("xyui-mono-text") => TextWrapping = TextWrapping.NoWrap;
    public override string CanonicalId => "XYUI-1-08";
}
