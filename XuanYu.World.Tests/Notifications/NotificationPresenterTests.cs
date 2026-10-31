using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

public sealed class NotificationPresenterTests
{
    [Fact]
    public void Repeated_notice_merges_and_raises_one_synchronous_change()
    {
        var presenter = new NotificationPresenter();
        var changed = 0;
        presenter.NotificationChanged += () => changed++;

        presenter.Notify(UiNotificationLevel.Success, "saved");
        presenter.Notify(UiNotificationLevel.Success, "saved");

        Assert.Equal(2, presenter.NotificationCount);
        Assert.Equal(2, presenter.NotificationSequence);
        Assert.Equal(2, changed);
    }

    [Fact]
    public void Lower_priority_notice_does_not_replace_or_notify()
    {
        var presenter = new NotificationPresenter();
        var changed = 0;
        presenter.NotificationChanged += () => changed++;

        presenter.Notify(UiNotificationLevel.Error, "failed");
        presenter.Notify(UiNotificationLevel.Info, "ready");

        Assert.Equal("failed", presenter.NotificationText);
        Assert.Equal(UiNotificationLevel.Error, presenter.NotificationLevel);
        Assert.Equal(1, changed);
    }
}
