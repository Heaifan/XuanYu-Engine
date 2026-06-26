# 9.1A-0 审计：EditorShellV2 布局重建方案

审计日期：2026/06/26  
审计目标：审计当前 EditorShell 布局结构，设计 EditorShellV2 并行方案，输出分阶段计划。  
禁止项：不恢复 stash、不修改旧 Shell、不修改 Vulkan/Gizmo/Transform。

---

## 一、当前 EditorShell 布局结构

### 1.1 axaml 布局（EditorShell.axaml）

```
Grid (4 行: 44px / * / 200px / 28px)
├── Row 0 菜单栏 (44px, 固定高度)
│   └── StackPanel: [文件] [编辑] [视图] [运行] [导出] [设置] [帮助]
│       水平排列，Spacing=24，无动态分区
├── Row 1 主内容区 (*, 占据剩余空间)
│   └── Grid (3 列: 260px / * / 300px)
│       ├── Col 0: ProjectWorldDockPanel (260px 固定宽度)
│       ├── Col 1: Viewport (自适应)
│       │   └── Grid (2 列: 44px / *)
│       │       ├── ViewportToolPalette (44px 工具条)
│       │       └── VulkanViewportHostPanel (原生视口)
│       └── Col 2: InspectorPanel (300px 固定宽度)
├── Row 2 DebugDockPanel (200px 固定高度)
│   └── 底部调试面板（控制台/诊断/输出页签）
└── Row 3 StatusBarPanel (28px 固定高度)
    └── 状态栏
```

### 1.2 当前问题清单

| # | 问题 | 影响 |
|---|---|---|
| P1 | **左侧 ProjectWorldDockPanel 固定 260px**，不可拖动调整 | 用户无法按需调整项目树宽度 |
| P2 | **右侧 InspectorPanel 固定 300px**，不可拖动调整 | 用户无法按需调整检查器宽度 |
| P3 | **底部 DebugDockPanel 固定 200px**，不可拖动调整 | 高分辨率下浪费空间，低分辨率下压缩 Viewport |
| P4 | **菜单栏仅用 StackPanel**，右侧无空间容纳性能显示/工具状态 | 性能监视器需嵌入式 border，只能靠 Grid 列 |
| P5 | **ViewportToolPalette 与 Viewport 同层 Grid**，44px 工具条占用 Viewport 水平空间 | 工具条作为覆盖层更合理 |
| P6 | **无停靠/浮动/页签系统** | 所有面板位置固定，无法拖拽重组 |
| P7 | **Shell 只支持一种布局**，无布局切换/保存/恢复 | 无法切换编辑/调试布局 |

### 1.3 当前职责分布

```
EditorShell (UserControl)
  │
  ├── axaml 布局 (Row 0-3)                 ← UI 结构
  │   └── 控件引用 (x:Name)
  │
  ├── axaml.cs (生命周期)                    ← 创建/附加/分离
  │   ├── EditorShell() → 组合根构建
  │   └── OnAttached/OnDetached → 生命周期
  │
  └── Composition (Shell/Composition/)
      ├── EditorShellComposition.cs        ← 拼装所有 Route
      ├── EditorShellCompositionRuntime.cs  ← 运行时辅助
      ├── EditorShellContext.cs             ← 全局上下文
      ├── EditorShellEventWiring.cs          ← 事件订阅
      ├── EditorShellLifecycle.cs           ← Attach/Detach
      ├── EditorShellControlRefs.cs         ← 控件查找
      ├── EditorShellRouteBuild.cs          ← Route 构造
      └── EditorShellRouteSet.cs            ← Route 集合
```

---

## 二、EditorShellV2 设计目标

| 目标 | 优先级 | 说明 |
|---|---|---|
| T1 | **P0** | **V2 与旧 Shell 并行存在**，旧 Shell 不受影响 |
| T2 | **P0** | **第一阶段只接 Viewport**，不接入复杂面板 |
| T3 | P1 | 可调整面板大小（Splitter 或拖拽分隔条） |
| T4 | P1 | 面板可折叠/隐藏 |
| T5 | P2 | 页签系统（底部多页签：控制台/诊断/输出） |
| T6 | P3 | 布局保存/恢复 |
| T7 | P3 | 面板浮动/停靠 |

### 第一阶段（9.1A-1）最小目标

```
新 Shell 并行 → 只接 Viewport → 菜单栏 + 简单状态栏
→ 不接 ProjectTree / Inspector / Diagnostics / Console
→ 旧 Shell 继续运行不变
```

