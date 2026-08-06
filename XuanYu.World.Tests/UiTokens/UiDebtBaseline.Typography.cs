// 旧 UI 债务基线（字号/圆角/高度/阴影/笔画，D2-F2 重生成）。
// 基线=审计矩阵 W01~W71 的自动化子集快照（D2-F2 重生成：父链定位 v3 + 真实属性名 + cs 八类规则，Unknown=0）。
// ALLOW-* = 正式允许清单（渲染/宿主/领域色，按路径+规则+API 模式+原因登记）。
namespace XuanYu.World.Tests.UiTokens;

internal static partial class UiDebtBaseline
{
    private static void AddTypography(System.Collections.Generic.List<BaselineEntry> list)
    {
        list.Add(new("W57", "XuanYu.Editor.UI/Foot/Foot.axaml", "Style:Button.logSummary", UiRuleKind.CornerRadius, "CornerRadius", "4"));
        list.Add(new("W57", "XuanYu.Editor.UI/Foot/Foot.axaml", "Style:Button.logFilter", UiRuleKind.CornerRadius, "CornerRadius", "4"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Foot/Foot.axaml", "Style:TextBlock.logHead", UiRuleKind.FontSize, "FontSize", "12"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Foot/Foot.axaml", "Style:ListBox.logList ListBoxItem", UiRuleKind.ControlHeight, "MinHeight", "28"));
        list.Add(new("W57", "XuanYu.Editor.UI/Foot/Foot.axaml", "Path:ROOT/UserControl/Grid/Border:1", UiRuleKind.CornerRadius, "CornerRadius", "6"));
        list.Add(new("W57", "XuanYu.Editor.UI/Foot/Foot.axaml", "Path:Name:LogList/ListBox/DataTemplate/Grid/Border:1", UiRuleKind.CornerRadius, "CornerRadius", "2"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Foot/Foot.axaml", "Path:ROOT/UserControl/Grid/Border/Grid/Grid/Grid/StackPanel/TextBlock:1", UiRuleKind.FontSize, "FontSize", "16"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Foot/LogDetailPanel.axaml", "Style:TextBlock.detailLabel", UiRuleKind.FontSize, "FontSize", "12"));
        list.Add(new("W60", "XuanYu.Editor.UI/Foot/LogDetailPanel.axaml", "Style:TextBox.detailBody", UiRuleKind.ControlHeight, "MinHeight", "42"));
        list.Add(new("W61", "XuanYu.Editor.UI/Foot/LogDetailPanel.axaml", "Path:ROOT/UserControl/Border/Grid/Grid/ScrollViewer/StackPanel/Border:1", UiRuleKind.CornerRadius, "CornerRadius", "5"));
        list.Add(new("W29", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:TabItem.leftTab", UiRuleKind.FontSize, "FontSize", "15"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:TabItem.leftTab", UiRuleKind.ControlHeight, "MinHeight", "28"));
        list.Add(new("W31", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:TabItem.leftTab", UiRuleKind.CornerRadius, "CornerRadius", "5"));
        list.Add(new("W31", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:Border.searchBox", UiRuleKind.CornerRadius, "CornerRadius", "6"));
        list.Add(new("W31", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:Border.treeRow", UiRuleKind.CornerRadius, "CornerRadius", "5"));
        list.Add(new("W32", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:Path.treeIcon", UiRuleKind.StrokeThickness, "StrokeThickness", "2.2"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:ListBox.treeList ListBoxItem", UiRuleKind.ControlHeight, "MinHeight", "28"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:MenuItem", UiRuleKind.ControlHeight, "MinHeight", "32"));
        list.Add(new("W29", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:TextBlock.treeText", UiRuleKind.FontSize, "FontSize", "13"));
        list.Add(new("W51", "XuanYu.Editor.UI/Right/LayerPanel.axaml", "Style:Border.activeMark", UiRuleKind.CornerRadius, "CornerRadius", "1.5"));
        list.Add(new("W51", "XuanYu.Editor.UI/Right/LayerPanel.axaml", "Style:Border.dropLine", UiRuleKind.CornerRadius, "CornerRadius", "1"));
        list.Add(new("W22", "XuanYu.Editor.UI/Top/Top.axaml", "Style:Border.commandRail", UiRuleKind.CornerRadius, "CornerRadius", "9"));
        list.Add(new("W22", "XuanYu.Editor.UI/Top/Top.axaml", "Style:Border.topGroup", UiRuleKind.CornerRadius, "CornerRadius", "0"));
        list.Add(new("W22", "XuanYu.Editor.UI/Top/Top.axaml", "Style:Border.statePill", UiRuleKind.CornerRadius, "CornerRadius", "7"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Top/Top.axaml", "Style:MenuItem", UiRuleKind.ControlHeight, "MinHeight", "32"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Top/Top.axaml", "Style:Button.cmdBtn", UiRuleKind.ControlHeight, "MinHeight", "32"));
        list.Add(new("W22", "XuanYu.Editor.UI/Top/Top.axaml", "Style:Button.cmdBtn", UiRuleKind.CornerRadius, "CornerRadius", "4"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Top/Top.axaml", "Style:ToggleButton.toolBtn", UiRuleKind.ControlHeight, "MinHeight", "32"));
        list.Add(new("W22", "XuanYu.Editor.UI/Top/Top.axaml", "Style:ToggleButton.toolBtn", UiRuleKind.CornerRadius, "CornerRadius", "4"));
        list.Add(new("W27", "XuanYu.Editor.UI/Top/Top.axaml", "Style:Path.topIcon", UiRuleKind.StrokeThickness, "StrokeThickness", "1.6"));
        list.Add(new("W10", "XuanYu.Editor.UI/Ui.axaml", "Style:Border.panel", UiRuleKind.CornerRadius, "CornerRadius", "6"));
        list.Add(new("W04", "XuanYu.Editor.UI/Ui.axaml", "Style:Border.panel", UiRuleKind.BoxShadow, "BoxShadow", "0 14 30 0 #160f172a"));
        list.Add(new("W10", "XuanYu.Editor.UI/Ui.axaml", "Style:Border.chrome", UiRuleKind.CornerRadius, "CornerRadius", "6"));
        list.Add(new("W10", "XuanYu.Editor.UI/Ui.axaml", "Style:Border.pill", UiRuleKind.CornerRadius, "CornerRadius", "5"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Ui.axaml", "Style:TabItem.sideTab", UiRuleKind.FontSize, "FontSize", "13"));
        list.Add(new("W09", "XuanYu.Editor.UI/Ui.axaml", "Style:TabItem.sideTab", UiRuleKind.ControlHeight, "MinHeight", "30"));
        list.Add(new("W10", "XuanYu.Editor.UI/Ui.axaml", "Style:TabItem.sideTab", UiRuleKind.CornerRadius, "CornerRadius", "5"));
        list.Add(new("W12", "XuanYu.Editor.UI/Ui.axaml", "Style:ListBoxItem", UiRuleKind.ControlHeight, "MinHeight", "34"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Ui.axaml", "Style:TextBlock.caption", UiRuleKind.FontSize, "FontSize", "12"));
    }
}
