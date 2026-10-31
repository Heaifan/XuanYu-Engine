using System.IO;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D5：弹窗宿主 + 日志空状态/回到底部结构合同。
public sealed class UiD5DialogAndLogContractTests
{
    static readonly string Win = Read("Shell/Windowing/WindowLifecycle/UiWin.axaml");
    static readonly string Dialog = Read("Shell/Windowing/DialogLifecycle/UiWin.DialogHost.cs");
    static readonly string UnsavedModal = Read("Shell/Windowing/Shortcuts/UnsavedChangesConfirmationWindow.axaml");
    static readonly string DialogInput = Read("Shell/Windowing/DialogLifecycle/UiWin.DialogHost.Input.cs");
    static readonly string Shortcuts = Read("Shell/Windowing/Shortcuts/UiWin.Shortcuts.cs");
    static readonly string Unsaved = Read("Shell/Windowing/DialogLifecycle/UiWin.UnsavedDialog.cs");
    static readonly string Foot = Read("Panels/Status/StatusWidgets/Foot.axaml");

    static string Read(string rel) => File.ReadAllText(Path.Combine(TestSourceRoot.Root,  "XuanYu.Editor.UI", rel));

    [Fact]
    public void Dialog_host_lives_in_main_window_overlay()
    {
        Assert.Contains("DialogOverlay", Win);
        Assert.Contains("DialogCard", Win);
        Assert.Contains("DialogCard_KeyDown", Win);
    }

    [Fact]
    public void Danger_dialog_defaults_to_non_dangerous_button()
    {
        // 危险弹窗：默认按钮（Enter 触发）是「取消」（非危险）；危险按钮「删除图层」写具体动作
        Assert.Contains("[(\"取消\", false, \"cancel\"), (actionText, true, \"ok\")], \"cancel\")", Dialog);
        Assert.Contains("ShowDanger(string title, string message, string actionText", Dialog);
    }

    [Fact]
    public void Enter_triggers_default_and_escape_cancels()
    {
        Assert.Contains("Key.Escape", DialogInput);
        Assert.Contains("Key.Enter", DialogInput);
        Assert.Contains("_dialogDefault", Dialog);
        Assert.Contains("TryHandleDialogKey(e)", Shortcuts);
        Assert.Contains("if (TryHandleDialogKey(e))", Shortcuts);
        Assert.Contains("_dialogTcs = null", Dialog);
    }

    [Fact]
    public void Unsaved_scene_dialog_uses_owned_modal_with_save_as_default_and_discard_dangerous()
    {
        Assert.Contains("x:Class=\"XuanYu.Editor.UI.UnsavedChangesConfirmationWindow\"", UnsavedModal);
        Assert.Contains("Content=\"保存\"", UnsavedModal);
        Assert.Contains("Content=\"不保存\"", UnsavedModal);
        Assert.Contains("Content=\"取消\"", UnsavedModal);
        Assert.Contains("UnsavedChangesConfirmationWindow.ShowAsync(this)", Unsaved);
    }

    [Fact]
    public void Log_empty_states_distinguish_initial_and_filtered()
    {
        Assert.Contains("暂无日志", Foot);
        Assert.Contains("没有匹配的日志", Foot);
        Assert.Contains("ShowNoFilterResults", Foot);
        Assert.Contains("清空筛选", Foot);
    }

    [Fact]
    public void Scroll_to_bottom_button_wired_to_tail_state()
    {
        Assert.Contains("ScrollToBottomButton", Foot);
        Assert.Contains("ScrollToBottom_Click", Foot);
        var cs = Read("Panels/Status/StatusWidgets/Foot.axaml.cs");
        Assert.Contains("TailStateChanged", cs);
        Assert.Contains("ForceFollow", cs);
    }

    [Fact]
    public void Log_auto_follow_keeps_user_scroll_pause_semantics()
    {
        var layout = Read("Panels/Status/LogControls/LogListAutoScrollController.Layout.cs");
        Assert.Contains("LogAutoScrollPolicy.ShouldFollow", layout); // 用户滚动维护尾部状态
        var controller = Read("Panels/Status/LogControls/LogListAutoScrollController.cs");
        Assert.Contains("_atTail && !_forceNext", controller);      // 阅读旧日志不强制拉回
    }

    [Fact]
    public void Log_empty_state_vm_flag_works()
    {
        var vm = new UiVm(null, () => true);
        Assert.False(vm.LogPresentation.ShowNoFilterResults); // 默认「全部」筛选下不显示筛选空态
    }
}
