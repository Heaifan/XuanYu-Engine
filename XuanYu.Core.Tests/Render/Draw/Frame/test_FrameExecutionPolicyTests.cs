using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

// R4-R3-R2：验证 Vulkan Present 循环帧执行顺序：
// WaitFence → ApplyPendingProjection → ResetFence → QueueSubmit。
// 禁止 Fence 等待前重录 CommandBuffer，禁止 Reset 后跳过提交。
public sealed class FrameExecutionPolicyTests
{
    private static int StepIndex(IReadOnlyList<FrameExecutionPolicy.FrameStep> order, FrameExecutionPolicy.FrameStep step)
    {
        for (var i = 0; i < order.Count; i++)
            if (order[i] == step) return i;
        return -1;
    }

    [Fact]
    public void Frame_order_is_WaitFence_before_ApplyPendingProjection()
    {
        var order = FrameExecutionPolicy.Order;

        var waitIdx = StepIndex(order, FrameExecutionPolicy.FrameStep.WaitFence);
        var applyIdx = StepIndex(order, FrameExecutionPolicy.FrameStep.ApplyPendingProjection);
        Assert.True(waitIdx < applyIdx,
            $"WaitFence({waitIdx}) must be before ApplyPendingProjection({applyIdx})");
    }

    [Fact]
    public void ApplyPendingProjection_is_before_ResetFence()
    {
        var order = FrameExecutionPolicy.Order;

        var applyIdx = StepIndex(order, FrameExecutionPolicy.FrameStep.ApplyPendingProjection);
        var resetIdx = StepIndex(order, FrameExecutionPolicy.FrameStep.ResetFence);
        Assert.True(applyIdx < resetIdx,
            $"ApplyPendingProjection({applyIdx}) must be before ResetFence({resetIdx})");
    }

    [Fact]
    public void ResetFence_is_before_QueueSubmit()
    {
        var order = FrameExecutionPolicy.Order;

        var resetIdx = StepIndex(order, FrameExecutionPolicy.FrameStep.ResetFence);
        var submitIdx = StepIndex(order, FrameExecutionPolicy.FrameStep.QueueSubmit);
        Assert.True(resetIdx < submitIdx,
            $"ResetFence({resetIdx}) must be before QueueSubmit({submitIdx})");
    }

    [Fact]
    public void Policy_has_exactly_four_steps()
    {
        var order = FrameExecutionPolicy.Order;

        Assert.Equal(4, order.Count);
    }

    [Fact]
    public void Policy_contains_all_required_steps()
    {
        var order = FrameExecutionPolicy.Order;

        Assert.Contains(FrameExecutionPolicy.FrameStep.WaitFence, order);
        Assert.Contains(FrameExecutionPolicy.FrameStep.ApplyPendingProjection, order);
        Assert.Contains(FrameExecutionPolicy.FrameStep.ResetFence, order);
        Assert.Contains(FrameExecutionPolicy.FrameStep.QueueSubmit, order);
    }

    [Fact]
    public void ResetFence_is_not_before_ApplyPendingProjection()
    {
        var order = FrameExecutionPolicy.Order;

        var applyIdx = StepIndex(order, FrameExecutionPolicy.FrameStep.ApplyPendingProjection);
        var resetIdx = StepIndex(order, FrameExecutionPolicy.FrameStep.ResetFence);
        // 旧错误顺序：ApplyPendingProjection 在 ResetFence 之后
        // 正确顺序：ApplyPendingProjection 在 ResetFence 之前
        Assert.False(resetIdx < applyIdx,
            "ResetFence must NOT be before ApplyPendingProjection (old bug)");
    }
}