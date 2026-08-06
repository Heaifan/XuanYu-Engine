using System.IO;
using System.Linq;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D4：D4 债务清零与行数门禁（基线只减不增；组件例外保留登记）。
public sealed class UiD4DebtClearedTests
{
    static readonly string RepoRoot = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..");

    [Fact]
    public void D4_files_have_no_debt_entries_except_registered_exceptions()
    {
        var d4Files = new[]
        {
            "XuanYu.Editor.UI/Right/Right.axaml",
            "XuanYu.Editor.UI/Right/MapEditorPanel.axaml",
            "XuanYu.Editor.UI/Right/LayerPanel.axaml",
            "XuanYu.Editor.UI/Right/LayerInspectorPanel.axaml",
        };
        var remaining = UiDebtBaseline.Entries
            .Where(e => d4Files.Contains(e.Path)).ToList();
        // 仅保留登记组件例外：activeMark 圆角 1.5、dropLine 圆角 1（规范 §5.4）
        Assert.Equal(2, remaining.Count);
        Assert.All(remaining, e =>
        {
            Assert.True((e.Locator == "Style:Border.activeMark" && e.Value == "1.5")
                || (e.Locator == "Style:Border.dropLine" && e.Value == "1"));
        });
    }

    [Fact]
    public void Baseline_total_shrinks_with_d5_migrations()
    {
        // D4 末 159 条 → D5 清除 16 条（Ui.axaml Button 状态色×5 + UnsavedDialog 代码 Window 颜色×11）
        Assert.Equal(143, UiDebtBaseline.Entries.Count);
    }

    [Fact]
    public void New_d4_files_stay_within_100_line_rule()
    {
        foreach (var rel in new[]
        {
            "XuanYu.Editor.UI/Right/InspectorPanel.axaml",
            "XuanYu.Editor.UI/Right/InspectorPanel.axaml.cs",
            "XuanYu.Editor.UI/Right/MapPagePanel.axaml",
            "XuanYu.Editor.UI/Right/MapPagePanel.axaml.cs",
            "XuanYu.Editor.UI/Right/MapFormPanel.axaml",
            "XuanYu.Editor.UI/Right/MapFormPanel.axaml.cs",
            "XuanYu.Editor.UI/Right/EditableFormLayoutModel.cs",
            "XuanYu.Editor.UI/Right/MapEditorLayoutModel.cs",
            "XuanYu.Editor.UI/Right/MapIdDisplayFormat.cs",
            "XuanYu.Editor.UI/Design/UiStyles.D4F1.axaml",
            "XuanYu.Editor.UI/Vm/Inspector/InspectorFieldRow.cs",
            "XuanYu.Editor.UI/Vm/Map/UiVm.MapEditor.Display.cs",
        })
            Assert.True(File.ReadAllLines(Path.Combine(RepoRoot, rel)).Length <= 100, $"{rel} 超 100 行");
    }
}
