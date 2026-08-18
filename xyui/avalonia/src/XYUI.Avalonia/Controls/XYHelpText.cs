namespace XYUI.Avalonia.Controls;

public sealed class XYHelpText : XyuiTextSurface
{
    public XYHelpText() : base("xyui-help-text") { }
    public override string CanonicalId => "XYUI-1-15";
    protected override string FormatText(string value) => $"ⓘ  {value}";
}
