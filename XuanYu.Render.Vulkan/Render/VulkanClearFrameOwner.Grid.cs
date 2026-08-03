using Silk.NET.Vulkan;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

// MAP-A-R1-D5-R1-F2：独立参考网格绘制。使用本 Pass 专属 160B PushConstant：
//   mat4 viewProjection @0    mat4 inverseViewProjection @64
//   vec4 cameraPosition @128  vec4 viewportAndFar @144 (xy=视口, z=Far, w=GridMaxDist)
// 不挤占场景 128B 布局，不修改实体/Gizmo PushConstants。
public sealed unsafe partial class VulkanClearFrameOwner
{
    const uint GridPushFloatCount = 40;

    void DrawReferenceGrid(CommandBuffer cb)
    {
        if (_gridPipeline.Handle == 0 || _gridPipelineLayout.Handle == 0) return;
        var scene = new float[GridPushFloatCount];
        fixed (float* pScene = scene)
        {
            var projection = _renderProjection;
            var camera = projection.Camera;
            var viewport = new ViewportState(
                0, 0, _extent.Width, _extent.Height,
                (int)_extent.Width, (int)_extent.Height, 1, _swapchainOwner.ResourceGeneration);
            var state = camera.ToViewProjection(viewport);
            var vulkanProjection = ToVulkanProjection(state.Projection);
            var viewProjection = state.View * vulkanProjection;
            FillMatrixTranspose(pScene, viewProjection);
            FillMatrixTransposeInverse(pScene + 16, viewProjection);
            pScene[32] = (float)camera.Position.X;
            pScene[33] = (float)camera.Position.Y;
            pScene[34] = (float)camera.Position.Z;
            pScene[35] = 1.0f;
            pScene[36] = _extent.Width;
            pScene[37] = _extent.Height;
            pScene[38] = (float)camera.FarPlane;
            pScene[39] = (float)(camera.FarPlane * 0.75); // gridMaxDistance：不满强度到 Far
            var range = new PushConstantRange
            {
                StageFlags = ShaderStageFlags.VertexBit | ShaderStageFlags.FragmentBit,
                Offset = 0,
                Size = GridPushFloatCount * 4
            };
            _vk.CmdPushConstants(cb, _gridPipelineLayout, range.StageFlags, 0,
                GridPushFloatCount * 4, pScene);
            _vk.CmdDraw(cb, RenderDrawPlan.ReferenceGridVertexCount, 1, 0, 0);
        }
    }
}
