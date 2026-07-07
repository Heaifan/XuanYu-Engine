using XuanYu.Render.Vulkan;

namespace XuanYu.Editor.UI;

public static class VulkanProbeRoute
{
    public static void Run(UiVm vm) => vm.LogVulkanProbe(VulkanApiProbe.Probe());
}
