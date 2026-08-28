using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public static class XyuiComponentTokens
{
    public const double DropDownButtonChevronTrackWidth = 34;
    public const double CheckboxRadius = 2;
    public const double RadioHaloSize = 22;
    public const double SwitchWidth = 34;
    public const double SwitchHeight = 18;
    public const double SwitchThumbSize = 14;
    public const double InputHeight = 32;
    public const double SelectHeight = 30;
    public const double TextAreaMinHeight = 54;
    public const double SliderTouchTargetMinHeight = 44;
    public const double SliderRailHeight = 4;
    public const double SliderThumbSize = 14;
    public const double SliderThumbActiveSize = 16;
    public const double ComboBoxChevronCellWidth = 32;

    public static ResourceDictionary CreateResources() => new()
    {
        ["XY.DropDownButton.ChevronTrack.Width"] = DropDownButtonChevronTrackWidth,
        ["XY.Size.Checkbox"] = 14d,
        ["XY.Size.Radio"] = 16d,
        ["XY.Size.Switch.Width"] = SwitchWidth,
        ["XY.Size.Switch.Height"] = SwitchHeight,
        ["XY.Checkbox.Radius"] = new CornerRadius(CheckboxRadius),
        ["XY.Radio.HaloSize"] = RadioHaloSize,
        ["XY.Switch.Width"] = SwitchWidth,
        ["XY.Switch.Height"] = SwitchHeight,
        ["XY.Switch.ThumbSize"] = SwitchThumbSize,
        ["XY.Input.Height"] = InputHeight,
        ["XY.Select.Height"] = SelectHeight,
        ["XY.TextArea.MinHeight"] = TextAreaMinHeight,
        ["XY.Slider.TouchTarget.MinHeight"] = SliderTouchTargetMinHeight,
        ["XY.Slider.Rail.Height"] = SliderRailHeight,
        ["XY.Slider.Thumb.Size"] = SliderThumbSize,
        ["XY.Slider.Thumb.ActiveSize"] = SliderThumbActiveSize,
        ["XY.ComboBox.ChevronCell.Width"] = ComboBoxChevronCellWidth,
    };
}
