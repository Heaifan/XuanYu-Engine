using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI3LiveExamplesFactory
{
    static Control CreateMenuLiveExamples() =>
        WrapCard(new Views.XYUIMenuLiveSampleView(), "XYUI3 声明式 AXAML 与 MVVM ICommand / Check / Radio 完整交互");

    static Control CreateMenuComposition()
    {
        var feedback = new TextBlock { Text = "当前选中模式：透视投影", Classes = { "xyui-text-caption" } };
        var radio1 = new XYMenuItem { Header = "透视投影 (Perspective)", CheckKind = XyuiMenuCheckKind.Radio, IsChecked = true };
        var radio2 = new XYMenuItem { Header = "正交投影 (Orthographic)", CheckKind = XyuiMenuCheckKind.Radio };
        var radio3 = new XYMenuItem { Header = "等轴投影 (Isometric)", CheckKind = XyuiMenuCheckKind.Radio };

        void SelectRadio(XYMenuItem target)
        {
            radio1.IsChecked = ReferenceEquals(target, radio1);
            radio2.IsChecked = ReferenceEquals(target, radio2);
            radio3.IsChecked = ReferenceEquals(target, radio3);
            feedback.Text = $"当前选中模式：{target.Header}";
        }
        radio1.Invoked += (_, _) => SelectRadio(radio1);
        radio2.Invoked += (_, _) => SelectRadio(radio2);
        radio3.Invoked += (_, _) => SelectRadio(radio3);

        var menu = new XYMenu(radio1, radio2, radio3, XYMenu.Separator(),
            new XYMenuItem { Header = "重置相机参数", Icon = XyuiVectorIcon.Browse });

        var panel = new StackPanel { Spacing = 8, Width = 280 };
        panel.Children.Add(menu);
        panel.Children.Add(feedback);
        return WrapCard(panel, "视图模式互斥单选组 · 状态流转与单选联动");
    }
}
