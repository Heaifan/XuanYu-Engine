namespace XuanYu.World.Terrain;

public readonly record struct TerrainRevision(int Value)
{
    public static TerrainRevision From(TerrainWorldStorage storage) =>
        new(storage.EditDelta.Revision);
}
