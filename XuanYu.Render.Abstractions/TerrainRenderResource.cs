using XuanYu.Core.Math;

namespace XuanYu.Render.Abstractions;

public enum TerrainRenderAvailability
{
    Available,
    NoTerrain,
    Unavailable
}

public sealed record TerrainRenderResource(
    string TerrainId,
    int Revision,
    TerrainHeightfield Heightfield,
    double CellSizeMeters = 1.0,
    Vector3d WorldOrigin = default,
    TerrainRenderAvailability Availability = TerrainRenderAvailability.Available)
{
    public TerrainRenderMetadata Metadata => Heightfield.Metadata;
    public int TriangleIndexCount => (Heightfield.Width - 1) * (Heightfield.Height - 1) * 6;
    public bool IsAvailable => Availability == TerrainRenderAvailability.Available;
}
