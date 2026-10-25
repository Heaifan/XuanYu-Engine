namespace XuanYu.Render.Abstractions;

// R4-R3-R2：Vulkan Present 循环帧执行顺序策略，供 VulkanPresentLoop 实现与测试共同使用。
// 顺序：WaitFence → ApplyPendingProjection → ResetFence → QueueSubmit。
// 禁止 Fence 等待前重录 CommandBuffer，禁止 Reset 后跳过提交。
public static class FrameExecutionPolicy
{
    public enum FrameStep : byte
    {
        WaitFence,
        ApplyPendingProjection,
        ResetFence,
        QueueSubmit
    }

    public static IReadOnlyList<FrameStep> Order => new[]
    {
        FrameStep.WaitFence,
        FrameStep.ApplyPendingProjection,
        FrameStep.ResetFence,
        FrameStep.QueueSubmit
    };
}