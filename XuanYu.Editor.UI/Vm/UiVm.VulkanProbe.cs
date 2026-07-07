using XuanYu.Render.Vulkan;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public void LogVulkanProbe(VulkanProbeResult result)
    {
        _logBus.Info(EditorLogSource.Render, EditorLogCategory.Backend, "【Vulkan探针】开始检测", "仅执行环境探测，不进入 Surface / Swapchain。");
        if (!result.Success)
        {
            _logBus.Error(EditorLogSource.Render, EditorLogCategory.Backend, "【Vulkan探针】运行结果：失败", $"【Vulkan探针】实例版本：{VulkanProbeLogFormatter.FormatVersion(result.InstanceVersion)}；【Vulkan探针】异常类型：{result.ErrorType}；【Vulkan探针】异常信息：{result.ErrorMessage}");
            RefreshLogBindings();
            return;
        }

        _logBus.Info(EditorLogSource.Render, EditorLogCategory.Backend, "【Vulkan探针】运行结果：成功", $"【Vulkan探针】实例版本：{VulkanProbeLogFormatter.FormatVersion(result.InstanceVersion)}；【Vulkan探针】物理设备数量：{result.Devices.Count}");
        foreach (var device in result.Devices)
            _logBus.Info(EditorLogSource.Render, EditorLogCategory.Backend, "【Vulkan设备】名称：" + device.Name, $"【Vulkan设备】类型：{VulkanProbeLogFormatter.FormatDeviceType(device.Type)}；【Vulkan设备】API版本：{VulkanProbeLogFormatter.FormatVersion(device.ApiVersion)}");
        RefreshLogBindings();
    }
}
