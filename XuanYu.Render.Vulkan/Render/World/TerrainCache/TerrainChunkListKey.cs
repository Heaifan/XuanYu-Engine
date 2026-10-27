namespace XuanYu.Render.Vulkan.Render.Terrain;

internal readonly record struct TerrainChunkListKey(
    string TerrainId,
    int Revision,
    int CellsPerChunk);
