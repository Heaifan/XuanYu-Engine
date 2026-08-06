// 旧 UI 债务基线（code-behind 色值，D2-F2 重生成）。
// 基线=审计矩阵 W01~W71 的自动化子集快照（D2-F2 重生成：父链定位 v3 + 真实属性名 + cs 八类规则，Unknown=0）。
// ALLOW-* = 正式允许清单（渲染/宿主/领域色，按路径+规则+API 模式+原因登记）。
namespace XuanYu.World.Tests.UiTokens;

internal static partial class UiDebtBaseline
{
    private static void AddCs(System.Collections.Generic.List<BaselineEntry> list)
    {
        list.Add(new("W71-ALLOW", "XuanYu.Editor.UI/TreeGuide.cs", "TreeGuide.Render", UiRuleKind.CsHexColor, "Hex", "#C7D7EA"));
        list.Add(new("ALLOW-RENDER", "XuanYu.Editor.UI/TreeGuide.cs", "TreeGuide.Render", UiRuleKind.CsHexColor, "ColorAPI", "Color.Parse("));
        list.Add(new("ALLOW-RENDER", "XuanYu.Editor.UI/TreeGuide.cs", "TreeGuide.Render", UiRuleKind.CsHexColor, "Brush", "new SolidColorBrush("));
        list.Add(new("ALLOW-WIN32", "XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs", "Win32ViewportHost.WS_CHILD", UiRuleKind.CsHexColor, "Uint", "0x40000000"));
        list.Add(new("ALLOW-WIN32", "XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs", "Win32ViewportHost.WS_CHILD", UiRuleKind.CsHexColor, "Uint", "0x10000000"));
        list.Add(new("W70", "XuanYu.Editor.UI/Vm/Logging/LogEntry.cs", "LogEntry.Accent", UiRuleKind.CsHexColor, "Hex", "#c75b5b"));
        list.Add(new("W70", "XuanYu.Editor.UI/Vm/Logging/LogEntry.cs", "LogEntry.Accent", UiRuleKind.CsHexColor, "Hex", "#d89b32"));
        list.Add(new("W70", "XuanYu.Editor.UI/Vm/Logging/LogEntry.cs", "LogEntry.Accent", UiRuleKind.CsHexColor, "Hex", "#4f7fb8", 2));
        list.Add(new("W70", "XuanYu.Editor.UI/Vm/Logging/LogEntry.cs", "LogEntry.Accent", UiRuleKind.CsHexColor, "Hex", "#6b7a90"));
        list.Add(new("W70", "XuanYu.Editor.UI/Vm/Logging/LogEntry.cs", "LogEntry.Accent", UiRuleKind.CsHexColor, "Hex", "#8b96a8"));
        list.Add(new("W71", "XuanYu.Editor.UI/Vm/Scene/UiVm.DocumentStatus.cs", "UiVm.DocumentStatusBackground", UiRuleKind.CsHexColor, "Hex", "#fff7df"));
        list.Add(new("W71", "XuanYu.Editor.UI/Vm/Scene/UiVm.DocumentStatus.cs", "UiVm.DocumentStatusBackground", UiRuleKind.CsHexColor, "Hex", "#fdeeee"));
        list.Add(new("W71", "XuanYu.Editor.UI/Vm/Scene/UiVm.DocumentStatus.cs", "UiVm.DocumentStatusBackground", UiRuleKind.CsHexColor, "Hex", "#eef7f1", 2));
        list.Add(new("W71", "XuanYu.Editor.UI/Vm/Scene/UiVm.DocumentStatus.cs", "UiVm.DocumentStatusBorderBrush", UiRuleKind.CsHexColor, "Hex", "#e7c66d"));
        list.Add(new("W71", "XuanYu.Editor.UI/Vm/Scene/UiVm.DocumentStatus.cs", "UiVm.DocumentStatusBorderBrush", UiRuleKind.CsHexColor, "Hex", "#e2aaaa"));
        list.Add(new("W71", "XuanYu.Editor.UI/Vm/Scene/UiVm.DocumentStatus.cs", "UiVm.DocumentStatusBorderBrush", UiRuleKind.CsHexColor, "Hex", "#c9e3d0"));
        list.Add(new("W71", "XuanYu.Editor.UI/Vm/Scene/UiVm.DocumentStatus.cs", "UiVm.DocumentStatusForeground", UiRuleKind.CsHexColor, "Hex", "#8a6417"));
        list.Add(new("W71", "XuanYu.Editor.UI/Vm/Scene/UiVm.DocumentStatus.cs", "UiVm.DocumentStatusForeground", UiRuleKind.CsHexColor, "Hex", "#a43f3f"));
        list.Add(new("W71", "XuanYu.Editor.UI/Vm/Scene/UiVm.DocumentStatus.cs", "UiVm.DocumentStatusForeground", UiRuleKind.CsHexColor, "Hex", "#1f7a4d"));
        list.Add(new("W71", "XuanYu.Editor.UI/Vm/Scene/UiVm.DocumentStatus.cs", "UiVm.SaveButtonBackground", UiRuleKind.CsHexColor, "Hex", "#fff6dd"));
        list.Add(new("W71", "XuanYu.Editor.UI/Vm/Scene/UiVm.DocumentStatus.cs", "UiVm.SaveButtonBorderBrush", UiRuleKind.CsHexColor, "Hex", "#d9ad43"));
    }
}
