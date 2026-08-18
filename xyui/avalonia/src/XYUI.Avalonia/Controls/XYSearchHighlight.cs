namespace XYUI.Avalonia.Controls;

public sealed class XYSearchHighlight : XyuiMarkedTextComponent
{
    public XYSearchHighlight() : base("xyui-search-highlight") { }
    public override string CanonicalId => "XYUI-1-23";
    protected override string FormatText(string value) => $"⌕  {value}";
}
