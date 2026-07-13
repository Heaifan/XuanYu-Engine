# ARCH-A-Plan：Editor.UI Vulkan 直接依赖边界审计与迁移计划

版本：v0.2.15.1-rz  
日期：2026-07-13 20:27:01  
类型：规划文档

## 目标

本轮只确认 `Editor.UI` 直接依赖 `Render.Vulkan` / `Silk.NET.Vulkan` 的真实边界，并规划后续最小迁移顺序。不修改运行逻辑，不删除旧探针，不新增组合根项目。

## 审计结论

当前 `Editor.UI` 仍直接依赖 Vulkan 实现，属于 ARCH-A 债务，不能视为已收口。

活跃直接依赖清单：

- `XuanYu.Editor.UI/XuanYu.Editor.UI.csproj`：直接引用 `Silk.NET.Vulkan`、`Silk.NET.Vulkan.Extensions.KHR`、`XuanYu.Render.Vulkan`。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanSurfaceBridgeProvider.cs`：在 UI 项目内装配 `VulkanNativeHostSurfaceBridge`。
- `XuanYu.Editor.UI/Vm/UiVm.VulkanProbe.cs`：UI VM 直接调用 Vulkan probe。
- `XuanYu.Editor.UI/VulkanProbeRoute.cs`：UI route 直接触发 Vulkan probe。

历史直接依赖清单：

- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanClearSession*.cs`：旧探针死代码仍含 Silk.NET Vulkan using，本轮不删除。

`XuanYu.Render.Abstractions` 当前不引用 Silk.NET / Vulkan 实现。全文命中仅来自历史说明注释，不构成实际依赖。

## 迁移顺序

1. `v0.2.15.2-rz`：在 `Render.Abstractions` 建立现有 NativeHost 渲染生命周期所需的最小契约；Vulkan 实现开始适配该契约。
2. 后续独立轮次：把 UI 内的桥接装配挪出 `Editor.UI`，再移除 UI 项目对 `Render.Vulkan` / `Silk.NET.Vulkan` 的直接引用。
3. 后续独立轮次：清理或归档 `VulkanClearSession.*` 旧探针。

## R1 边界

`v0.2.15.2-rz` 只允许：

- 新增或扩展 `Render.Abstractions` 中的最小生命周期契约；
- 让 `Render.Vulkan` 实现该契约；
- 保持现有 UI 调用链可构建。

`v0.2.15.2-rz` 禁止：

- 删除 `Editor.UI` 的旧 Vulkan 调用链；
- 移除 `XuanYu.Editor.UI.csproj` 的 Vulkan / Silk 引用；
- 新增 `Editor.App` 或其他组合根项目；
- 改动渲染行为、Resize 行为、PresentLoop 行为。

## 架构图

```svg
<svg xmlns="http://www.w3.org/2000/svg" width="720" height="260" viewBox="0 0 720 260">
  <rect x="30" y="40" width="180" height="70" fill="#eef4ff" stroke="#335c9c"/>
  <text x="120" y="78" text-anchor="middle" font-size="15">Editor.UI</text>
  <text x="120" y="96" text-anchor="middle" font-size="12">temporary Vulkan debt</text>
  <rect x="270" y="40" width="180" height="70" fill="#f3fff0" stroke="#3f7d3a"/>
  <text x="360" y="78" text-anchor="middle" font-size="15">Render.Abstractions</text>
  <text x="360" y="96" text-anchor="middle" font-size="12">minimal lifecycle contract</text>
  <rect x="510" y="40" width="180" height="70" fill="#fff4ee" stroke="#9c5633"/>
  <text x="600" y="78" text-anchor="middle" font-size="15">Render.Vulkan</text>
  <text x="600" y="96" text-anchor="middle" font-size="12">Vulkan implementation</text>
  <line x1="210" y1="75" x2="270" y2="75" stroke="#333" marker-end="url(#a)"/>
  <line x1="510" y1="75" x2="450" y2="75" stroke="#333" marker-end="url(#a)"/>
  <line x1="120" y1="120" x2="600" y2="120" stroke="#b43" stroke-dasharray="6 5"/>
  <text x="360" y="145" text-anchor="middle" font-size="12" fill="#8a3928">current direct dependency to remove later</text>
  <defs><marker id="a" markerWidth="8" markerHeight="8" refX="7" refY="4" orient="auto"><path d="M0,0 L8,4 L0,8 Z" fill="#333"/></marker></defs>
</svg>
```

## 验收口径

- 本轮为纯文档计划。
- `changelog.md` 和 `file-tree.md` 必须同步。
- R1 进入代码前，必须保持本计划边界，不扩大到 UI 旧链路删除。
