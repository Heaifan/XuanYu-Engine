namespace XuanYu.Render.Abstractions.Diagnostics.Performance;

public enum PerformanceFrameMode { Single, Double }
public enum PerformanceUnit { Nanoseconds, Milliseconds, Bytes, Count, Boolean }
public enum PerformanceSampleStatus
{
    Valid, Missing, Unsupported, Invalid, Timeout, Exception
}

public enum PerformanceMetric
{
    DemReadDecode, WorldTerrainBuild, LodSelection, MeshBuild,
    GpuResourceUploadApi, FenceWait, AcquireApi, CommandBufferRecord,
    QueueSubmitApi, QueuePresentApi, CpuFrameTotal, GpuRenderPass,
    SelectedNodeCount, SubmittedChunkCount, SubmittedTriangleCount,
    LodBudgetSaturated, CacheHit, CacheMiss, ResourceCreated,
    ResourceEvicted, ResourceRetired, UploadBytes, CacheResidentBytes,
    CacheHighWaterBytes, ProcessWorkingSetBytes, ProcessPrivateBytes,
    DriverVramBytes
}