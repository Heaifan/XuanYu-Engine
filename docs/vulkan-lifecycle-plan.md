# RZ-VK0: Vulkan 生命周期与架构边界方案

日期：2026-07-07

## 1. 总目标

RZ-VK0 只制定规则，不接入新的 Vulkan 代码。

本阶段目标是为重构后的玄域引擎建立 Vulkan 重新接入的架构边界、生命周期规则、性能规则和后续里程碑，避免后续再次出现以下问题：

- 鼠标拖动导致窗口卡死。
- PointerMove 高频写日志。
- Inspector、Diagnostics、LogPanel 被高频刷新。
- Vulkan 生命周期和 UI 生命周期混在一起。
- Surface、Swapchain、CommandBuffer 销毁顺序混乱。
- 旧探针代码直接变成正式渲染路径。
- 文件只是为了压行数拆分，但职责边界仍然不清。

本阶段不要求视口显示 Vulkan 画面，不要求创建 Vulkan Instance、Device、Surface 或 Swapchain。

## 2. 架构分层

目标分层如下：

```text
XuanYu.Core
  基础数据、数学类型、WorldState、Entity 等核心模型。

XuanYu.Render.Abstractions
  渲染接口、渲染请求、渲染快照、Picking 输入输出类型。
  不包含 Vulkan 具体实现。

XuanYu.Render.Vulkan
  Vulkan Runtime、Device、Surface、Swapchain、CommandBuffer、Frame 渲染实现。

XuanYu.Editor.UI
  Avalonia UI、面板、按钮、Inspector、LogPanel、Diagnostics 等编辑器界面。

XuanYu.Editor.Win
  Windows 平台宿主，负责 HWND、NativeHost、窗口生命周期与 Vulkan 绑定入口。
```

职责边界：

- `Core` 只表达引擎状态和基础模型。
- `Render.Abstractions` 只定义渲染世界如何被消费。
- `Render.Vulkan` 只实现 Vulkan 后端。
- `Editor.UI` 只负责 Avalonia 界面和用户交互。
- `Editor.Win` 只负责 Windows 原生宿主和平台桥接。

## 3. 依赖方向

允许依赖方向：

```text
XuanYu.Editor.UI
  -> XuanYu.Core
  -> XuanYu.Render.Abstractions

XuanYu.Editor.Win
  -> XuanYu.Editor.UI
  -> XuanYu.Render.Abstractions
  -> XuanYu.Render.Vulkan

XuanYu.Render.Vulkan
  -> XuanYu.Render.Abstractions
  -> XuanYu.Core

XuanYu.Render.Abstractions
  -> XuanYu.Core

XuanYu.Core
  -> 不依赖 Editor
  -> 不依赖 Vulkan
  -> 不依赖 Avalonia
```

禁止依赖方向：

1. `XuanYu.Editor.UI` 禁止直接引用 `Silk.NET.Vulkan`。
2. `XuanYu.Editor.UI` 禁止直接持有 Vulkan Instance、Device、Surface、Swapchain。
3. `XuanYu.Render.Vulkan` 禁止引用 EditorShell、Inspector、LogPanel、Diagnostics UI。
4. `XuanYu.Core` 禁止引用 Editor、Avalonia、Vulkan。
5. Vulkan 后端禁止反向修改 `WorldState`。
6. Vulkan 后端禁止直接刷新 UI。

当前仓库若存在历史或探针阶段的 Vulkan 引用，应视为后续迁移债务。RZ-VK0 不迁移、不删除、不新增，只定义目标边界。

## 4. Vulkan 生命周期定义

### 4.1 VulkanRuntime 生命周期

```text
生命周期：编辑器进程生命周期
创建时机：编辑器启动后，准备启用 Vulkan 后端时
销毁时机：编辑器关闭时
```

负责：

- Vulkan API 入口。
- Instance。
- Validation Layer。
- PhysicalDevice 选择。
- LogicalDevice。
- Queue。

禁止：

1. 禁止因鼠标移动重建 `VulkanRuntime`。
2. 禁止因视口刷新重建 `VulkanRuntime`。
3. 禁止因 Inspector 更新重建 `VulkanRuntime`。

### 4.2 VulkanViewportSession 生命周期

```text
生命周期：单个视口窗口生命周期
创建时机：视口 NativeHost / HWND 创建完成后
销毁时机：视口销毁或窗口关闭时
```

负责：

- HWND 绑定。
- Surface。
- Swapchain。
- DepthBuffer。
- Framebuffer。
- RenderPass / Pipeline 生命周期入口。

禁止：

1. 禁止 EditorShell 直接操作 Surface / Swapchain。
2. 禁止 UI 面板直接触发 Swapchain 重建。
3. 禁止鼠标移动触发 ViewportSession 重建。

### 4.3 Swapchain 生命周期

