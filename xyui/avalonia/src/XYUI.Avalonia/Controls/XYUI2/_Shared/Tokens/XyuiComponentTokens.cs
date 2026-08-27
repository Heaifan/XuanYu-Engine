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
    };
}
