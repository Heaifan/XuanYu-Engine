using Xunit;

namespace XuanYu.Core.Tests.Viewport;

public sealed class CompositionSpikeContractTests
{
    [Fact]
    public void Spike_declares_native_host_overlay_and_no_popup()
    {
        var root = LocateRepositoryRoot();
        var xaml = File.ReadAllText(Path.Combine(root,
            "XuanYu.Editor.UI", "Viewport", "CompositionSpike", "CompositionSpikeView.axaml"));

        Assert.Contains("VulkanNativeHost", xaml);
        Assert.Contains("Button", xaml);
        Assert.Contains("TextBlock", xaml);
        Assert.Contains("Border", xaml);
        Assert.DoesNotContain("Popup", xaml, StringComparison.Ordinal);
        Assert.DoesNotContain("Window", xaml, StringComparison.Ordinal);
    }

    [Fact]
    public void Spike_result_is_explicitly_partial_when_native_child_host_is_used()
    {
        var root = LocateRepositoryRoot();
        var code = File.ReadAllText(Path.Combine(root,
            "XuanYu.Editor.UI", "Viewport", "CompositionSpike", "CompositionSpikeResult.cs"));

        Assert.Contains("Partial", code);
        Assert.Contains("WS_CHILD", File.ReadAllText(Path.Combine(root,
            "XuanYu.Editor.UI", "Viewport", "Vulkan", "Win32ViewportHost.cs")));
    }

    static string LocateRepositoryRoot()
    {
        var directory = AppContext.BaseDirectory;
        while (directory is not null && !File.Exists(Path.Combine(directory, "XuanYu.Engine.slnx")))
            directory = Directory.GetParent(directory)?.FullName;
        return directory ?? throw new DirectoryNotFoundException("仓库根目录未找到");
    }
}
