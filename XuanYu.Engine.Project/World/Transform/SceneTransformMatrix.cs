namespace XuanYu.Engine.Project.World.Transform;

/// <summary>
/// SceneTransform ↔ 变换矩阵的转换入口。
/// 纯数据转换，不依赖渲染层。
/// 当前阶段仅 Position 生效。
/// </summary>
public static class SceneTransformMatrix
{
    /// <summary>从 SceneTransform 提取平移分量。</summary>
    public static (float X, float Y, float Z) GetTranslation(SceneTransform t) =>
        ((float)t.Position.X, (float)t.Position.Y, (float)t.Position.Z);
}
