namespace XuanYu.Editor.MapDocument;

// MAP-A-R2-D1：图层领域模型。用于组织地图元素，不承担渲染管线功能。
// IsFixed=true 表示固定层（如"基础地图"层），不可删除。
public sealed record MapLayer(
    MapLayerId LayerId,
    string DisplayName,
    int Order,
    bool IsVisible,
    bool IsLocked,
    bool IsFixed = false);
