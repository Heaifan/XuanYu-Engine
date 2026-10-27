using XuanYu.Render.Vulkan.Render;
using System.Threading;
using XuanYu.Render.Abstractions.Diagnostics;
using XuanYu.Render.Vulkan.Render.Label;

namespace XuanYu.Render.Vulkan.Session;

public sealed partial class VulkanRenderSession
{
    bool FailResize(string reason, int width, int height)
    {
        _presentLoop.PublishLifecycle(RendererEventCodes.ResizeFailed, RendererEventLevel.Error,
            _swapchainOwner.ResourceGeneration, width, height,
            _swapchainOwner.Extent.Width, _swapchainOwner.Extent.Height, reason);
        _log?.Invoke(VulkanClearFrameLogFormatter.PresentError($"Resize 失败，RenderSession 将释放：{reason}"));
        _resizeStopping = false;
        MarkFailed($"Resize 失败：{reason}");
        TryDispose();
        return false;
    }

    bool RestartAfterResizeSkip(int width, int height)
    {
        _resizeStopping = false;
        _log?.Invoke(VulkanClearFrameLogFormatter.ResizeFastSkipped(_generation, width, height));
        return _presentLoop.Start() || FailResize("Present 泵重启失败", width, height);
    }

    void MarkFailed(string reason)
    {
        if (Interlocked.CompareExchange(ref _failureReason, reason, null) is not null) return;
        Volatile.Write(ref _failed, 1);
        _log?.Invoke(VulkanClearFrameLogFormatter.PresentFatal(reason));
    }

    public bool TryDispose()
    {
        if (_disposed) return true;
        if (_disposeBlocked) return false;
        if (!_presentLoop.Stop()) return false;
        try
        {
            _presentLoop.Dispose();
            _pipeline?.Dispose();
            _terrainPipeline?.Dispose();
            _clearFrame.DisposeTerrain();
            _vectorOverlayPipeline?.Dispose();
            _vectorStrokePipeline?.Dispose();
            _mapLabelPipeline?.Dispose();
            _skyPipeline?.Dispose();
            _gridPipelines?.Dispose();
            _clearFrame.Dispose();
        }
        catch (VulkanLabelUploadCompletionUnknownException error)
        {
            _disposeBlocked = true;
            _log?.Invoke($"Vulkan 资源释放阻断，保留 Device 以避免释放 GPU 在途资源：{error.Message}");
            return false;
        }
        _disposed = true;
        _log?.Invoke(VulkanClearFrameLogFormatter.SessionDisposed());
        return true;
    }

    public void Dispose() => TryDispose();
}
