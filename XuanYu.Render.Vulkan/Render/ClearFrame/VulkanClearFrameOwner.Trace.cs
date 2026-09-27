using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    int _commandBufferGeneration;
    int _framebufferGeneration;
    bool _gridTraceLogged;
    bool _gridInDrawPlan;
    bool _gridCommandRecorded;
    bool _gridVkDrawIssued;
    bool _axisCommandRecorded;
    bool _originCommandRecorded;
    bool _gridViewProjectionFinite;
    bool _gridInverseViewProjectionFinite;
    bool _gridPlaneIntersectionValid;

    void ResetGridTrace()
    {
        _gridInDrawPlan = false;
        _gridCommandRecorded = false;
        _gridVkDrawIssued = false;
        _axisCommandRecorded = false;
        _originCommandRecorded = false;
    }

    void TraceGridDraw(RenderDrawKind kind, bool canRecord)
    {
        if (kind == RenderDrawKind.EditorReferenceGrid)
        {
            _gridInDrawPlan = true;
            _gridCommandRecorded |= canRecord;
        }
        if (kind == RenderDrawKind.WorldAxes)
        {
            _axisCommandRecorded |= canRecord;
        }
        if (kind == RenderDrawKind.WorldOrigin) _originCommandRecorded |= canRecord;
    }

    void TraceRecordCommands(int viewCount)
    {
        _recordCommandTraceCount++;
        var entityCount = _hasRenderProjection ? _renderProjection.EntityCount : 0;
        var allGridPipelinesReady = _gridPipeline.Handle != 0 && _gridPipelineLayout.Handle != 0 &&
            _axesPipeline.Handle != 0 && _axesPipelineLayout.Handle != 0 &&
            _originPipeline.Handle != 0 && _originPipelineLayout.Handle != 0;
        if (_gridTraceLogged || !_hasRenderProjection || !allGridPipelinesReady) return;
        _gridTraceLogged = true;
        _lastLoggedCommandEntityCount = entityCount;
        _lastLoggedCommandViewCount = viewCount;
        var time = DateTime.Now.ToString("HH:mm:ss");
        Console.Error.WriteLine(
            $"{time} 【调试】【命令缓冲】命令缓冲录制摘要；次数={_recordCommandTraceCount}；线程编号={Environment.CurrentManagedThreadId}；实体数={entityCount}；视图数={viewCount}");
        Console.Error.WriteLine(
            $"{time} 【取证】【WorldGrid】GridPipelineReady={_gridPipeline.Handle != 0 && _gridPipelineLayout.Handle != 0}; GridInDrawPlan={_gridInDrawPlan}; GridCommandRecorded={_gridCommandRecorded}; GridVkDrawIssued={_gridVkDrawIssued}; AxisPipelineReady={_axesPipeline.Handle != 0 && _axesPipelineLayout.Handle != 0}; AxisCommandRecorded={_axisCommandRecorded}; OriginPipelineReady={_originPipeline.Handle != 0 && _originPipelineLayout.Handle != 0}; OriginCommandRecorded={_originCommandRecorded}; CommandBufferGeneration={_commandBufferGeneration}; FramebufferGeneration={_framebufferGeneration}; SwapchainGeneration={_swapchainOwner.ResourceGeneration}");
        Console.Error.WriteLine(
            $"{time} 【取证】【WorldGridMath】Viewport={_extent.Width}x{_extent.Height}; CameraPosition={_renderProjection.Camera.Position}; ViewProjectionFinite={_gridViewProjectionFinite}; InverseViewProjectionFinite={_gridInverseViewProjectionFinite}; GridPlaneIntersection={(_gridPlaneIntersectionValid ? "VALID" : "INVALID")}; FineScale={_referenceGridLevels.FineSpacing}; CoarseScale={_referenceGridLevels.CoarseSpacing}; FineAlpha={_referenceGridLevels.FineWeight}; CoarseAlpha={_referenceGridLevels.CoarseWeight}; FinalAlpha=GPU_FRAGMENT_DERIVATIVE_DEPENDENT");
    }

    void TraceGridMath(bool viewProjectionFinite, bool inverseFinite, bool planeIntersectionValid)
    {
        _gridViewProjectionFinite = viewProjectionFinite;
        _gridInverseViewProjectionFinite = inverseFinite;
        _gridPlaneIntersectionValid = planeIntersectionValid;
    }
}
