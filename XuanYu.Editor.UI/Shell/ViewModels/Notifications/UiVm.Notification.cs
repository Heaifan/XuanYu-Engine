namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly NotificationPresenter _notificationPresenter = new();
    public event Action? NotificationChanged;
    public NotificationPresenter NotificationPanel => _notificationPresenter;

    public void NotifyInfo(string text) => _notificationPresenter.Notify(UiNotificationLevel.Info, text);
    public void NotifySuccess(string text) => _notificationPresenter.Notify(UiNotificationLevel.Success, text);
    public void NotifyWarning(string text) => _notificationPresenter.Notify(UiNotificationLevel.Warning, text);
    public void NotifyError(string text) => _notificationPresenter.Notify(UiNotificationLevel.Error, text);
    public void DismissNotification() => _notificationPresenter.Dismiss();
    public bool ShouldAutoDismissNotification(DateTime now, TimeSpan lifetime) =>
        _notificationPresenter.ShouldAutoDismiss(now, lifetime);

    void InitNotificationPresenter()
    {
        _notificationPresenter.NotificationChanged += () => NotificationChanged?.Invoke();
    }
}
