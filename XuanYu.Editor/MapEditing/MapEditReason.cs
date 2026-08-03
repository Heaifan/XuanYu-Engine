namespace XuanYu.Editor.MapEditing;

// MAP-A-R2-D2：地图编辑原因（内容变更事件携带）。
public enum MapEditReason
{
    NewMap = 0,
    Rename = 1,
    Resize = 2,
    BaseHeightChanged = 3,
    Undo = 4,
    Redo = 5,
    Replace = 6
}
