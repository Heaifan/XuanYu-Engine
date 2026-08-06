using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Automation;
using Avalonia.Controls;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiTokens;

public sealed class UiD6AccessibilityContractTests
{
    static readonly string RepoRoot = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..");
    static readonly string UiDir = Path.Combine(RepoRoot, "XuanYu.Editor.UI");

    [Fact]
    public void New_editor_controls_declare_accessible_names()
    {
        var required = new[]
        {
            "复制完整 MapId", "新建地图", "打开地图", "保存地图", "聚焦地图",
            "地图宽度", "地图深度", "基础高度", "应用地图属性", "撤销地图修改",
            "重做地图修改", "添加图层", "删除图层",
            "显示或隐藏图层", "锁定或解锁图层", "展开或折叠日志", "回到底部"
        };

        var xaml = string.Join("\n", Directory.GetFiles(UiDir, "*.axaml",
            SearchOption.AllDirectories).Select(File.ReadAllText));
        Assert.All(required, name => Assert.Contains(
            $"AutomationProperties.Name=\"{name}\"", xaml));
    }

    [Fact]
    public void Automation_fallback_never_exports_internal_codes()
    {
        var panel = new StackPanel();
        var userButton = new Button { Content = "新建地图" };
        var internalButton = new Button { Content = "ARCH-UI-SPEC-R1-D6" };
        panel.Children.Add(userButton);
        panel.Children.Add(internalButton);

        UiAutomationNamer.Apply(panel);

        Assert.Equal("新建地图", AutomationProperties.GetName(userButton));
        Assert.True(string.IsNullOrWhiteSpace(
            AutomationProperties.GetName(internalButton)));
    }

    [Fact]
    public void Xaml_visible_text_does_not_expose_d6_or_arch_codes()
    {
        var pattern = new Regex("(Content|Text|ToolTip\\.Tip)=\"([^\"]*)\"");
        var leaks = Directory.GetFiles(UiDir, "*.axaml", SearchOption.AllDirectories)
            .SelectMany(f => pattern.Matches(File.ReadAllText(f))
                .Select(m => $"{Path.GetFileName(f)}:{m.Groups[2].Value}"))
            .Where(v => v.Contains("ARCH-", StringComparison.Ordinal)
                || v.Contains("D6", StringComparison.Ordinal))
            .ToList();

        Assert.True(leaks.Count == 0, string.Join("\n", leaks));
    }
}
