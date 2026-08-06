using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D5（二次纠偏，用户方案）：未保存地图判断（9 项）。
// HasUnsavedMapChanges = MapSession.IsDirty || HasPendingMapFormChanges；
// 默认地图处于内存基线（初始不误判）；「保存并新建」因无真实地图持久化停止上报（D6 恢复）。
public sealed class UiD5UnsavedFlowTests
{
    static UiVm NewVm() => new(null, () => true);

    [Fact]
    public void Initial_default_map_has_no_unsaved_changes()
    {
        var vm = NewVm();
        Assert.False(vm.MapSession.IsDirty);          // 默认地图处于内存基线
        Assert.False(vm.HasPendingMapFormChanges);
        Assert.False(vm.HasUnsavedMapChanges);        // 初始无未保存 → 新建不弹窗
    }
    [Fact]
    public void Editing_input_alone_marks_unsaved()
    {
        // 仅修改输入框（未应用）→ 有未保存修改
        var vm = NewVm();
        vm.MapWidthText = "12000";
        Assert.True(vm.HasPendingMapFormChanges);
        Assert.True(vm.HasUnsavedMapChanges);
        vm.MapWidthText = "10000"; // 改回原值 → 一致
        Assert.False(vm.HasPendingMapFormChanges);
    }
    [Fact]
    public void Applied_properties_keep_unsaved_until_saved()
    {
        // 应用属性后 MapSession Dirty → 仍有未保存修改（即使表单与模型一致）
        var vm = NewVm();
        vm.MapWidthText = "20000";
        vm.MapDepthText = "8000";
        vm.RunCommand.Execute("应用地图属性");
        Assert.True(vm.MapSession.IsDirty);
        Assert.False(vm.HasPendingMapFormChanges); // 表单已与模型一致
        Assert.True(vm.HasUnsavedMapChanges);      // 但会话 Dirty → 仍为未保存
    }
    [Fact]
    public void Layer_edit_alone_marks_unsaved()
    {
        // 仅修改图层 → 有未保存修改（IsDirty 捕获）
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        Assert.True(vm.MapSession.IsDirty);
        Assert.False(vm.HasPendingMapFormChanges);
        Assert.True(vm.HasUnsavedMapChanges);
    }
    [Fact]
    public void Dirty_with_matching_form_still_unsaved()
    {
        // Dirty 且表单相同 → 仍为未保存（仍弹确认）
        var vm = NewVm();
        vm.RunCommand.Execute("添加图层");
        Assert.True(vm.HasUnsavedMapChanges);
        Assert.False(vm.HasPendingMapFormChanges);
    }
    [Fact]
    public void Save_and_new_is_not_offered_without_persistence()
    {
        // 停止上报：地图持久化（真实保存到资产文件）尚未接入（D6）——
        // 弹窗不得提供「保存并新建」（禁止用「应用属性」冒充保存）
        var unsaved = System.IO.File.ReadAllText(System.IO.Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Win", "UiWin.UnsavedDialog.cs"));
        Assert.Contains("不保存并新建", unsaved);
        Assert.DoesNotContain("[(\"保存并新建\"", unsaved);
        var scene = System.IO.File.ReadAllText(System.IO.Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Win", "UiWin.SceneCommands.cs"));
        Assert.Contains("地图持久化（真实保存到资产文件）尚未接入（D6）", scene);
        Assert.DoesNotContain("RunCommand.Execute(\"应用地图属性\")", scene); // 未用应用冒充保存
    }
    [Fact]
    public void Discard_and_new_is_the_explicit_danger_action()
    {
        // 不保存并新建 = 明确丢弃后新建（危险按钮、具体动作、默认焦点=取消）
        var unsaved = System.IO.File.ReadAllText(System.IO.Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Win", "UiWin.UnsavedDialog.cs"));
        Assert.Contains("[(\"取消\", false, \"cancel\"), (\"不保存并新建\", true, \"discard\")], \"cancel\")", unsaved);
    }
    [Fact]
    public void Cancel_keeps_map_and_all_changes()
    {
        // 取消 → 不新建（原地图和全部修改保持不变）——确认流程仅 discard 放行
        var scene = System.IO.File.ReadAllText(System.IO.Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Win", "UiWin.SceneCommands.cs"));
        Assert.Contains("return choice == \"discard\";", scene);
    }
}
