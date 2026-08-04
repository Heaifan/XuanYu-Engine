namespace XuanYu.Editor.MapEditing;

// MAP-A-R2-D2/D3-A1：地图编辑原因（内容变更事件携带）。
public enum MapEditReason
{
    NewMap = 0,
    Rename = 1,
    Resize = 2,
    BaseHeightChanged = 3,
    Undo = 4,
    Redo = 5,
    Replace = 6,
    MapPropertiesChanged = 7 // A1：宽度/深度/基础高度一次原子提交
}
