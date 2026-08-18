using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public sealed class XYSelectableText : SelectableTextBlock
{
    public XYSelectableText() => Classes.Add("xyui-selectable-text");
    public string CanonicalId => "XYUI-1-21";
}
