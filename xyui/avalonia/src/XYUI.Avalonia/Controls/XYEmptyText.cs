namespace XYUI.Avalonia.Controls;

using XYUI.Avalonia.Vector;

public sealed class XYEmptyText : XyuiVectorTextSurface
{
    public XYEmptyText() : base("xyui-empty-text", XyuiVectorIcon.Empty, XyuiVectorMarkPlacement.Inline) { }
    public override string CanonicalId => "XYUI-1-22";
}
