namespace XuanYu.Editor.Assets;

public readonly record struct StaticModelColor(double R, double G, double B, double A)
{
    public static StaticModelColor Neutral { get; } = new(0.8, 0.8, 0.8, 1.0);
}
