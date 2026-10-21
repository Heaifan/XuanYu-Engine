namespace XuanYu.Render.Abstractions.Diagnostics;

public sealed partial class RendererEventSession
{
    public void PublishLifecycleEvent(string code, RendererEventLevel level, uint? generation = null,
        int? requestedWidth = null, int? requestedHeight = null, uint? actualWidth = null,
        uint? actualHeight = null, int? imageViewCount = null, string? vulkanResult = null,
        string? reason = null) => _runtime.TryWrite(NewEvent(code, RendererEventChannel.Runtime, level,
            generation: generation, requestedWidth: requestedWidth, requestedHeight: requestedHeight,
            actualWidth: actualWidth, actualHeight: actualHeight, imageViewCount: imageViewCount,
            vulkanResult: vulkanResult, reason: reason));

    public void PublishTerrainFrame(string code, long frameId, uint imageIndex,
        long cameraRevision, string? terrainId = null, int? revision = null, string? reason = null)
    {
        if (_probe is null) return;
        var item = NewEvent(code, RendererEventChannel.Probe, RendererEventLevel.Info,
            frameId, imageIndex, cameraRevision) with
        { TerrainId = terrainId, TerrainRevision = revision, Reason = reason };
        _probe.TryWrite(item);
    }
}
