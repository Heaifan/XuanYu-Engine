using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D5（纠偏）：日志空态互斥与通知合并/关闭/优先级行为测试。
public sealed class UiD5CorrectionNotifyTests
{
    static UiVm NewVm() => new(null, () => true);

    [Fact]
    public void Log_empty_states_are_mutually_exclusive()
    {
        var vm = NewVm();
        // 初始（「全部」筛选 + 无日志）：只显示初始空态
        Assert.True(vm.ShowInitialLogEmpty);
        Assert.False(vm.ShowNoFilterResults);
        // 筛选无结果：只显示筛选空态
        vm.SelectLogFilterCommand.Execute("错误");
        Assert.False(vm.ShowInitialLogEmpty);
        Assert.True(vm.ShowNoFilterResults);
        // 有日志：两个空态都不显示
        vm.SelectLogFilterCommand.Execute("全部");
        vm.NotifyLogCopied(); // public 日志写入（复制日志）
        Assert.False(vm.ShowInitialLogEmpty);
        Assert.False(vm.ShowNoFilterResults);
    }

    [Fact]
    public void Notification_can_be_dismissed()
    {
        var vm = NewVm();
        vm.NotifyInfo("信息");
        Assert.True(vm.HasNotification);
        vm.DismissNotification();
        Assert.False(vm.HasNotification);
        vm.DismissNotification(); // 已关闭再次关闭无副作用
        Assert.False(vm.HasNotification);
    }

    [Fact]
    public void Same_category_notifications_merge_with_count()
    {
        var vm = NewVm();
        vm.NotifySuccess("保存成功");
        vm.NotifySuccess("保存成功");
        vm.NotifySuccess("保存成功");
        Assert.Equal("保存成功", vm.NotificationText);
        Assert.Equal(3, vm.NotificationCount);
        Assert.True(vm.ShowNotificationCount);
    }

    [Fact]
    public void Error_notification_is_not_overwritten_by_lower_priority()
    {
        var vm = NewVm();
        vm.NotifyError("保存失败");
        vm.NotifySuccess("保存成功"); // 低优先级不覆盖
        Assert.Equal(UiNotificationLevel.Error, vm.NotificationLevel);
        Assert.Equal("保存失败", vm.NotificationText);
        vm.NotifyWarning("磁盘不足"); // Warning(2) < Error(3)：仍不覆盖
        Assert.Equal(UiNotificationLevel.Error, vm.NotificationLevel);
    }

    [Fact]
    public void Higher_priority_overwrites_lower_priority()
    {
        var vm = NewVm();
        vm.NotifyInfo("编辑器已就绪");
        vm.NotifyError("致命错误");
        Assert.Equal(UiNotificationLevel.Error, vm.NotificationLevel);
    }
}
