using System.IO;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D5：弹窗宿主 + 日志空状态/回到底部结构合同。
public sealed class UiD5DialogAndLogContractTests
{
    static readonly string Win = Read("Win/UiWin.axaml");
    static readonly string Dialog = Read("Win/UiWin.DialogHost.cs");
    static readonly string Unsaved = Read("Win/UiWin.UnsavedDialog.cs");
    static readonly string Foot = Read("Foot/Foot.axaml");

    static string Read(string rel) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", rel));

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
        // 危险弹窗：默认按钮（Enter 触发）是「取消」（非危险）；危险按钮「继续」不接收默认焦点
        Assert.Contains("[(\"取消\", false, \"cancel\"), (\"继续\", true, \"ok\")], \"cancel\")", Dialog);
    }

    [Fact]
    public void Enter_triggers_default_and_escape_cancels()
    {
        Assert.Contains("Key.Escape", Dialog);
        Assert.Contains("Key.Enter", Dialog);
        Assert.Contains("_dialogDefault", Dialog);
    }

    [Fact]
    public void Unsaved_dialog_uses_host_with_save_as_default_and_discard_dangerous()
    {
        Assert.Contains("[(\"保存\", false, \"save\"), (\"不保存\", true, \"discard\"), (\"取消\", false, \"cancel\")], \"save\")", Unsaved);
        Assert.DoesNotContain("new Window", Unsaved);   // 代码构建 Window 已移除
        Assert.DoesNotContain("Brush.Parse", Unsaved);  // 代码颜色已移除
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
        var cs = Read("Foot/Foot.axaml.cs");
        Assert.Contains("TailStateChanged", cs);
        Assert.Contains("ForceFollow", cs);
    }

    [Fact]
    public void Log_auto_follow_keeps_user_scroll_pause_semantics()
    {
        var layout = Read("Foot/LogListAutoScrollController.Layout.cs");
        Assert.Contains("LogAutoScrollPolicy.ShouldFollow", layout); // 用户滚动维护尾部状态
        var controller = Read("Foot/LogListAutoScrollController.cs");
        Assert.Contains("_atTail && !_forceNext", controller);      // 阅读旧日志不强制拉回
    }

    [Fact]
    public void Log_empty_state_vm_flag_works()
    {
        var vm = new UiVm(null, () => true);
        Assert.False(vm.ShowNoFilterResults); // 默认「全部」筛选下不显示筛选空态
    }
}
