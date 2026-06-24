namespace XuanYu.Engine.Render.ViewportNavigation;

/// <summary>轴端在屏幕上的投影。</summary>
public sealed record AxisProjection(
    ViewportNavigationElement Element,
    float ScreenX,
    float ScreenY,
    float Depth,
    float Radius,
    (float R, float G, float B) Color);
