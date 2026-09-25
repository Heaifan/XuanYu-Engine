using System;
using System.Collections.Generic;
using Avalonia;
using XuanYu.Core.Math;

namespace XuanYu.Editor.UI;

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
    public const double CenterRadius = 11.0;
    public const double OrbitHitRadius = 19.0;
    public const double PositiveEndpointRadius = 8.5;
    public const double NegativeEndpointRadius = 7.5;
    public const double PositiveHitRadius = 12.0;
    public const double NegativeHitRadius = 12.0;
    public const double HitRadius = PositiveHitRadius;
    public const double AxisWidth = 5.0;

    public static int ActiveIndexFor(string view) => view switch
    {
        "+X 视图" => 0,
        "-X 视图" => 1,
        "+Y 视图" => 2,
        "-Y 视图" => 3,
        StandardViewResolver.Top => 4,
        StandardViewResolver.Bottom => 5,
        _ => -1
    };

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

    static Point ProjectStable(Vector3d d, Vector3d right, Vector3d up, Point center, int axis, double depth)
    {
        var sx = d.Dot(right);
        var sy = -d.Dot(up);
        if (Math.Sqrt((sx * sx) + (sy * sy)) < 0.2)
        {
            if (depth > 0.0) return center;
            var fallback = axis switch
            {
                0 => new Vector(1, 0), 1 => new Vector(-1, 0),
                2 => new Vector(0.75, 0.66), 3 => new Vector(-0.75, -0.66),
                4 => new Vector(0, -1), _ => new Vector(0, 1)
            };
            return center + (fallback * AxisRadius);
        }
        return new Point(center.X + (sx * AxisRadius), center.Y + (sy * AxisRadius));
    }

    public static double Depth(Vector3d d, Vector3d forward) => -d.Dot(forward);
    public static double HitRadiusFor(GizmoEndpoint endpoint) =>
        endpoint.IsPositive ? PositiveHitRadius : NegativeHitRadius;

    public static IReadOnlyList<GizmoEndpoint> Compute(
        Vector3d right, Vector3d up, Vector3d forward, Point center)
    {
        var list = new List<GizmoEndpoint>(6);
        foreach (var (name, direction, positive) in Directions)
        {
            var depth = Depth(direction, forward);
            var visible = true;
            var front = Math.Clamp((depth + 1.0) / 2.0, 0.0, 1.0);
            var alpha = positive ? 0.55 + (0.45 * front) : 0.40 + (0.38 * front);
            var radius = positive ? PositiveEndpointRadius : NegativeEndpointRadius;
            var axis = name switch { "+X" or "-X" => 0, "+Y" or "-Y" => 1, _ => 2 };
            var screen = ProjectStable(direction, right, up, center, axis, depth);
            list.Add(new GizmoEndpoint(name, positive, screen, depth, alpha, radius, visible));
        }
        list.Sort((a, b) => a.Depth.CompareTo(b.Depth));
        return list;
    }
}
