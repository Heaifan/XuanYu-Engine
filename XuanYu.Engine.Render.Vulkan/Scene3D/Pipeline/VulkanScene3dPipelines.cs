using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D;

/// <summary>
/// Grid (LineList) + Unit (TriangleList) Graphics Pipeline 编排器。
/// 共享状态就地构建（保证指针有效期），管线创建委托给 TryCreate。
/// </summary>
public static unsafe partial class VulkanScene3dPipelines
{
    public static bool Create(Vk vk, Silk.NET.Vulkan.Device dev,
        RenderPass renderPass, PipelineLayout pipelineLayout,
        ShaderModule vertModule, ShaderModule fragModule,
        uint viewportWidth, uint viewportHeight,
        out Pipeline gridPipeline, out Pipeline unitPipeline,
        out string errorMessage)
    {
        gridPipeline = default; unitPipeline = default; errorMessage = string.Empty;

        // ── Vertex input ─────────────────────────────────────
        var vertexBinding = new VertexInputBindingDescription { Binding = 0, Stride = 28, InputRate = VertexInputRate.Vertex };
        var vertexAttribs = new[]
        {
            new VertexInputAttributeDescription { Location = 0, Binding = 0, Format = Format.R32G32B32Sfloat, Offset = 0 },
            new VertexInputAttributeDescription { Location = 1, Binding = 0, Format = Format.R32G32B32A32Sfloat, Offset = 12 }
        };

        fixed (VertexInputAttributeDescription* pAttr = vertexAttribs)
        {
            var viCI = new PipelineVertexInputStateCreateInfo
            {
                SType = StructureType.PipelineVertexInputStateCreateInfo,
                VertexBindingDescriptionCount = 1, PVertexBindingDescriptions = &vertexBinding,
                VertexAttributeDescriptionCount = 2, PVertexAttributeDescriptions = pAttr
            };

            // ── Viewport / Scissor ───────────────────────────
            var viewport = new Viewport { X = 0, Y = 0, Width = viewportWidth, Height = viewportHeight, MinDepth = 0, MaxDepth = 1 };
            var scissor = new Rect2D(new Offset2D(0, 0), new Extent2D(viewportWidth, viewportHeight));
            var vsCI = new PipelineViewportStateCreateInfo { SType = StructureType.PipelineViewportStateCreateInfo, ViewportCount = 1, PViewports = &viewport, ScissorCount = 1, PScissors = &scissor };

            // ── Rasterizer / Depth / Multisample / Blend ─────
            var rsCI = BuildRasterizerState();
            var dsCI = BuildDepthStencilState();
            var msCI = BuildMultisampleState();
            var blendAtt = new PipelineColorBlendAttachmentState
            {
                BlendEnable = Vk.False,
                ColorWriteMask = ColorComponentFlags.RBit | ColorComponentFlags.GBit | ColorComponentFlags.BBit | ColorComponentFlags.ABit
            };
            var cbCI = new PipelineColorBlendStateCreateInfo { SType = StructureType.PipelineColorBlendStateCreateInfo, AttachmentCount = 1, PAttachments = &blendAtt };

            // ── Shader stages (stackalloc, valid until return)─
            var name0 = Marshal.StringToHGlobalAnsi("main");
            var name1 = Marshal.StringToHGlobalAnsi("main");
            var stages = stackalloc PipelineShaderStageCreateInfo[2];
            stages[0] = new PipelineShaderStageCreateInfo { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.VertexBit, Module = vertModule, PName = (byte*)name0 };
            stages[1] = new PipelineShaderStageCreateInfo { SType = StructureType.PipelineShaderStageCreateInfo, Stage = ShaderStageFlags.FragmentBit, Module = fragModule, PName = (byte*)name1 };

            // ── Grid pipeline ────────────────────────────────
            if (!TryCreate(vk, dev, renderPass, pipelineLayout,
                    &viCI, &vsCI, &rsCI, &dsCI, &msCI, &cbCI, stages,
                    PrimitiveTopology.LineList, out gridPipeline, out errorMessage))
            { Marshal.FreeHGlobal(name0); Marshal.FreeHGlobal(name1); return false; }

            // ── Unit pipeline ────────────────────────────────
            if (!TryCreate(vk, dev, renderPass, pipelineLayout,
                    &viCI, &vsCI, &rsCI, &dsCI, &msCI, &cbCI, stages,
                    PrimitiveTopology.TriangleList, out unitPipeline, out errorMessage))
            { vk.DestroyPipeline(dev, gridPipeline, null); gridPipeline = default; Marshal.FreeHGlobal(name0); Marshal.FreeHGlobal(name1); return false; }

            Marshal.FreeHGlobal(name0); Marshal.FreeHGlobal(name1);
        }
        return true;
    }
}
