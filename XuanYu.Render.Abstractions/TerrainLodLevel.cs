namespace XuanYu.Render.Abstractions;

public enum TerrainLodLevel
{
    Lod0,
    Lod1,
    Lod2,
    Lod3,
    Lod4
}

public static class TerrainLodLevelExtensions
{
    public static int Stride(this TerrainLodLevel level) => 1 << (int)level;
}
