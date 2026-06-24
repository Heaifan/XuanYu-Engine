namespace XuanYu.Engine.Editor.Windows.Panels.HierarchyVisual;

/// <summary>
/// 一个可见节点的树干位置。
/// Depth 从 0 开始；Root 为 0。
/// </summary>
public sealed record HierarchyBranchInfo(
    int Depth,
    bool IsLastSibling,
    bool[] AncestorHasNextSibling);
