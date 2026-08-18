using Avalonia.Media;

namespace XYUI.Avalonia.Controls;

public sealed class XYRichText : XyuiTextComponent
{
    public XYRichText() : base("xyui-rich-text") { TextWrapping = TextWrapping.Wrap; }
    public override string CanonicalId => "XYUI-1-20";
}
