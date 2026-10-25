namespace XuanYu.Render.Abstractions.Diagnostics;

public sealed partial class RendererEventSession
{
    public void PublishFrameEvent(string code, RendererEventLevel level, long frameId,
        uint imageIndex, long cameraRevision, int? frameSlot = null,
        int? framesInFlight = null, uint? swapchainGeneration = null)
    {
        var item = NewEvent(code, RendererEventChannel.Runtime, level, frameId, imageIndex, cameraRevision)
            with { FrameSlot = frameSlot, FramesInFlight = framesInFlight,
                SwapchainGeneration = swapchainGeneration };
        if (code is RendererEventCodes.FrameSubmitted or RendererEventCodes.FramePresented)
        {
            _probe?.TryWrite(item with { Channel = RendererEventChannel.Probe });
            return;
        }
        _runtime.TryWrite(item);
        if (_probe is not null) _probe.TryWrite(item with { Channel = RendererEventChannel.Probe });
    }
}
