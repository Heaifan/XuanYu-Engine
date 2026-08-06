namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4/D4-F1：可编辑表单行（EditableFormRow）布局模式——内容宽 ≥360 左右、
// <360 整组标签在上、输入控件在下。仅适用于真实输入控件（TextBox/数字框/下拉框），
// 不得套用到普通只读属性（只读键值行始终单行双列）。纯逻辑、可脱离 GPU 测试。
public enum EditableFormMode { Wide, Narrow }

public static class EditableFormLayoutModel
{
    public const double WideThreshold = 360;   // UI Spec §7.1 表单纵向切换阈值
    public const double LabelColumnWidth = 96; // 与 Control.LabelColumn.Width 一致（§5.3）
    public const double FieldMinWidth = 128;   // 与 Control.Field.MinWidth 一致（§5.3）

    public static EditableFormMode ModeFor(double contentWidth) =>
        contentWidth < WideThreshold ? EditableFormMode.Narrow : EditableFormMode.Wide;
}
