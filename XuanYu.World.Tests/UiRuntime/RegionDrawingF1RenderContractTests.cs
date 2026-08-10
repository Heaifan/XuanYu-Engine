using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Render.Vulkan.Render.VectorOverlay;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class RegionDrawingF1RenderContractTests
{
    [Fact]
    public void R17_first_hit_draft_has_no_empty_primitive_and_passes_vulkan_validator()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.SelectToolCommand.Execute("区域绘制");
        var viewport = new ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, viewport);
        var hit = FindHit(vm, projection);

        vm.RegionDrawingPointerPressed(hit.X, hit.Y, viewport);
        var resource = vm.RenderProjection.Projection!.VectorOverlayResources.Single();

        Assert.All(resource.Primitives, x => Assert.True(x.IndexCount > 0));
        Assert.True(VulkanVectorOverlayValidator.Validate(resource, out var error), error);
    }

    static (double X, double Y) FindHit(UiVm vm, ViewProjectionState projection)
    {
        foreach (var x in Enumerable.Range(0, 17).Select(i => i * 50.0))
        foreach (var y in Enumerable.Range(0, 13).Select(i => i * 50.0))
            if (MapSurfacePicker.TryPick(vm.MapSession.CurrentMap, projection, x, y, out _))
                return (x, y);
        throw new InvalidOperationException("未找到测试用地面命中点。");
    }
}
