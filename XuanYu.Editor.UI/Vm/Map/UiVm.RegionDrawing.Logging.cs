namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void LogRegionDrawingStarted() => LogRegionDrawingInfo("开始区域绘制");
    void LogRegionDrawingCreated() => LogRegionDrawingInfo("区域创建成功");
    void LogRegionDrawingCanceled() => LogRegionDrawingInfo("已取消区域绘制");

    void LogRegionDrawingError(string message)
    {
        _logBus.Error(EditorLogSource.Input, EditorLogCategory.Command, message, "区域绘制操作失败。");
        RefreshLogBindings();
    }

    void LogRegionDrawingInfo(string message)
    {
        _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command, message, "区域绘制低频状态。");
        RefreshLogBindings();
    }
}
