namespace XYUI.Avalonia.Controls;

using XYUI.Avalonia.Vector;

public sealed class XYWarningText : XyuiVectorTextSurface
{
    public XYWarningText() : base("xyui-warning-text", XyuiVectorIcon.Warning, XyuiVectorMarkPlacement.Inline) { }
    public override string CanonicalId => "XYUI-1-17";
}
