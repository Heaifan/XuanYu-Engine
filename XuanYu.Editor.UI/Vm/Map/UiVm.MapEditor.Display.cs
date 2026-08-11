namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4（补充裁决 §四）/D5：MapId 与路径的显示层属性（完整值不变）+ 表单错误状态。
public sealed partial class UiVm
{
    // R2-F1：Dataset 只能使用正式 map.json Manifest 路径，不能把旧 .xymap 会话路径当作 MapRoot。
    public string CurrentMapManifestPath => _mapManifestOwner.CurrentPath ?? "";

    public string MapIdDisplay => MapIdDisplayFormat.Format(MapIdText);
    public string MapPathDisplay => string.IsNullOrEmpty(MapPath) ? "—" : MapPath;
    public bool IsMapFormError => !string.IsNullOrEmpty(MapEditError);
    // ARCH-UI-SPEC-R1-D5-FINAL：地图状态四态（内存基线 ≠ 已保存到磁盘）。
    // 无路径+无修改 → 未落盘；无路径+有修改 → 未保存；有路径+无修改 → 已保存；有路径+有修改 → 有未保存修改。
    public string MapStatusText
    {
        get
        {
            var hasFilePath = !string.IsNullOrWhiteSpace(MapPath) ||
                !string.IsNullOrWhiteSpace(MapSession.CurrentFilePath);
            if (!hasFilePath)
                return HasUnsavedMapChanges ? "未保存" : "未落盘";
            return HasUnsavedMapChanges ? "有未保存修改" : "已保存";
        }
    }
    // D5 二次纠偏（按用户方案）：表单值与当前模型不一致 = 待提交表单修改
    public bool HasPendingMapFormChanges =>
        !TryParseMeters(MapWidthText, "宽度", out var width, out _) || width != MapSession.CurrentMap.SizeMeters.Width ||
        !TryParseMeters(MapDepthText, "深度", out var depth, out _) || depth != MapSession.CurrentMap.SizeMeters.Depth ||
        !TryParseMeters(MapBaseHeightText, "基础高度", out var height, out _) || height != MapSession.CurrentMap.Surface.BaseHeightMeters;
    // D5 二次纠偏（按用户方案）：真实未保存 = 会话语义 Dirty（图层/显隐/锁定/已应用未落盘等）|| 待提交表单修改
    public bool HasUnsavedMapChanges => MapSession.IsDirty || _mapManifestOwner.IsDirty || HasPendingMapFormChanges;
}
