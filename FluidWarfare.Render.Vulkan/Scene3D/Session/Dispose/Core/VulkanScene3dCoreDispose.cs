using System.Runtime.InteropServices;

namespace FluidWarfare.Render.Vulkan.Scene3D.Session;

/// <summary>
/// Device / Surface / DebugMessenger / Instance 释放步骤。
/// 顺序要求：Device → Surface → DebugMessenger → Instance（Instance 最后）。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    private void DisposeDeviceStep()
    {
        if (_devOk && _device.Handle != 0)
            _vk!.DestroyDevice(_device, null);
    }

    private void DisposeSurfaceStep()
    {
        if (_surfOk && _fnDestroySurface != 0)
        {
            var fn = Marshal.GetDelegateForFunctionPointer<DestroySurfaceFn>(_fnDestroySurface);
            fn(_instance, _surface, null);
        }
    }

    private void DisposeDebugMessengerStep()
    {
        if (_debugMessengerScope is not null)
        {
            _debugMessengerScope.Dispose();
            _debugMessengerScope = null;
        }
    }

    private void DisposeInstanceStep()
    {
        if (_instOk)
            _vk!.DestroyInstance(_instance, null);
    }
}
