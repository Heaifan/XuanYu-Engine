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
        Assert.Contains("UpdateAsync", File.ReadAllText(Path.Combine(Root, "VulkanCompositionResources.cs")));
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
}
