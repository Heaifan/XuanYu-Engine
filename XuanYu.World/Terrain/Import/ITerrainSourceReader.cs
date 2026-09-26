using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Terrain.Import;

public interface ITerrainSourceReader
{
    TerrainSourceData Read(string path);
}
