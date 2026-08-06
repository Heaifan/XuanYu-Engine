namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4/D4-F1 纠偏：面板紧凑密度模式——内容宽 <320 进紧凑密度。
// 职责仅限面板密度（根 Padding、分组间距、字段行距、按钮组布局、辅助文字密度）；
// 与 EditableFormLayoutModel（<360 输入表单上下）并存且互不替代。
// 只读资产摘要始终单行双列，不参与任何模式切换。纯逻辑、可脱离 GPU 测试。
public enum MapEditorDensityMode { Standard, Compact }

public static class MapEditorLayoutModel
{
    public const double CompactThreshold = 320; // UI Spec §7.1 面板紧凑模式阈值

    public static MapEditorDensityMode ModeFor(double contentWidth) =>
        contentWidth < CompactThreshold ? MapEditorDensityMode.Compact : MapEditorDensityMode.Standard;
}
