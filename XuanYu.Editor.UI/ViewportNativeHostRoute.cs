using XuanYu.Render.Vulkan;

namespace XuanYu.Editor.UI;

public static class ViewportNativeHostRoute
{
    public static void Report(UiVm? vm, NativeHostHandleSnapshot snapshot) =>
        vm?.LogNativeHostLifecycle(snapshot);
}
