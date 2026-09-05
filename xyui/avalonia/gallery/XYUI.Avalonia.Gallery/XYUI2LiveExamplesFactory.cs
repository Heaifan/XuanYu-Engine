using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2LiveExamplesFactory
{
    public static bool Supports(string id) => id is
        "XYUI-2-01" or "XYUI-2-02" or "XYUI-2-03" or "XYUI-2-04" or "XYUI-2-05" or "XYUI-2-06" or
        "XYUI-2-07" or "XYUI-2-08" or "XYUI-2-09" or "XYUI-2-10" or "XYUI-2-11" or "XYUI-2-12" or
        "XYUI-2-13" or "XYUI-2-14" or "XYUI-2-15" or "XYUI-2-16" or "XYUI-2-17" or "XYUI-2-18";

    public static Control? Create(string id) => id switch
    {
        "XYUI-2-01" => ButtonExamples(),
        "XYUI-2-02" => IconButtonExamples(),
        "XYUI-2-03" => ToggleButtonExamples(),
        "XYUI-2-04" => SplitButtonExamples(),
        "XYUI-2-05" => DropDownButtonExamples(),
        "XYUI-2-06" => CheckboxExamples(),
        "XYUI-2-07" => RadioButtonExamples(),
        "XYUI-2-08" => SwitchExamples(),
        "XYUI-2-09" => TextFieldExamples(),
        "XYUI-2-10" => NumberFieldExamples(),
        "XYUI-2-11" => SliderExamples(),
        "XYUI-2-12" => ComboBoxExamples(),
        "XYUI-2-13" => SelectExamples(),
        "XYUI-2-14" => TextAreaExamples(),
        "XYUI-2-15" => SearchFieldExamples(),
        "XYUI-2-16" => PasswordFieldExamples(),
        "XYUI-2-17" => DatePickerExamples(),
        "XYUI-2-18" => TimePickerExamples(),
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
