using Xunit;

namespace XuanYu.Viewport.CompositionSpike.Tests;

public sealed class CompositionSpikeContractTests
{
    static string Root
    {
        get
        {
            var path = Directory.GetCurrentDirectory();
            while (!File.Exists(Path.Combine(path, "XuanYu.Engine.slnx"))) path = Directory.GetParent(path)!.FullName;
            return path;
        }
    }

    [Fact]
    public void Uses_gpu_composition_import_chain()
    {
        var text = File.ReadAllText(Path.Combine(Root, "CompositionSurfaceHost.cs"));
        Assert.Contains("CreateDrawingSurface", text);
        Assert.Contains("TryGetCompositionGpuInterop", text);
        Assert.Contains("SetElementChildVisual", text);
        Assert.Contains("ImportImage", File.ReadAllText(Path.Combine(Root, "VulkanCompositionResources.cs")));
        var resources = File.ReadAllText(Path.Combine(Root, "VulkanCompositionResources.cs"));
        Assert.Contains("UpdateWithSemaphoresAsync", resources);
        Assert.Contains("VulkanProperties", resources);
        Assert.Contains("TransferSrcOptimal", resources);
    }

    [Fact]
    public void Does_not_use_native_window_or_cpu_readback_workarounds()
    {
        var files = new[] { "CompositionSurfaceHost.cs", "VulkanCompositionResources.cs", "CompositionSpikeControl.axaml", "MainWindow.axaml" }
            .Select(x => Path.Combine(Root, x));
        var text = string.Join("\n", files.Select(File.ReadAllText));
        Assert.DoesNotContain("NativeControlHost", text);
        Assert.DoesNotContain("WS_CHILD", text);
        Assert.DoesNotContain("RenderTargetBitmap", text);
        Assert.DoesNotContain("Readback", text);
    }

    [Fact]
    public void Spike_forces_vulkan_backend_and_uses_1213()
    {
        var project = File.ReadAllText(Path.Combine(Root, "XuanYu.Viewport.CompositionSpike", "XuanYu.Viewport.CompositionSpike.csproj"));
        Assert.Contains("Version=\"12.1.3\"", project);
        Assert.DoesNotContain("12.0.4", project);
        var program = File.ReadAllText(Path.Combine(Root, "XuanYu.Viewport.CompositionSpike", "Program.cs"));
        Assert.Contains("Win32RenderingMode.Vulkan", program);
        Assert.Contains("VulkanOptions", program);
    }
}
