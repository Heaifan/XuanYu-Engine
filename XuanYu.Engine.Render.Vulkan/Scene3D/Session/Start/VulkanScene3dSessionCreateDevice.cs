using System.Runtime.InteropServices;
using Silk.NET.Vulkan;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.Session;

/// <summary>
/// PhysicalDevice 选择 + LogicalDevice 创建。
/// </summary>
unsafe partial class VulkanScene3dSession
{
    private bool SelectDevice()
    {
        uint count = 0;
        if (_vk!.EnumeratePhysicalDevices(_instance, ref count, null) != Result.Success || count == 0)
            return false;

        var devices = new Silk.NET.Vulkan.PhysicalDevice[count];
        fixed (Silk.NET.Vulkan.PhysicalDevice* p = devices)
            _vk.EnumeratePhysicalDevices(_instance, ref count, p);

        var fnSupport = LoadProc("vkGetPhysicalDeviceSurfaceSupportKHR");
        if (fnSupport == 0) return false;
        var supportFn = Marshal.GetDelegateForFunctionPointer<SurfaceSupportFn>(fnSupport);

        foreach (var d in devices)
        {
            uint qc = 0;
            _vk.GetPhysicalDeviceQueueFamilyProperties(d, ref qc, null);
            var qProps = new QueueFamilyProperties[qc];
            fixed (QueueFamilyProperties* qp = qProps)
                _vk.GetPhysicalDeviceQueueFamilyProperties(d, ref qc, qp);

            for (uint i = 0; i < qc; i++)
            {
                if (qProps[i].QueueCount > 0 && qProps[i].QueueFlags.HasFlag(QueueFlags.GraphicsBit))
                {
                    int supported = 0;
                    supportFn(d, i, _surface, &supported);
                    if (supported != 0)
                    {
                        _physicalDevice = d;
                        _queueIndex = i;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool CreateDevice()
    {
        var qp = 1.0f;
        var qci = new DeviceQueueCreateInfo
        {
            SType = StructureType.DeviceQueueCreateInfo,
            QueueFamilyIndex = _queueIndex,
            QueueCount = 1,
            PQueuePriorities = &qp
        };
        var se = Marshal.StringToHGlobalAnsi("VK_KHR_swapchain");
        try
        {
            var exts = stackalloc byte*[] { (byte*)se };
            var dci = new DeviceCreateInfo
            {
                SType = StructureType.DeviceCreateInfo,
                QueueCreateInfoCount = 1,
                PQueueCreateInfos = &qci,
                EnabledExtensionCount = 1,
                PpEnabledExtensionNames = exts
            };
            var result = _vk!.CreateDevice(_physicalDevice, &dci, null, out _device);
            if (result == Result.Success)
            {
                _devOk = true;
                _deviceCreateCount++;
            }
            return result == Result.Success;
        }
        finally { Marshal.FreeHGlobal(se); }
    }
}
