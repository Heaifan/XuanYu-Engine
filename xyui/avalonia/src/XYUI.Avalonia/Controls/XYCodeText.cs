namespace XYUI.Avalonia.Controls;

public sealed class XYCodeText : XyuiTextSurface
{
    public XYCodeText() : base("xyui-code-text") { }
    public override string CanonicalId => "XYUI-1-07";
    protected override string FormatText(string value) => $"</>  {value}";
}
