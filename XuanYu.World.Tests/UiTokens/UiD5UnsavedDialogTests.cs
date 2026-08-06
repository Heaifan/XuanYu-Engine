using System;
using System.IO;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D5-FINAL：未保存地图弹窗——正式文案（无内部编号）+ 行为合同（仅 discard 放行）。
public sealed class UiD5UnsavedDialogTests
{
    static string UnsavedDialog() => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..",
        "XuanYu.Editor.UI", "Win", "UiWin.UnsavedDialog.cs"));
    static string SceneCommands() => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..",
        "XuanYu.Editor.UI", "Win", "UiWin.SceneCommands.cs"));
    static string DialogHost() => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..",
        "XuanYu.Editor.UI", "Win", "UiWin.DialogHost.cs"));

    static string ConfirmNewMapMethod(string scene)
    {
        var start = scene.IndexOf("ConfirmNewMapUnsaved(UiVm vm)", StringComparison.Ordinal);
        var end = scene.IndexOf("async Task<bool> SaveSceneAs", StringComparison.Ordinal);
        return scene.Substring(start, end - start);
    }

    [Fact]
    public void Dialog_text_has_no_internal_governance_ids()
    {
        // 用户可见文案 = 地图弹窗 ShowDialogCore 调用段（标题/正文/按钮）
        var text = UnsavedDialog();
        var start = text.LastIndexOf("ShowDialogCore", StringComparison.Ordinal);
        var userText = text.Substring(start);
        Assert.DoesNotContain("D5", userText);
        Assert.DoesNotContain("D6", userText);
        Assert.DoesNotContain("MAP-A", userText);
        Assert.DoesNotContain("ARCH-UI-SPEC", userText);
    }

    [Fact]
    public void Dialog_uses_official_wording()
    {
        var d = UnsavedDialog();
        Assert.Contains("未保存的地图修改", d);
        Assert.Contains("当前地图有未保存的修改。当前版本暂不支持保存地图后新建。请选择取消，或不保存并新建。", d);
    }

    [Fact]
    public void No_save_and_new_button()
    {
        Assert.DoesNotContain("[(\"保存并新建\"", UnsavedDialog());
    }

    [Fact]
    public void No_vague_continue_button()
    {
        Assert.DoesNotContain("(\"继续\"", UnsavedDialog());
    }

    [Fact]
    public void Buttons_are_exactly_cancel_and_discard_and_new()
    {
        Assert.Contains("[(\"取消\", false, \"cancel\"), (\"不保存并新建\", true, \"discard\")], \"cancel\")", UnsavedDialog());
    }

    [Fact]
    public void Default_focus_is_cancel()
    {
        Assert.Contains("], \"cancel\")", UnsavedDialog()); // 默认值（Enter 触发）为取消
    }

    [Fact]
    public void Enter_does_not_execute_dangerous_action()
    {
        // Enter 只触发默认按钮（取消）；危险按钮「不保存并新建」不由 Enter 触发
        var host = DialogHost();
        Assert.Contains("if (e.Key != Key.Enter || _dialogDefault is null) return;", host);
        Assert.Contains("CompleteDialog((string)_dialogDefault.Content!)", host);
    }

    [Fact]
    public void Escape_equals_cancel()
    {
        Assert.Contains("if (e.Key == Key.Escape) { CompleteDialog(\"cancel\"); e.Handled = true; return; }", DialogHost());
    }
}
