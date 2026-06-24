using XuanYu.Engine.Core.Identity;

namespace XuanYu.Engine.Editor.WorldHierarchy;

/// <summary>
/// World 层级树的只读快照。
/// 保存 EntityId → Node 索引和 EntityId → 祖先 NodeId 路径，支持 O(1) Reveal。
/// </summary>
public sealed record WorldHierarchyTree(
    WorldHierarchyNode Root,
    int NodeCount,
    int EntityNodeCount,
    IReadOnlyDictionary<string, WorldHierarchyNode> EntityNodes,
    IReadOnlyDictionary<string, IReadOnlyList<string>> EntityAncestorNodeIds)
{
    /// <summary>
    /// 空的层级树（仅 World Root 无子节点）。
    /// </summary>
    public static readonly WorldHierarchyTree Empty = new(
        new WorldHierarchyNode(
            "world:root",
            WorldHierarchyNodeKind.WorldRoot,
            "World", null, null, "world", false, []),
        1, 0,
        new Dictionary<string, WorldHierarchyNode>(),
        new Dictionary<string, IReadOnlyList<string>>());

    /// <summary>
    /// 按 EntityId 查找节点（使用字符串形式匹配）。
    /// </summary>
    public WorldHierarchyNode? FindEntity(string entityId)
    {
        return EntityNodes.TryGetValue(entityId, out var node) ? node : null;
    }

    /// <summary>
    /// 获取实体节点的祖先 NodeId 路径（从 Root 到父节点）。
    /// </summary>
    public IReadOnlyList<string>? GetAncestorNodeIds(string entityId)
    {
        return EntityAncestorNodeIds.TryGetValue(entityId, out var path) ? path : null;
    }
}
