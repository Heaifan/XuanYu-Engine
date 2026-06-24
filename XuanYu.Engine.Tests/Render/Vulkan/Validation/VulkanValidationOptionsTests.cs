using XuanYu.Engine.Render.Vulkan.Validation;

namespace XuanYu.Engine.Tests.Render.Vulkan.Validation;

public sealed class VulkanValidationOptionsTests
{
    [Fact]
    public void FromEnvironment_ShouldReturnFalse_WhenVariableMissing()
    {
        var saved = Environment.GetEnvironmentVariable("FW_VULKAN_VALIDATION");
        try
        {
            Environment.SetEnvironmentVariable("FW_VULKAN_VALIDATION", null);
            var opts = VulkanValidationOptions.FromEnvironment();
            Assert.False(opts.IsRequested);
        }
        finally
        {
            Environment.SetEnvironmentVariable("FW_VULKAN_VALIDATION", saved);
        }
    }

    [Fact]
    public void FromEnvironment_ShouldReturnTrue_WhenVariableEqualsOne()
    {
        var saved = Environment.GetEnvironmentVariable("FW_VULKAN_VALIDATION");
        try
        {
            Environment.SetEnvironmentVariable("FW_VULKAN_VALIDATION", "1");
            var opts = VulkanValidationOptions.FromEnvironment();
            Assert.True(opts.IsRequested);
        }
        finally
        {
            Environment.SetEnvironmentVariable("FW_VULKAN_VALIDATION", saved);
        }
    }

    [Fact]
    public void FromEnvironment_ShouldReturnFalse_WhenVariableNotOne()
    {
        var saved = Environment.GetEnvironmentVariable("FW_VULKAN_VALIDATION");
        try
        {
            Environment.SetEnvironmentVariable("FW_VULKAN_VALIDATION", "0");
            var opts = VulkanValidationOptions.FromEnvironment();
            Assert.False(opts.IsRequested);
        }
        finally
        {
            Environment.SetEnvironmentVariable("FW_VULKAN_VALIDATION", saved);
        }
    }
}
