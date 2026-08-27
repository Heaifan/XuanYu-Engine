namespace XYUI.Avalonia.Controls;

using XYUI.Avalonia.Vector;

public sealed class XYCodeText : XyuiVectorTextSurface
{
    public XYCodeText() : base("xyui-code-text", XyuiVectorIcon.Code, XyuiVectorMarkPlacement.BottomRight) { }
    protected override double CornerMarkStrokeThickness => 1.25;
    public override string CanonicalId => "XYUI-1-07";
}
