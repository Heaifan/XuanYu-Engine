// 旧 UI 债务基线（字号/圆角/高度/阴影/笔画，ARCH-UI-SPEC-R1-D2 自动生成）。
// 基线=审计矩阵 W01~W71 的自动化子集快照；D3~D6 整改删除硬编码时同步删除对应条目。
namespace XuanYu.World.Tests.UiTokens;

internal static partial class UiDebtBaseline
{
    private static void AddTypography(System.Collections.Generic.List<BaselineEntry> list)
    {
        list.Add(new("W57", "XuanYu.Editor.UI/Foot/Foot.axaml", UiRuleKind.CornerRadius, "CornerRadius", "2"));
        list.Add(new("W57", "XuanYu.Editor.UI/Foot/Foot.axaml", UiRuleKind.CornerRadius, "CornerRadius", "4", 2));
        list.Add(new("W61", "XuanYu.Editor.UI/Foot/LogDetailPanel.axaml", UiRuleKind.CornerRadius, "CornerRadius", "5"));
        list.Add(new("W60", "XuanYu.Editor.UI/Foot/LogDetailPanel.axaml", UiRuleKind.ControlHeight, "Height", "42"));
        list.Add(new("W29", "XuanYu.Editor.UI/Left/Left.Styles.axaml", UiRuleKind.FontSize, "FontSize", "15"));
        list.Add(new("W31", "XuanYu.Editor.UI/Left/Left.Styles.axaml", UiRuleKind.CornerRadius, "CornerRadius", "5", 2));
        list.Add(new("W32", "XuanYu.Editor.UI/Left/Left.Styles.axaml", UiRuleKind.StrokeThickness, "StrokeThickness", "2.2"));
        list.Add(new("W51", "XuanYu.Editor.UI/Right/LayerPanel.axaml", UiRuleKind.CornerRadius, "CornerRadius", "4", 2));
        list.Add(new("W51", "XuanYu.Editor.UI/Right/LayerPanel.axaml", UiRuleKind.CornerRadius, "CornerRadius", "1.5"));
        list.Add(new("W51", "XuanYu.Editor.UI/Right/LayerPanel.axaml", UiRuleKind.CornerRadius, "CornerRadius", "1"));
        list.Add(new("W50", "XuanYu.Editor.UI/Right/LayerPanel.axaml", UiRuleKind.ControlHeight, "Height", "25"));
        list.Add(new("W46", "XuanYu.Editor.UI/Right/MapEditorPanel.axaml", UiRuleKind.CornerRadius, "CornerRadius", "5"));
        list.Add(new("W37", "XuanYu.Editor.UI/Right/Right.axaml", UiRuleKind.FontSize, "FontSize", "15"));
        list.Add(new("W40", "XuanYu.Editor.UI/Right/Right.axaml", UiRuleKind.StrokeThickness, "StrokeThickness", "1.6"));
        list.Add(new("W22", "XuanYu.Editor.UI/Top/Top.axaml", UiRuleKind.CornerRadius, "CornerRadius", "9"));
        list.Add(new("W22", "XuanYu.Editor.UI/Top/Top.axaml", UiRuleKind.CornerRadius, "CornerRadius", "7"));
        list.Add(new("W22", "XuanYu.Editor.UI/Top/Top.axaml", UiRuleKind.CornerRadius, "CornerRadius", "4", 2));
        list.Add(new("W27", "XuanYu.Editor.UI/Top/Top.axaml", UiRuleKind.StrokeThickness, "StrokeThickness", "1.6"));
        list.Add(new("W10", "XuanYu.Editor.UI/Ui.axaml", UiRuleKind.CornerRadius, "CornerRadius", "5", 2));
        list.Add(new("W04", "XuanYu.Editor.UI/Ui.axaml", UiRuleKind.BoxShadow, "BoxShadow", "0 14 30 0 #160f172a"));
        list.Add(new("W09", "XuanYu.Editor.UI/Ui.axaml", UiRuleKind.ControlHeight, "Height", "30"));
        list.Add(new("W12", "XuanYu.Editor.UI/Ui.axaml", UiRuleKind.ControlHeight, "Height", "34"));
    }
}
