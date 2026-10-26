using System.Numerics;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    static void FillMatrixTranspose(float* target, Matrix4x4 matrix)
    {
        target[0] = matrix.M11; target[1] = matrix.M12;
        target[2] = matrix.M13; target[3] = matrix.M14;
        target[4] = matrix.M21; target[5] = matrix.M22;
        target[6] = matrix.M23; target[7] = matrix.M24;
        target[8] = matrix.M31; target[9] = matrix.M32;
        target[10] = matrix.M33; target[11] = matrix.M34;
        target[12] = matrix.M41; target[13] = matrix.M42;
        target[14] = matrix.M43; target[15] = matrix.M44;
    }

    // F2：求逆后转置存储（GLSL 列主序），供参考网格 Pass 的 inverseViewProjection 使用。
    static void FillMatrixTransposeInverse(float* target, Matrix4x4 matrix)
    {
        if (!Matrix4x4.Invert(matrix, out var inv))
        {
            inv = Matrix4x4.Identity;
        }
        FillMatrixTranspose(target, inv);
    }

    static Matrix4x4 ToVulkanProjection(Matrix4x4 projection)
    {
        projection.M12 = -projection.M12;
        projection.M22 = -projection.M22;
        projection.M32 = -projection.M32;
        projection.M42 = -projection.M42;
        return projection;
    }
}
