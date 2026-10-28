using XuanYu.Core.Map;
using XuanYu.World;

namespace XuanYu.World.Map;

// MAP-A-R1-D3/D4：当前 World 地图状态所有者。加载/切换/卸载，暴露高度查询与渲染快照。
public sealed class WorldMapStateOwner
{
    WorldMapState? _current;

    public WorldMapState? CurrentMap => _current;
    public bool HasMap => _current is not null;

    public void Load(WorldMapState map)
    {
        _current = map;
    }

    public void Unload()
    {
        _current = null;
    }

    public SurfaceQueryResult QuerySurface(double worldX, double worldY) =>
        _current?.QuerySurface(worldX, worldY) ?? SurfaceQueryResult.NoTerrain;

    // 世界 X/Y（水平面）→ 地表 Z。无地图或地图外返回失败。
    public bool TryGetSurfaceHeight(double worldX, double worldY, out double surfaceZ)
    {
        var result = QuerySurface(worldX, worldY);
        surfaceZ = result.IsValid ? result.SurfaceZ : 0.0;
        return result.IsValid;
    }
}
