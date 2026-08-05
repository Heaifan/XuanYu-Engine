using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D3-F2：地图日志显示映射（纯函数，内部枚举/错误码保持英文）。
public sealed partial class UiVm
{
    public static string FormatMapEditReason(MapEditReason reason) => reason switch
    {
        MapEditReason.NewMap => "新建地图",
        MapEditReason.Rename => "重命名",
        MapEditReason.Resize => "调整尺寸",
        MapEditReason.BaseHeightChanged => "基础高度修改",
        MapEditReason.Undo => "撤销",
        MapEditReason.Redo => "重做",
        MapEditReason.Replace => "替换地图",
        MapEditReason.MapPropertiesChanged => "地图属性修改",
        MapEditReason.LayerAdded => "添加图层",
        MapEditReason.LayerRemoved => "删除图层",
        MapEditReason.LayerRenamed => "重命名图层",
        MapEditReason.LayerMoved => "调整图层顺序",
        MapEditReason.LayerVisibilityChanged => "图层可见性",
        MapEditReason.LayerLockChanged => "图层锁定",
        _ => reason.ToString()
    };

    public static string FormatSurfaceKind(string kind) => kind switch
    {
        MapSurfaceKinds.Flat => "平面",
        MapSurfaceKinds.GentleHillsV1 => "缓丘",
        _ => kind
    };

    public static string FormatErrorCode(string code) => code switch
    {
        "InvalidMapName" => "地图名称无效",
        "InvalidMapSize" => "地图尺寸无效",
        "RegionWouldBeOutOfBounds" => "区域越界",
        "NotOnWriteThread" => "非写线程",
        "NoUndoAvailable" => "无撤销历史",
        "NoRedoAvailable" => "无重做历史",
        "UnknownLayer" => "未知图层",
        "UnknownRegion" => "未知区域",
        "SystemLayerProtected" => "系统图层保护",
        "LayerRemovalRejected" => "删除图层被拒绝",
        "LayerMoveRejected" => "调整顺序被拒绝",
        "InvalidLayerName" => "图层名称无效",
        "NotRegionLayer" => "非区域图层",
        _ => code
    };

    public static string FormatBoolean(bool value) => value ? "是" : "否";
}
