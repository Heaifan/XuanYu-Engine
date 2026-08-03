namespace XuanYu.World.Map;

// MAP-A-R2-D1-F1：图层角色（稳定标识，不依赖中文名称识别）。
// Base=基础地图层（唯一、Order 必须 0、不可删除、不可承载区域）；
// Region=区域层（承载区域）；Custom=自定义层（承载区域）。
public enum MapLayerKind
{
    Base = 0,
    Region = 1,
    Custom = 2
}
