namespace FluidWarfare.Editor.Windows.Panels.HierarchyVisual;

/// <summary>
/// 树节点的分支信息，用于绘制树干连接线。
/// 由树索引在构建时生成。
/// </summary>
public sealed record HierarchyBranchInfo(
    int Depth,
    bool IsLastSibling,
    bool[] AncestorHasNextSibling);
