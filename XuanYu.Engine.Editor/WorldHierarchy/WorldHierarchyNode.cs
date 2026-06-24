using FluidWarfare.Core.Identity;

namespace FluidWarfare.Editor.WorldHierarchy;

/// <summary>
/// 层级树中的一个节点。不可变创建，构建后 Children 集合不可修改。
/// NodeId 格式：world:root / group:<type> / entity:&lt;EntityId&gt;
/// EntityId 仅 Entity 节点持有，Root 和 Group 为 null。
/// </summary>
public sealed record WorldHierarchyNode(
    string NodeId,
    WorldHierarchyNodeKind Kind,
    string DisplayName,
    string? SecondaryText,
    EntityId? EntityId,
    string? IconKind,
    bool IsSelectable,
    IReadOnlyList<WorldHierarchyNode> Children)
{
    /// <summary>
    /// 子孙节点总数（含自身）。
    /// </summary>
    public int DescendantCount { get; internal set; }
}
