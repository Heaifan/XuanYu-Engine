namespace FluidWarfare.Render.Selection.Pointer;

/// <summary>
/// Pointer Picking 结果类型优先级：Entity > Ground > None。
/// </summary>
public enum ScenePointerPickKind
{
    /// <summary>未命中任何对象。</summary>
    None,

    /// <summary>命中单位实体（AABB 优先）。</summary>
    Entity,

    /// <summary>命中空白地面。</summary>
    Ground
}
