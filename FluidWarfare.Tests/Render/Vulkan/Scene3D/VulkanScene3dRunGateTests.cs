using FluidWarfare.Render.Vulkan.Scene3D;

namespace FluidWarfare.Tests.Render.Vulkan.Scene3D;

public sealed class VulkanScene3dRunGateTests
{
    [Fact]
    public void Isolated_ShouldSetCanRunFalse()
    {
        var gate = VulkanScene3dRunGate.Isolated("测试隔离");
        Assert.False(gate.CanRun);
    }

    [Fact]
    public void Isolated_ShouldPreserveMessage()
    {
        var gate = VulkanScene3dRunGate.Isolated("测试隔离消息");
        Assert.Equal("测试隔离消息", gate.Message);
    }

    [Fact]
    public void Ready_ShouldSetCanRunTrue()
    {
        var gate = VulkanScene3dRunGate.Ready("测试就绪");
        Assert.True(gate.CanRun);
    }

    [Fact]
    public void Ready_ShouldPreserveMessage()
    {
        var gate = VulkanScene3dRunGate.Ready("测试就绪消息");
        Assert.Equal("测试就绪消息", gate.Message);
    }

    [Fact]
    public void Evaluate_WithoutEnv_ShouldReturnIsolated()
    {
        var saved = Environment.GetEnvironmentVariable("FW_ENABLE_SCENE3D");
        try
        {
            Environment.SetEnvironmentVariable("FW_ENABLE_SCENE3D", null);
            var gate = VulkanScene3dRunGate.Evaluate();
            Assert.False(gate.CanRun);
            Assert.Contains("未设置 FW_ENABLE_SCENE3D=1", gate.Message);
        }
        finally
        {
            Environment.SetEnvironmentVariable("FW_ENABLE_SCENE3D", saved);
        }
    }

    [Fact]
    public void Evaluate_WithEnvSet_ShouldStillReturnIsolated()
    {
        var saved = Environment.GetEnvironmentVariable("FW_ENABLE_SCENE3D");
        try
        {
            Environment.SetEnvironmentVariable("FW_ENABLE_SCENE3D", "1");
            var gate = VulkanScene3dRunGate.Evaluate();
            Assert.False(gate.CanRun);
            Assert.Contains("SPIR-V", gate.Message);
        }
        finally
        {
            Environment.SetEnvironmentVariable("FW_ENABLE_SCENE3D", saved);
        }
    }
}
