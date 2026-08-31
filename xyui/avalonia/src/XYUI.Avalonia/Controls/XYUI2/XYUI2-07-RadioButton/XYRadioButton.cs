using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public partial class XYRadioButton : RadioButton
{
    public XYRadioButton()
    {
        Classes.Add("xyui-radio-button");
        Focusable = true;
    }
}
