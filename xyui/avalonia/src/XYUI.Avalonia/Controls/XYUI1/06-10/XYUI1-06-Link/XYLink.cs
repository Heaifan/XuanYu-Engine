using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public sealed class XYLink : Button
{
    public XYLink()
    {
        Classes.Add("xyui-1-component");
        Classes.Add("xyui-link");
        Classes.Add("xyui-focusable");
    }
    public string CanonicalId => "XYUI-1-06";
}
