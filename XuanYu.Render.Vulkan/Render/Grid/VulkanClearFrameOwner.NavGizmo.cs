using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

// MAP-A-R1-D5-R1-F3-F1：导航 Gizmo Overlay Pass —— 屏幕空间、深度测试/写入关闭、最后绘制。
// PushConstant 80B（20 float）：
//   vec4 cameraRight @0    vec4 cameraUp @16    vec4 cameraForward @32
//   vec4 viewportAndDpi @48 (xy=视口尺寸 px; z=DPI; w=未用)
//   vec4 gizmoParams @64 (x=区域尺寸 DIP 88; y=边距 DIP 12; z=悬停端点索引 -1=无; w=未用)
public sealed unsafe partial class VulkanClearFrameOwner
{
    const uint NavGizmoPushFloatCount = 20;
    public const uint NavGizmoPushSize = NavGizmoPushFloatCount * 4;

    Silk.NET.Vulkan.Pipeline _navGizmoPipeline;
    PipelineLayout _navGizmoPipelineLayout;

    public void SetNavGizmoPipeline(Silk.NET.Vulkan.Pipeline pipeline, PipelineLayout layout)
    {
        _navGizmoPipeline = pipeline;
        _navGizmoPipelineLayout = layout;
        if (_views.Length > 0 && !RecordCommandBuffers(_views)) throw new InvalidOperationException("导航 Gizmo 管线注入后 CommandBuffer 重录失败");
    }

    void DrawNavigationGizmo(CommandBuffer cb)
    {
        if (_navGizmoPipeline.Handle == 0 || _navGizmoPipelineLayout.Handle == 0) return;
        var scene = new float[NavGizmoPushFloatCount];
        var camera = _renderProjection.Camera;
        scene[0] = (float)camera.Right.X;
        scene[1] = (float)camera.Right.Y;
        scene[2] = (float)camera.Right.Z;
        scene[4] = (float)camera.Up.X;
        scene[5] = (float)camera.Up.Y;
        scene[6] = (float)camera.Up.Z;
        scene[8] = (float)camera.Forward.X;
        scene[9] = (float)camera.Forward.Y;
        scene[10] = (float)camera.Forward.Z;
        scene[12] = _extent.Width;
        scene[13] = _extent.Height;
        scene[14] = (float)_renderProjection.ViewportDpiScale;
        scene[16] = 96.0f; // 区域尺寸 DIP（F3-F3）
        scene[17] = 14.0f; // 边距 DIP（F3-F3）
        scene[18] = _renderProjection.AssistState.NavGizmoHoverIndex;
        scene[19] = (float)_renderProjection.ViewportDpiScale;
        fixed (float* pScene = scene)
        {
            var range = new PushConstantRange
            {
                StageFlags = ShaderStageFlags.VertexBit | ShaderStageFlags.FragmentBit,
                Offset = 0,
                Size = NavGizmoPushFloatCount * 4
            };
            _vk.CmdPushConstants(cb, _navGizmoPipelineLayout, range.StageFlags, 0,
                NavGizmoPushFloatCount * 4, pScene);
            _vk.CmdDraw(cb, RenderDrawPlan.FullscreenTriangleVertexCount, 1, 0, 0);
        }
    }
}
