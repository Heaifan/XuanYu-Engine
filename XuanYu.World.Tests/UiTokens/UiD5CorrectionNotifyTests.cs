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
        Assert.True(vm.LogPresentation.ShowInitialLogEmpty);
        Assert.False(vm.LogPresentation.ShowNoFilterResults);
        // 筛选无结果：只显示筛选空态
        vm.LogPresentation.SelectLogFilterCommand.Execute("错误");
        Assert.False(vm.LogPresentation.ShowInitialLogEmpty);
        Assert.True(vm.LogPresentation.ShowNoFilterResults);
        // 有日志：两个空态都不显示
        vm.LogPresentation.SelectLogFilterCommand.Execute("全部");
        vm.NotifyLogCopied(); // public 日志写入（复制日志）
        Assert.False(vm.LogPresentation.ShowInitialLogEmpty);
        Assert.False(vm.LogPresentation.ShowNoFilterResults);
    }

    [Fact]
    public void Notification_can_be_dismissed()
    {
        var vm = NewVm();
        vm.NotifyInfo("信息");
        Assert.True(vm.NotificationPanel.HasNotification);
        vm.DismissNotification();
        Assert.False(vm.NotificationPanel.HasNotification);
        vm.DismissNotification(); // 已关闭再次关闭无副作用
        Assert.False(vm.NotificationPanel.HasNotification);
    }

    [Fact]
    public void Same_category_notifications_merge_with_count()
    {
        var vm = NewVm();
        vm.NotifySuccess("保存成功");
        vm.NotifySuccess("保存成功");
        vm.NotifySuccess("保存成功");
        Assert.Equal("保存成功", vm.NotificationPanel.NotificationText);
        Assert.Equal(3, vm.NotificationPanel.NotificationCount);
        Assert.True(vm.NotificationPanel.ShowNotificationCount);
    }

    [Fact]
    public void Error_notification_is_not_overwritten_by_lower_priority()
    {
        var vm = NewVm();
        vm.NotifyError("保存失败");
        vm.NotifySuccess("保存成功"); // 低优先级不覆盖
        Assert.Equal(UiNotificationLevel.Error, vm.NotificationPanel.NotificationLevel);
        Assert.Equal("保存失败", vm.NotificationPanel.NotificationText);
        vm.NotifyWarning("磁盘不足"); // Warning(2) < Error(3)：仍不覆盖
        Assert.Equal(UiNotificationLevel.Error, vm.NotificationPanel.NotificationLevel);
    }

    [Fact]
    public void Higher_priority_overwrites_lower_priority()
    {
        var vm = NewVm();
        vm.NotifyInfo("编辑器已就绪");
        vm.NotifyError("致命错误");
        Assert.Equal(UiNotificationLevel.Error, vm.NotificationPanel.NotificationLevel);
    }
}
