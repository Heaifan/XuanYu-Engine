using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XuanYu.Editor.UI;

public enum UiNotificationLevel { Info, Success, Warning, Error }

public sealed class NotificationPresenter : INotifyPropertyChanged
{
    UiNotificationLevel _level;
    string _text = "";
    int _count = 1, _sequence;
    DateTime _createdAt;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action? NotificationChanged;
    public UiNotificationLevel NotificationLevel => _level;
    public string NotificationText => _text;
    public bool HasNotification => _text.Length > 0;
    public int NotificationSequence => _sequence;
    public int NotificationCount => _count;
    public bool ShowNotificationCount => _count > 1;
    public DateTime CreatedAt => _createdAt;
    public bool IsNotificationInfo => _level == UiNotificationLevel.Info;
    public bool IsNotificationSuccess => _level == UiNotificationLevel.Success;
    public bool IsNotificationWarning => _level == UiNotificationLevel.Warning;
    public bool IsNotificationError => _level == UiNotificationLevel.Error;

    public void Notify(UiNotificationLevel level, string text)
    {
        var priority = PriorityOf(level);
        var current = HasNotification ? PriorityOf(_level) : -1;
        if (current > priority) return;
        if (current == priority && _text == text) _count++;
        else { _level = level; _text = text; _count = 1; _createdAt = DateTime.Now; }
        _sequence++;
        Publish();
    }

    public void Dismiss()
    {
        if (!HasNotification) return;
        _text = ""; _count = 1; _sequence++;
        Publish();
    }

    public bool ShouldAutoDismiss(DateTime now, TimeSpan lifetime) =>
        HasNotification && lifetime > TimeSpan.Zero && now - CreatedAt >= lifetime;

    void Publish()
    {
        foreach (var name in new[] { nameof(NotificationLevel), nameof(NotificationText), nameof(HasNotification),
            nameof(NotificationSequence), nameof(NotificationCount), nameof(ShowNotificationCount), nameof(CreatedAt),
            nameof(IsNotificationInfo), nameof(IsNotificationSuccess), nameof(IsNotificationWarning), nameof(IsNotificationError) })
            OnPropertyChanged(name);
        NotificationChanged?.Invoke();
    }

    static int PriorityOf(UiNotificationLevel level) => level switch
    {
        UiNotificationLevel.Error => 3,
        UiNotificationLevel.Warning => 2,
        UiNotificationLevel.Success => 1,
        _ => 0
    };

    void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
