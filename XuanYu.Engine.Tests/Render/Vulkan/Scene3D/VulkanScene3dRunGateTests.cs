using FluidWarfare.Render.Vulkan.Scene3D;
using FluidWarfare.Render.Vulkan.Shaders;

namespace FluidWarfare.Tests.Render.Vulkan.Scene3D;

[Collection("CompiledShadersState")]
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
    public void Evaluate_Default_ShouldReturnReady_WhenShadersValidated()
    {
        // 默认无 FW_DISABLE_SCENE3D + shader 已验证 → Ready
        CompiledShaders.Reset();
        CompiledShaders.Basic3dVert = new uint[] { 0x07230203 };
        CompiledShaders.Basic3dFrag = new uint[] { 0x07230203 };
        CompiledShaders.Basic3dVertexValidatedBySpirvVal = true;
        CompiledShaders.Basic3dFragmentValidatedBySpirvVal = true;

        var saved = Environment.GetEnvironmentVariable("FW_DISABLE_SCENE3D");
        try
        {
            Environment.SetEnvironmentVariable("FW_DISABLE_SCENE3D", null);
            var gate = VulkanScene3dRunGate.Evaluate();
            Assert.True(gate.CanRun);
        }
        finally
        {
            CompiledShaders.Reset();
            Environment.SetEnvironmentVariable("FW_DISABLE_SCENE3D", saved);
        }
    }

    [Fact]
    public void Evaluate_WithDisableVar_ShouldReturnIsolated()
    {
        var saved = Environment.GetEnvironmentVariable("FW_DISABLE_SCENE3D");
        try
        {
            Environment.SetEnvironmentVariable("FW_DISABLE_SCENE3D", "1");
            var gate = VulkanScene3dRunGate.Evaluate();
            Assert.False(gate.CanRun);
            Assert.Contains("FW_DISABLE_SCENE3D", gate.Message);
        }
        finally
        {
            Environment.SetEnvironmentVariable("FW_DISABLE_SCENE3D", saved);
        }
    }

    [Fact]
    public void Evaluate_ShouldReturnIsolated_WhenShadersMissing()
    {
        var saved = Environment.GetEnvironmentVariable("FW_DISABLE_SCENE3D");
        try
        {
            CompiledShaders.Reset();
            Environment.SetEnvironmentVariable("FW_DISABLE_SCENE3D", null);
            var gate = VulkanScene3dRunGate.Evaluate();
            Assert.False(gate.CanRun);
            Assert.Contains("SPIR-V 字节码缺失", gate.Message);
        }
        finally
        {
            CompiledShaders.Reset();
            Environment.SetEnvironmentVariable("FW_DISABLE_SCENE3D", saved);
        }
    }

    [Fact]
    public void Evaluate_WithShadersValidated_ShouldReturnReady()
    {
        var saved = Environment.GetEnvironmentVariable("FW_DISABLE_SCENE3D");
        try
        {
            CompiledShaders.Reset();
            CompiledShaders.Basic3dVert = new uint[] { 0x07230203 };
            CompiledShaders.Basic3dFrag = new uint[] { 0x07230203 };
            CompiledShaders.Basic3dVertexValidatedBySpirvVal = true;
            CompiledShaders.Basic3dFragmentValidatedBySpirvVal = true;

            Environment.SetEnvironmentVariable("FW_DISABLE_SCENE3D", null);
            var gate = VulkanScene3dRunGate.Evaluate();
            Assert.True(gate.CanRun);
            Assert.Contains("已就绪", gate.Message);
        }
        finally
        {
            CompiledShaders.Reset();
            Environment.SetEnvironmentVariable("FW_DISABLE_SCENE3D", saved);
        }
    }

    [Fact]
    public void Evaluate_ShouldPreserveReadyMessage()
    {
        CompiledShaders.Reset();
        CompiledShaders.Basic3dVert = new uint[] { 0x07230203 };
        CompiledShaders.Basic3dFrag = new uint[] { 0x07230203 };
        CompiledShaders.Basic3dVertexValidatedBySpirvVal = true;
        CompiledShaders.Basic3dFragmentValidatedBySpirvVal = true;

        var saved = Environment.GetEnvironmentVariable("FW_DISABLE_SCENE3D");
        try
        {
            Environment.SetEnvironmentVariable("FW_DISABLE_SCENE3D", null);
            var gate = VulkanScene3dRunGate.Evaluate();
            Assert.Contains("已就绪", gate.Message);
        }
        finally
        {
            CompiledShaders.Reset();
            Environment.SetEnvironmentVariable("FW_DISABLE_SCENE3D", saved);
        }
    }
}
