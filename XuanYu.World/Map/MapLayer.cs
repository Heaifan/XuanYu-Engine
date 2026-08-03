namespace XuanYu.World.Map;

// MAP-A-R2-D1：图层领域模型（领域权威层）。用于组织地图元素，不承担渲染管线功能。
// Kind 标识图层角色（Base 层不可承载区域、不可删除）。
public sealed record MapLayer(
    MapLayerId LayerId,
    string DisplayName,
    int Order,
    MapLayerKind Kind,
    bool IsVisible = true,
    bool IsLocked = false);
