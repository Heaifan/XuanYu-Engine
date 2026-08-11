namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void LogDatasetOutcome(bool success, string action, string id, string type, string reason)
    {
        var message = success
            ? $"数据集{action}成功：{id}{(string.IsNullOrEmpty(type) ? "" : $"（{type}）")}"
            : $"数据集{action}失败：{id}（{type}）；原因：{reason}";
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command, message,
            "Dataset Create/Register 最终结果。命令收到日志不代表提交成功。");
        RefreshLogBindings();
    }
}
