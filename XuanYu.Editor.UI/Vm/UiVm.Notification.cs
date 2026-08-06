using System;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D5：四级通知状态机（Info/Success/Warning/Error）。
// 不刷屏：只保留最新一条通知（序列号递增）；技术详情由调用方写入既有日志系统。
// 纯逻辑、无 Avalonia 依赖，可脱离 UI 测试。
public enum UiNotificationLevel { Info, Success, Warning, Error }

public sealed partial class UiVm
{
    public event Action? NotificationChanged;

    UiNotificationLevel _notifyLevel;
    string _notifyText = "";
    int _notifySequence;

    public UiNotificationLevel NotificationLevel => _notifyLevel;
    public string NotificationText => _notifyText;
    public bool HasNotification => _notifyText.Length > 0;
    public int NotificationSequence => _notifySequence;

    public bool IsNotificationInfo => _notifyLevel == UiNotificationLevel.Info;
    public bool IsNotificationSuccess => _notifyLevel == UiNotificationLevel.Success;
    public bool IsNotificationWarning => _notifyLevel == UiNotificationLevel.Warning;
    public bool IsNotificationError => _notifyLevel == UiNotificationLevel.Error;

    public void NotifyInfo(string text) => SetNotification(UiNotificationLevel.Info, text);
    public void NotifySuccess(string text) => SetNotification(UiNotificationLevel.Success, text);
    public void NotifyWarning(string text) => SetNotification(UiNotificationLevel.Warning, text);
    public void NotifyError(string text) => SetNotification(UiNotificationLevel.Error, text);

    void SetNotification(UiNotificationLevel level, string text)
    {
        _notifyLevel = level;
        _notifyText = text;
        _notifySequence++;
        NotificationChanged?.Invoke();
    }
}
