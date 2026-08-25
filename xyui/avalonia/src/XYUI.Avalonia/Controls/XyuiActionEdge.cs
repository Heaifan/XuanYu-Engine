using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Spatial;

namespace XYUI.Avalonia.Controls;

// XYUI-2 Action Edge：Button 家族底部状态边。Default 3 DIP / Hover 4 DIP；
// 只表达动作语义，不承担 Focus 语义（Focus 走 Foundation Focus Outline）。
public sealed class XyuiActionEdge : Border
{
    public const double DefaultHeight = 3;
    public const double HoverHeight = 4;

    public XyuiActionEdge()
    {
        Classes.Add("xyui-action-edge");
        VerticalAlignment = VerticalAlignment.Bottom;
        IsHitTestVisible = false;
        CornerRadius = new CornerRadius(0, 0, XyuiSpatialTokens.RadiusButton, XyuiSpatialTokens.RadiusButton);
    }
}
