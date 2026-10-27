using System.Text;
using XuanYu.Render.Vulkan;

namespace XuanYu.World.Tests.Render.DeviceContracts;

public sealed class VulkanDeviceContractConsistencyTests
{
    [Fact]
    public void Instance_result_rejects_success_without_owner()
    {
        Assert.Throws<ArgumentException>(() => new VulkanInstanceResult(true, null, 0));
        Assert.False(new VulkanInstanceResult(false, null, 0).Success);
    }

    [Fact]
    public void Surface_result_rejects_success_without_owner()
    {
        Assert.Throws<ArgumentException>(() => new VulkanSurfaceResult(true, null));
        Assert.False(new VulkanSurfaceResult(false, null).Success);
    }

    [Fact]
    public void Instance_extension_names_are_plain_and_encoded_with_one_terminator()
    {
        var names = new[]
        {
            VulkanInstanceExtensions.Surface,
            VulkanInstanceExtensions.Win32Surface,
            VulkanInstanceExtensions.Properties2,
            VulkanInstanceExtensions.SurfaceMaintenance1,
            VulkanInstanceExtensions.SurfaceCapabilities2
        };

        foreach (var name in names)
        {
            Assert.DoesNotContain('\0', name);
            var bytes = VulkanExtensionNameEncoder.Encode(name);
            Assert.Equal(name + "\0", Encoding.UTF8.GetString(bytes));
            Assert.Equal(1, bytes.Count(value => value == 0));
        }
    }

    [Fact]
    public void Extension_encoder_rejects_embedded_terminator()
    {
        Assert.Throws<ArgumentException>(() => VulkanExtensionNameEncoder.Encode("VK_EXT_example\0suffix"));
    }
}
