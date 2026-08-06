namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4（补充裁决 §4.3）：地图编辑器密度模式——内容宽 <320 进紧凑模式。
// 纯布局逻辑；标准与紧凑模式共享同一 ViewModel 状态，只调整布局密度。
public enum MapEditorDensityMode { Standard, Compact }

public static class MapEditorLayoutModel
{
    public const double CompactThreshold = 320; // 内容宽 <320 → 紧凑模式（UI Spec §7.1）

    public static MapEditorDensityMode ModeFor(double contentWidth) =>
        contentWidth < CompactThreshold ? MapEditorDensityMode.Compact : MapEditorDensityMode.Standard;
}
