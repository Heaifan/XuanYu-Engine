using System.Runtime.InteropServices;

namespace FluidWarfare.Render.Vulkan.Camera;

/// <summary>
/// 3D 相机矩阵计算。
/// 提供 View、Projection 和 ViewProjection 矩阵。
/// 使用 Vulkan 坐标系：NDC 深度范围 0..1，Y 向下（已处理翻转）。
/// 矩阵以列优先 float[16] 格式存储，适合 Push Constant 直接使用。
/// </summary>
public static class VulkanCameraMatrices
{
    /// <summary>
    /// 计算 ViewProjection 矩阵，以列优先 float[16] 格式返回。
    /// </summary>
    public static float[] ComputeVulkanMVP(
        VulkanCameraInfo camera,
        float aspectRatio)
    {
        var view = LookAt(
            camera.PositionX, camera.PositionY, camera.PositionZ,
            camera.TargetX, camera.TargetY, camera.TargetZ,
            camera.UpX, camera.UpY, camera.UpZ);

        var proj = PerspectiveVulkan(
            camera.FieldOfViewDegrees,
            aspectRatio,
            camera.NearPlane,
            camera.FarPlane);

        return Mul(proj, view);
    }

    /// <summary>
    /// LookAt 矩阵（列优先 float[16]）。
    /// </summary>
    private static float[] LookAt(
        float eyeX, float eyeY, float eyeZ,
        float centerX, float centerY, float centerZ,
        float upX, float upY, float upZ)
    {
        // Forward = normalize(center - eye)
        var fLen = (float)Math.Sqrt(
            (centerX - eyeX) * (centerX - eyeX) +
            (centerY - eyeY) * (centerY - eyeY) +
            (centerZ - eyeZ) * (centerZ - eyeZ));
        if (fLen < 1e-10f) fLen = 1;
        var fX = (centerX - eyeX) / fLen;
        var fY = (centerY - eyeY) / fLen;
        var fZ = (centerZ - eyeZ) / fLen;

        // Side = normalize(forward × up)
        var sX = fY * upZ - fZ * upY;
        var sY = fZ * upX - fX * upZ;
        var sZ = fX * upY - fY * upX;
        var sLen = (float)Math.Sqrt(sX * sX + sY * sY + sZ * sZ);
        if (sLen < 1e-10f) sLen = 1;
        sX /= sLen; sY /= sLen; sZ /= sLen;

        // Up = side × forward
        var uX = sY * fZ - sZ * fY;
        var uY = sZ * fX - sX * fZ;
        var uZ = sX * fY - sY * fX;

        // View matrix (column-major):
        // [ side.x    up.x    -forward.x    0 ]
        // [ side.y    up.y    -forward.y    0 ]
        // [ side.z    up.z    -forward.z    0 ]
        // [ -s·e     -u·e      f·e          1 ]
        return
        [
            sX, sY, sZ, 0,
            uX, uY, uZ, 0,
            -fX, -fY, -fZ, 0,
            -(sX * eyeX + sY * eyeY + sZ * eyeZ),
            -(uX * eyeX + uY * eyeY + uZ * eyeZ),
            fX * eyeX + fY * eyeY + fZ * eyeZ,
            1
        ];
    }

    /// <summary>
    /// 透视投影矩阵（Vulkan NDC：深度 0..1，Y 翻转）。
    /// </summary>
    private static float[] PerspectiveVulkan(
        float fovDeg, float aspect, float near, float far)
    {
        var f = 1.0f / (float)Math.Tan(fovDeg * Math.PI / 360.0);
        var range = near - far;

        // Vulkan 投影：深度范围 0..1，Y 翻转
        return
        [
            f / aspect, 0, 0, 0,
            0, -f, 0, 0,
            0, 0, far / range, -1,
            0, 0, near * far / range, 0
        ];
    }

    /// <summary>
    /// 4x4 矩阵乘法（列优先）。
    /// result = a × b
    /// </summary>
    private static float[] Mul(float[] a, float[] b)
    {
        var r = new float[16];
        for (var col = 0; col < 4; col++)
        {
            for (var row = 0; row < 4; row++)
            {
                var sum = 0.0f;
                for (var k = 0; k < 4; k++)
                {
                    sum += a[k * 4 + row] * b[col * 4 + k];
                }
                r[col * 4 + row] = sum;
            }
        }
        return r;
    }
}
