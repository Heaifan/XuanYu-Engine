using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public class XYSelect : ComboBox
{
    public XYSelect() { Classes.Add("xyui-select"); IsEditable = false; }
}
