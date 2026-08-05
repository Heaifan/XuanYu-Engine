// 旧 UI 债务基线（字号/圆角/高度/阴影/笔画，D2-F1 重生成）。
// 基线=审计矩阵 W01~W71 的自动化子集快照（D2-F1 重生成，含稳定定位 Locator，注释已剥离）。
namespace XuanYu.World.Tests.UiTokens;

internal static partial class UiDebtBaseline
{
    private static void AddTypography(System.Collections.Generic.List<BaselineEntry> list)
    {
        list.Add(new("W57", "XuanYu.Editor.UI/Foot/Foot.axaml", "Style:Button.logSummary", UiRuleKind.CornerRadius, "CornerRadius", "4"));
        list.Add(new("W57", "XuanYu.Editor.UI/Foot/Foot.axaml", "Style:Button.logFilter", UiRuleKind.CornerRadius, "CornerRadius", "4"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Foot/Foot.axaml", "Style:TextBlock.logHead", UiRuleKind.FontSize, "FontSize", "12"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Foot/Foot.axaml", "Style:ListBox.logList ListBoxItem", UiRuleKind.ControlHeight, "MinHeight", "28"));
        list.Add(new("W57", "XuanYu.Editor.UI/Foot/Foot.axaml", "Elm:Border", UiRuleKind.CornerRadius, "CornerRadius", "6"));
        list.Add(new("W57", "XuanYu.Editor.UI/Foot/Foot.axaml", "Elm:Border", UiRuleKind.CornerRadius, "CornerRadius", "2"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Foot/Foot.axaml", "Elm:TextBlock", UiRuleKind.FontSize, "FontSize", "16"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Foot/LogDetailPanel.axaml", "Style:TextBlock.detailLabel", UiRuleKind.FontSize, "FontSize", "12"));
        list.Add(new("W60", "XuanYu.Editor.UI/Foot/LogDetailPanel.axaml", "Style:TextBox.detailBody", UiRuleKind.ControlHeight, "MinHeight", "42"));
        list.Add(new("W61", "XuanYu.Editor.UI/Foot/LogDetailPanel.axaml", "Elm:Border", UiRuleKind.CornerRadius, "CornerRadius", "5"));
        list.Add(new("W29", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:TabItem.leftTab", UiRuleKind.FontSize, "FontSize", "15"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:TabItem.leftTab", UiRuleKind.ControlHeight, "MinHeight", "28"));
        list.Add(new("W31", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:TabItem.leftTab", UiRuleKind.CornerRadius, "CornerRadius", "5"));
        list.Add(new("W31", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:Border.searchBox", UiRuleKind.CornerRadius, "CornerRadius", "6"));
        list.Add(new("W31", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:Border.treeRow", UiRuleKind.CornerRadius, "CornerRadius", "5"));
        list.Add(new("W32", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:Path.treeIcon", UiRuleKind.StrokeThickness, "StrokeThickness", "2.2"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:ListBox.treeList ListBoxItem", UiRuleKind.ControlHeight, "MinHeight", "28"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:MenuItem", UiRuleKind.ControlHeight, "MinHeight", "32"));
        list.Add(new("W29", "XuanYu.Editor.UI/Left/Left.Styles.axaml", "Style:TextBlock.treeText", UiRuleKind.FontSize, "FontSize", "13"));
        list.Add(new("W51", "XuanYu.Editor.UI/Right/LayerInspectorPanel.axaml", "Style:Border.infoPanel", UiRuleKind.CornerRadius, "CornerRadius", "6"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Right/LayerInspectorPanel.axaml", "Style:TextBlock.key", UiRuleKind.FontSize, "FontSize", "12"));
        list.Add(new("W50", "XuanYu.Editor.UI/Right/LayerPanel.axaml", "Style:Button.layerTool", UiRuleKind.ControlHeight, "MinHeight", "25"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Right/LayerPanel.axaml", "Style:Button.layerTool", UiRuleKind.FontSize, "FontSize", "12"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Right/LayerPanel.axaml", "Style:ToggleButton.layerSwitch", UiRuleKind.ControlHeight, "Height", "24"));
        list.Add(new("W51", "XuanYu.Editor.UI/Right/LayerPanel.axaml", "Style:ToggleButton.layerSwitch", UiRuleKind.CornerRadius, "CornerRadius", "4"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Right/LayerPanel.axaml", "Style:ToggleButton.layerLockSwitch", UiRuleKind.ControlHeight, "Height", "24"));
        list.Add(new("W51", "XuanYu.Editor.UI/Right/LayerPanel.axaml", "Style:ToggleButton.layerLockSwitch", UiRuleKind.CornerRadius, "CornerRadius", "4"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Right/LayerPanel.axaml", "Style:Path.layerIcon", UiRuleKind.StrokeThickness, "StrokeThickness", "1.5"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Right/LayerPanel.axaml", "Style:TextBlock.layerName", UiRuleKind.FontSize, "FontSize", "13"));
        list.Add(new("W51", "XuanYu.Editor.UI/Right/LayerPanel.axaml", "Style:Border.kindTagRegion", UiRuleKind.CornerRadius, "CornerRadius", "3"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Right/LayerPanel.axaml", "Style:TextBlock.kindTagRegionText", UiRuleKind.FontSize, "FontSize", "10"));
        list.Add(new("W51", "XuanYu.Editor.UI/Right/LayerPanel.axaml", "Style:Border.kindTagSystem", UiRuleKind.CornerRadius, "CornerRadius", "3"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Right/LayerPanel.axaml", "Style:TextBlock.kindTagSystemText", UiRuleKind.FontSize, "FontSize", "10"));
        list.Add(new("W51", "XuanYu.Editor.UI/Right/LayerPanel.axaml", "Style:Border.activeMark", UiRuleKind.CornerRadius, "CornerRadius", "1.5"));
        list.Add(new("W51", "XuanYu.Editor.UI/Right/LayerPanel.axaml", "Style:Border.dropLine", UiRuleKind.CornerRadius, "CornerRadius", "1"));
        list.Add(new("W46", "XuanYu.Editor.UI/Right/MapEditorPanel.axaml", "Style:Border.infoPanel", UiRuleKind.CornerRadius, "CornerRadius", "6"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Right/MapEditorPanel.axaml", "Style:TextBlock.key", UiRuleKind.FontSize, "FontSize", "12"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Right/MapEditorPanel.axaml", "Style:TextBlock.value", UiRuleKind.FontSize, "FontSize", "13"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Right/MapEditorPanel.axaml", "Style:Button", UiRuleKind.FontSize, "FontSize", "12"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Right/MapEditorPanel.axaml", "Style:TabItem.layerSubTab", UiRuleKind.FontSize, "FontSize", "14"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Right/MapEditorPanel.axaml", "Style:TabItem.layerSubTab", UiRuleKind.ControlHeight, "MinHeight", "32"));
        list.Add(new("W46", "XuanYu.Editor.UI/Right/MapEditorPanel.axaml", "Style:TabItem.layerSubTab", UiRuleKind.CornerRadius, "CornerRadius", "5"));
        list.Add(new("W51", "XuanYu.Editor.UI/Right/Right.axaml", "Style:Border.infoPanel", UiRuleKind.CornerRadius, "CornerRadius", "6"));
        list.Add(new("W40", "XuanYu.Editor.UI/Right/Right.axaml", "Style:Path.panelIcon", UiRuleKind.StrokeThickness, "StrokeThickness", "1.6"));
        list.Add(new("W37", "XuanYu.Editor.UI/Right/Right.axaml", "Style:TextBlock.panelTitle", UiRuleKind.FontSize, "FontSize", "15"));
        list.Add(new("W37", "XuanYu.Editor.UI/Right/Right.axaml", "Style:TextBlock.key", UiRuleKind.FontSize, "FontSize", "12"));
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
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Ui.axaml", "Style:TextBlock.section", UiRuleKind.FontSize, "FontSize", "12"));
        list.Add(new("W71-GEN", "XuanYu.Editor.UI/Ui.axaml", "Style:TextBlock.caption", UiRuleKind.FontSize, "FontSize", "12"));
    }
}
