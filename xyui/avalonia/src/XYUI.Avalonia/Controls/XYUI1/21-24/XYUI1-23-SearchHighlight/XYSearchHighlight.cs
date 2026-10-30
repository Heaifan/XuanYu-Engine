namespace XYUI.Avalonia.Controls;

using XYUI.Avalonia.Vector;
using XYUI.Avalonia.Spatial;

public sealed class XYSearchHighlight : XyuiVectorTextSurface
{
    protected override double CornerMarkTextGap => XyuiSpatialTokens.Space2;
    public XYSearchHighlight() : base("xyui-search-highlight", XyuiVectorIcon.Search, XyuiVectorMarkPlacement.TopRight) { }
    public override string CanonicalId => "XYUI-1-23";
}
