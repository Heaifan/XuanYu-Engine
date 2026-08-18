namespace XYUI.Avalonia.Controls;

public sealed class XYErrorText : XyuiTextSurface
{
    public XYErrorText() : base("xyui-error-text") { }
    public override string CanonicalId => "XYUI-1-16";
    protected override string FormatText(string value) => $"✕  {value}";
}
