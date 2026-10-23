using System.Reflection;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.Terrain.Offscreen;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanOffscreenTerrainRendererConstructorContractTests
{
    [Fact]
    public void Legacy_constructor_signature_remains_available()
    {
        var signature = new[]
        {
            typeof(RenderProjection), typeof(uint), typeof(uint), typeof(Action<string>)
        };

        Assert.NotNull(typeof(VulkanOffscreenTerrainRenderer).GetConstructor(signature));
    }

    [Fact]
    public void Cache_budget_constructors_remain_available()
    {
        var budgetOnly = new[] { typeof(RenderProjection), typeof(uint), typeof(uint), typeof(ulong) };
        var withLog = new[]
        {
            typeof(RenderProjection), typeof(uint), typeof(uint), typeof(Action<string>), typeof(ulong)
        };

        Assert.NotNull(typeof(VulkanOffscreenTerrainRenderer).GetConstructor(budgetOnly));
        Assert.NotNull(typeof(VulkanOffscreenTerrainRenderer).GetConstructor(withLog));
    }
}
