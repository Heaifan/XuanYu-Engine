namespace FluidWarfare.Render.Camera.Navigation;

/// <summary>
/// Vulkan 正交投影矩阵计算（Z-Up，深度范围 0..1）。
/// </summary>
public static class SceneOrthographicProjection
{
    /// <summary>
    /// 计算列优先 float[16] 正交投影矩阵。
    /// Vulkan NDC：X[-1,1], Y[-1,1], Z[0,1]。
    /// </summary>
    /// <param name="orthoHeight">视口垂直世界范围的一半。</param>
    /// <param name="aspect">宽高比（width/height）。</param>
    /// <param name="near">近裁剪面。</param>
    /// <param name="far">远裁剪面。</param>
    /// <returns>列优先 float[16] 正交矩阵。</returns>
    public static float[] ComputeVulkanOrthographic(
        float orthoHeight,
        float aspect,
        float near,
        float far)
    {
        var orthoWidth = orthoHeight * aspect;
        var halfW = orthoWidth / 2f;
        var halfH = orthoHeight / 2f;
        var range = near - far;

        // Vulkan 正交矩阵（深度 0..1，Y 翻转）
        // 列优先 float[16]
        return
        [
            1f / halfW, 0, 0, 0,
            0, -1f / halfH, 0, 0,
            0, 0, 1f / range, 0,
            0, 0, near / range, 1
        ];
    }

    /// <summary>
    /// 从轨道相机状态的 OrthographicHeight 计算正交矩阵。
    /// </summary>
    public static float[] ComputeFromCameraState(
        SceneOrbitCameraState state,
        float aspect)
    {
        return ComputeVulkanOrthographic(
            state.OrthographicHeight,
            aspect,
            state.NearPlane,
            state.FarPlane);
    }
}
