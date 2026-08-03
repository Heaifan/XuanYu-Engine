using System;
using System.Collections.Generic;
using Avalonia;
using XuanYu.Core.Math;

namespace XuanYu.Editor.UI;

// F3-D2/F3-F3：导航 Gizmo 布局纯数学——六个世界方向投影到 Gizmo 屏幕平面。
// 投影：screenX = dot(d, Right)；screenY = -dot(d, Up)；depth = dot(d, Forward)。
// 深度从远到近排序：背向端点先绘制（小、淡），朝向端点后绘制（大、标签）。
// F3-F3：轴正对相机（投影长度 < 6 DIP）时只显示朝向端点（置于中心球中央），隐藏背向端点与轴线。
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
    public const double AxisRadius = 27.0;
    public const double CenterRadius = 13.0;
    public const double PositiveEndpointRadius = 9.0;
    public const double NegativeEndpointRadius = 5.0;
    public const double HitRadius = 13.0;
    public const double AxisWidth = 1.5;
    public const double FacingProjectionLimit = 6.0; // 轴正对相机判定（屏幕投影长度，DIP）

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
            var sx = direction.Dot(right);
            var sy = -direction.Dot(up);
            var projectionLength = Math.Sqrt((sx * sx) + (sy * sy)) * AxisRadius;
            var facingCamera = projectionLength < FacingProjectionLimit;
            // F3-F3：正对相机时只显示朝向端点（位于中心球中央）；背向端点与轴线隐藏。
            var visible = !facingCamera || depth > 0.0;
            // 背向 28~35% Alpha、侧向 70~85%、朝向 100%（F3-F3 合同）。
            double alpha;
            if (depth < -0.35) alpha = 0.30;
            else if (depth < 0.35) alpha = 0.78;
            else alpha = 1.0;
            var radius = positive ? PositiveEndpointRadius : NegativeEndpointRadius;
            var screen = facingCamera ? center : Project(direction, right, up, center);
            list.Add(new GizmoEndpoint(name, positive, screen, depth, alpha, radius, visible));
        }

        list.Sort((a, b) => a.Depth.CompareTo(b.Depth));
        return list;
    }
}