---

## 三、新旧 Shell 共存策略

### 3.1 方案：独立目录 + 运行时选择

```
旧 Shell 路径：
  Shell/EditorShell.axaml
  Shell/EditorShell.axaml.cs
  Shell/Composition/
  Shell/Navigation/
  Shell/Input/
  Shell/Picking/
  Shell/Transform/
  ...

新 ShellV2 路径（建议）：
  ShellV2/EditorShellV2.axaml
  ShellV2/EditorShellV2.axaml.cs
  ShellV2/Composition/
  ShellV2/Panels/
  ShellV2/Viewport/
  ...
```

### 3.2 共存方式

1. **旧 Shell 不受影响** — 不删除、不修改旧 Shell 的运行路径。
2. **新 Shell 独立构造** — `EditorShellV2` 是一个新的 `UserControl`，不继承旧 Shell。
3. **运行时选择** — 通过配置/环境变量/菜单项切换显示哪个 Shell。默认仍是旧 Shell。
4. **复用原则** — 所有 Route、Context、Wiring 逻辑在旧 Shell 中定义，V2 通过组合引用它们。不复制代码。

### 3.3 复用 vs 隔离

| 组件 | 复用策略 |
|---|---|
| `NativeViewportHostControl` / `VulkanViewportHostPanel` | **直接复用**（与 Shell 无关） |
| `EditorShellComposition` / `Context` / `Route` 体系 | **建议复用**，V2 通过 ShellV2Composition 适配 |
| `EditorShellEventWiring` | **V2 需重建 Wiring**，因为控件引用和事件名称不同 |
| Panel 控件（InspectorPanel / DebugDockPanel 等） | **直接复用**，V2 按需放置 |
| 输入路由（RawInput / Navigation / Picking） | **复用**（它们绑定到 ViewportHostPanel，不直接依赖 Shell） |

---

## 四、Viewport 最小接入方案（9.1A-1）

### 4.1 最小 shell 布局

```
EditorShellV2.axaml — 第一阶段

Grid (2 行: 44px / *)
├── Row 0: 菜单栏（精简版，只有文件/运行/设置/帮助）
└── Row 1: VulkanViewportHostPanel（全区域覆盖）
    └── 覆盖层: ViewportToolPalette（浮在 Viewport 左上角）
```

### 4.2 需要接通的 Route

| Route | 优先级 | 说明 |
|---|---|---|
| `VulkanViewportHostPanel` 事件 | P0 | RawInput、Pick、Navigation、SceneTool |
| `EditorShellRawInputRoute` | P0 | 键盘/鼠标原始输入转发 |
| `EditorViewportInputRoute` | P0 | 视口输入路由 |
| `EditorShellOverlayNavigationRoute` | P0 | Overlay 导航 |
| `EditorShellPickingRoute` | P0 | 拾取路由 |
| `EditorShellViewportRedrawRoute` | P0 | 重绘调度 |
| `EditorShellViewportFocusRoute` | P1 | 焦点路由（后续） |
| `EditorShellScene3dCommandRoute` | P1 | 3D Session 命令（后续） |
| `EditorShellProjectBootstrapRoute` | P1 | 项目加载（后续） |
| `EditorShellAttachRoute` / `DetachRoute` | P0 | 生命周期 |
| `EditorShellWindowRoute` | P1 | 窗口菜单 |
| `EditorShellSelectionSyncRoute` | P2 | 选择同步（后续） |
| `EditorShellTransformRoute` | P2 | Transform 编辑（后续） |

### 4.3 不需要接通的（第一阶段）

| 组件 | 原因 |
|---|---|
| `InspectorPanel` | 后续 9.1A-2 接入 |
| `ProjectWorldDockPanel` | 后续 9.1A-2 接入 |
| `DebugDockPanel` | 后续 9.1A-2 接入 |
| `StatusBarPanel` | 第二阶段接入 |
| `EditorShellLogRoute` | 后续接入 |
| `EditorShellHierarchyRoute` | 后续接入 |
| `EditorShellScrubRoute` | 后续接入 |
| `EditorShellTransformRoute` | 后续接入 |

---

## 五、后续面板接入顺序

| 阶段 | 接入面板 | 说明 |
|---|---|---|
| **9.1A-1** | **Viewport 仅** | V2 最小骨架：菜单栏 + Viewport + 输入路由 |
| 9.1A-2 | Inspector + StatusBar | 编辑核心面板 |
| 9.1A-3 | ProjectTree | 内容浏览面板 |
| 9.1A-4 | Diagnostics + Console | 底部调试面板（页签系统） |
| 9.1A-5 | 布局可调（Splitter）+ 折叠/隐藏 | 面板交互优化 |
| 9.1后续 | 布局保存/恢复 | 持久化 |

