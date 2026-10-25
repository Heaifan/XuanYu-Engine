namespace XuanYu.Render.Abstractions;

// Editor World View Baseline 的渲染投影；不属于 Imported Map / Terrain Content。
public readonly record struct ReferencePlaneRenderSnapshot(double ElevationMeters)
{
    public static ReferencePlaneRenderSnapshot Default { get; } = new(0.0);
}
