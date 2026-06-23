using FluidWarfare.Render.Vulkan.Camera;
using Silk.NET.Vulkan;

namespace FluidWarfare.Render.Vulkan.Scene3D;

/// <summary>诊断探针的 MVP 矩阵计算与逐对象变换。</summary>
public static unsafe partial class VulkanScene3dRenderer
{
    static float[] ProbeComputeMVP(VulkanCameraInfo camera, Extent2D extent, out float aspect)
    {
        aspect = extent.Width / (float)extent.Height;
        return VulkanCameraMatrices.ComputeVulkanMVP(camera, aspect);
    }

    static void ProbeComputePerObjectMVP(ReadOnlySpan<VulkanScene3dUnitDrawInfo> unitDraws, float[] vp,
        out VulkanScene3dCommandRecorder.UnitDrawData[] unitMvpData, out int renderedUnitCount)
    {
        var unitMvpList = new List<float[]>();
        foreach (var draw in unitDraws)
        {
            var trans = VulkanMatrixOperations.CreateTranslation(draw.X, draw.Y, draw.Z);
            var scale = VulkanMatrixOperations.CreateScale(draw.Scale);
            unitMvpList.Add(VulkanMatrixOperations.Mul(vp, VulkanMatrixOperations.Mul(trans, scale)));
        }
        unitMvpData = unitMvpList.Select(mvp =>
            new VulkanScene3dCommandRecorder.UnitDrawData(mvp, VulkanScene3dPushConstants.NormalTint)).ToArray();
        renderedUnitCount = unitDraws.Length;
    }
}
