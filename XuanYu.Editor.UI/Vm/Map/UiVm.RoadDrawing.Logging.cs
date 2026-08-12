namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void LogRoadDrawingCreated() => LogRoadDrawingInfo("道路创建成功");
    void LogRoadDrawingCanceled() => LogRoadDrawingInfo("已取消道路绘制");
    void LogRoadDrawingInfo(string message) { _logBus.Info(EditorLogSource.Input, EditorLogCategory.Command, message, "道路绘制低频状态。"); RefreshLogBindings(); }
}
