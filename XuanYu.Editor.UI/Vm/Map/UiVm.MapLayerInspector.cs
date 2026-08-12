using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D4：图层检查器入口（右侧检查器选中图层时显示）。
// 名称提交采用"输入框文本 + CommitLayerRename"（Enter/失焦提交，逐键不触发会话命令）。
public sealed partial class UiVm
{
    string _layerInspectorNameText = "";

    public string LayerInspectorNameText
    {
        get => _layerInspectorNameText;
        set => Set(ref _layerInspectorNameText, value); // 暂存输入框文本；提交由 CommitLayerRename 执行
    }

    public string LayerInspectorKindText => SelectedLayer?.KindTagText ?? "";

    public string LayerInspectorIdText => SelectedLayer?.LayerId.Value ?? "";

    public string LayerInspectorOrderText => SelectedLayer is { } layer
        ? MapSession.CurrentMap.Layers.First(l => l.LayerId == layer.LayerId).Order.ToString()
        : "";

    public bool LayerInspectorIsRegion =>
        SelectedLayer is { } layer &&
        MapLayerRules.Find(MapSession.CurrentMap.Layers, layer.LayerId) is { Kind: MapLayerKind.Region };

    public bool LayerInspectorIsSystem => SelectedLayer?.IsSystem ?? false;

    public bool LayerInspectorVisible
    {
        get => SelectedLayer is { } layer && MapLayerRules.Find(
            MapSession.CurrentMap.Layers, layer.LayerId) is { IsVisible: true };
        set
        {
            if (SelectedLayer is { } layer) SetLayerVisibility(layer.LayerId, value);
        }
    }

    public bool LayerInspectorLocked
    {
        get => SelectedLayer is { } layer && MapLayerRules.Find(
            MapSession.CurrentMap.Layers, layer.LayerId) is { IsLocked: true };
        set
        {
            if (SelectedLayer is { } layer) SetLayerLock(layer.LayerId, value);
        }
    }

    // 提交检查器名称（Enter/失焦）：从输入框文本走会话命令，成功后列表/日志同步。
    public void CommitLayerRename(string text) => _ = CommitLayerRenameAsync(text);

    public async Task<bool> CommitLayerRenameAsync(string text)
    {
        if (SelectedLayer is not { } layer) return false;
        if (TryGetDatasetIdForLayer(layer.LayerId, out var datasetId))
        {
            SelectDataset(datasetId);
            return await RenameSelectedDatasetAsync(text);
        }
        var result = MapSession.RenameLayer(layer.LayerId, text);
        if (!result.IsSuccess) { FailLayerEdit("图层重命名", result); return false; }
        var after = MapSession.CurrentMap.Layers.First(l => l.LayerId == layer.LayerId).DisplayName;
        LogLayer($"重命名图层：{layer.Name} → {after}");
        FooterMessage = $"图层已重命名：{after}。";
        LayerInspectorNameText = after;
        return true;
    }
}
