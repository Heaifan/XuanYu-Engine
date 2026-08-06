using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D5（纠偏）：审查要求的结构合同测试（焦点环 / 焦点陷阱 / 日志零原始色 / Inter 零残留 / 无压缩行）。
public sealed class UiD5CorrectionStructureTests
{
    static readonly string Repo = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");

    static string Read(string rel) => File.ReadAllText(Path.Combine(Repo, "XuanYu.Editor.UI", rel));

    [Fact]
    public void Focus_ring_contract_is_two_dip_plus_one_dip_offset()
    {
        var d5 = Read("Design/UiStyles.D5.axaml");
        Assert.Contains("FocusAdorner", d5);
        Assert.Contains("BorderThickness=\"2\"", d5); // 2 DIP 焦点框
        Assert.Contains("Margin=\"-1\"", d5);          // 1 DIP 外偏移
        Assert.Contains("Color.Focus", d5);            // 焦点非仅颜色（形状环 + 色）
    }

    [Fact]
    public void Focus_trap_cycles_inside_dialog_only()
    {
        // 3 个按钮：Tab 循环 0→1→2→0；Shift+Tab 反向；无焦点时从起点
        Assert.Equal(1, XuanYu.Editor.UI.DialogFocusTrap.NextIndex(3, 0, false));
        Assert.Equal(2, XuanYu.Editor.UI.DialogFocusTrap.NextIndex(3, 1, false));
        Assert.Equal(0, XuanYu.Editor.UI.DialogFocusTrap.NextIndex(3, 2, false));
        Assert.Equal(1, XuanYu.Editor.UI.DialogFocusTrap.NextIndex(3, 2, true));
        Assert.Equal(0, XuanYu.Editor.UI.DialogFocusTrap.NextIndex(3, -1, false));
        Assert.Equal(-1, XuanYu.Editor.UI.DialogFocusTrap.NextIndex(0, 0, false));
    }

    [Fact]
    public void Dialog_close_restores_focus_to_previous_control()
    {
        var dialog = Read("Win/UiWin.DialogHost.cs");
        Assert.Contains("_focusBeforeDialog = CurrentFocus()", dialog);
        Assert.Contains("_focusBeforeDialog?.Focus()", dialog);
    }

    [Fact]
    public void Log_panel_has_no_unregistered_raw_colors()
    {
        var foot = Read("Foot/Foot.axaml");
        var rawHex = Regex.Matches(foot, "#[0-9a-fA-F]{6,8}").Select(m => m.Value).ToArray();
        Assert.Empty(rawHex);
    }

    [Fact]
    public void Inter_font_has_zero_residue()
    {
        // D1 冻结链：Microsoft YaHei UI → Segoe UI → Noto Sans CJK SC → 系统 sans-serif（禁止 Inter 字体）
        // 用单词边界避免 Interval/Internal 等子串误报
        foreach (var file in Directory.EnumerateFiles(Path.Combine(Repo, "XuanYu.Editor.UI"), "*", SearchOption.AllDirectories)
                     .Where(f => f.EndsWith(".axaml") || f.EndsWith(".cs"))
                     .Where(f => !f.Contains("\\obj\\") && !f.Contains("/obj/")))
        {
            var text = File.ReadAllText(file);
            Assert.False(Regex.IsMatch(text, @"\bInter\b"), $"Inter residue in {file}");
        }
    }

    [Fact]
    public void No_hand_written_file_compresses_statements_to_escape_line_limit()
    {
        // 纠偏：禁止同一行多个公共属性/语句压缩（UiVm.Logging 系列已按职责拆分）
        var logging = Read("Vm/Logging/UiVm.Logging.cs");
        Assert.DoesNotContain("; public", logging);
        var state = Read("Vm/Logging/UiVm.Logging.State.cs");
        Assert.DoesNotContain("; public", state);
        var mapEditor = Read("Vm/Map/UiVm.MapEditor.cs");
        Assert.DoesNotContain("; public", mapEditor);
    }

    [Fact]
    public void Validation_files_are_split_by_responsibility()
    {
        Assert.True(File.Exists(Path.Combine(Repo, "XuanYu.Editor.UI", "Vm", "Logging", "UiVm.Logging.State.cs")));
        Assert.True(File.Exists(Path.Combine(Repo, "XuanYu.Editor.UI", "Vm", "Logging", "UiVm.Logging.Refresh.cs")));
        Assert.True(File.Exists(Path.Combine(Repo, "XuanYu.Editor.UI", "Vm", "Map", "UiVm.MapEditor.Validation.cs")));
    }
}