```text
生命周期：视口尺寸生命周期
创建时机：Surface 可用且视口尺寸有效时
重建时机：视口尺寸变化、DPI 变化、Swapchain out of date、最小化后恢复
销毁时机：视口销毁或尺寸重建前
```

关键规则：

- Resize 事件不能来一次就重建一次 Swapchain。
- Resize 只能标记 dirty。
- 下一帧或尺寸稳定后统一重建。
- 最小化、0 尺寸、无效尺寸时不能创建 Swapchain。
- Swapchain 重建必须有统一入口，不能从多个 UI 事件散射触发。

### 4.4 CommandBuffer 生命周期

```text
生命周期：帧生命周期或 FrameResource 生命周期
```

负责：

- Acquire。
- Record。
- Submit。
- Present。

禁止：

1. 禁止 CommandBuffer 持有 Editor UI 状态。
2. 禁止 CommandBuffer 直接读取 Inspector。
3. 禁止 CommandBuffer 直接修改 WorldState。
4. 禁止 CommandBuffer 写 LogPanel。

### 4.5 PreviewTransform 生命周期

```text
生命周期：一次鼠标拖动生命周期
开始：PointerPressed / Gizmo Drag Start
更新：PointerMove
结束：MouseUp Commit 或 Esc Cancel
```

拖动中只允许：

1. 更新 `PreviewTransform`。
2. 请求视口重绘。
3. 更新内存中的轻量状态。

拖动中禁止：

1. 禁止写 `WorldState`。
2. 禁止刷新 Inspector。
3. 禁止刷新 Diagnostics。
4. 禁止刷新 LogPanel。
5. 禁止提交 Undo/Redo 历史。
6. 禁止输出大量 UI 日志。

## 5. 帧调度规则

渲染请求必须合并，而不是排队堆积。

错误方式：

```text
鼠标移动 100 次
  -> 排队 100 个 RenderRequest
```

正确方式：

```text
鼠标移动 100 次
  -> 只保留最新 Preview 状态
  -> 只保留一个待渲染请求
  -> 下一帧绘制最新状态
```

后续应设计 `RenderRequestGate`：

1. 合并高频渲染请求。
2. 防止 RenderRequest 队列堆积。
3. 保证渲染使用最新状态，而不是逐条消费旧状态。
4. 区分立即请求、下一帧请求和低优先级请求。
5. 在 UI 线程和渲染线程之间保持明确边界。

## 6. Preview / Commit 交互规则

核心规则：

```text
拖动中，只预览，不汇报。
松手后，才提交，才汇报。
```

PointerMove 高频阶段禁止：

1. 禁止直接写 UI 日志。
2. 禁止直接刷新 LogPanel。
3. 禁止刷新 Inspector。
4. 禁止刷新 Diagnostics。
5. 禁止写 `WorldState`。
6. 禁止创建或销毁 Vulkan 大资源。
7. 禁止重建 Swapchain。
8. 禁止排队堆积 RenderRequest。

MouseUp / Commit 阶段允许：

1. 写 `WorldState`。
2. 刷新 Inspector。
3. 刷新 Diagnostics。
4. 写一条总结日志。
5. 提交 Undo/Redo 历史。

Esc / Cancel 阶段允许：

1. 清理 Preview 状态。
2. 请求一次视口重绘。
3. 写一条取消摘要日志。
4. 不写 `WorldState`。
5. 不提交 Undo/Redo 历史。

## 7. 日志与诊断规则

### 7.1 高频阶段

PointerMove / DragMove 阶段只允许：

1. 内存计数。
2. 更新最后一次坐标。
3. 更新最后一次预览状态。

禁止：

1. 禁止每次 PointerMove 都写 LogPanel。
2. 禁止每次 PointerMove 都追加 `ObservableCollection`。
3. 禁止每次 PointerMove 都滚动底部日志。
4. 禁止每次 PointerMove 都刷新 Diagnostics 文本。
5. 禁止每帧 Acquire / Present / RenderFrame 写普通 UI 日志。

### 7.2 低频阶段

MouseUp / Commit / Cancel 阶段允许输出一条总结日志。

示例：

```text
【移动完成】
对象：Cube_001
预览次数：36
起点：0,0,0
终点：0,0,2
结果：成功
```

日志字段必须使用中文字段与中文结构描述。Vulkan 生命周期日志也必须保持低频，只记录初始化、失败、Swapchain 重建、释放等摘要事件。

## 8. Resize / Swapchain 重建规则

Resize 不能直接等价于 Swapchain 重建。

推荐流程：

```text
Resize event
  -> ViewportResizeGate 标记 dirty
  -> 记录最新宽高和 DPI
  -> 跳过 0 尺寸和无效尺寸
  -> 下一帧或稳定窗口后统一 RecreateSwapchain
```

`ViewportResizeGate` 职责：

1. 合并连续 Resize。
2. 跳过重复尺寸。
3. 跳过 0 尺寸。
4. 标记 Swapchain dirty。
5. 将重建交给 ViewportSession 的统一入口。

Swapchain 重建规则：

