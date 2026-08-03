# ARCH-B-Plan：编辑器状态所有权与交互事务边界
版本：v0.2.16.1-rz  
日期：2026-07-13 23:33:18  
类型：规划文档

## 目标

本轮只审计当前编辑器状态、写入路径和未来交互接入点，规划 ARCH-B 最小分轮方案。  
不修改运行逻辑，不新增状态框架，不改 Vulkan、Resize、Present、Bridge 生命周期。

ARCH-B 要解决的问题是：

```text
谁持有正式编辑器状态？
谁有权修改状态？
UI 是否能直接写场景、选择、工具和事务？
Viewport、层级树、Inspector 如何同步同一份结果？
Preview / Commit / Cancel 如何隔离？
```

## 当前状态所有权清单

| 状态类别 | 当前持有者 | 写入入口 | 风险判断 |
| --- | --- | --- | --- |
| 当前工具 | `UiVm` 的 `_activeTool` 与工具布尔标志 | `SelectToolCommand` → `SelectTool` | 多个布尔标志派生自同一工具字符串，R1 应收口为单一正式工具状态。 |
| 当前选择 | `UiVm` 的 `_selectedProjectItem` / `_selectedHierarchyItem` / `SelectionTitle` / `SelectionSubtitle` / `HasSelection` | `SelectedProjectItem`、`SelectedHierarchyItem` setter → `ApplySelection` | 选择结果由 UI VM 直接写入，未来 Viewport Picking 接入后会形成多写入者风险。 |
| Inspector 显示 | `Right.axaml` 绑定 `SelectionTitle` / `SelectionSubtitle`，并包含静态路径字段 | 由选择状态间接驱动 | Inspector 目前只读显示，但依赖 UI VM 选择字段，不应成为选择决策者。 |
| 左侧页签 | `UiVm.LeftTabIndex` | `Left.axaml` 双向绑定 | 属于纯 UI 布局状态，不应进入 ARCH-B 正式编辑器 Owner。 |
| 日志打开状态 | `UiVm.IsLogOpen` | `ToggleLogCommand`，并触发 NativeHost 布局同步 | 是 UI 布局状态，但会影响 Viewport 尺寸请求；需继续与编辑器正式状态隔离。 |
| Footer 状态 | `FooterMessage` / `FooterMode` / `FooterState` | `Run`、`SelectTool`、`ApplySelection` | 当前是 UI 摘要状态，未来应从正式状态变化或命令结果派生。 |
| 日志数据 | `EditorLogBuffer` / `EditorLogBus` / `_logFilter` / `_selectedLogEntry` | `LogCommand`、`LogTool`、`LogVulkanLifecycle`、日志控件选中事件 | 日志是诊断系统，不应成为正式编辑器状态来源。 |
| NativeHost 生命周期 | `VulkanNativeHost`、`NativeHostResizeCoalescer`、`ViewportNativeHostRoute` | Attach / Resize / Detach / Dispose / LayoutSync | 属于视口宿主状态，不是场景状态；只应向渲染桥发请求和记录诊断。 |
| 渲染后端状态 | `XuanYu.Render.Vulkan` 内部 Session / Swapchain / Present | App 注入的抽象 Bridge | ARCH-B 不进入 Vulkan；渲染状态继续由后端拥有。 |
| 示例项目/层级/调试文本 | `UiText` / `DebugText` 静态数组 | 无运行时写入 | 是静态占位数据，R2 前不能当作真实 Scene Model。 |

## 当前写入路径

```text
顶部命令按钮
→ UiVm.Run
→ FooterMessage / FooterState / 日志

顶部工具按钮
→ UiVm.SelectTool
→ _activeTool / 工具布尔标志 / FooterMode / FooterMessage / 日志

左侧项目树或层级树绑定
→ UiVm.SelectedProjectItem / SelectedHierarchyItem
→ ApplySelection
→ SelectionTitle / SelectionSubtitle / HasSelection / FooterState

日志面板
→ Foot.axaml.cs
→ SetSelectedEntries / SelectedLogEntry
→ 日志复制文本与详情显示

日志栏展开收起
→ UiVm.IsLogOpen
→ VulkanNativeHost.LayoutSync
→ NativeHostResizeCoalescer
→ Bridge.Resize

Vulkan 生命周期消息
→ ViewportNativeHostRoute
→ UiVm.LogVulkanLifecycle / LogNativeHostLifecycle
→ EditorLogBus
```

