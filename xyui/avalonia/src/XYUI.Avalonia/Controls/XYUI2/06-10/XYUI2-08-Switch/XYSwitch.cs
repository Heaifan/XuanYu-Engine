using Avalonia.Controls.Primitives;

namespace XYUI.Avalonia.Controls;

public partial class XYSwitch : ToggleButton
{
    public XYSwitch()
    {
        Classes.Add("xyui-switch");
        Focusable = true;
    }
}
