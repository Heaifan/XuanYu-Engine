namespace XuanYu.Render.Abstractions;

public sealed record TerrainRenderResource(
    string TerrainId,
    int Revision,
    TerrainHeightfield Heightfield,
    double CellSizeMeters = 1.0)
{
    public int TriangleIndexCount => (Heightfield.Width - 1) * (Heightfield.Height - 1) * 6;
}
