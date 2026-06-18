namespace FluidWarfare.Render.ViewportNavigation;

/// <summary>矩形区域（像素坐标）。</summary>
public readonly record struct Rect(float X, float Y, float W, float H);

/// <summary>圆形区域（像素坐标）。</summary>
public readonly record struct Circle(float CenterX, float CenterY, float Radius);
