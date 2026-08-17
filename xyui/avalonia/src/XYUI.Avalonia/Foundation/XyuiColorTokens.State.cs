namespace XYUI.Avalonia.Foundation;

public static partial class XyuiColorTokens
{
    // XY.State.* —— 0.2-F 交互状态色 + Disabled/ReadOnly/Locked 三态家族
    public static readonly XyuiColorToken[] State =
    [
        XyuiColorToken.Parse("XY.State.Color.Hover", "#E7EDF2/#2B3944"),
        XyuiColorToken.Parse("XY.State.Color.Pressed", "#D7E2EA/#344751"),
        XyuiColorToken.Parse("XY.State.Color.Selected", "#D8E7F2/#35536A"),
        XyuiColorToken.Parse("XY.State.Color.Active", "#D0E1ED/#36566B"),
        XyuiColorToken.Parse("XY.State.Color.Focus", "#5C8FB4/#699CC0"),
        XyuiColorToken.Parse("XY.State.Color.Dragging", "#E5E9EC/#303B43"),
        XyuiColorToken.Parse("XY.State.Color.DropTarget.Background", "#D5ECE5/#2B4C42"),
        XyuiColorToken.Parse("XY.State.Color.DropTarget.Border", "#3F8B78/#79B39F"),
        XyuiColorToken.Parse("XY.State.Disabled.Background", "#F0F2F3/#20282E"),
        XyuiColorToken.Parse("XY.State.Disabled.Text", "#A8B2B8/#697983"),
        XyuiColorToken.Parse("XY.State.Disabled.Border", "#D8DEE2/#354149"),
        XyuiColorToken.Parse("XY.State.ReadOnly.Background", "#F7F9FA/#263139"),
        XyuiColorToken.Parse("XY.State.ReadOnly.Text", "#647681/#B3BFC6"),
        XyuiColorToken.Parse("XY.State.ReadOnly.Border", "#C5CFD6/#465660"),
        XyuiColorToken.Parse("XY.State.Locked.Background", "#F3EEE5/#383126"),
        XyuiColorToken.Parse("XY.State.Locked.Text", "#8A6A38/#D0AD72"),
        XyuiColorToken.Parse("XY.State.Locked.Border", "#BCA378/#856F4E"),
    ];
}