## 审计发现

1. `UiVm` 同时承担工具状态、选择状态、Footer 摘要、日志聚合、NativeHost 诊断入口和抽象 Bridge factory 持有，职责偏宽。
2. 选择状态目前只有 UI VM 写入，但未来层级树、Viewport Picking、Inspector 都会需要同一份选择，必须先建立唯一 Owner。
3. 工具状态目前由字符串和多组布尔值共同表达，后续 Gizmo 接入时容易出现“ActiveTool 与 IsMoveTool 不一致”。
4. Preview / Commit / Cancel 当前没有正式事务边界，未来拖拽 Preview 若直接写 `UiVm` 或场景字段，会污染正式状态。
5. 日志系统已有独立 buffer/filter，但与 `UiVm` 强耦合；ARCH-B 不需要重构日志，只需禁止日志成为状态决策来源。
6. `NativeHostResizeCoalescer` 的尺寸合并属于视口宿主请求，不应被纳入编辑器状态 Owner。
7. `UiText` / `DebugText` 是静态占位，不是可变场景模型；R1 不应围绕它们设计持久化结构。

## ARCH-B 最小目标结构

```text
UI View / ViewModel
→ 提交明确命令
→ Editor State Owner
→ 校验并写入正式状态
→ 发布只读快照或事件结果
→ UI / Viewport / Inspector 同步显示
```

Preview / Commit / Cancel 的边界：

```text
Begin：保存事务起始快照
Preview：只更新高频临时预览与必要渲染请求
Commit：一次性写入正式编辑器状态
Cancel：恢复事务起始快照
```

## 禁区

- 不新增全局 Event Bus。
- 不引入第三方 DI 或状态框架。
- 不设计通用 ECS。
- 不开发真实 Picking。
- 不开发 Gizmo 图形。
- 不开发场景存档。
- 不修改 Vulkan、Swapchain、Present、Resize、Bridge 生命周期。
- 不把窗口尺寸、日志过滤、主题、布局等纯 UI 状态塞进正式 Editor State Owner。

## 分轮方案

### v0.2.16.2-rz — ARCH-B-R1

建立最小 Editor State Owner 和只读快照边界。  
只覆盖马上要支撑 Viewport / Picking / Gizmo 的状态：

- 当前选择；
- 当前工具；
- 工具捕获状态；
- 当前交互事务；
- 视口请求状态。

R1 不预设新增 `Editor.Core` 项目，是否新增项目由实现前的文件职责审计决定。

### v0.2.16.3-rz — ARCH-B-R2

收口选择状态和活动工具状态：

```text
层级树或测试入口提交选择命令
→ State Owner 校验
→ 发布选择变化
→ 层级树 / Inspector / Viewport 同步
```

本轮仍不做真实 Picking，可用层级树或受控测试入口验证。

### v0.2.16.4-rz — ARCH-B-R3

建立 Preview / Commit / Cancel 最小事务骨架：

- Begin 保存起始快照；
- Preview 不写正式状态；
- Commit 只提交一次；
- Cancel 恢复起始快照。

不实现完整 Move Gizmo，只建立可测试事务边界。

### v0.2.16.5-rz — ARCH-B-R4

增加 ARCH-B 守卫、测试和总收口：

- UI 不直接写正式编辑器状态；
- 选择状态只有一个 Owner；
- Preview 不写 Commit 状态；
- Cancel 能恢复起始快照；
- ViewModel 不退化为全局状态容器；
- 5+100；
- 0 warning / 0 error。

## SVG 状态流图

