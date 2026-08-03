using System;
using System.Collections.Generic;
using Avalonia;
using XuanYu.Core.Math;

namespace XuanYu.Editor.UI;

// F3-D2：导航 Gizmo 布局纯数学——六个世界方向投影到 Gizmo 屏幕平面。
// 投影：screenX = dot(d, Right)；screenY = -dot(d, Up)；depth = dot(d, Forward)。
// 深度从远到近排序：背向端点先绘制（小、淡），朝向端点后绘制（大、标签）。
public sealed record GizmoEndpoint(
    string Name,
    bool IsPositive,
    Point Screen,
    double Depth,
    double Alpha,
    double Radius,
    bool IsVisible);

public static class NavigationGizmoLayout
{
    public const double GizmoSize = 88.0;
    public const double AxisRadius = 25.0;
    public const double CenterRadius = 13.0;
    public const double PositiveEndpointRadius = 9.0;
    public const double NegativeEndpointRadius = 5.5;
    public const double HitRadius = 13.0;
    public const double AxisWidth = 1.5;

    // 六方向（Z-Up 右手系）。顺序固定，供绘制与命中统一使用。
    public static readonly IReadOnlyList<(string Name, Vector3d Direction, bool Positive)> Directions =
    [
        ("+X", new Vector3d(1, 0, 0), true),
        ("-X", new Vector3d(-1, 0, 0), false),
        ("+Y", new Vector3d(0, 1, 0), true),
        ("-Y", new Vector3d(0, -1, 0), false),
        ("+Z", new Vector3d(0, 0, 1), true),
        ("-Z", new Vector3d(0, 0, -1), false),
    ];

    public static Point Project(Vector3d d, Vector3d right, Vector3d up, Point center)
    {
        var sx = d.Dot(right);
        var sy = -d.Dot(up);
        return new Point(center.X + (sx * AxisRadius), center.Y + (sy * AxisRadius));
    }

    public static double Depth(Vector3d d, Vector3d forward) => d.Dot(forward);

    // 计算六个端点并按深度从远（小）到近（大）排序（命中优先级 = 绘制倒序）。
    // 输入相机姿态三正交向量（来自 NavigationCameraSnapshot，不重建 CameraState）。
    public static IReadOnlyList<GizmoEndpoint> Compute(
        Vector3d right, Vector3d up, Vector3d forward, Point center)
    {
        var list = new List<GizmoEndpoint>(6);
        foreach (var (name, direction, positive) in Directions)
        {
            var depth = Depth(direction, forward);
            // 背向（depth<0）：35~45% Alpha、小端点；侧向（|depth|<0.35）：70~85%；朝向：100%。
            double alpha;
            if (depth < -0.35) alpha = 0.40;
            else if (depth < 0.35) alpha = 0.78;
            else alpha = 1.0;
            var radius = positive ? PositiveEndpointRadius : NegativeEndpointRadius;
            list.Add(new GizmoEndpoint(name, positive, Project(direction, right, up, center),
                depth, alpha, radius, IsVisible: true));
        }
        list.Sort((a, b) => a.Depth.CompareTo(b.Depth));
        return list;
    }

    // 轴正对相机时投影长度趋零——端点收缩到中心附近，不产生 NaN（Avalonia Point 有界）。
    public static Point ClampToBounds(Point p) => new(
        double.IsFinite(p.X) ? Math.Clamp(p.X, 0.0, GizmoSize) : GizmoSize * 0.5,
        double.IsFinite(p.Y) ? Math.Clamp(p.Y, 0.0, GizmoSize) : GizmoSize * 0.5);
}
