using System;
using System.Collections.Generic;
using Avalonia;
using XuanYu.Core.Math;

namespace XuanYu.Editor.UI;

// A 版：三轴端点投影到 Gizmo 屏幕平面；正对相机时使用稳定后备方向，避免端点压到 Orbit 中心。
// 投影：screenX = dot(d, Right)；screenY = -dot(d, Up)；depth = dot(d, Forward)。
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
    public const double GizmoSize = 96.0;
    public const double Margin = 14.0;
    public const double AxisRadius = 24.0;
    public const double CenterRadius = 14.0;
    public const double PositiveEndpointRadius = 9.0;
    public const double HitRadius = 12.0;
    public const double AxisWidth = 3.0;

    public static int ActiveIndexFor(string view) => view switch
    {
        "+X 视图" => 0,
        "+Y 视图" => 1,
        StandardViewResolver.Top => 2,
        _ => -1
    };

    // A 版固定三轴端点；顺序固定，供绘制、命中和 hover 索引统一使用。
    public static readonly IReadOnlyList<(string Name, Vector3d Direction, bool Positive)> Directions =
    [
        ("+X", new Vector3d(1, 0, 0), true),
        ("+Y", new Vector3d(0, 1, 0), true),
        ("+Z", new Vector3d(0, 0, 1), true),
    ];

    public static Point Project(Vector3d d, Vector3d right, Vector3d up, Point center)
    {
        var sx = d.Dot(right);
        var sy = -d.Dot(up);
        return new Point(center.X + (sx * AxisRadius), center.Y + (sy * AxisRadius));
    }

    static Point ProjectStable(Vector3d d, Vector3d right, Vector3d up, Point center, int axis)
    {
        var sx = d.Dot(right);
        var sy = -d.Dot(up);
        if (Math.Sqrt((sx * sx) + (sy * sy)) < 0.2)
        {
            var fallback = axis switch
            {
                0 => new Vector(1, 0),
                1 => new Vector(-0.75, 0.66),
                _ => new Vector(0, -1)
            };
            return center + (fallback * AxisRadius);
        }
        return new Point(center.X + (sx * AxisRadius), center.Y + (sy * AxisRadius));
    }

    public static double Depth(Vector3d d, Vector3d forward) => d.Dot(forward);

    // 计算三个端点并按深度从远（小）到近（大）排序（命中优先级 = 绘制倒序）。
    // 输入相机姿态三正交向量（来自 NavigationCameraSnapshot，不重建 CameraState）。
    public static IReadOnlyList<GizmoEndpoint> Compute(
        Vector3d right, Vector3d up, Vector3d forward, Point center)
    {
        var list = new List<GizmoEndpoint>(3);
        foreach (var (name, direction, positive) in Directions)
        {
            var depth = Depth(direction, forward);
            var visible = true;
            const double alpha = 1.0;
            var radius = PositiveEndpointRadius;
            var axis = name switch { "+X" => 0, "+Y" => 1, _ => 2 };
            var screen = ProjectStable(direction, right, up, center, axis);
            list.Add(new GizmoEndpoint(name, positive, screen, depth, alpha, radius, visible));
        }

        list.Sort((a, b) => a.Depth.CompareTo(b.Depth));
        return list;
    }
}
