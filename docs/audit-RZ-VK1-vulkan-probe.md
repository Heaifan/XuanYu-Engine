# RZ-VK1: Vulkan 依赖接入与环境探针审计

日期：2026-07-07

## 结论

本轮完成 Vulkan 最小依赖接入与环境探针，未进入真实渲染。

## 本轮目标

- 接入 `Silk.NET.Vulkan` 到独立渲染项目。
- 枚举 Vulkan Instance 版本。
- 枚举 PhysicalDevice。
- 输出中文诊断日志。
- 保持 UI 布局不变。
- 不创建 Surface、Swapchain、LogicalDevice、CommandPool、CommandBuffer。

## 文件清单

新增：

- `XuanYu.Render.Vulkan/XuanYu.Render.Vulkan.csproj`
- `XuanYu.Render.Vulkan/VulkanApiProbe.cs`
- `XuanYu.Render.Vulkan/VulkanDeviceInfo.cs`
- `XuanYu.Render.Vulkan/VulkanProbeLogFormatter.cs`
- `XuanYu.Render.Vulkan/VulkanProbeResult.cs`
- `XuanYu.Editor.UI/VulkanProbeRoute.cs`
- `XuanYu.Editor.UI/Vm/UiVm.VulkanProbe.cs`
- `docs/audit-RZ-VK1-vulkan-probe.md`

修改：

- `XuanYu.Editor.UI/XuanYu.Editor.UI.csproj`
- `XuanYu.Editor.UI/Bootstrap/App.axaml.cs`

## 架构落点

```text
Editor.UI -> Render.Vulkan -> Silk.NET.Vulkan
```

UI 只保留一个薄入口 `VulkanProbeRoute.Run(vm)`，探针本体放在 `XuanYu.Render.Vulkan`。

## 日志格式

输出中文字段：

- `【Vulkan探针】开始检测`
- `【Vulkan探针】运行结果：成功 / 失败`
- `【Vulkan探针】实例版本：x.x.x`
- `【Vulkan探针】物理设备数量：N`
- `【Vulkan设备】名称：...`
- `【Vulkan设备】类型：...`
- `【Vulkan设备】API版本：...`
- `【Vulkan探针】异常类型：...`
- `【Vulkan探针】异常信息：...`

## 验证范围

- 已接入 `Silk.NET.Vulkan`。
- 未接入 Surface。
- 未接入 Swapchain。
- 未接入 LogicalDevice。
- 未修改 UI 布局。
- 未修改输入逻辑。

## 下一步建议

进入 `RZ-VK2`，单独处理 Windows HWND / NativeHost 生命周期，再谈 Surface。
