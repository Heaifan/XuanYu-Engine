using Silk.NET.Vulkan;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Pipeline;
namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    Silk.NET.Vulkan.Pipeline _skyPipeline;
    PipelineLayout _skyPipelineLayout;
    Silk.NET.Vulkan.Pipeline _gridPipeline;
    PipelineLayout _gridPipelineLayout;
    // WORLD-D-R1：天空管线（深度不写）注入；与主管线共用 PushConstants 布局。
    public void SetSkyPipeline(Silk.NET.Vulkan.Pipeline pipeline, PipelineLayout layout)
    {
        _skyPipeline = pipeline;
        _skyPipelineLayout = layout;
        if (_views.Length > 0 && !RecordCommandBuffers(_views)) throw new InvalidOperationException("Pipeline 注入后 CommandBuffer 重录失败");
    }    void RecordDraw(CommandBuffer cb)
    {
        if (_pipeline.Handle == 0 || _pipelineLayout.Handle == 0) return;
        var viewport = new[] { new Viewport { X = 0, Y = 0, Width = _extent.Width, Height = _extent.Height, MinDepth = 0, MaxDepth = 1 } };
        var scissor = new[] { new Rect2D { Offset = new Offset2D { X = 0, Y = 0 }, Extent = _extent } };
        var scene = new float[VulkanScenePushConstants.FloatCount];
        fixed (Viewport* pVp = viewport)
        fixed (Rect2D* pSc = scissor)
        fixed (float* pScene = scene)
        {
            _vk.CmdSetViewport(cb, 0, 1, pVp);
            _vk.CmdSetScissor(cb, 0, 1, pSc);
            BindProceduralVertexBuffer(cb);
            if (!_hasRenderProjection) return;
            _staticModels.RetainOnly(_renderProjection.Entities.Select(e => e.StaticModelKey));
            _vectorOverlays.RetainOnly(_renderProjection.VectorOverlayResources.Select(r => r.Key));
            foreach (var draw in RenderDrawPlan.GetFrameDrawPlan(_renderProjection))
            {
                if (draw.Kind == RenderDrawKind.MapGround) continue;
                BindFramePipeline(cb, draw.Kind);
                if (draw.Kind == RenderDrawKind.MapGround && _mapSurfaceIndexBuffer is not null)
                    DrawMapSurface(cb, pScene);
                else if (draw.Kind == RenderDrawKind.MapBounds && _mapBoundsVertexBuffer is not null)
                    DrawMapBounds(cb, pScene);
                else if (draw.Kind == RenderDrawKind.MapVectorOverlay)
                    DrawVectorOverlay(cb, pScene, draw.EntityIndex);
                // F2-R2/F3-F1：网格/轴/原点/导航 Gizmo 独立全屏 Pass。
                if (draw.Kind == RenderDrawKind.EditorReferenceGrid) DrawReferenceGrid(cb);
                else if (draw.Kind == RenderDrawKind.WorldOrigin) DrawWorldOrigin(cb);
                else if (draw.Kind == RenderDrawKind.WorldAxes) DrawWorldAxes(cb);
                else if (draw.Kind == RenderDrawKind.ScaleIndicatorOverlay) DrawScaleIndicator(cb);
                else if (draw.Kind == RenderDrawKind.NavigationGizmo) DrawNavigationGizmo(cb);
                else if (draw.Kind == RenderDrawKind.EditorViewPlaneGrid) DrawViewPlaneGrid(cb);
                else if (draw.Kind < RenderDrawKind.EntityFill) DrawAssist(cb, pScene, draw);
                else if (draw.EntityIndex >= 0) DrawEntity(cb, pScene, draw);
                else DrawGizmo(cb, pScene, draw);
            }
        }
    }
    void BindProceduralVertexBuffer(CommandBuffer cb)
    {
        if (_proceduralVertexBuffer is null) return;
        var buffer = _proceduralVertexBuffer.Buffer;
        ulong offset = 0;
        _vk.CmdBindVertexBuffers(cb, 0, 1, &buffer, &offset);
    }
    void DrawEntity(CommandBuffer cb, float* scene, RenderDrawPlan.FrameEntry draw)
    {
        var entity = _renderProjection.Entities[draw.EntityIndex];
        if (entity.EntityType == RenderEntityType.StaticModel)
        {
            DrawStaticModel(cb, scene, entity);
            return;
        }
        var entityMode = draw.EntityType == RenderEntityType.Cube ? -1.0f : -2.0f;
        var selectionMode = draw.Kind == RenderDrawKind.EntityOutline ? 2.0f : (entity.IsSelected ? 1.0f : 0.0f);
        FillScenePushConstants(scene, _renderProjection, entity.Position, entity.Rotation, entity.Scale, 0.0f, selectionMode, entityMode);
        PushSceneConstants(cb, scene);
        _vk.CmdDraw(cb, (uint)draw.VertexCount, 1, 0, 0);
    }
}
