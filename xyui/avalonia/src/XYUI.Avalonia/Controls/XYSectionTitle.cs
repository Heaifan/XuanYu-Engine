namespace XYUI.Avalonia.Controls;

public sealed class XYSectionTitle : XyuiTextSurface
{
    public XYSectionTitle() : base("xyui-section-title") { }
    public override string CanonicalId => "XYUI-1-05";
    protected override string FormatText(string value) => $"▌  {value}";
}
