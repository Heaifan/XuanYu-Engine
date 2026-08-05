namespace XuanYu.Editor.UI;

// MAP-A-R2-D4：图层操作低频中文日志（复用既有日志总线）。
// 只记录用户有意义的动作；禁止记录鼠标经过/每帧状态/绑定刷新/列表测量。
public sealed partial class UiVm
{
    void LogLayer(string message)
    {
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command, message, "图层低频操作。");
        RefreshLogBindings();
    }
}
