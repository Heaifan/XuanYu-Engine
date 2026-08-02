using XuanYu.Core.Map;

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

    // 世界 X/Y（水平面）→ 地表 Z。无地图或地图外返回失败。
    public bool TryGetSurfaceHeight(double worldX, double worldY, out double surfaceZ)
    {
        if (_current is null)
        {
            surfaceZ = 0.0;
            return false;
        }

        return _current.TryGetSurfaceHeight(worldX, worldY, out surfaceZ);
    }

    public MapRenderSnapshot BuildRenderSnapshot() =>
        _current is { } map
            ? new MapRenderSnapshot(
                map.MapId, map.Name, map.WidthMeters, map.DepthMeters,
                map.SurfaceKind, map.BaseHeightMeters, map.AmplitudeMeters,
                map.WavelengthMeters, map.Seed,
                map.SunDirectionX, map.SunDirectionY, map.SunDirectionZ,
                map.SunIntensity, map.AmbientIntensity)
            : MapRenderSnapshot.Empty;
}
