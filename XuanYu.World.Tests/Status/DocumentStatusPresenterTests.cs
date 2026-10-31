using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Status;

public sealed class DocumentStatusPresenterTests
{
    [Fact]
    public void Dirty_and_busy_state_drive_document_status_and_save_button_projection()
    {
        var dirty = true;
        var presenter = new DocumentStatusPresenter(() => dirty);

        Assert.Equal("状态：未保存", presenter.DocumentStatusText);
        Assert.True(presenter.IsSaveButtonHighlighted);
        Assert.True(presenter.CanRunSaveCommand);

        presenter.SetSceneBusy(true);

        Assert.True(presenter.IsSceneBusy);
        Assert.False(presenter.IsSaveButtonHighlighted);
        Assert.False(presenter.CanRunSaveCommand);
        dirty = false;
        Assert.Equal("状态：就绪", presenter.DocumentStatusText);
    }

    [Fact]
    public void Footer_status_updates_notify_the_owner_and_project_failure_colors()
    {
        var presenter = new DocumentStatusPresenter(() => false);
        var notifications = new List<string>();
        presenter.PropertyChanged += (_, args) => notifications.Add(args.PropertyName!);

        presenter.SetFooterState("状态：保存失败");

        Assert.Equal("状态：保存失败", presenter.DocumentStatusText);
        Assert.Equal("#fdeeee", presenter.DocumentStatusBackground);
        Assert.Equal("#e2aaaa", presenter.DocumentStatusBorderBrush);
        Assert.Equal("#a43f3f", presenter.DocumentStatusForeground);
        Assert.Contains(nameof(presenter.FooterState), notifications);
        Assert.Contains(nameof(presenter.DocumentStatusText), notifications);
        Assert.Contains(nameof(presenter.CanRunSaveCommand), notifications);
    }
}
