using System.IO;

namespace XuanYu.World.Tests.UiTokens;

// ARCH-UI-SPEC-R1-D4-F1（纠偏 v2）：按钮统一文本合同真实接线与正式布局——
// uiTextButton 必须被真实页面引用（地图 7 + 调试 4），且布局符合网格合同。
public sealed class UiD4F1ButtonContractTests
{
    static readonly string Ui = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Design", "UiStyles.D4F1.axaml"));

    static readonly string MapPage = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "MapPagePanel.axaml"));

    static readonly string MapForm = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "MapFormPanel.axaml"));

    static readonly string Right = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "EditorRightTabs.axaml"));

    [Fact]
    public void Unified_button_style_provides_text_contract()
    {
        // 统一文本合同：NoWrap + CharacterEllipsis + MaxLines=1 + Tooltip 完整名称 + 字体 12
        Assert.Contains("Button.uiTextButton", Ui);
        Assert.Contains("TextWrapping\" Value=\"NoWrap\"", Ui);
        Assert.Contains("TextTrimming\" Value=\"CharacterEllipsis\"", Ui);
        Assert.Contains("MaxLines\" Value=\"1\"", Ui);
        Assert.Contains("ContentTemplate", Ui);   // 模板化实现（非逐按钮手写）
        Assert.Contains("ToolTip.Tip", Ui);       // Tooltip = 完整按钮名称（绑定 Content）
        Assert.Contains("Font.Label.Size", Ui);   // 按钮文字 12
        Assert.Contains("MinHeight\" Value=\"28\"", Ui); // 统一高度 28
        Assert.Contains("MinWidth\" Value=\"0\"", Ui);   // 覆盖全局 Button 52
    }

    [Fact]
    public void All_map_buttons_reference_unified_button_contract()
    {
        // 纠偏 v2：不得只定义不接线——4 个资产按钮在 MapPagePanel、3 个属性按钮在 MapFormPanel
        foreach (var label in new[] { "新建地图", "打开地图", "保存地图", "聚焦地图" })
        {
            Assert.Contains($"Content=\"{label}\"", MapPage);
            Assert.Contains("Classes=\"uiTextButton\"", MapPage);
        }
        foreach (var label in new[] { "应用修改", "撤销地图修改", "重做地图修改" })
        {
            Assert.Contains($"Content=\"{label}\"", MapForm);
            Assert.Contains("Classes=\"uiTextButton\"", MapForm);
        }
        Assert.Equal(4, CountOccurrences(MapPage, "Classes=\"uiTextButton\""));
        Assert.Equal(3, CountOccurrences(MapForm, "Classes=\"uiTextButton\""));
    }

    [Fact]
    public void Debug_buttons_reference_unified_button_contract()
    {
        foreach (var label in new[] { "开始", "预览", "提交", "取消" })
        {
            Assert.Contains($"Content=\"{label}\"", Right);
            Assert.Contains("Classes=\"uiTextButton\"", Right);
        }
        Assert.Equal(4, CountOccurrences(Right, "Classes=\"uiTextButton\""));
    }

    [Fact]
    public void Map_property_buttons_use_two_row_grid_with_apply_spanning()
    {
        // 纠偏 v2 正式布局：禁止三按钮横向 StackPanel；应用修改跨两列第一行；撤销/重做第二行等宽
        Assert.Contains("ColumnDefinitions=\"*,*\"", MapForm);
        Assert.Contains("Grid.ColumnSpan=\"2\"", MapForm);
        Assert.Contains("Grid.Row=\"1\"", MapForm);
        Assert.DoesNotContain("<StackPanel Orientation=\"Horizontal\" Spacing=\"6\" Margin=\"0,6,0,0\">", MapForm);
    }

    [Fact]
    public void Map_asset_buttons_use_stretched_2x2_grid()
    {
        // 纠偏 v2：真正的 2×2 等宽网格，按钮水平拉伸且 MinWidth=0（覆盖全局 52）
        Assert.Contains("<UniformGrid Columns=\"2\" ColumnSpacing=\"6\" RowSpacing=\"6\">", MapPage);
        Assert.Equal(4, CountOccurrences(MapPage, "HorizontalAlignment=\"Stretch\""));
        Assert.Equal(4, CountOccurrences(MapPage, "MinWidth=\"0\""));
        Assert.Equal(4, CountOccurrences(MapPage, "MinHeight=\"28\""));
    }

    static int CountOccurrences(string text, string needle) =>
        System.Text.RegularExpressions.Regex.Matches(text, System.Text.RegularExpressions.Regex.Escape(needle)).Count;
}
