using System.Threading;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanPresentLoop
{
    int _state;
    int _fatalReported;
    string? _failureReason;

    public VulkanPresentLoopState State => (VulkanPresentLoopState)Volatile.Read(ref _state);
    public string? FailureReason => Volatile.Read(ref _failureReason);

    bool MarkFailed(string reason)
    {
        if (Interlocked.CompareExchange(ref _fatalReported, 1, 0) != 0) return false;
        Volatile.Write(ref _failureReason, reason);
        Volatile.Write(ref _state, (int)VulkanPresentLoopState.Failed);
        return true;
    }

    void CompleteWorker()
    {
        var failed = State == VulkanPresentLoopState.Failed;
        Volatile.Write(ref _state, (int)VulkanPresentLoopStatePolicy.AfterWorkerExit(failed));
    }
}
