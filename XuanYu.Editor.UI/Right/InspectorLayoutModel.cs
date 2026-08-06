namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4（G03/补充裁决）：检查器表单模式——内容宽 ≥360 左右布局、<360 整组上下。
// 纯布局逻辑（确定性、无 Avalonia 依赖），宽度判定以内容区域为准，可脱离 GPU 测试。
public enum InspectorFormMode { Wide, Narrow }

public static class InspectorLayoutModel
{
    public const double WideThreshold = 360;   // 内容宽 <360 → 整组上下布局（UI Spec §7.1）
    public const double LabelColumnWidth = 96; // 与 Control.LabelColumn.Width 一致（§5.3）
    public const double FieldMinWidth = 128;   // 与 Control.Field.MinWidth 一致（§5.3）

    public static InspectorFormMode ModeFor(double contentWidth) =>
        contentWidth < WideThreshold ? InspectorFormMode.Narrow : InspectorFormMode.Wide;
}
