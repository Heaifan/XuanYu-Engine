using XuanYu.Render.Abstractions;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    readonly object _sceneLock = new();
    RenderProjectionResult _pendingRenderProjection;
    bool _renderProjectionPending;

    public void QueueRenderProjection(RenderProjectionResult projection)
    {
        lock (_sceneLock)
        {
            if (!_renderProjectionPending && projection.Success &&
                _hasRenderProjection && _renderProjection == projection.Projection) return;
            _pendingRenderProjection = projection;
            _renderProjectionPending = true;
        }
    }

    public bool TryApplyPendingRenderProjection()
    {
        RenderProjectionResult projection;
        lock (_sceneLock)
        {
            if (!_renderProjectionPending) return true;
            projection = _pendingRenderProjection;
            _renderProjectionPending = false;
        }
        if (!projection.Success)
        {
            ClearRenderProjection();
            Log(VulkanClearFrameLogFormatter.RenderProjectionSkipped(
                projection.FailureReason ?? "未知原因"));
            return true;
        }
        _renderProjection = projection.Projection;
        _hasRenderProjection = true;
        return _views.Length == 0 || RecordCommandBuffers(_views);
    }
}
