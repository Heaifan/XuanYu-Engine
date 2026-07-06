# 9.1A-Freeze：EditorShellV2 冻结文档

冻结日期：2026/06/26  
冻结决定：暂停 9.1A 后续开发，ShellV2 现有代码暂时保留，不继续扩展，直到人工重新确认编辑器布局路线。  
冻结合约：不回滚、不继续、不扩大。旧 EditorShell 继续作为默认主线。

---

## 一、为什么冻结 ShellV2

ShellV2 在 9.1A-1 ~ 9.1A-3R 期间完成了最小骨架和 Viewport 输入链路接入（相机导航、Picking、MoveGizmo press/drag/release/Esc Cancel），但以下原因导致不适合继续向前推进：

| 原因 | 说明 |
|---|---|
| **布局目标未确认** | ShellV2 的布局方案（菜单栏 + Viewport 全区域 + 覆盖层工具条）来自 9.1A-0 方案审计，但尚未与你的实际编辑器使用场景核对。 |
| **逐层接入顺序未收敛** | 原计划 9.1A-4 StatusBar → 9.1A-5 Inspector，但每步都会引出新的依赖（WorldState、SelectionSync、TransformApply），继续推可能变成“逐层造整个 EditorShell 框架”。 |
| **旧 Shell 仍稳定** | 旧 EditorShell 在 9.0X / 9.0Y 封版后运行正常，没有必须换 Shell 的硬性故障。ShellV2 不存在修复紧急缺陷的压力。 |
| **旁路实验代码不应消耗主线精力** | ShellV2 本质是旁路实验。继续逐层接入面板相当于重做一套完整编辑器，但与旧 Shell 的功能重叠度高，维护成本双倍。 |

---

## 二、当前 ShellV2 已完成内容

| 层级 | 状态 | 阶段 |
|---|---|---|
| **最小骨架**：菜单栏 + Viewport（覆盖层工具条预留） | ✅ 完成 | 9.1A-1 |
| **Viewport 渲染**：NativeHostInfoChanged → StartupProbe → 渲染循环 | ✅ 完成 | 9.1A-1 |
| **相机导航**：中键旋转 + Shift+中键平移 + 滚轮缩放 | ✅ 完成 | 9.1A-2 / 9.1A-2R |
| **Picking**：点击实体 → 更新选择状态（不刷新 Inspector/Diagnostics） | ✅ 完成 | 9.1A-3 |
| **MoveGizmo**：hover / press / drag preview / release / Esc Cancel | ✅ 完成 | 9.1A-3 / 9.1A-3R |
| Preview 不刷新重 UI | ✅ 完成 | 9.1A-3R |
| Esc Cancel 走 9.0X 捕获释放闭环 | ✅ 完成 | 9.1A-3R |
| 实例输入状态机（非 static） | ✅ 完成 | 9.1A-2R |
| 所有生产文件 ≤100 行 | ✅ 完成 | 贯穿 |

---

## 三、当前 ShellV2 未完成内容

| 功能 | 计划阶段 | 冻结说明 |
|---|---|---|
| StatusBar | 9.1A-4 | **暂停**，等待布局路线确认 |
| WorldState 引用（决定正确的 Gizmo 初始位置） | 9.1A-4 | **暂停**，当前使用 Vector3d.Zero |
| Inspector | 9.1A-5 | **暂停** |
| ProjectTree | 9.1A-3（原计划） | **暂停** |
| Diagnostics / Console | 9.1A-4（原计划） | **暂停** |
| 布局可调（Splitter） | 9.1A-5 | **暂停** |
| Overlay 导航 UI | 未排期 | **暂停** |

---

## 四、旧 Shell 当前地位

> **旧 EditorShell 是默认主线，不受影响。**

| 检查项 | 状态 |
|---|---|
| `MainWindow.axaml` → 旧 `EditorShell` | ✅ 未修改 |
| `App.axaml.cs` 默认 `MainWindow` | ✅ 未修改 |
| `Shell/EditorShell.axaml` 布局 | ✅ 未修改 |
| `Shell/Composition/` 全部路由 | ✅ 未修改 |
| 9.0X / 9.0Y 封版成果 | ✅ 保留 |

