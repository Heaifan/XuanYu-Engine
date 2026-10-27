using System.IO;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanDeviceCreationSafetyTests
{
    [Fact]
    public void Instance_creation_cleans_up_a_native_handle_if_owner_delivery_fails()
    {
        var source = ReadDeviceFile("Device", "Instance", "Create", "VulkanInstanceOwner.cs");
        var create = source.IndexOf("vk.CreateInstance", StringComparison.Ordinal);
        var rollback = source.IndexOf("vk.DestroyInstance(created, null)", create, StringComparison.Ordinal);
        var catcher = source.IndexOf("catch (Exception ex)", create, StringComparison.Ordinal);

        Assert.True(create >= 0 && catcher > create && rollback > catcher,
            "A created VkInstance must be destroyed if result/owner delivery throws.");
    }

    [Fact]
    public void Surface_creation_cleans_up_a_native_handle_if_owner_delivery_fails()
    {
        var source = ReadDeviceFile("Device", "Surface", "VulkanSurfaceOwner.cs");
        var create = source.IndexOf("CreateWin32Surface", StringComparison.Ordinal);
        var catcher = source.IndexOf("catch (Exception ex)", create, StringComparison.Ordinal);
        var rollback = source.IndexOf("DestroySurface(instance, surface", catcher, StringComparison.Ordinal);

        Assert.True(create >= 0 && catcher > create && rollback > catcher,
            "A created VkSurfaceKHR must be destroyed if result/owner delivery throws.");
    }

    [Fact]
    public void Physical_device_enumeration_reports_vk_result_failures()
    {
        var enumeration = ReadDeviceFile("Device", "Physical", "VulkanPhysicalDeviceQuery.cs");
        var queueQuery = ReadDeviceFile("Device", "Queue", "VulkanQueueFamilySelector.cs");

        Assert.Equal(2, CountOccurrences(enumeration, "vk.EnumeratePhysicalDevices"));
        Assert.Equal(2, CountOccurrences(enumeration, "Result.Success"));
        Assert.Contains("GetPhysicalDeviceSurfaceSupport", queueQuery, StringComparison.Ordinal);
        Assert.Contains("Result.Success", queueQuery, StringComparison.Ordinal);
    }

    static string ReadDeviceFile(params string[] path)
    {
        var segments = new[] { Path.GetFullPath(TestSourceRoot.Root), "XuanYu.Render.Vulkan" }
            .Concat(path).ToArray();
        var fullPath = Path.Combine(segments);
        Assert.True(File.Exists(fullPath), $"文件缺失：{fullPath}");
        return File.ReadAllText(fullPath);
    }

    static int CountOccurrences(string source, string value) =>
        (source.Length - source.Replace(value, string.Empty, StringComparison.Ordinal).Length) / value.Length;
}