---

## 六、文件/目录规划草案

```
ShellV2/
├── EditorShellV2.axaml                  # V2 布局（先只接 Viewport）
├── EditorShellV2.axaml.cs               # V2 生命周期
├── Composition/
│   ├── EditorShellV2Composition.cs      # V2 组合根（轻量，路由复用旧 Shell）
│   ├── EditorShellV2Context.cs          # V2 上下文（精简，只持有所需引用）
│   └── EditorShellV2EventWiring.cs      # V2 事件接线（只接 Viewport 事件）
└── README.md                            # V2 设计说明
```

### 目录规则

- `ShellV2/` 根目录建议 ≤5 文件。
- `ShellV2/Composition/` 建议 ≤3 文件。
- 后续面板增多时，按 `ShellV2/Panels/` 子目录拆分。
- 每个文件 ≤100 行（复杂基础设施 ≤150 行）。
- 新增/移动文件必须更新 `file-tree.md`。

---

## 七、风险分析

| 优先级 | 风险 | 说明 |
|---|---|---|
| **P0** | V2 构造时 Avalonia 控件树查找（`FindControl`）失败 | 如果 V2 的 axaml 结构与旧 Shell 不同，`FindControl` 可能找不到 `x:Name`。**解决方案**：V2 使用自己的 axaml 命名空间，不引用旧 Shell 的 `x:Name`。 |
| **P1** | V2 启动调度与旧 Shell 冲突 | 两套 Shell 共享同一 `Dispatcher`。**解决方案**：V2 独立启动，不依赖旧 Shell 的初始化链。 |
| **P1** | `EditorShellCompositionRuntime` 强引用 `EditorShellContext` | 旧 Context 包含全部面板引用，V2 需要新 Context 但复用旧 Route。**解决方案**：V2Composition 创建自己的精简 Context，只引用 V2 需要的控件。 |
| **P2** | Viewport 输入事件被旧 Shell 同时消费 | V2 和旧 Shell 如果同时持有 Viewport 引用，事件会双重分发。**解决方案**：同一时间只有一个 Shell 实例活跃。 |

---

## 八、9.1A-1 最小任务建议

```
9.1A-1 任务:
1. 创建 ShellV2/ 目录
2. 创建 EditorShellV2.axaml（仅菜单栏 + Viewport 全区域）
3. 创建 EditorShellV2.axaml.cs（生命周期，调用 V2 Composition）
4. 创建 ShellV2/Composition/EditorShellV2Context.cs（精简上下文）
5. 创建 ShellV2/Composition/EditorShellV2Composition.cs（轻量组合根）
6. 创建 ShellV2/Composition/EditorShellV2EventWiring.cs（仅 Viewport 事件）
7. 在 MainWindow 或启动入口添加运行时 Shell 切换机制
8. 验证 Viewport 渲染和输入正常工作
9. 更新 file-tree.md
10. 提交
```

---

## 九、结论

| 项目 | 结论 |
|---|---|
| 当前 Shell 问题是否明确 | ✅ 7 个问题已记录（P1-P7） |
| V2 目标是否明确 | ✅ 三阶段优先级已规划 |
| 并行策略是否可行 | ✅ 独立目录 + 运行时选择 |
| 复用策略是否合理 | ✅ Route 复用，Wiring 重建 |
| 9.1A-1 是否可以开始 | ✅ 最小骨架任务已列出 |

---

## 十、当前工作树状态

```bash
git status
# ?? EditorShell.Performance.cs          # 类别 C：不进 9.1A
# ?? AI_ONBOARDING_MANUAL_XuanYu_Engine.md  # 文档
# stash@{0}: (6 文件)                    # 类别 B/C：不进 9.1A
```

---

## 十一、变更清单

| 文件 | 行数 | 变更类型 |
|---|---|---|
| `docs/audit-EditorShellV2-plan-9.1A-0.md` | 新文件 | 布局重建方案审计 |

未修改任何源代码。

---

## 十二、禁止项确认

- [x] 未恢复 stash
- [x] 未修改旧 Shell 运行逻辑
- [x] 无 UI 代码改动
- [x] 无 Vulkan / Gizmo / Transform 改动
- [x] 纯文档提交
