namespace XuanYu.World.Map;

// MAP-A-R2-D4：图层角色（稳定标识，不依赖中文名称识别）。
// Ground=地面层（固定 1 个、Order 0、不可删除/重命名/排序、不承载区域）；
// Boundary=边界层（固定 1 个、Order 1、系统层、不承载区域）；
// Region=区域层（至少 1 个，可添加/删除/重命名/排序，承载区域）。
public enum MapLayerKind
{
    Ground = 0,
    Boundary = 1,
    Region = 2
}
