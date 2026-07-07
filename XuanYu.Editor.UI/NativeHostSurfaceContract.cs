using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan;

namespace XuanYu.Editor.UI;

// VK3-A：把现有 NativeHost 生命周期快照映射为渲染层交接句柄。
// 只搬运 HWND / 尺寸 / Hinstance，不引入任何 Vulkan 实现。
// 真正的 Attach/Detach 接线在 VK3-C 由组合根完成。
public static class NativeHostSurfaceContract
{
    public static NativeHostSurfaceHandle ToSurfaceHandle(NativeHostHandleSnapshot snap) =>
        new(snap.Hwnd, Win32ViewportHost.ModuleHandle, snap.Width, snap.Height, snap.DpiScale);
}
