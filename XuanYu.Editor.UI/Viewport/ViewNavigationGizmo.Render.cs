using System;
using Avalonia;
using Avalonia.Media;

namespace XuanYu.Editor.UI;

// F3-D2：导航 Gizmo 绘制——中心球 + 三根世界轴 + 六个正负方向端点 + X/Y/Z 标签。
// 玄域低饱和配色（不照搬 Blender 高饱和 RGB）；控件背景完全透明，无白色底板。
public sealed partial class ViewNavigationGizmo
{
    static readonly IBrush CenterFill = new SolidColorBrush(Color.Parse("#CDD6DF"));
    static readonly IPen CenterEdge = new Pen(new SolidColorBrush(Color.Parse("#718096")), 1.0);
    static readonly IBrush HoverRing = new SolidColorBrush(Color.Parse("#E8EEF5"));

    static Color AxisColor(string name) => name switch
    {
        "X" => Color.Parse("#C18A55"),   // 淡金褐
        "Y" => Color.Parse("#5F87A7"),   // 蓝灰
        "Z" => Color.Parse("#A9B8C7"),   // 浅钢灰
        _ => Color.Parse("#A9B8C7"),
    };

    public override void Render(DrawingContext dc)
    {
        var camera = NavigationCamera;
        var center = Center;
        if (camera is null)
        {
            dc.DrawEllipse(CenterFill, CenterEdge, center, NavigationGizmoLayout.CenterRadius, NavigationGizmoLayout.CenterRadius);
            return;
        }
        var endpoints = NavigationGizmoLayout.Compute(
            camera.Right, camera.Up, camera.Forward, center);
        // 先画轴（背向在底部），再画中心球，最后画正方向端点与标签。
        foreach (var e in endpoints)
        {
            var color = AxisColor(e.Name[0].ToString());
            var brush = new SolidColorBrush(color, (float)e.Alpha);
            var screen = NavigationGizmoLayout.ClampToBounds(e.Screen);
            dc.DrawLine(new Pen(brush, NavigationGizmoLayout.AxisWidth), center, screen);
        }
        dc.DrawEllipse(CenterFill, CenterEdge, center, NavigationGizmoLayout.CenterRadius, NavigationGizmoLayout.CenterRadius);
        foreach (var e in endpoints)
        {
            if (!e.IsPositive) continue;
            var screen = NavigationGizmoLayout.ClampToBounds(e.Screen);
            var color = AxisColor(e.Name[0].ToString());
            var brush = new SolidColorBrush(color, (float)e.Alpha);
            dc.DrawEllipse(brush, null, screen, e.Radius, e.Radius);
            var label = new FormattedText(e.Name[0].ToString(),
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, Typeface.Default, 10.0, Brushes.White);
            var labelPos = new Point(screen.X - (label.Width * 0.5), screen.Y - (label.Height * 0.5));
            dc.DrawText(label, labelPos);
            // Hover 反馈：当前端点外围 1.5 DIP 浅色环。
            if (e.Name == HoverEndpoint)
                dc.DrawEllipse(null, new Pen(HoverRing, 1.5), screen, e.Radius + 2.0, e.Radius + 2.0);
        }
    }
}
