namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4（补充裁决 §四）/D5：MapId 与路径的显示层属性（完整值不变）+ 表单错误状态。
public sealed partial class UiVm
{
    public string MapIdDisplay => MapIdDisplayFormat.Format(MapIdText);
    public string MapPathDisplay => string.IsNullOrEmpty(MapPath) ? "—" : MapPath;
    public bool IsMapFormError => !string.IsNullOrEmpty(MapEditError);
    // D5 纠偏：新建地图未保存流程依据——表单文本与已应用地图值不一致（含无效输入）
    public bool HasUnsavedMapChanges =>
        !TryParseMeters(MapWidthText, "宽度", out var width, out _) || width != MapSession.CurrentMap.SizeMeters.Width ||
        !TryParseMeters(MapDepthText, "深度", out var depth, out _) || depth != MapSession.CurrentMap.SizeMeters.Depth ||
        !TryParseMeters(MapBaseHeightText, "基础高度", out var height, out _) || height != MapSession.CurrentMap.Surface.BaseHeightMeters;
}
