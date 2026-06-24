namespace FluidWarfare.Render.Vulkan.Camera;

/// <summary>
/// 射线构建状态，区分技术失败和有效未命中。
/// </summary>
public enum SceneRayBuildStatus
{
    /// <summary>射线成功构建。</summary>
    Success,

    /// <summary>快照不可用或无效。</summary>
    SnapshotUnavailable,

    /// <summary>快照视口尺寸与当前窗口不一致（Resize 后未 Present）。</summary>
    SnapshotExtentMismatch,

    /// <summary>像素超出视口范围。</summary>
    PixelOutOfBounds,

    /// <summary>逆矩阵无效或矩阵奇异。</summary>
    MatrixInvalid,

    /// <summary>反投影后方向为零向量。</summary>
    DirectionInvalid
}
