using Silk.NET.Vulkan;
using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    internal enum DrawOwner { EditorBackground, EditorReferenceGrid, WorldOrigin, WorldAxes,
        MapGround, MapBounds, Terrain, VectorOverlay, Entity, Gizmo, ScaleIndicatorOverlay,
        NavigationGizmo, EditorViewPlaneGrid }

    internal readonly record struct PipelineReadiness(bool Main, bool Sky, bool Grid, bool Origin,
        bool Axes, bool Navigation, bool ScaleIndicator, bool ViewPlaneGrid, bool Terrain,
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

    internal static bool IsPipelineReady(DrawOwner owner, PipelineReadiness r) => owner switch
    {
        DrawOwner.EditorBackground => r.Sky, DrawOwner.EditorReferenceGrid => r.Grid,
        DrawOwner.WorldOrigin => r.Origin, DrawOwner.WorldAxes => r.Axes,
        DrawOwner.NavigationGizmo => r.Navigation, DrawOwner.ScaleIndicatorOverlay => r.ScaleIndicator,
        DrawOwner.EditorViewPlaneGrid => r.ViewPlaneGrid, DrawOwner.Terrain => r.Terrain,
        DrawOwner.VectorOverlay => r.VectorOverlay,
        DrawOwner.MapGround or DrawOwner.MapBounds or DrawOwner.Entity or DrawOwner.Gizmo => r.Main,
        _ => throw new ArgumentOutOfRangeException(nameof(owner), owner, "Unsupported draw owner")
    };

    void DispatchDraw(CommandBuffer cb, float* scene, RenderDrawPlan.FrameEntry draw)
    {
        switch (draw.Kind)
        {
            case RenderDrawKind.EditorBackground: DrawAssist(cb, scene, draw); break;
            case RenderDrawKind.EditorReferenceGrid: DrawReferenceGrid(cb); break;
            case RenderDrawKind.WorldOrigin: DrawWorldOrigin(cb); break;
            case RenderDrawKind.WorldAxes: DrawWorldAxes(cb); break;
            case RenderDrawKind.MapGround: DrawMapSurface(cb, scene); break;
            case RenderDrawKind.MapBounds: DrawMapBounds(cb, scene); break;
            case RenderDrawKind.Terrain: DrawTerrain(cb, scene, draw.EntityIndex); break;
            case RenderDrawKind.MapVectorOverlay: DrawVectorOverlay(cb, scene, draw.EntityIndex); break;
            case RenderDrawKind.EntityFill or RenderDrawKind.EntityOutline: DrawEntity(cb, scene, draw); break;
            case RenderDrawKind.MoveGizmo or RenderDrawKind.RotateGizmo or RenderDrawKind.ScaleGizmo: DrawGizmo(cb, scene, draw); break;
            case RenderDrawKind.ScaleIndicatorOverlay: DrawScaleIndicator(cb); break;
            case RenderDrawKind.NavigationGizmo: DrawNavigationGizmo(cb); break;
            case RenderDrawKind.EditorViewPlaneGrid: DrawViewPlaneGrid(cb); break;
            default: throw new ArgumentOutOfRangeException(nameof(draw.Kind), draw.Kind, "Unsupported RenderDrawKind");
        }
    }

    bool CanRecordDraw(RenderDrawKind kind)
    {
        var r = new PipelineReadiness(_pipeline.Handle != 0 && _pipelineLayout.Handle != 0,
            _skyPipeline.Handle != 0 && _skyPipelineLayout.Handle != 0,
            _gridPipeline.Handle != 0 && _gridPipelineLayout.Handle != 0,
            _originPipeline.Handle != 0 && _originPipelineLayout.Handle != 0,
            _axesPipeline.Handle != 0 && _axesPipelineLayout.Handle != 0,
            _navGizmoPipeline.Handle != 0 && _navGizmoPipelineLayout.Handle != 0,
            _scaleIndicatorPipeline.Handle != 0 && _scaleIndicatorPipelineLayout.Handle != 0,
            _viewPlaneGridPipeline.Handle != 0 && _viewPlaneGridPipelineLayout.Handle != 0,
            _terrainPipeline.Handle != 0 && _terrainPipelineLayout.Handle != 0,
            _vectorOverlayPipeline.Handle != 0 && _vectorOverlayPipelineLayout.Handle != 0 &&
            _vectorStrokePipeline.Handle != 0 && _vectorStrokePipelineLayout.Handle != 0);
        return IsPipelineReady(ResolveDrawOwner(kind), r);
    }
}
