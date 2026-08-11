using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D3：外壳尺寸与页签宿主结构合同（W15/W16/W18~W21/W43/G01/K01/K05）。
// 值断言直接对照 UI Spec 1.0 §7.1 与 §10.1；基线清除断言保证「只减不增」。
public sealed class UiTopTabStripContractTests
{
    static readonly string RepoRoot = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..");
    static string UiFile(string rel) => Path.Combine(RepoRoot, rel);

    static string Read(string rel) => File.ReadAllText(UiFile(rel));

    [Fact]
    public void Main_window_matches_spec_initial_and_minimum_size()
    {
        var axaml = Read("XuanYu.Editor.UI/Win/UiWin.axaml");
        Assert.Contains("Width=\"1360\"", axaml);
        Assert.Contains("Height=\"820\"", axaml);
        Assert.Contains("MinWidth=\"1024\"", axaml);
        Assert.Contains("MinHeight=\"640\"", axaml);
        Assert.Contains("Color.Bg.Application", axaml);       // W17：背景 Token 化
        Assert.DoesNotContain("#e9eef5", axaml);              // 旧值必须随迁移清除
    }

    [Fact]
    public void Root_layout_matches_min_panel_and_viewport_contract()
    {
        var axaml = Read("XuanYu.Editor.UI/Root/UiRoot.axaml");
        Assert.Contains("MinWidth=\"220\"", axaml);   // W19：左侧层级树最小 220
        Assert.Contains("MinWidth=\"300\"", axaml);   // W20：右侧面板最小 300
        Assert.Contains("MinWidth=\"480\"", axaml);   // 视口最小可用区域 480
        Assert.Contains("MinHeight=\"320\"", axaml);  // 视口高度维 320
        Assert.Contains("MinHeight=\"32\"", axaml);   // 日志折叠态 32（展开态 120~420 在 UiRoot.axaml.cs）
        Assert.Contains("Color.Border.Strong", axaml);      // 分隔条=可调整边界（规范 §9.1）
        Assert.Contains("Color.Hover.Bg", axaml);           // 分隔条悬停（规范 §9.3）
        Assert.Contains("Color.Border.Default", axaml);     // 视口 1 DIP 浅灰分隔
        Assert.DoesNotContain("#dce4ef", axaml);
        Assert.DoesNotContain("#9fb5d6", axaml);
        Assert.DoesNotContain("#C9D2DC", axaml);
    }

    [Fact]
    public void Root_codebehind_clamp_matches_shell_mins()
    {
        var cs = Read("XuanYu.Editor.UI/Root/UiRoot.axaml.cs");
        Assert.Contains("RootMargin = 12", cs);
        Assert.Contains("(LeftColumn, 270, 220, 420)", cs);
        Assert.Contains("(RightColumn, 340, 300, 480)", cs);
        Assert.Contains("LogRowFloor = 120", cs); // 展开态最小 120（规范 §7.1）
    }

    [Fact]
    public void Tab_host_template_is_single_line_with_overflow_controls()
    {
        var tpl = Read("XuanYu.Editor.UI/Right/TopTabStripTemplate.axaml");
        Assert.Contains("x:Key=\"TopTabStripTemplate\"", tpl);
        Assert.Contains("ItemsPresenter Name=\"PART_TabStrip\"", tpl);
        Assert.Contains("StackPanel Orientation=\"Horizontal\"", tpl);   // 单行：横向 StackPanel
        Assert.Contains("HorizontalScrollBarVisibility=\"Hidden\"", tpl); // 宽度充足无滚动控件
        Assert.Contains("VerticalScrollBarVisibility=\"Disabled\"", tpl); // 禁止换行/纵向滚动
        Assert.Contains("x:Name=\"TabScrollLeft\"", tpl);
        Assert.Contains("x:Name=\"TabScrollRight\"", tpl);
        Assert.Contains("x:Name=\"TabAllTabs\"", tpl);
        Assert.Contains("x:Name=\"TabFadeLeft\"", tpl);
        Assert.Contains("x:Name=\"TabFadeRight\"", tpl);
        Assert.Contains("x:Name=\"OverflowHintPopup\"", tpl);
        Assert.Contains("LinearGradientBrush", tpl);                      // 渐隐提示
        Assert.Contains("滚动鼠标滚轮或点击箭头查看更多页签。", tpl);       // 一次性提示文案（§10.1-7）
        Assert.DoesNotContain("WrapPanel", tpl);
        Assert.DoesNotContain("WrapPanel", Read("XuanYu.Editor.UI/Right/EditorRightTabs.axaml"));
    }

    [Fact]
    public void Right_panel_hosts_template_and_exact_real_tab_set()
    {
        var right = Read("XuanYu.Editor.UI/Right/EditorRightTabs.axaml");
        Assert.Contains("Right/TopTabStripTemplate.axaml", right);
        Assert.Contains("TopTabStripTemplate", right);                    // Template Setter 引用
        Assert.Contains("x:Name=\"SideTabs\"", right);
        var headers = Regex.Matches(right, "<TabItem[^>]*Header=\"([^\"]+)\"")
            .Select(m => m.Groups[1].Value).ToArray();
        Assert.Equal(["检查器", "调试"], headers); // Map Context 已由 EDITOR-A-R3 迁入左侧与 Inspector。
    }
}
