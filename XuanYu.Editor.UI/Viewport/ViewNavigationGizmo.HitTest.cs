using System;
using System.Collections.Generic;
using Avalonia;

namespace XuanYu.Editor.UI;

// F3-D3：导航 Gizmo 命中测试——六端点与中心球。
// 命中半径 ≥13 DIP；重叠时最靠近相机的端点优先（按深度倒序检查）；
// 控件区域（88×88）外不捕获——点击继续进入实体 Picking / 框选 / 相机 / 变换 Gizmo。
public sealed record GizmoHitResult(string? Endpoint, bool HitCenter, bool HitGizmo)
{
    public bool IsEndpoint => Endpoint is not null;
}

public static class NavigationGizmoHitTest
{
    // 端点命中：按绘制深度倒序（最靠前优先）；命中半径 HitRadius。
    public static GizmoHitResult Hit(IReadOnlyList<GizmoEndpoint> endpoints, Point point, Point center)
    {
        for (var i = endpoints.Count - 1; i >= 0; i--)
        {
            var e = endpoints[i];
            if (!e.IsVisible) continue;
            if (Distance(point, e.Screen) <= NavigationGizmoLayout.HitRadius)
                return new GizmoHitResult(e.Name, false, true);
        }
        var hitCenter = Distance(point, center) <= NavigationGizmoLayout.CenterRadius;
        if (hitCenter) return new GizmoHitResult(null, true, true);
        return new GizmoHitResult(null, false, false);
    }

    // 控件区域外（88×88）不截获输入。
    public static bool IsInsideGizmo(Point point) =>
        point.X >= 0.0 && point.X <= NavigationGizmoLayout.GizmoSize &&
        point.Y >= 0.0 && point.Y <= NavigationGizmoLayout.GizmoSize;

    static double Distance(Point a, Point b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        return Math.Sqrt((dx * dx) + (dy * dy));
    }
}
