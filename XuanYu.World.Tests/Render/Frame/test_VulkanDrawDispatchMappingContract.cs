using System.IO;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanDrawDispatchMappingContractTests
{
    [Fact]
    public void Dispatch_routes_through_the_shared_draw_owner_mapping()
    {
        var source = File.ReadAllText(Path.Combine(TestSourceRoot.Root, "XuanYu.Render.Vulkan", "Render",
            "Scene", "Dispatch", "VulkanClearFrameOwner.DrawDispatch.cs"));
        var dispatchStart = source.IndexOf("void DispatchDraw", StringComparison.Ordinal);
        var readinessStart = source.IndexOf("bool CanRecordDraw", dispatchStart, StringComparison.Ordinal);
        Assert.True(dispatchStart >= 0 && readinessStart > dispatchStart);
        var dispatch = source[dispatchStart..readinessStart];

        Assert.Contains("switch (ResolveDrawOwner(draw.Kind))", dispatch);
        Assert.DoesNotContain("switch (draw.Kind)", dispatch);
        Assert.DoesNotContain("case RenderDrawKind.", dispatch);
    }
}
