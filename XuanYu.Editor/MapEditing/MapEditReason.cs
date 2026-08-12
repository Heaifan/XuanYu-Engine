namespace XuanYu.Editor.MapEditing;

// MAP-A-R2-D2/D3-A1/D4：地图编辑原因（内容变更事件携带）。
public enum MapEditReason
{
    NewMap = 0,
    Rename = 1,
    Resize = 2,
    BaseHeightChanged = 3,
    Undo = 4,
    Redo = 5,
    Replace = 6,
    MapPropertiesChanged = 7, // A1：宽度/深度/基础高度一次原子提交
    LayerAdded = 8, // D4：添加区域图层
    LayerRemoved = 9, // D4：删除区域图层
    LayerRenamed = 10, // D4：重命名图层
    LayerMoved = 11, // D4：调整图层顺序
    LayerVisibilityChanged = 12, // D4：修改图层可见性
    LayerLockChanged = 13, // D4：修改图层锁定状态
    RegionCreated = 14, // R3-D1：正式创建区域
    RegionDeleted = 15, // R3-D1：正式删除区域
    RuntimeProjection = 16, // MAP-DATA-A-R1：Dataset Manifest 的运行时图层投影
    RoadCreated = 17,
    RegionGeometryEdited = 18,
    RoadGeometryEdited = 19
}
