using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Viewport;

public sealed class RegionDrawingInputModifierTests
{
    [Fact]
    public void Native_mouse_modifier_reads_alt_from_mk_alt_bit()
    {
        var message = new NativePointerMessage(NativePointerMessage.Move, 0x0020, 0, 0, 0, 0, 0, 0);
        Assert.True(message.IsAltDown);
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
}
