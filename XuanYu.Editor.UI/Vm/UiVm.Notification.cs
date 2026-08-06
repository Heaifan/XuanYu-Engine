using System;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D5（纠偏）：四级通知状态机（Info/Success/Warning/Error）。
// 不刷屏：同类同文案合并计数（「保存成功 ×5」）；高优先级（Error/Warning）不被低优先级覆盖；
// 可关闭（DismissNotification）；CreatedAt 记录生命周期（自动消失策略 D6）；
// 技术详情由调用方写入既有日志系统。纯逻辑、无 Avalonia 依赖，可脱离 UI 测试。
public enum UiNotificationLevel { Info, Success, Warning, Error }

public sealed partial class UiVm
{
    public event Action? NotificationChanged;

    UiNotificationLevel _notifyLevel;
    string _notifyText = "";
    int _notifyCount = 1;
    int _notifySequence;
    DateTime _createdAt;

    public UiNotificationLevel NotificationLevel => _notifyLevel;
    public string NotificationText => _notifyText;
    public bool HasNotification => _notifyText.Length > 0;
    public int NotificationSequence => _notifySequence;
    public int NotificationCount => _notifyCount;
    public bool ShowNotificationCount => _notifyCount > 1;
    public DateTime CreatedAt => _createdAt;

    public bool IsNotificationInfo => _notifyLevel == UiNotificationLevel.Info;
    public bool IsNotificationSuccess => _notifyLevel == UiNotificationLevel.Success;
    public bool IsNotificationWarning => _notifyLevel == UiNotificationLevel.Warning;
    public bool IsNotificationError => _notifyLevel == UiNotificationLevel.Error;

    public void NotifyInfo(string text) => Notify(UiNotificationLevel.Info, text);
    public void NotifySuccess(string text) => Notify(UiNotificationLevel.Success, text);
    public void NotifyWarning(string text) => Notify(UiNotificationLevel.Warning, text);
    public void NotifyError(string text) => Notify(UiNotificationLevel.Error, text);

    // D5 纠偏：高优先级通知不被低优先级覆盖；同级别同文案合并为计数
    void Notify(UiNotificationLevel level, string text)
    {
        var priority = PriorityOf(level);
        var existingPriority = _notifyText.Length > 0 ? PriorityOf(_notifyLevel) : -1;
        if (existingPriority > priority) return;
        if (existingPriority == priority && _notifyText == text)
        {
            _notifyCount++;
        }
        else
        {
            _notifyLevel = level;
            _notifyText = text;
            _notifyCount = 1;
            _createdAt = DateTime.Now;
        }
        _notifySequence++;
        NotificationChanged?.Invoke();
    }

    public void DismissNotification()
    {
        if (_notifyText.Length == 0) return;
        _notifyText = "";
        _notifyCount = 1;
        _notifySequence++;
        NotificationChanged?.Invoke();
    }

    static int PriorityOf(UiNotificationLevel level) => level switch
    {
        UiNotificationLevel.Error => 3,
        UiNotificationLevel.Warning => 2,
        UiNotificationLevel.Success => 1,
        _ => 0
    };
}
