using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport;

public sealed class RegionDrawingInputModifierTests
{
    [Fact]
    public void Native_mouse_modifier_is_independent_from_xbutton1()
    {
        var message = new NativePointerMessage(NativePointerMessage.Move, 0x0020, 0, 0, 0, 0, 0, 0);
        Assert.False(message.IsAltDown);
        Assert.True((message with { AltDown = true }).IsAltDown);
        Assert.False(new NativePointerMessage(NativePointerMessage.Move, 0, 0, 0, 0, 0, 0, 0).IsAltDown);
    }

    [Fact]
    public void Avalonia_and_native_hosts_forward_modifier_to_region_preview()
    {
        var root = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI");
        var pointer = File.ReadAllText(Path.Combine(root, "Viewport", "Vulkan", "VulkanNativeHost.AvaloniaPointer.cs"));
        var native = File.ReadAllText(Path.Combine(root, "Viewport", "Vulkan", "VulkanNativeHost.Pointer.cs"));
        Assert.Contains("KeyModifiers.Alt", pointer);
        Assert.Contains("message.IsAltDown", native);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Native_region_preview_preserves_alt_suppression(bool alt)
    {
        var message = new NativePointerMessage(NativePointerMessage.Move, 0, 0, 0, 0, 0, 0, 0, alt);
        Assert.Equal(alt, NativePointerRoutePolicy.ShouldSuppressRegionSnap(
            message, NativePointerRoute.RegionPreview));
    }
}
