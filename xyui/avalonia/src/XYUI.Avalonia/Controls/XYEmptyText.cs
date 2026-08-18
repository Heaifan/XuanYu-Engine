namespace XYUI.Avalonia.Controls;

public sealed class XYEmptyText : XyuiMarkedTextComponent
{
    public XYEmptyText() : base("xyui-empty-text") { }
    public override string CanonicalId => "XYUI-1-22";
    protected override string FormatText(string value) => $"—  {value}  —";
}
