using System.ComponentModel;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Logging;

public sealed class LogPerformanceGovernanceTests
{
    [Fact]
    public void Camera_wheel_does_not_emit_success_info_log()
    {
        var vm = new UiVm(null, () => true);

        for (var i = 0; i < 100; i++) Assert.True(vm.DollyCamera(1));

        Assert.DoesNotContain(vm.LogItems, item => item.Message == "相机 Dolly 已执行");
    }

    [Fact]
    public void High_frequency_wheel_does_not_refresh_log_ui()
    {
        var vm = new UiVm(null, () => true);
        var refreshes = vm.LogCollectionRefreshCount;

        for (var i = 0; i < 100; i++) Assert.True(vm.DollyCamera(1));

        Assert.Equal(refreshes, vm.LogCollectionRefreshCount);
    }

    [Fact]
    public void Log_items_keep_a_stable_collection_identity()
    {
        var vm = new UiVm(null, () => true);
        var first = vm.LogItems;

        Assert.True(vm.BeginCameraNavigation(1, 10, 10, false, 800, 600));

        Assert.Same(first, vm.LogItems);
    }

    [Fact]
    public void Log_append_does_not_invalidate_filter_state()
    {
        var vm = new UiVm(null, () => true);
        var notifications = new List<string?>();
        vm.PropertyChanged += Capture(notifications);

        Assert.True(vm.BeginCameraNavigation(1, 10, 10, false, 800, 600));

        Assert.DoesNotContain(nameof(vm.LogSearchText), notifications);
        Assert.DoesNotContain(nameof(vm.LogSourceFilter), notifications);
        Assert.DoesNotContain(nameof(vm.SelectedLogEntry), notifications);
        Assert.DoesNotContain(nameof(vm.IsLogFilterAll), notifications);
    }

    [Fact]
    public void Filter_change_rebuilds_projection_without_changing_buffer_counts()
    {
        var vm = new UiVm(null, () => true);
        Assert.True(vm.BeginCameraNavigation(1, 10, 10, false, 800, 600));
        var total = vm.LogItems.Count;

        vm.SelectLogFilterCommand.Execute("信息");

        Assert.Equal(total.ToString(), vm.AllCount);
        Assert.All(vm.LogItems, item => Assert.Equal(EditorLogLevel.Info, item.Level));
    }

    [Fact]
    public void Projection_append_does_not_rebuild_existing_items()
    {
        var cache = new LogProjectionCache();
        var first = NewEntry("first");
        var second = NewEntry("second");

        cache.Sync([first]);
        cache.Sync([first, second]);

        Assert.Equal(0, cache.RebuildCount);
        Assert.Same(first, cache.Items[0]);
        Assert.Same(second, cache.Items[1]);
    }

    [Fact]
    public void Warning_and_error_entries_remain_visible()
    {
        var vm = new UiVm(null, () => true);
        Assert.False(vm.DollyCamera(double.NaN));

        Assert.Contains(vm.LogItems, item => item.Level == EditorLogLevel.Error);
    }

    static LogEntry NewEntry(string message) => new(
        "12:00", EditorLogLevel.Info, EditorLogSource.Editor,
        EditorLogCategory.Command, message);

    static PropertyChangedEventHandler Capture(List<string?> names) =>
        (_, args) => names.Add(args.PropertyName);
}
