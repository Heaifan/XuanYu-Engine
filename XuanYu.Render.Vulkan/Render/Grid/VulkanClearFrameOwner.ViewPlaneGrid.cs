using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

// F3-F4：正交标准视图的视图平面网格绘制（±X→YZ / ±Y→XZ，以世界原点为基准）。
// PushConstant 192B（48 float）：
//   0-39 同参考网格（VP/InvVP/相机/视口+far 由 FillGridPushConstants 填充）
//   40-43 gridScale（Fine/Coarse Spacing + 权重）
//   44-47 planeNormal（平面法线，轴向：YZ=(1,0,0) / XZ=(0,1,0)）
public sealed unsafe partial class VulkanClearFrameOwner
{
    const uint ViewPlaneGridFloatCount = 48;
    public const uint ViewPlaneGridPushSize = ViewPlaneGridFloatCount * 4;

    Silk.NET.Vulkan.Pipeline _viewPlaneGridPipeline;
    PipelineLayout _viewPlaneGridPipelineLayout;

    public void SetViewPlaneGridPipeline(Silk.NET.Vulkan.Pipeline pipeline, PipelineLayout layout)
    {
        _viewPlaneGridPipeline = pipeline;
        _viewPlaneGridPipelineLayout = layout;
        if (_views.Length > 0 && !RecordCommandBuffers(_views)) throw new InvalidOperationException("Pipeline 注入后 CommandBuffer 重录失败");
    }

    void DrawViewPlaneGrid(CommandBuffer cb)
    {
        if (_viewPlaneGridPipeline.Handle == 0 || _viewPlaneGridPipelineLayout.Handle == 0) return;
        var scene = new float[ViewPlaneGridFloatCount];
        FillGridPushConstants(scene, _renderProjection);
        var levels = ReferenceGridScale.Compute(_lastViewportMetric.MetersPerDip);
        scene[40] = (float)levels.FineSpacing;
        scene[41] = (float)levels.CoarseSpacing;
        scene[42] = (float)levels.FineWeight;
        scene[43] = (float)levels.CoarseWeight;
        FillPlaneNormal(scene, _renderProjection.Assist.ViewPlaneGrid);
        PushViewPlaneGridConstants(cb, scene);
        _vk.CmdDraw(cb, RenderDrawPlan.ReferenceGridVertexCount, 1, 0, 0);
    }

    static void FillPlaneNormal(float[] scene, EditorViewPlaneGridKind kind)
    {
        // 平面过原点，法线符号不影响网格；YZ=±X、XZ=±Y。
        scene[44] = kind == EditorViewPlaneGridKind.YZ ? 1.0f : 0.0f;
        scene[45] = kind == EditorViewPlaneGridKind.XZ ? 1.0f : 0.0f;
        scene[46] = 0.0f;
        scene[47] = 0.0f;
    }

    void PushViewPlaneGridConstants(CommandBuffer cb, float[] scene)
    {
        fixed (float* pScene = scene)
        {
            var range = new PushConstantRange
            {
                StageFlags = ShaderStageFlags.VertexBit | ShaderStageFlags.FragmentBit,
                Offset = 0,
                Size = ViewPlaneGridFloatCount * 4
            };
            _vk.CmdPushConstants(cb, _viewPlaneGridPipelineLayout, range.StageFlags, 0,
                ViewPlaneGridFloatCount * 4, pScene);
        }
    }
}