```svg
<svg xmlns="http://www.w3.org/2000/svg" width="1040" height="430" viewBox="0 0 1040 430">
  <rect width="1040" height="430" fill="#f6f8fb"/>
  <text x="520" y="36" text-anchor="middle" font-size="22" font-weight="bold" font-family="Microsoft YaHei">ARCH-B 最小状态所有权边界</text>

  <rect x="50" y="95" width="190" height="74" rx="8" fill="#dbeafe" stroke="#2563eb"/>
  <text x="145" y="126" text-anchor="middle" font-size="16" font-family="Microsoft YaHei">UI / ViewModel</text>
  <text x="145" y="150" text-anchor="middle" font-size="13" font-family="Microsoft YaHei">只提交意图</text>

  <rect x="305" y="72" width="190" height="62" rx="8" fill="#fef3c7" stroke="#d97706"/>
  <text x="400" y="100" text-anchor="middle" font-size="16" font-family="Microsoft YaHei">命令入口</text>
  <text x="400" y="121" text-anchor="middle" font-size="13" font-family="Microsoft YaHei">请求修改状态</text>

  <rect x="305" y="160" width="190" height="62" rx="8" fill="#ffffff" stroke="#64748b"/>
  <text x="400" y="188" text-anchor="middle" font-size="16" font-family="Microsoft YaHei">查询入口</text>
  <text x="400" y="209" text-anchor="middle" font-size="13" font-family="Microsoft YaHei">读取只读快照</text>

  <rect x="570" y="92" width="220" height="110" rx="10" fill="#dcfce7" stroke="#16a34a" stroke-width="2"/>
  <text x="680" y="126" text-anchor="middle" font-size="17" font-weight="bold" font-family="Microsoft YaHei">状态所有者</text>
  <text x="680" y="152" text-anchor="middle" font-size="13" font-family="Microsoft YaHei">唯一正式写入者</text>
  <text x="680" y="176" text-anchor="middle" font-size="13" font-family="Microsoft YaHei">校验事务与状态转换</text>

  <rect x="850" y="106" width="140" height="74" rx="8" fill="#e2e8f0" stroke="#64748b"/>
  <text x="920" y="137" text-anchor="middle" font-size="16" font-family="Microsoft YaHei">结果事件</text>
  <text x="920" y="160" text-anchor="middle" font-size="13" font-family="Microsoft YaHei">事实已发生</text>

  <rect x="195" y="295" width="170" height="70" rx="8" fill="#fff7ed" stroke="#ea580c"/>
  <text x="280" y="324" text-anchor="middle" font-size="16" font-family="Microsoft YaHei">预览</text>
  <text x="280" y="347" text-anchor="middle" font-size="13" font-family="Microsoft YaHei">高频临时状态</text>

  <rect x="435" y="295" width="170" height="70" rx="8" fill="#dcfce7" stroke="#16a34a"/>
  <text x="520" y="324" text-anchor="middle" font-size="16" font-family="Microsoft YaHei">提交</text>
  <text x="520" y="347" text-anchor="middle" font-size="13" font-family="Microsoft YaHei">一次正式写入</text>

  <rect x="675" y="295" width="170" height="70" rx="8" fill="#fee2e2" stroke="#dc2626"/>
  <text x="760" y="324" text-anchor="middle" font-size="16" font-family="Microsoft YaHei">取消</text>
  <text x="760" y="347" text-anchor="middle" font-size="13" font-family="Microsoft YaHei">恢复起始快照</text>

  <path d="M240 122 L305 103" stroke="#334155" stroke-width="2" marker-end="url(#arrow)"/>
  <path d="M305 191 L240 150" stroke="#334155" stroke-width="2" marker-end="url(#arrow)"/>
  <path d="M495 103 L570 132" stroke="#334155" stroke-width="2" marker-end="url(#arrow)"/>
  <path d="M570 178 L495 191" stroke="#334155" stroke-width="2" marker-end="url(#arrow)"/>
  <path d="M790 145 L850 145" stroke="#334155" stroke-width="2" marker-end="url(#arrow)"/>
  <path d="M680 202 L280 295" stroke="#334155" stroke-width="2" marker-end="url(#arrow)"/>
  <path d="M680 202 L520 295" stroke="#334155" stroke-width="2" marker-end="url(#arrow)"/>
  <path d="M680 202 L760 295" stroke="#334155" stroke-width="2" marker-end="url(#arrow)"/>

  <text x="520" y="402" text-anchor="middle" font-size="15" fill="#475569" font-family="Microsoft YaHei">UI 提交意图，状态所有者决定结果；预览不污染正式状态</text>
  <defs><marker id="arrow" markerWidth="10" markerHeight="10" refX="9" refY="3" orient="auto"><path d="M0,0 L0,6 L9,3 z" fill="#334155"/></marker></defs>
</svg>
```

## 验收路径

- 本轮为文档规划和版本推进。
- 必须同步 `changelog.md`、`file-tree.md`、窗口标题和 `run.bat` 版本号。
- 自动验证应覆盖 ARCH-A 守卫、5+100、`git diff --check` 和解决方案构建。
- 下一轮进入代码前，必须保持本计划禁区，不提前开发 Picking / Gizmo / 场景存档。
