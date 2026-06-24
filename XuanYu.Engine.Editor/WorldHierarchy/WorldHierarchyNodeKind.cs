namespace FluidWarfare.Editor.WorldHierarchy;

/// <summary>
/// 层级节点类型。Root 和 Group 不可选择为实体，Entity 可选择。
/// </summary>
public enum WorldHierarchyNodeKind
{
    /// <summary>World 根节点。</summary>
    WorldRoot,

    /// <summary>实体分类分组（如"单位"）。</summary>
    EntityGroup,

    /// <summary>单个 World 实体。</summary>
    Entity
}
