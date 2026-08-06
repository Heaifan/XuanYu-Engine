// 旧 UI 债务基线（AXAML 色值 1/2，D2-F2 重生成）。
// 基线=审计矩阵 W01~W71 的自动化子集快照（D2-F2 重生成：父链定位 v3 + 真实属性名 + cs 八类规则，Unknown=0）。
// ALLOW-* = 正式允许清单（渲染/宿主/领域色，按路径+规则+API 模式+原因登记）。
namespace XuanYu.World.Tests.UiTokens;

internal static partial class UiDebtBaseline
{
    private static void AddAxaml1(System.Collections.Generic.List<BaselineEntry> list)
    {
        list.Add(new("W62", "XuanYu.Editor.UI/Foot/LogDetailPanel.axaml", "Style:TextBlock.detailLabel", UiRuleKind.HexColor, "Foreground", "#64748b"));
        list.Add(new("W62", "XuanYu.Editor.UI/Foot/LogDetailPanel.axaml", "Style:TextBlock.detailValue", UiRuleKind.HexColor, "Foreground", "#243246"));
        list.Add(new("W62", "XuanYu.Editor.UI/Foot/LogDetailPanel.axaml", "Style:TextBlock.detailTitle", UiRuleKind.HexColor, "Foreground", "#172337"));
        list.Add(new("W62", "XuanYu.Editor.UI/Foot/LogDetailPanel.axaml", "Style:TextBox.detailBody", UiRuleKind.HexColor, "Background", "#f2f6fb"));
        list.Add(new("W62", "XuanYu.Editor.UI/Foot/LogDetailPanel.axaml", "Path:ROOT/UserControl/Border:1", UiRuleKind.HexColor, "BorderBrush", "#d9e2ee"));
        list.Add(new("W62", "XuanYu.Editor.UI/Foot/LogDetailPanel.axaml", "Path:ROOT/UserControl/Border/Grid/Grid/TextBlock:1", UiRuleKind.HexColor, "Foreground", "#243246"));
        list.Add(new("W62", "XuanYu.Editor.UI/Foot/LogDetailPanel.axaml", "Path:ROOT/UserControl/Border/Grid/Grid/ScrollViewer/StackPanel/Border:1", UiRuleKind.HexColor, "Background", "#edf4ff"));
        list.Add(new("W30", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:TabItem.leftTab", UiRuleKind.HexColor, "Foreground", "#6b7688"));
        list.Add(new("W30", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:TabItem.leftTab:selected", UiRuleKind.HexColor, "Background", "#edf4ff"));
        list.Add(new("W30", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:TabItem.leftTab:selected", UiRuleKind.HexColor, "Foreground", "#185aa6"));
        list.Add(new("W30", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:TabItem.leftTab:selected", UiRuleKind.HexColor, "BorderBrush", "#8cb2e2"));
        list.Add(new("W30", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:Border.searchBox", UiRuleKind.HexColor, "Background", "#f5f8fc"));
        list.Add(new("W30", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:Border.searchBox", UiRuleKind.HexColor, "BorderBrush", "#d7e1ee"));
        list.Add(new("W30", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:Border.treeRow:pointerover", UiRuleKind.HexColor, "Background", "#f1f7ff"));
        list.Add(new("W34", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:Border.treeRow.selected", UiRuleKind.HexColor, "Background", "#e7f1ff"));
        list.Add(new("W33", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:Path.treeIcon", UiRuleKind.HexColor, "Stroke", "#2F80C9"));
        list.Add(new("W30", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:MenuItem", UiRuleKind.HexColor, "Foreground", "#2f3d52"));
        list.Add(new("W30", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:MenuItem:pointerover", UiRuleKind.HexColor, "Background", "#edf3fb"));
        list.Add(new("W30", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:TextBlock.treeText", UiRuleKind.HexColor, "Foreground", "#27354a"));
        list.Add(new("W35", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:TextBlock.selectedText", UiRuleKind.HexColor, "Foreground", "#165ca8"));
        list.Add(new("W30", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:TextBlock.emptyTitle", UiRuleKind.HexColor, "Foreground", "#334155"));
        list.Add(new("W36", "XuanYu.Editor.UI/Left/Left.axaml", "Path:Name:HierarchyList/ListBox/ContextMenu/MenuItem:3", UiRuleKind.HexColor, "Foreground", "#9b2f2f"));
    }
}
