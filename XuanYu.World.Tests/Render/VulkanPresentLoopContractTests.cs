using System.IO;

namespace XuanYu.World.Tests.Render;

// VK-PERF-R1：Present 循环合同测试——防性能轮回归：
// 无投影受控等待 / 无逐帧日志 / 投影语义不变 / 模式日志只在创建重建 / 无新依赖。
public sealed class VulkanPresentLoopContractTests
{
    static string RenderVulkanFile(string path)
    {
        var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
        var full = Path.Combine(root, "XuanYu.Render.Vulkan", path);
        Assert.True(File.Exists(full), $"文件缺失：{full}");
        return File.ReadAllText(full);
    }

    [Fact]
    public void No_busy_loop_without_projection()
    {
        var loop = RenderVulkanFile(Path.Combine("Render", "Present", "VulkanPresentLoop.cs"));
        Assert.Contains("Thread.Sleep(16)", loop); // 无投影分支保持低频受控等待，禁止忙循环
    }

    [Fact]
    public void No_per_frame_log()
    {
        var loop = RenderVulkanFile(Path.Combine("Render", "Present", "VulkanPresentLoop.cs"));
        Assert.Contains("_firstPresentLogged", loop);   // 首帧日志一次性门控
        Assert.DoesNotContain("Debug.WriteLine", loop); // 禁逐帧诊断
        Assert.DoesNotContain("Console.WriteLine", loop);
    }

    [Fact]
    public void No_projection_semantic_change()
    {
        var loop = RenderVulkanFile(Path.Combine("Render", "Present", "VulkanPresentLoop.cs"));
        var owner = RenderVulkanFile(Path.Combine("Render", "ClearFrame", "VulkanClearFrameOwner.cs"));
        Assert.DoesNotContain("ClearRenderProjection", loop); // Present 循环不清理投影
        Assert.Contains("_hasRenderProjection = true;", owner); // Set 置真：持续状态语义保留
    }

    [Fact]
    public void Selected_mode_is_logged_once_at_create_or_recreate()
    {
        var caps = RenderVulkanFile(Path.Combine("Swapchain", "VulkanSwapchainCapabilities.cs"));
        var loop = RenderVulkanFile(Path.Combine("Render", "Present", "VulkanPresentLoop.cs"));
        Assert.Contains("呈现模式=", caps);   // 创建/重建时记录实际模式
        Assert.DoesNotContain("呈现模式=", loop); // 帧循环不重复记录
    }

    [Fact]
    public void No_new_dependency()
    {
        var csproj = RenderVulkanFile("XuanYu.Render.Vulkan.csproj");
        var refs = csproj.Split('\n').Count(l => l.Contains("PackageReference Include"));
        Assert.Equal(2, refs); // 仅 Silk.NET.Vulkan 与 KHR 扩展，无新增第三方依赖
    }
}
