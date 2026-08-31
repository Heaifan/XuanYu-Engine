namespace XYUI.Avalonia.Controls;

using XYUI.Avalonia.Vector;

public sealed class XYErrorText : XyuiVectorTextSurface
{
    public XYErrorText() : base("xyui-error-text", XyuiVectorIcon.Error, XyuiVectorMarkPlacement.Inline) { }
    public override string CanonicalId => "XYUI-1-16";
}
