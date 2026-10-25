using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Render.Abstractions;

// 参考平面逻辑上无界；此值对象只描述当前帧的 camera-relative 渲染 patch。
public readonly record struct ReferencePlanePatchPlacement(
    double CenterX, double CenterY, double WidthMeters, double DepthMeters)
{
    public double MinX => CenterX - WidthMeters / 2.0;
    public double MaxX => CenterX + WidthMeters / 2.0;
    public double MinY => CenterY - DepthMeters / 2.0;
    public double MaxY => CenterY + DepthMeters / 2.0;
    public bool Contains(Vector3d point) =>
        point.X >= MinX && point.X <= MaxX && point.Y >= MinY && point.Y <= MaxY;

    public static ReferencePlanePatchPlacement From(
        MapRenderSnapshot map, ViewProjectionState state, double groundHeight)
    {
        if (ReferencePlaneFootprint.TryCreate(state, groundHeight, out var footprint))
        {
            var marginX = System.Math.Max(1.0, footprint.Width * 0.25);
            var marginY = System.Math.Max(1.0, footprint.Depth * 0.25);
            return new(footprint.CenterX, footprint.CenterY,
                footprint.Width + (marginX * 2), footprint.Depth + (marginY * 2));
        }
        var distance = System.Math.Max(state.Camera.Position.Length, 1.0);
        var span = System.Math.Max(System.Math.Max(map.WidthMeters, map.DepthMeters), distance * 4.0);
        return new(state.Camera.Position.X, state.Camera.Position.Y, span, span);
    }

    public static ReferencePlanePatchPlacement From(
        ViewProjectionState state, double groundHeight)
    {
        if (ReferencePlaneFootprint.TryCreate(state, groundHeight, out var footprint))
        {
            var marginX = System.Math.Max(1.0, footprint.Width * 0.25);
            var marginY = System.Math.Max(1.0, footprint.Depth * 0.25);
            return new(footprint.CenterX, footprint.CenterY,
                footprint.Width + (marginX * 2), footprint.Depth + (marginY * 2));
        }
        var distance = System.Math.Max(state.Camera.Position.Length, 1.0);
        var span = distance * 4.0;
        return new(state.Camera.Position.X, state.Camera.Position.Y, span, span);
    }
}
