using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Logging;

public sealed class LogPresenterTests
{
    [Fact]
    public void Log_bus_write_synchronously_refreshes_projection_and_notifications()
    {
        var presenter = new LogPresenter();
        var notifications = new List<string>();
        presenter.PropertyChanged += (_, args) => notifications.Add(args.PropertyName!);

        presenter.LogBus.Info(EditorLogSource.Render, EditorLogCategory.Backend, "render");

        Assert.Single(presenter.LogItems);
        Assert.Contains(nameof(presenter.LogItems), notifications);
        Assert.Contains(nameof(presenter.AllCount), notifications);
    }

    [Fact]
    public void Refresh_builds_filtered_and_specialized_projections()
    {
        var presenter = new LogPresenter();
        var notifications = new List<string>();
        presenter.PropertyChanged += (_, args) => notifications.Add(args.PropertyName!);
        presenter.LogBus.Info(EditorLogSource.Render, EditorLogCategory.Backend, "render");
        presenter.LogBus.Warning(EditorLogSource.Editor, EditorLogCategory.Command, "warning");
        presenter.LogBus.Info(EditorLogSource.Build, EditorLogCategory.Command, "build");
        presenter.LogBus.Info(EditorLogSource.Task, EditorLogCategory.Command, "task");

        presenter.Refresh();

        Assert.Equal(4, presenter.LogItems.Count);
        Assert.Single(presenter.ProblemItems);
        Assert.Single(presenter.BuildItems);
        Assert.Single(presenter.TaskItems);
        Assert.Contains(nameof(presenter.LogItems), notifications);
        Assert.Contains(nameof(presenter.LogSummary), notifications);
        Assert.Equal("4", presenter.AllCount);
    }

    [Fact]
    public void Search_source_filter_and_clear_keep_one_buffer_and_refresh_projections()
    {
        var presenter = new LogPresenter();
        presenter.LogBus.Info(EditorLogSource.Render, EditorLogCategory.Backend, "resize");
        presenter.LogBus.Info(EditorLogSource.Editor, EditorLogCategory.Command, "save");
        presenter.Refresh();

        presenter.SetSource(EditorLogSource.Render);
        presenter.SetSearchText("resize");

        Assert.Single(presenter.LogItems);
        Assert.Equal("resize", presenter.SearchText);
        presenter.Clear();
        Assert.Empty(presenter.LogItems);
        Assert.Equal("0", presenter.AllCount);
    }
}
