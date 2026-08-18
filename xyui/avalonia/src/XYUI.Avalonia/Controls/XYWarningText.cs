namespace XYUI.Avalonia.Controls;

public sealed class XYWarningText : XyuiTextSurface
{
    public XYWarningText() : base("xyui-warning-text") { }
    public override string CanonicalId => "XYUI-1-17";
    protected override string FormatText(string value) => $"△  {value}";
}
