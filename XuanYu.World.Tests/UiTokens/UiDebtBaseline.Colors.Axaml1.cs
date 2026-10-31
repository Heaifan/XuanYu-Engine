// 旧 UI 债务基线（AXAML 色值 1/2，D2-F2 重生成）。
// 基线=审计矩阵 W01~W71 的自动化子集快照（D2-F2 重生成：父链定位 v3 + 真实属性名 + cs 八类规则，Unknown=0）。
// ALLOW-* = 正式允许清单（渲染/宿主/领域色，按路径+规则+API 模式+原因登记）。
namespace XuanYu.World.Tests.UiTokens;

internal static partial class UiDebtBaseline
{
    private static void AddAxaml1(System.Collections.Generic.List<BaselineEntry> list)
    {
        list.Add(new("W62", "XuanYu.Editor.UI/Panels/Status/LogDetails/LogDetailPanel.axaml", "Style:TextBlock.detailLabel", UiRuleKind.HexColor, "Foreground", "#64748b"));
        list.Add(new("W62", "XuanYu.Editor.UI/Panels/Status/LogDetails/LogDetailPanel.axaml", "Style:TextBlock.detailValue", UiRuleKind.HexColor, "Foreground", "#243246"));
        list.Add(new("W62", "XuanYu.Editor.UI/Panels/Status/LogDetails/LogDetailPanel.axaml", "Style:TextBlock.detailTitle", UiRuleKind.HexColor, "Foreground", "#172337"));
        list.Add(new("W62", "XuanYu.Editor.UI/Panels/Status/LogDetails/LogDetailPanel.axaml", "Style:TextBox.detailBody", UiRuleKind.HexColor, "Background", "#f2f6fb"));
        list.Add(new("W62", "XuanYu.Editor.UI/Panels/Status/LogDetails/LogDetailPanel.axaml", "Path:ROOT/UserControl/Border:1", UiRuleKind.HexColor, "BorderBrush", "#d9e2ee"));
        list.Add(new("W62", "XuanYu.Editor.UI/Panels/Status/LogDetails/LogDetailPanel.axaml", "Path:ROOT/UserControl/Border/Grid/Grid/TextBlock:1", UiRuleKind.HexColor, "Foreground", "#243246"));
        list.Add(new("W62", "XuanYu.Editor.UI/Panels/Status/LogDetails/LogDetailPanel.axaml", "Path:ROOT/UserControl/Border/Grid/Grid/ScrollViewer/StackPanel/Border:1", UiRuleKind.HexColor, "Background", "#edf4ff"));
    }
}
