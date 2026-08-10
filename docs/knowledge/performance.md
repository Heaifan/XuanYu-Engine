# Performance 性能知识

## K-PERF-001 Preview 高频路径与 Commit 重路径必须分离

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：PointerMove、Preview、Commit、Diagnostics、Inspector、Frame Budget
**适用范围**：Gizmo 拖动、Region Draft、Terrain Brush、Scrub、动画时间轴、任何 30～240Hz 交互路径。

**首次关键修复**：2026-06-25 00:18（UTC+08:00）
**版本**：`v0.1.8.7-fix`
**Commit**：`3bf341fd2e577498738e905512f3d4f6d8638037`
**后续审计**：`v0.1.8.8-fix` · 2026-06-25 21:41；`v0.1.8.9-fix` · 2026-06-25 22:56
**来源**：`docs/archive/changelog/changelog-2026-06.md`

### 问题

PointerMoved、Drag Preview 等路径可能每秒触发数十到数百次。如果每帧同步刷新 Inspector、Diagnostics、PickSnapshot、WorldState、Hierarchy 和 UI 日志，单个组件都“不慢”也会叠加成明显卡顿。

### 工程规则

Preview 与 Commit 是两种不同成本等级：

**Preview 允许**：

- 更新临时 Transform / Draft；
- 更新 Render Preview；
- 合并/调度轻量 Redraw；
- 必要的轻量 Hit/Pick。

**Preview 默认禁止**：

- 刷新 Inspector；
- 刷新 Diagnostics/DebugDock；
- 重建完整 PickSnapshot；
- 写正式 WorldState/History；
- 高频 UI Log；
- 重建 Hierarchy。

这些重操作应进入 Commit/Cancel 或降频路径。

### 真实历史示例

`v0.1.8.7-fix` 将 TransformPreview 的 Inspector、Diagnostics、PickSnapshot 重工作移除。`v0.1.8.8-fix` 用全链路 Probe 验证 Preview 帧：UI=否、WorldState=否、Diagnostics=否、Inspector=否、PickSnapshot=跳过；Commit 帧才执行正式更新。`v0.1.8.9-fix` 又发现 Preview 帧完成回调仍可能触发 Diagnostics refresh，并继续清除该残留。

### 未来应用示例

Region 顶点拖动时：

```text
PointerMove
→ Draft vertex update
→ Overlay buffer update
→ Redraw
```

松开鼠标后：

```text
Commit
→ MapEditSession
→ History
→ Dirty
→ Inspector
→ Low-frequency log
```

不要在每个 Move 都创建 History Entry 或刷新整个 Map Inspector。

### 禁止做法

- “先都刷新，卡了再优化”。
- 用 Debug/日志验证性能问题，却让日志本身进入高频 UI Sink。
- Preview/Commit 共用一个不分阶段的大刷新方法。

### 验证方法

对高频链植入低开销 Probe，统计 1 次拖动中 Inspector/Diagnostics/WorldState/PickSnapshot 次数；Preview 期应为 0 或明确受控的低频值。Probe 自身不得成为 UI 重负载。

**关联 Incident**：INC-2026-06-25-001
**关联 Knowledge**：K-ASSET-002
