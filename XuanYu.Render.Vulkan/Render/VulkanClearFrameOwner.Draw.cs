using System.Numerics;
using Silk.NET.Vulkan;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Render.Vulkan.Pipeline;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    void RecordDraw(CommandBuffer cb)
    {
        if (_pipeline.Handle == 0 || _pipelineLayout.Handle == 0) return;
        Viewport* pVp = stackalloc Viewport[1];
        pVp[0] = new Viewport { X = 0, Y = 0, Width = _extent.Width, Height = _extent.Height, MinDepth = 0, MaxDepth = 1 };
        Rect2D* pSc = stackalloc Rect2D[1];
        pSc[0] = new Rect2D { Offset = new Offset2D { X = 0, Y = 0 }, Extent = _extent };
        float* scene = stackalloc float[20];
        FillScenePushConstants(scene);

        _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _pipeline);
        _vk.CmdPushConstants(cb, _pipelineLayout, ShaderStageFlags.VertexBit, 0, VulkanScenePushConstants.SizeInBytes, scene);
        _vk.CmdSetViewport(cb, 0, 1, pVp);
        _vk.CmdSetScissor(cb, 0, 1, pSc);
        _vk.CmdDraw(cb, _sceneSnapshot.IsSelected ? 21u : 3u, 1, 0, 0);
    }

    void FillScenePushConstants(float* target)
    {
        var viewport = new ViewportState(0, 0, _extent.Width, _extent.Height, (int)_extent.Width, (int)_extent.Height, 1, _swapchainOwner.ResourceGeneration);
        var camera = DefaultEditorCamera.Create(_swapchainOwner.ResourceGeneration);
        var state = ViewProjectionState.Create(camera, viewport);
        var projection = ToVulkanProjection(state.Projection);
        var viewProjection = state.View * projection;
        FillMatrixTranspose(target, viewProjection);
        var position = _sceneSnapshot.RenderPosition;
        target[16] = (float)position.X;
        target[17] = (float)position.Y;
        target[18] = (float)position.Z;
        target[19] = 1.0f;
    }

    static void FillMatrixTranspose(float* target, Matrix4x4 matrix)
    {
        target[0] = matrix.M11; target[1] = matrix.M12; target[2] = matrix.M13; target[3] = matrix.M14;
        target[4] = matrix.M21; target[5] = matrix.M22; target[6] = matrix.M23; target[7] = matrix.M24;
        target[8] = matrix.M31; target[9] = matrix.M32; target[10] = matrix.M33; target[11] = matrix.M34;
        target[12] = matrix.M41; target[13] = matrix.M42; target[14] = matrix.M43; target[15] = matrix.M44;
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
