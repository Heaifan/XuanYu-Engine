namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly NotificationPresenter _notificationPresenter = new();
    public event Action? NotificationChanged;

    public UiNotificationLevel NotificationLevel => _notificationPresenter.NotificationLevel;
    public string NotificationText => _notificationPresenter.NotificationText;
    public bool HasNotification => _notificationPresenter.HasNotification;
    public int NotificationSequence => _notificationPresenter.NotificationSequence;
    public int NotificationCount => _notificationPresenter.NotificationCount;
    public bool ShowNotificationCount => _notificationPresenter.ShowNotificationCount;
    public DateTime CreatedAt => _notificationPresenter.CreatedAt;
    public bool IsNotificationInfo => _notificationPresenter.IsNotificationInfo;
    public bool IsNotificationSuccess => _notificationPresenter.IsNotificationSuccess;
    public bool IsNotificationWarning => _notificationPresenter.IsNotificationWarning;
    public bool IsNotificationError => _notificationPresenter.IsNotificationError;

    public void NotifyInfo(string text) => _notificationPresenter.Notify(UiNotificationLevel.Info, text);
    public void NotifySuccess(string text) => _notificationPresenter.Notify(UiNotificationLevel.Success, text);
    public void NotifyWarning(string text) => _notificationPresenter.Notify(UiNotificationLevel.Warning, text);
    public void NotifyError(string text) => _notificationPresenter.Notify(UiNotificationLevel.Error, text);
    public void DismissNotification() => _notificationPresenter.Dismiss();
    public bool ShouldAutoDismissNotification(DateTime now, TimeSpan lifetime) =>
        _notificationPresenter.ShouldAutoDismiss(now, lifetime);

    void InitNotificationPresenter()
    {
        _notificationPresenter.PropertyChanged += (_, args) => OnPropertyChanged(args.PropertyName);
        _notificationPresenter.NotificationChanged += () => NotificationChanged?.Invoke();
    }
}
