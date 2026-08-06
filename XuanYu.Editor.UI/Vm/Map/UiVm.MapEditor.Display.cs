namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4（补充裁决 §四）/D5：MapId 与路径的显示层属性（完整值不变）+ 表单错误状态。
public sealed partial class UiVm
{
    public string MapIdDisplay => MapIdDisplayFormat.Format(MapIdText);
    public string MapPathDisplay => string.IsNullOrEmpty(MapPath) ? "—" : MapPath;
    public bool IsMapFormError => !string.IsNullOrEmpty(MapEditError);
}
