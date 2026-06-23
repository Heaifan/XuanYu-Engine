# 8.7.8H-5 — EditorShell 收口审计与白名单例外登记

审计日期：2026-06-23
目标文件：`FluidWarfare.Editor.Windows/Shell/EditorShell.axaml.cs`

---

## 1. 当前行数统计

| 口径 | 行数 |
|------|------|
| 含 using 总行数 | **491** |
| using 指令行数 | 95 |
| Body（不含 using） | **~396** |
| 原始行数（8.7.6 前） | 3,041 |
| **累计减少** | **2,550** |

## 2. H 阶段完成情况

| 阶段 | 减行 | 焦点 | 状态 |
|------|------|------|------|
| 8.7.6 Route 化重构 | -2,071 | 26+ Route 类提取 | ✅ |
| H-2A | -244 | Overlay 导航 + 地面指针 + Picking | ✅ |
| H-2B | -60 | Transform 编辑 + Scrub | ✅ |
| H-2C | -36 | Viewport 重绘 + Vulkan Redraw | ✅ |
| H-2D | -7 | 窗口菜单命令 | ✅ |
| H-2E | -33 | 层级树构建 + 选择同步 | ✅ |
| H-2F | -13 | Startup Vulkan Probe | ✅ |
| H-2G | -9 | 项目加载 + World Bootstrap | ✅ |
| H-4A | -160 | Raw 输入 + Frame Selected + 空删除 + 尺寸工具 | ✅ |
| H-4B | -5 | 日志委托 + 视口焦点 + Scene3D 命令 | ✅ |
| **总计** | **-2,550** | **15 个子路由文件** | **✅** |

## 3. 当前剩余代码分类

| 类别 | 预估行数 | 说明 |
|------|----------|------|
| 字段声明（UI 控件 + Route 引用 + 状态） | ~80 | 必须 Shell 持有 |
| 构造函数（查找 + RouteBuild + 路由创建 + 初始化） | ~100 | 组合根装配 |
| SubscribePanelEvents | ~40 | 事件接线总控 |
| 生命周期（Attach/Detach） | ~33 | Avalonia 框架要求 |
| BuildInputRequest + BuildScene3dCommandRequest | ~15 | 30+ 参数管家方法 |
| Transform 管线（InitTransformApplication + BuildTransformStartSnapshot + ApplyPreviewPosition + CancelActiveTransform） | ~37 | **H-4C 候选，已建议暂缓** |
| Overlay/Picking/Transform 薄转发（HandleViewportEscape + ScheduleScene3dFrame + HandleViewportPick + ApplyEntityTransform + CompleteGroundPlacement + HandleViewportToolChanged） | ~30 | 已尽可能薄 |
| ApplyScene3dCommandResult | ~7 | Session 状态管理 |
| ShowWorldEntitySelection 薄转发 | ~2 | Shell 级回调中转 |
| HandleViewportEscape | ~9 | 遗留组合根调度 |
| 工具/调试 | ~43 | 不含 using |
| **Body 合计** | **~396** | |

## 4. 低风险可拆项评估

| 项 | 收益 | 风险 | 建议 |
|----|------|------|------|
| H-4C Transform 管线 | ~30 行 | **高** — Gizmo/移动/Scrub/Preview/Commit 全链路 | **暂缓** |
| HandleViewportEscape 提取 | ~9 行 | 低 — 仅与 groundPlacementState 交互 | 可做但不急 |
| ScheduleScene3dFrame 内联/路由化 | ~5 行 | 中 — 被 15+ 处引用 | 建议保留 |

**结论：没有值得立即做的低风险/高收益项。**

## 5. 白名单保留原因

EditorShell 当前白名单不可删除，原因如下：

1. **组合根本质**：EditorShell 是 Avalonia UserControl，必须承担控件查找（FindControls）、Route 创建（RouteBuild）、事件接线（SubscribePanelEvents）职责。这部分约 220 行不可移出。

2. **回调连接总站**：Shell 持有 11 个 H-x 子路由 + 26 个 Build Route + 7 个 UI 控件引用。这些字段在构造期从 RouteBuild 分发到各子路由。这种"星型拓扑"是组合根的天然结构。

3. **Transform 管线不可硬拆**：InitTransformApplication + BuildTransformStartSnapshot 等 4 个方法共 ~37 行，是 Gizmo 拖拽管线的创建/快照逻辑。将其移出需同时迁移 Applier 实例的生命周期管理，风险高且收益低。

4. **BuildInputRequest 管家方法**：接收 30+ 参数，是 Shell 对输入管线的唯一入口。无法移出。

5. **继续压到 ≤100 行需架构变革**：不是继续用小刀拆分可以完成的。真正走到 ≤100 需要引入 Shell CompositionRoot 架构或完全重写 Shell，而这超出了 8.7.8 的范围。

## 6. 后续禁止新增到 EditorShell 的内容

EditorShell 当前是**只出不进**状态。

| 类别 | 禁止 | 替代 |
|------|------|------|
| 业务逻辑 | ❌ 不得新增 | 新建 Route / 子模块 |
| 事件订阅 | ❌ 不得新增到 SubscribePanelEvents | 在 Route 内部订阅 |
| 字段持有 | ❌ 不得新增业务状态字段 | 放入对应 Route |
| 方法体 | ❌ 不得新增超过 3 行的方法 | 放入 Route |
| 薄转发 | ❌ 不得新增 | 已在 Shell 的保留 |

但允许在 Shell 保留：

| 类别 | 允许 | 理由 |
|------|------|------|
| Route 字段声明 | ✅ | 组合根必须有 |
| Route 创建代码 | ✅ | 必须由 Shell 创建 |
| 事件接线 | ✅ | 必须由 Shell 绑定 |
| UI 控件引用 | ✅ | 必须由 Shell 持有 |
| 回调闭包 | ✅ | 路由间的连线 |
| 日志/诊断 | ✅ | Shell 级操作 |

## 7. 新增职责应放哪里

| 职责 | 目标目录 |
|------|----------|
| 新的命令/功能 | `Shell/Commands/` |
| 新的诊断/日志 | `Shell/Diagnostics/` |
| 新的输入处理 | `Shell/Input/` |
| 新的生命周期 | `Shell/Lifecycle/` |
| 新的菜单操作 | `Shell/Menu/` |
| 新的导航 | `Shell/Navigation/` |
| 新的面板操作 | `Shell/Panels/` |
| 新的 Picking | `Shell/Picking/` |
| 新的项目操作 | `Shell/Project/` |
| 新的 Scene3D 操作 | `Shell/Scene3D/` |
| 新的选择逻辑 | `Shell/Selection/` |
| 新的启动逻辑 | `Shell/Startup/` |
| 新的 Transform 操作 | `Shell/Transform/` |
| 新的视口操作 | `Shell/Viewport/` |
| 新的窗口命令 | `Shell/Windows/` |

## 8. 结论

| 维度 | 结论 |
|------|------|
| **H 阶段完成** | ✅ **可以宣告完成** |
| **建议 H-4C** | ❌ **暂缓** — Transform 管线收益低、风险高 |
| **白名单** | ✅ **保留** — 组合根例外 |
| **Shell ≤100 行** | ❌ 当前架构下不可行 |
| **后续策略** | 只出不进，新增职责必须进 Route / 子模块 |
