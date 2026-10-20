namespace XuanYu.Render.Abstractions.Diagnostics;

public static class RendererEventCodes
{
    public const string SessionStarted = "RendererSessionStarted";
    public const string SessionClosed = "RendererSessionClosed";
    public const string FrameSubmitted = "FrameSubmitted";
    public const string FramePresented = "FramePresented";
    public const string QueueSubmitFailed = "QueueSubmitFailed";
    public const string QueuePresentFailed = "QueuePresentFailed";
    public const string QueuePresentOutOfDate = "QueuePresentOutOfDate";
    public const string DroppedEventsSummary = "DroppedEventsSummary";
    public const string ResizeStarted = "ResizeStarted";
    public const string ResizeCompleted = "ResizeCompleted";
    public const string ResizeSkipped = "ResizeSkipped";
    public const string ResizeFailed = "ResizeFailed";
    public const string PresentLoopStarted = "PresentLoopStarted";
    public const string PresentLoopStopped = "PresentLoopStopped";
    public const string AcquireOutOfDate = "AcquireOutOfDate";
    public const string AcquireSuboptimal = "AcquireSuboptimal";
    public const string PresentSuboptimal = "PresentSuboptimal";
    public const string PresentRecoveryRequested = "PresentRecoveryRequested";
    public const string VulkanOperationFailed = "VulkanOperationFailed";
    public const string PresentFatal = "PresentFatal";
    public const string SwapchainCreating = "SwapchainCreating";
    public const string SwapchainCreated = "SwapchainCreated";
    public const string SwapchainRecreated = "SwapchainRecreated";
    public const string SwapchainSkipped = "SwapchainSkipped";
    public const string SwapchainFailed = "SwapchainFailed";
    public const string SwapchainReleased = "SwapchainReleased";
}
