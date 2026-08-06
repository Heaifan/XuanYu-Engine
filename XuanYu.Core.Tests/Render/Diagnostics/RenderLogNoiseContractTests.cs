using System.IO;

namespace XuanYu.Core.Tests.Render.Diagnostics;

public sealed class RenderLogNoiseContractTests
{
    static string Root => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
    static string Read(string path) => File.ReadAllText(Path.Combine(Root, path));

    [Fact]
    public void Map_no_rebuild_has_no_user_log()
    {
        var source = Read("XuanYu.Render.Vulkan/Render/Map/VulkanClearFrameOwner.MapSurface.cs");
        Assert.DoesNotContain("资源键已变化=否", source);
    }

    [Fact]
    public void Periodic_traces_do_not_emit_every_hundredth_cycle()
    {
        var command = Read("XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameOwner.Trace.cs");
        var snapshot = Read("XuanYu.Editor.UI/Vm/Scene/UiVm.Scene.cs");
        Assert.DoesNotContain("% 100", command);
        Assert.DoesNotContain("% 100", snapshot);
        Assert.Contains("_recordCommandTraceCount != 1", command);
        Assert.Contains("_renderSnapshotPublishCount != 1", snapshot);
    }
}