ShellV2 切换方式：手动修改 `App.axaml.cs` 中 `desktop.MainWindow = new MainWindowV2()`，不属默认路径。

---

## 五、冻结期间禁止事项

- [ ] 不恢复 stash@{0}
- [ ] 不接 StatusBar
- [ ] 不接 Inspector
- [ ] 不接 ProjectTree
- [ ] 不接 Diagnostics / Console
- [ ] 不修改 Vulkan
- [ ] 不修改 Gizmo / Transform
- [ ] 不替换旧 Shell 默认入口
- [ ] 不继续扩展 ShellV2 功能
- [ ] 不回滚 ShellV2 现有代码

---

## 六、后续路线候选方案

| 方案 | 操作 | 代价 | 收益 |
|---|---|---|---|
| **A. 继续 ShellV2** | 先人工确认布局草图，再按 9.1A-4 → 9.1A-5 顺序逐层接入 | 需先投入布局讨论，后续开发时间 | 最终得到干净的新 Shell |
| **B. ShellV2 → ExperimentalShell** | 重命名 ShellV2 目录，明确标记为实验代码，不承诺替代旧 Shell | 低（仅重命名） | 降低旁人看到 ShellV2 误以为是正式方案的认知风险 |
| **C. 回滚 ShellV2** | `git revert` ShellV2 相关提交 | 中（失去已有实验代码） | 仓库零旁路代码 |
| **D. 不改 Shell，改造旧 Shell** | 在旧 EditorShell 上做增量改造，不做全新并行 Shell | 低（利用现有稳定路线） | 避免双 Shell 维护成本。但需评估旧 Shell 结构是否支持增量改造 |

---

## 七、当前文件结构（冻结快照）

```
ShellV2/
├── EditorShellV2.axaml                    (38 行)
├── EditorShellV2.axaml.cs                 (26 行)
└── Composition/
    ├── EditorShellV2Context.cs              (61 行)
    ├── EditorShellV2Composition.cs          (99 行)
    ├── EditorShellV2Routes.cs               (40 行)
    ├── EditorShellV2InputWiring.cs          (87 行)
    └── Input/
        ├── EditorShellV2InputState.cs       (18 行)
        ├── EditorShellV2PickingWiring.cs    (42 行)
        └── EditorShellV2SceneToolWiring.cs  (75 行)

ShellV2/ 总计 8 文件，约 486 行（含空行和注释）。
另含 MainWindowV2.axaml / MainWindowV2.axaml.cs（28 行）。
```

---

## 八、下一步建议

> **不写代码。先回答编辑器布局目标问题。**

| 问题 | 用途 |
|---|---|
| 你对当前窗口最不满意的是哪几块？ | 找真实痛点 |
| 是想像 Unity / Unreal / Blender，还是自定义 RTS 编辑器？ | 定风格 |
| 左侧项目树是否必须一直显示？ | 决定分区 |
| Inspector 应该放右侧还是底部？ | 决定主布局 |
| Diagnostics / Console 是否默认隐藏？ | 防止 UI 拥挤 |
| Viewport 是否应该最大化优先？ | 决定编辑器重心 |
| 是否需要停靠 / 浮动 / 可折叠面板？ | 决定复杂度 |

---

## 九、变更清单

| 文件 | 行数 | 变更类型 |
|---|---|---|
| `docs/audit-EditorShellV2-freeze-9.1A-Freeze.md` | 新文件 | 冻结文档 |

未修改任何源代码。

---

## 十、禁止项确认

- [x] 没有源代码改动
- [x] 没有继续开发 ShellV2
- [x] 没有恢复 stash
- [x] 没有接 StatusBar / Inspector / ProjectTree
- [x] 文档明确 ShellV2 进入冻结状态
- [x] 文档明确旧 Shell 仍是默认主线
- [x] 文档明确后续继续前必须人工确认布局路线
