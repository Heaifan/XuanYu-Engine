// 旧 UI 债务基线（AXAML 色值 2/2，D2-F2 重生成）。
// 基线=审计矩阵 W01~W71 的自动化子集快照（D2-F2 重生成：父链定位 v3 + 真实属性名 + cs 八类规则，Unknown=0）。
// ALLOW-* = 正式允许清单（渲染/宿主/领域色，按路径+规则+API 模式+原因登记）。
namespace XuanYu.World.Tests.UiTokens;

internal static partial class UiDebtBaseline
{
    private static void AddAxaml2(System.Collections.Generic.List<BaselineEntry> list)
    {
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:Border.commandRail", UiRuleKind.HexColor, "Background", "#fbfdff"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:Border.commandRail", UiRuleKind.HexColor, "BorderBrush", "#d5dfec"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:Border.topGroup", UiRuleKind.HexColor, "BorderBrush", "#d0dae8"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:Border.statePill", UiRuleKind.HexColor, "Background", "#eef7f1"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:Border.statePill", UiRuleKind.HexColor, "BorderBrush", "#c9e3d0"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:MenuItem", UiRuleKind.HexColor, "Foreground", "#2f3d52"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:MenuItem:pointerover", UiRuleKind.HexColor, "Background", "#edf3fb"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:Button.cmdBtn:pointerover", UiRuleKind.HexColor, "Background", "#edf3fb"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:Button.cmdBtn:pressed", UiRuleKind.HexColor, "Background", "#dfeaf8"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:ToggleButton.toolBtn:pointerover", UiRuleKind.HexColor, "Background", "#edf3fb"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:ToggleButton.toolBtn:pressed", UiRuleKind.HexColor, "Background", "#dfeaf8"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:ToggleButton.toolBtn:checked", UiRuleKind.HexColor, "Background", "#eef5ff"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:ToggleButton.toolBtn:checked", UiRuleKind.HexColor, "BorderBrush", "#94b9e8"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:ToggleButton.toolBtn:checked", UiRuleKind.HexColor, "Foreground", "#185aa6"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:Path.topIcon", UiRuleKind.HexColor, "Stroke", "#5d6d83"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:Button.cmdBtn:pointerover Path.topIcon", UiRuleKind.HexColor, "Stroke", "#365d8d"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:ToggleButton.toolBtn:pointerover Path.topIcon", UiRuleKind.HexColor, "Stroke", "#365d8d"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Style:ToggleButton.toolBtn:checked Path.topIcon", UiRuleKind.HexColor, "Stroke", "#185aa6"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Path:ROOT/UserControl/Grid/Border/Grid/Border:2", UiRuleKind.HexColor, "Background", "#f3f6fb"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Path:ROOT/UserControl/Grid/Border/Grid/Border:2", UiRuleKind.HexColor, "BorderBrush", "#d9e2ee"));
        list.Add(new("W26", "XuanYu.Editor.UI/Top/Top.axaml", "Path:ROOT/UserControl/Grid/Border/Grid/Border/TextBlock:2", UiRuleKind.HexColor, "Foreground", "#3b4a60"));
        list.Add(new("W02", "XuanYu.Editor.UI/Ui.axaml", "Style:Window", UiRuleKind.HexColor, "Foreground", "#172033"));
        list.Add(new("W05", "XuanYu.Editor.UI/Ui.axaml", "Style:Border.panel", UiRuleKind.HexColor, "Background", "#fbfcff"));
        list.Add(new("W05", "XuanYu.Editor.UI/Ui.axaml", "Style:Border.panel", UiRuleKind.HexColor, "BorderBrush", "#d9e0ec"));
        list.Add(new("W02", "XuanYu.Editor.UI/Ui.axaml", "Style:Border.panel", UiRuleKind.HexColor, "BoxShadow", "#160f172a"));
        list.Add(new("W02", "XuanYu.Editor.UI/Ui.axaml", "Style:Border.chrome", UiRuleKind.HexColor, "Background", "#edf2f8"));
        list.Add(new("W02", "XuanYu.Editor.UI/Ui.axaml", "Style:Border.chrome", UiRuleKind.HexColor, "BorderBrush", "#d7dfeb"));
        list.Add(new("W02", "XuanYu.Editor.UI/Ui.axaml", "Style:Border.pill", UiRuleKind.HexColor, "Background", "#e8eef7"));
        list.Add(new("W08", "XuanYu.Editor.UI/Ui.axaml", "Style:Button", UiRuleKind.HexColor, "Background", "#eef3fa"));
        list.Add(new("W02", "XuanYu.Editor.UI/Ui.axaml", "Style:Button", UiRuleKind.HexColor, "BorderBrush", "#d4ddea"));
        list.Add(new("W08", "XuanYu.Editor.UI/Ui.axaml", "Style:Button", UiRuleKind.HexColor, "Foreground", "#26324a"));
        list.Add(new("W08", "XuanYu.Editor.UI/Ui.axaml", "Style:Button:pointerover", UiRuleKind.HexColor, "Background", "#e4edf8"));
        list.Add(new("W08", "XuanYu.Editor.UI/Ui.axaml", "Style:Button:pointerover", UiRuleKind.HexColor, "BorderBrush", "#9fb5d6"));
        list.Add(new("W02", "XuanYu.Editor.UI/Ui.axaml", "Style:TabItem.sideTab", UiRuleKind.HexColor, "Foreground", "#6d788a"));
        list.Add(new("W11", "XuanYu.Editor.UI/Ui.axaml", "Style:TabItem.sideTab:pointerover", UiRuleKind.HexColor, "Background", "#f2f6fb"));
        list.Add(new("W11", "XuanYu.Editor.UI/Ui.axaml", "Style:TabItem.sideTab:pointerover", UiRuleKind.HexColor, "Foreground", "#47627f"));
        list.Add(new("W11", "XuanYu.Editor.UI/Ui.axaml", "Style:TabItem.sideTab:selected", UiRuleKind.HexColor, "Foreground", "#185aa6"));
        list.Add(new("W11", "XuanYu.Editor.UI/Ui.axaml", "Style:TabItem.sideTab:selected", UiRuleKind.HexColor, "Background", "#edf4ff"));
        list.Add(new("W11", "XuanYu.Editor.UI/Ui.axaml", "Style:TabItem.sideTab:selected", UiRuleKind.HexColor, "BorderBrush", "#8cb2e2"));
        list.Add(new("W14", "XuanYu.Editor.UI/Ui.axaml", "Style:TextBlock.muted", UiRuleKind.HexColor, "Foreground", "#758197"));
        list.Add(new("W14", "XuanYu.Editor.UI/Ui.axaml", "Style:TextBlock.caption", UiRuleKind.HexColor, "Foreground", "#758197"));
        list.Add(new("W63", "XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml", "Path:ROOT/UserControl/Grid/Border:1", UiRuleKind.HexColor, "BorderBrush", "#C9D2DC"));
        list.Add(new("W63", "XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml", "Name:FallbackLayer", UiRuleKind.HexColor, "Background", "#E8EEF5"));
        list.Add(new("W63", "XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml", "Path:Name:FallbackLayer/StackPanel/TextBlock:1", UiRuleKind.HexColor, "Foreground", "#4A5A70"));
        list.Add(new("W63", "XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml", "Name:FallbackText", UiRuleKind.HexColor, "Foreground", "#6B7688"));
    }
}
