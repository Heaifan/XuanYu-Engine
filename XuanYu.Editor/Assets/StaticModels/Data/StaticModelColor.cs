namespace XuanYu.Editor.Assets;

public readonly record struct StaticModelColor(double R, double G, double B, double A)
{
    public static StaticModelColor Neutral { get; } = new(0.72, 0.73, 0.76, 1.0);
}
