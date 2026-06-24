namespace XuanYu.Engine.Render.Vulkan.Camera;

/// <summary>4x4 矩阵运算（列优先）。用于模型变换与矩阵乘法。</summary>
public static class VulkanMatrixOperations
{
    public static float[] Mul(float[] a, float[] b)
    {
        var r = new float[16];
        for (var col = 0; col < 4; col++) for (var row = 0; row < 4; row++) { var s = 0.0f; for (var k = 0; k < 4; k++) s += a[k * 4 + row] * b[col * 4 + k]; r[col * 4 + row] = s; }
        return r;
    }

    public static float[] CreateTranslation(float x, float y, float z) => new float[] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, x, y, z, 1 };
    public static float[] CreateScale(float s) => new float[] { s, 0, 0, 0, 0, s, 0, 0, 0, 0, s, 0, 0, 0, 0, 1 };
}
