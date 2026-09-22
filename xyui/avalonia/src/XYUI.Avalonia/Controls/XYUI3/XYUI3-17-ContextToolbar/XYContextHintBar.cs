using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed class XYContextHintBar : Border
{
    public XYContextHintBar(string text = "Enter 执行       Esc 关闭")
    {
        Classes.Add("xyui-context-hint-bar"); Height = 30; Child = new TextBlock { Text = text, Classes = { "xyui-context-hint-text" }, VerticalAlignment = VerticalAlignment.Center };
    }
}
