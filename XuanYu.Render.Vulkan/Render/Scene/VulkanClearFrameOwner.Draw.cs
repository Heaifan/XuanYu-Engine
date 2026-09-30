using Silk.NET.Vulkan;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Pipeline;
namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    internal enum DrawOwner
    {
        EditorBackground,
        EditorReferenceGrid,
        WorldOrigin,
        WorldAxes,
        MapGround,
        MapBounds,
        Terrain,
        VectorOverlay,
        Entity,
        Gizmo,
        ScaleIndicatorOverlay,
        NavigationGizmo,
        EditorViewPlaneGrid
    }

    internal readonly record struct PipelineReadiness(
        bool Main,
        bool Sky,
        bool Grid,
        bool Origin,
        bool Axes,
        bool Navigation,
        bool ScaleIndicator,
        bool ViewPlaneGrid,
        bool Terrain,
        bool VectorOverlay);

    internal static DrawOwner ResolveDrawOwner(RenderDrawKind kind) => kind switch
    {
        RenderDrawKind.EditorBackground => DrawOwner.EditorBackground,
        RenderDrawKind.EditorReferenceGrid => DrawOwner.EditorReferenceGrid,
        RenderDrawKind.WorldOrigin => DrawOwner.WorldOrigin,
        RenderDrawKind.WorldAxes => DrawOwner.WorldAxes,
        RenderDrawKind.MapGround => DrawOwner.MapGround,
        RenderDrawKind.MapBounds => DrawOwner.MapBounds,
        RenderDrawKind.Terrain => DrawOwner.Terrain,
        RenderDrawKind.MapVectorOverlay => DrawOwner.VectorOverlay,
        RenderDrawKind.EntityFill or RenderDrawKind.EntityOutline => DrawOwner.Entity,
        RenderDrawKind.MoveGizmo or RenderDrawKind.RotateGizmo or RenderDrawKind.ScaleGizmo => DrawOwner.Gizmo,
        RenderDrawKind.ScaleIndicatorOverlay => DrawOwner.ScaleIndicatorOverlay,
        RenderDrawKind.NavigationGizmo => DrawOwner.NavigationGizmo,
        RenderDrawKind.EditorViewPlaneGrid => DrawOwner.EditorViewPlaneGrid,
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, "Unsupported RenderDrawKind")
    };

    internal static bool IsPipelineReady(DrawOwner owner, PipelineReadiness readiness) => owner switch
    {
        DrawOwner.EditorBackground => readiness.Sky,
        DrawOwner.EditorReferenceGrid => readiness.Grid,
        DrawOwner.WorldOrigin => readiness.Origin,
        DrawOwner.WorldAxes => readiness.Axes,
        DrawOwner.NavigationGizmo => readiness.Navigation,
        DrawOwner.ScaleIndicatorOverlay => readiness.ScaleIndicator,
        DrawOwner.EditorViewPlaneGrid => readiness.ViewPlaneGrid,
        DrawOwner.Terrain => readiness.Terrain,
        DrawOwner.VectorOverlay => readiness.VectorOverlay,
        DrawOwner.MapGround or DrawOwner.MapBounds or DrawOwner.Entity or DrawOwner.Gizmo => readiness.Main,
        _ => throw new ArgumentOutOfRangeException(nameof(owner), owner, "Unsupported draw owner")
    };

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
            TerrainStats = default;
            _terrainCache?.RetainOnly(_renderProjection.TerrainResources);
            _staticModels.RetainOnly(_renderProjection.Entities.Select(e => e.StaticModelKey));
            _vectorOverlays.RetainOnly(_renderProjection.VectorOverlayResources.Select(r => r.Key));
            foreach (var draw in RenderDrawPlan.GetFrameDrawPlan(_renderProjection))
            {
                var canRecord = CanRecordDraw(draw.Kind);
                TraceGridDraw(draw.Kind, canRecord);
                if (!canRecord) continue;
                BindFramePipeline(cb, draw.Kind);
                DispatchDraw(cb, pScene, draw);
            }
        }
    }

    void DispatchDraw(CommandBuffer cb, float* scene, RenderDrawPlan.FrameEntry draw)
    {
        switch (draw.Kind)
        {
            case RenderDrawKind.EditorBackground:
                DrawAssist(cb, scene, draw);
                break;
            case RenderDrawKind.EditorReferenceGrid:
                DrawReferenceGrid(cb);
                break;
            case RenderDrawKind.WorldOrigin:
                DrawWorldOrigin(cb);
                break;
            case RenderDrawKind.WorldAxes:
                DrawWorldAxes(cb);
                break;
            case RenderDrawKind.MapGround:
                if (_mapSurfaceIndexBuffer is not null) DrawMapSurface(cb, scene);
                break;
            case RenderDrawKind.MapBounds:
                if (_mapBoundsVertexBuffer is not null) DrawMapBounds(cb, scene);
                break;
            case RenderDrawKind.Terrain:
                DrawTerrain(cb, scene, draw.EntityIndex);
                break;
            case RenderDrawKind.MapVectorOverlay:
                DrawVectorOverlay(cb, scene, draw.EntityIndex);
                break;
            case RenderDrawKind.EntityFill:
            case RenderDrawKind.EntityOutline:
                DrawEntity(cb, scene, draw);
                break;
            case RenderDrawKind.MoveGizmo:
            case RenderDrawKind.RotateGizmo:
            case RenderDrawKind.ScaleGizmo:
                DrawGizmo(cb, scene, draw);
                break;
            case RenderDrawKind.ScaleIndicatorOverlay:
                DrawScaleIndicator(cb);
                break;
            case RenderDrawKind.NavigationGizmo:
                DrawNavigationGizmo(cb);
                break;
            case RenderDrawKind.EditorViewPlaneGrid:
                DrawViewPlaneGrid(cb);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(draw.Kind), draw.Kind, "Unsupported RenderDrawKind");
        }
    }

    bool CanRecordDraw(RenderDrawKind kind)
    {
        var owner = ResolveDrawOwner(kind);
        var readiness = new PipelineReadiness(
            Main: _pipeline.Handle != 0 && _pipelineLayout.Handle != 0,
            Sky: _skyPipeline.Handle != 0 && _skyPipelineLayout.Handle != 0,
            Grid: _gridPipeline.Handle != 0 && _gridPipelineLayout.Handle != 0,
            Origin: _originPipeline.Handle != 0 && _originPipelineLayout.Handle != 0,
            Axes: _axesPipeline.Handle != 0 && _axesPipelineLayout.Handle != 0,
            Navigation: _navGizmoPipeline.Handle != 0 && _navGizmoPipelineLayout.Handle != 0,
            ScaleIndicator: _scaleIndicatorPipeline.Handle != 0 && _scaleIndicatorPipelineLayout.Handle != 0,
            ViewPlaneGrid: _viewPlaneGridPipeline.Handle != 0 && _viewPlaneGridPipelineLayout.Handle != 0,
            Terrain: _terrainPipeline.Handle != 0 && _terrainPipelineLayout.Handle != 0,
            VectorOverlay: _vectorOverlayPipeline.Handle != 0 && _vectorOverlayPipelineLayout.Handle != 0 &&
                _vectorStrokePipeline.Handle != 0 && _vectorStrokePipelineLayout.Handle != 0);
        var ready = IsPipelineReady(owner, readiness);
        if (!ready) Log($"Vulkan Draw 不可记录：Kind={kind}; Owner={owner}");
        return ready;
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
