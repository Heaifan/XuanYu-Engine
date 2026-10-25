namespace XuanYu.Render.Abstractions;

// F3-F4：正交标准视图的视图平面网格类型。None=不显示；
// YZ=±X 视图（YZ 平面）、XZ=±Y 视图（XZ 平面）。
// ±Z（顶/底）视图复用现有地面网格（Z=0 平面即 XY 平面），不需要额外平面。
public enum EditorViewPlaneGridKind
{
    None = 0,
    YZ = 1,
    XZ = 2,
}