1. 重建前必须等待相关帧资源安全释放。
2. 销毁旧 Swapchain 前必须确认 GPU 不再使用旧资源。
3. 重建失败时保持 fallback 或上一稳定状态。
4. 重建成功和失败只记录低频摘要日志。
5. 重建逻辑不得由 Inspector、LogPanel、Diagnostics 直接触发。

## 9. 5+100 落地规则

规则：

1. 每个文件只承担一个明确职责。
2. 单文件超过 100 行必须拆分。
3. 目录直接文件超过 5 个时，优先分子目录。
4. 不允许为了压行数把多个职责挤进一行。
5. 不允许用 `partial class` 掩盖职责过大。
6. 不允许把多个生命周期混在一个类里。

后续建议文件职责示例：

```text
VulkanRuntime.cs
  只负责 Vulkan Runtime 生命周期。

VulkanDevicePicker.cs
  只负责选择 PhysicalDevice。

VulkanQueueFamilyQuery.cs
  只负责队列族查询。

VulkanViewportSession.cs
  只负责视口级生命周期协调。

VulkanSurfaceOwner.cs
  只负责 Surface 创建与销毁。

VulkanSwapchainOwner.cs
  只负责 Swapchain 创建、重建与销毁。

VulkanFrameLoop.cs
  只负责 Acquire / Record / Submit / Present。

RenderRequestGate.cs
  只负责合并渲染请求。

ViewportResizeGate.cs
  只负责合并 Resize 请求。

PreviewTransformState.cs
  只负责拖动预览状态。

RenderSceneSnapshotBuilder.cs
  只负责 WorldState 到 RenderSceneSnapshot 的转换。
```

## 10. 后续 VK-1 到 VK-10 里程碑

```text
RZ-VK0：文档与架构边界
RZ-VK1：Vulkan 能力探针
RZ-VK2：Windows HWND 生命周期
RZ-VK3：Surface + Swapchain 清屏
RZ-VK4：RenderRequest 合并与帧调度
RZ-VK5：静态场景渲染
RZ-VK6：RenderSceneSnapshot
RZ-VK7：Preview / Commit 交互边界
RZ-VK8：Picking
RZ-VK9：Gizmo
RZ-VK10：性能压测与收口
```

重点说明：

- 真正重点是 RZ-VK3 和 RZ-VK4。
- 必须先完成 RZ-VK0 到 RZ-VK2，避免生命周期边界不清。
- RZ-VK8 和 RZ-VK9 必须在 Preview / Commit 规则稳定后再进入。

## 11. 验收标准

RZ-VK0 完成后必须满足：

1. 新增 `docs/vulkan-lifecycle-plan.md`。
2. 文档包含架构分层。
3. 文档包含依赖方向。
4. 文档包含 Vulkan 生命周期定义。
5. 文档包含 Preview / Commit 规则。
6. 文档包含日志限流规则。
7. 文档包含 RenderRequest 合并规则。
8. 文档包含 Resize / Swapchain 合并重建规则。
9. 文档包含 5+100 落地方式。
10. 文档包含后续 VK-1 到 VK-10 里程碑。
11. 不接入实际 Vulkan 代码。
12. 不修改现有 UI。
13. 不修改现有输入逻辑。
14. 不新增 `Silk.NET.Vulkan` 引用。
15. build 通过。
16. test 通过，或确认当前仓库没有可运行测试项目。

## 12. 风险与禁止事项

风险：

1. 当前仓库已经存在历史 Vulkan 探针代码，后续容易被误认为正式架构。
2. 当前 `Editor.UI` 若直接引用 Vulkan 包，和目标依赖方向不一致，需要在后续迁移阶段收口。
3. Resize、PointerMove、Inspector 刷新都可能成为性能问题入口。
4. 如果没有 RenderRequest 合并，高频输入会重新造成 UI 卡顿。
5. 如果没有 Preview / Commit 边界，拖动交互会污染 `WorldState`、Undo/Redo 和日志系统。

禁止事项：

1. 禁止在 RZ-VK0 新增 Vulkan 代码。
2. 禁止在 RZ-VK0 新增 `Silk.NET.Vulkan` 引用。
3. 禁止在 RZ-VK0 修改 UI 布局。
4. 禁止在 RZ-VK0 修改输入逻辑。
5. 禁止在 RZ-VK0 修改日志面板逻辑。
6. 禁止把旧 VulkanClearProbe 直接搬回正式路径。
7. 禁止接入 Picking。
8. 禁止接入 Gizmo。
9. 禁止接入 Transform Preview。

## 13. RZ-VK1 前置条件

进入 RZ-VK1 前，需要先确认：

1. 当前分支和远端基线稳定。
2. RZ-VK0 文档已提交。
3. Vulkan 探针只能放在目标架构允许的位置。
4. 探针输出只能是能力摘要，不能写每帧日志。
5. 任何新项目或包引用必须符合本文依赖方向。
