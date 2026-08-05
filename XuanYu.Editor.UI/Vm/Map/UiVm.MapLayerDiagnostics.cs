using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D4/D4-F2：图层操作低频中文日志（复用既有日志总线）。
// 只记录用户有意义的动作；禁止记录鼠标经过/每帧状态/绑定刷新/列表测量。
public sealed partial class UiVm
{
    void LogLayer(string message, string? detail = null)
    {
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
            message, detail ?? "图层低频操作。");
        RefreshLogBindings();
    }

    // F2：锁定/解锁动作日志——消息列简洁（动作+名称+类型），详情列带 LayerId 与状态变化。
    void LogLayerLockChanged(MapLayer layer, bool before, bool after)
    {
        var action = after ? "锁定" : "解锁";
        var kind = layer.Kind == MapLayerKind.Region ? "区域" : "系统";
        LogLayer($"{action}图层：{layer.DisplayName}（{kind}）",
            $"LayerId={layer.LayerId.Value}；状态：{(before ? "已锁定" : "未锁定")} → {(after ? "已锁定" : "未锁定")}");
    }
}
