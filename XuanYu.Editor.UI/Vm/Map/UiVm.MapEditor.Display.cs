namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4（补充裁决 §四）：MapId 与路径的显示层压缩属性（完整值不变）。
public sealed partial class UiVm
{
    public string MapIdDisplay => MapIdDisplayFormat.Format(MapIdText);
    public string MapPathDisplay => string.IsNullOrEmpty(MapPath) ? "—" : MapPath;
}
