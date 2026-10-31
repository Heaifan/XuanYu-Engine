using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public partial class XYCheckbox : CheckBox
{
    public XYCheckbox()
    {
        Classes.Add("xyui-checkbox");
        Focusable = true;
    }
}
