using System.Linq;
using XuanYu.Editor.UI;
using XuanYu.World.Tests.Assets;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D5（纠偏）：危险操作 fail-closed 与字段级校验行为测试。
public sealed class UiD5CorrectionBehaviorTests
{
    static UiVm NewVm() => new(null, () => true);

    static string Logs(UiVm vm) => string.Join("\n", vm.LogItems.Select(e => e.Message));

    [Fact]
    public void Cancel_dangerous_confirmation_does_not_execute()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        var count = vm.LayerItems.Count;
        vm.DangerousCommandConfirmRequested += _ => { };
        vm.RunCommand.Execute("删除图层");
        Assert.True(vm.IsDangerousCommandPending("删除图层"));
        vm.CancelDangerousCommand("删除图层");
        Assert.Equal(count, vm.LayerItems.Count); // 取消不执行
        Assert.False(vm.IsDangerousCommandPending("删除图层"));
        Assert.Contains("已取消", Logs(vm));
    }
    [Fact]
    public void Confirm_executes_exactly_once()
    {
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        var count = vm.LayerItems.Count;
        vm.DangerousCommandConfirmRequested += _ => { };
        vm.RunCommand.Execute("删除图层");
        vm.ConfirmDangerousCommand("删除图层");
        Assert.Equal(count - 1, vm.LayerItems.Count); // 确认执行一次
        vm.ConfirmDangerousCommand("删除图层");       // 再次确认无效（pending 已清）
        Assert.Equal(count - 1, vm.LayerItems.Count);
    }
    [Fact]
    public void New_map_without_changes_skips_confirmation()
    {
        var vm = NewVm();
        Assert.False(vm.HasUnsavedMapChanges); // 初始表单与地图值一致 → 直接新建（不弹窗）
        vm.MapWidthText = "12000";
        Assert.True(vm.HasUnsavedMapChanges); // 修改表单 → 未保存
        vm.MapWidthText = "10000";
        Assert.False(vm.HasUnsavedMapChanges); // 改回原值 → 一致
    }
    [Fact]
    public void Field_error_marks_only_the_invalid_field()
    {
        var vm = NewVm();
        vm.MapWidthText = "abc";
        vm.RunCommand.Execute("应用地图属性");
        Assert.NotEqual("", vm.MapWidthError);
        Assert.Equal("", vm.MapDepthError);
        Assert.Equal("", vm.MapBaseHeightError);
        Assert.Equal("宽度", vm.FirstInvalidField);
        Assert.Contains("宽度必须是", vm.FormErrorSummary);
        Assert.Equal("abc", vm.MapWidthText); // 校验失败不清空输入
    }
    [Fact]
    public void Editing_field_clears_its_error_immediately()
    {
        var vm = NewVm();
        vm.MapWidthText = "abc";
        vm.RunCommand.Execute("应用地图属性");
        Assert.NotEqual("", vm.MapWidthError);
        vm.MapWidthText = "100"; // ValidateOnInput：输入即清除
        Assert.Equal("", vm.MapWidthError);
        Assert.Equal("", vm.MapDepthError);
    }
    [Fact]
    public void First_invalid_field_is_the_earliest_invalid()
    {
        var vm = NewVm();
        vm.MapWidthText = "100";   // 合法
        vm.MapDepthText = "xyz";   // 非法
        vm.MapBaseHeightText = "bad"; // 非法
        vm.RunCommand.Execute("应用地图属性");
        Assert.Equal("", vm.MapWidthError);
        Assert.NotEqual("", vm.MapDepthError);
        Assert.NotEqual("", vm.MapBaseHeightError);
        Assert.Equal("深度", vm.FirstInvalidField);
    }
    [Fact]
    public async Task Failed_scene_open_offers_retry_and_retry_reloads()
    {
        // Null 对话框服务：失败不自动重试（fail-safe）；Fake 服务可返回重试
        var fake = new FakeDialogService { RetryResult = false };
        var vm = new UiVm(null, () => true, dialogService: fake);
        var opened = await vm.OpenSceneAsync("C:/不存在.xyscene");
        Assert.False(opened);
        Assert.Contains(fake.Shown, d => d.Title.Contains("打开场景失败"));
    }
}
