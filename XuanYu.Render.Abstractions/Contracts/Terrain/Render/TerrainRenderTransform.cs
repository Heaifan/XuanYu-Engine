namespace XuanYu.Render.Abstractions;

public readonly record struct TerrainRenderTransform(double VerticalExaggeration)
{
    public static TerrainRenderTransform Default => new(1.0);
    public double VisualHeight(double logicalHeight) => logicalHeight * VerticalExaggeration;
    public double LogicalHeight(double visualHeight) => visualHeight / VerticalExaggeration;
}
