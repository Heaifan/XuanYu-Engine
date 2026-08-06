using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D5：四级通知状态机——级别/文案/单条覆盖（不刷屏）/事件/序列。
public sealed class UiD5NotificationTests
{
    static UiVm NewVm() => new(null, () => true);

    [Fact]
    public void Notify_info_sets_info_level()
    {
        var vm = NewVm();
        vm.NotifyInfo("编辑器已就绪");
        Assert.True(vm.HasNotification);
        Assert.Equal(UiNotificationLevel.Info, vm.NotificationLevel);
        Assert.Equal("编辑器已就绪", vm.NotificationText);
        Assert.True(vm.IsNotificationInfo);
        Assert.False(vm.IsNotificationSuccess);
        Assert.False(vm.IsNotificationWarning);
        Assert.False(vm.IsNotificationError);
    }

    [Fact]
    public void Four_levels_set_distinct_states()
    {
        var vm = NewVm();
        vm.NotifySuccess("已保存");
        Assert.True(vm.IsNotificationSuccess);
        vm.NotifyWarning("磁盘空间不足");
        Assert.True(vm.IsNotificationWarning);
        vm.NotifyError("地图属性无效");
        Assert.True(vm.IsNotificationError);
    }

    [Fact]
    public void Newest_notification_replaces_previous_without_flood()
    {
        var vm = NewVm();
        vm.NotifyInfo("第一条");
        vm.NotifyInfo("第二条");
        vm.NotifyWarning("警告");
        // 不刷屏：只保留最新一条
        Assert.Equal("警告", vm.NotificationText);
        Assert.Equal(UiNotificationLevel.Warning, vm.NotificationLevel);
        Assert.Equal(3, vm.NotificationSequence);
    }

    [Fact]
    public void Notification_changed_event_raised_once_per_set()
    {
        var vm = NewVm();
        var count = 0;
        vm.NotificationChanged += () => count++;
        vm.NotifyError("错误");
        vm.NotifyInfo("信息");
        Assert.Equal(2, count);
        Assert.Equal(2, vm.NotificationSequence);
    }

    [Fact]
    public void Map_property_apply_emits_success_notification()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("应用地图属性"); // 默认地图属性合法
        Assert.True(vm.IsNotificationSuccess);
        Assert.Equal("地图属性已应用", vm.NotificationText);
    }

    [Fact]
    public void Map_property_apply_invalid_input_emits_error_notification()
    {
        var vm = NewVm();
        vm.MapWidthText = "abc";
        vm.RunCommand.Execute("应用地图属性");
        Assert.True(vm.IsNotificationError);
        Assert.True(vm.IsMapFormError);
    }
}
