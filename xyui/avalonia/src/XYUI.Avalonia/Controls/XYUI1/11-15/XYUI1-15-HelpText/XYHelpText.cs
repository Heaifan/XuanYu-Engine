namespace XYUI.Avalonia.Controls;

using XYUI.Avalonia.Vector;

public sealed class XYHelpText : XyuiVectorTextSurface
{
    public XYHelpText() : base("xyui-help-text", XyuiVectorIcon.Info, XyuiVectorMarkPlacement.Inline) { }
    public override string CanonicalId => "XYUI-1-15";
}
