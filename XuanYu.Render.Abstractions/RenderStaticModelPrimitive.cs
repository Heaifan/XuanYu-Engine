namespace XuanYu.Render.Abstractions;

public readonly record struct RenderStaticModelPrimitive(
    int FirstIndex,
    int IndexCount,
    int BaseVertex,
    RenderStaticModelColor BaseColor);

public readonly record struct RenderStaticModelColor(double R, double G, double B, double A)
{
    public static RenderStaticModelColor Neutral { get; } = new(0.8, 0.8, 0.8, 1.0);
}
