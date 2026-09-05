using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2LiveExamplesFactory
{
    public static bool Supports(string id) => id is
        "XYUI-2-01" or "XYUI-2-02" or "XYUI-2-03" or "XYUI-2-04" or "XYUI-2-05" or "XYUI-2-06";

    public static Control? Create(string id) => id switch
    {
        "XYUI-2-01" => ButtonExamples(),
        "XYUI-2-02" => IconButtonExamples(),
        "XYUI-2-03" => ToggleButtonExamples(),
        "XYUI-2-04" => SplitButtonExamples(),
        "XYUI-2-05" => DropDownButtonExamples(),
        "XYUI-2-06" => CheckboxExamples(),
        _ => null
    };

    static StackPanel SceneHost(params Control[] scenes)
    {
        var panel = new StackPanel { Spacing = 16 };
        foreach (var s in scenes) panel.Children.Add(s);
        return panel;
    }

    static StackPanel Scene(string title, Control sample)
    {
        return new StackPanel
        {
            Spacing = 6,
            Children =
            {
                new XYCaption { Text = title },
                new Border
                {
                    Background = new global::Avalonia.Media.SolidColorBrush(global::Avalonia.Media.Color.FromArgb(16, 255, 255, 255)),
                    CornerRadius = new global::Avalonia.CornerRadius(4),
                    Padding = new global::Avalonia.Thickness(12, 10),
                    Child = sample
                }
            }
        };
    }
}
