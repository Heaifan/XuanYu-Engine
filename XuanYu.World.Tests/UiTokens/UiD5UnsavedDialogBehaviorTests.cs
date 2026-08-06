using System;
using System.IO;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D5-FINAL：未保存地图弹窗行为合同（无修改直接新建/仅 discard 放行/不调用任何保存）。
public sealed class UiD5UnsavedDialogBehaviorTests
{
    static string SceneCommands() => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..",
        "XuanYu.Editor.UI", "Win", "UiWin.SceneCommands.cs"));

    static string ConfirmNewMapMethod(string scene)
    {
        var start = scene.IndexOf("ConfirmNewMapUnsaved(UiVm vm)", StringComparison.Ordinal);
        var end = scene.IndexOf("async Task<bool> SaveSceneAs", StringComparison.Ordinal);
        return scene.Substring(start, end - start);
    }

    [Fact]
    public void No_changes_new_map_skips_dialog()
    {
        var method = ConfirmNewMapMethod(SceneCommands());
        Assert.Contains("if (!vm.HasUnsavedMapChanges) return true;", method);
        var vm = new UiVm(null, () => true);
        Assert.False(vm.HasUnsavedMapChanges); // 初始无未保存 → 直接新建
    }

    [Fact]
    public void Session_dirty_triggers_dialog()
    {
        var vm = new UiVm(null, () => true);
        vm.RunCommand.Execute("添加图层"); // 图层修改 → IsDirty
        Assert.True(vm.HasUnsavedMapChanges);
    }

    [Fact]
    public void Pending_form_only_triggers_dialog()
    {
        var vm = new UiVm(null, () => true);
        vm.MapWidthText = "12000"; // 仅待提交表单
        Assert.True(vm.HasUnsavedMapChanges);
    }

    [Fact]
    public void Cancel_keeps_map_layers_history_and_input()
    {
        var method = ConfirmNewMapMethod(SceneCommands());
        Assert.Contains("return choice == \"discard\";", method); // 非 discard（含取消）→ 不新建
    }

    [Fact]
    public void Discard_and_new_executes_exactly_once()
    {
        var method = ConfirmNewMapMethod(SceneCommands());
        Assert.Contains("var choice = await ShowUnsavedMapChangesDialog();", method);
        Assert.Contains("return choice == \"discard\";", method); // 单次放行
    }

    [Fact]
    public void New_map_clears_old_history()
    {
        var vm = new UiVm(null, () => true);
        vm.MapWidthText = "12000";
        vm.MapDepthText = "12000";
        vm.RunCommand.Execute("应用地图属性");
        Assert.True(vm.MapSession.CanUndo);
        vm.NewMap(); // 新建地图（CreateNewMap 清空历史）
        Assert.False(vm.MapSession.CanUndo); // 旧历史不能继续撤销
    }

    [Fact]
    public void Confirm_flow_does_not_call_scene_save()
    {
        var method = ConfirmNewMapMethod(SceneCommands());
        Assert.DoesNotContain("SaveScene", method);
    }

    [Fact]
    public void Confirm_flow_does_not_call_nonexistent_map_save()
    {
        var method = ConfirmNewMapMethod(SceneCommands());
        Assert.DoesNotContain("MapSave", method);
        Assert.DoesNotContain("应用地图属性", method); // 未用应用冒充保存
    }

    [Fact]
    public void Missing_confirmation_service_blocks_new_map()
    {
        var method = ConfirmNewMapMethod(SceneCommands());
        Assert.Contains("return choice == \"discard\";", method); // 任何非 discard 结果（含服务异常）→ 不新建
    }
}
