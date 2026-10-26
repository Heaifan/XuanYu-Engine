using System;
using Silk.NET.Vulkan;

namespace XuanYu.Render.Vulkan.Render;

internal enum AcquireDecision
{
    Acquired,
    RetryAfterRecreate,
    TimeoutRetry,
    StopOrFatal
}

internal static class VulkanAcquireResultPolicy
{
    internal const int MaxRecoveryRetries = 5;

    internal static bool CanRetryRecovery(int retryCount) => retryCount < MaxRecoveryRetries;

    internal static AcquireDecision Decide(Result result, Func<bool>? recover)
    {
        if (result == Result.Success || result == Result.SuboptimalKhr)
            return AcquireDecision.Acquired;
        if (result == Result.Timeout)
            return AcquireDecision.TimeoutRetry;
        if (result == Result.ErrorOutOfDateKhr)
            return recover?.Invoke() == false ? AcquireDecision.StopOrFatal : AcquireDecision.RetryAfterRecreate;
        return AcquireDecision.StopOrFatal;
    }

    internal static bool HasValidImageIndex(uint index, int imageCount, bool acquiredCurrentSwapchain) =>
        acquiredCurrentSwapchain && index < imageCount;
}
