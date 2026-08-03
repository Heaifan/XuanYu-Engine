# 9.0Y-3 审计：Gizmo 链路封版验证

验证日期：2026/06/26  
验证目标：确认 Gizmo 链路已闭环，9.0Y 可封版。  
验证方法：汇总 9.0Y-0/1/2 结论 + 边界检查 + Build/Test 基线确认。不修改代码。  
禁止项：不修改 Vulkan、UI、Gizmo 数学、输入生命周期、不恢复 stash。

---

## 一、9.0Y 阶段总览

| 子阶段 | 提交 | 性质 | 核心产出 |
|---|---|---|---|
| **9.0Y-0** | `b008006` | stash 清点 | 将 stash 9 文件分 3 类：A 进 9.0Y / B 延后 / C 不进 |
| **9.0Y-1** | `dfa969e` | 审计 + 纳入 | 提取类别 A 4 文件，审计 DragPlane 退化/状态机/可见性 |
| **9.0Y-2** | `d663f6f` | 测试补强 | +15 测试锁定 Gizmo 行为 |
| **9.0Y-3** | **当前** | **封版验证** | **汇总确认，给出封版结论** |

### 完整提交链

```
d663f6f test(gizmo): 9.0Y-2 Gizmo 链路最小测试补强
dfa969e audit(gizmo): 9.0Y-1 Gizmo 链路审计
b008006 docs(audit): 9.0Y-0 Gizmo 链路封版前 stash 清点
7a03d28 docs(audit): 9.0X-3 输入生命周期封版验证
5361271 fix(NativeHost): 9.0X-2 Esc 取消释放 Win32 Capture
6d62e9f docs(audit): 9.0X-1 输入生命周期审计
```

---

## 二、当前工作树与 stash 状态

### 2.1 git status

```bash
?? XuanYu.Engine.Editor.Windows/Shell/EditorShell.Performance.cs    # 类别 C：不进 9.0Y
?? docs/AI_ONBOARDING_MANUAL_XuanYu_Engine.md                       # 文档
```

未跟踪文件 2 个，均为类别 C（性能监视器 / 文档），不属 9.0Y。

### 2.2 stash list

```
stash@{0}: 9.0X-2R: 暂存非 9.0X-2 的工作树变更
           （性能监视器/Gizmo/Probe等），待 9.0X-3 验证后恢复
```

已从中提取类别 A 4 文件，剩余 5 文件保留：

| 剩余文件 | 类别 | 建议 |
|---|---|---|
| `GizmoDragProbe.cs` | C — Probe 基础设施 | 不进 9.0Y |
| `EditorShell.axaml` | C — UI / 性能监视器 | 不进 9.0Y |
| `EditorShell.axaml.cs` | C — 性能监视器 | 不进 9.0Y |
| `EditorConsoleOutput.cs` | C — Probe 基础设施 | 不进 9.0Y |
| `EditorProbe.cs` | C — Probe 基础设施 | 不进 9.0Y |
| `EditorShellPickingRoute.cs` | B — 地面十字标记退役 | 延后独立提交 |

---

## 三、Gizmo 链路封版表

### 3.1 已纳入的类别 A 文件

| 文件 | 行数 | 变更摘要 | 审计结论 |
|---|---|---|---|
| `AxisDragAnchorBuilder.cs` | 67 | DragPlane 退化降级：ScreenProjection 失败 → 射线-平面求交 | ✅ 数学正确，回退链完整 |
| `MoveGizmoInteraction.cs` | 28 | EndDrag 同时清 ActiveElement + HoveredElement | ✅ 状态机收口 |
| `MoveGizmoFrameSource.cs` | 91 | 可见性条件从 OR→AND，选中实体即显示 Gizmo | ✅ 行为更直觉 |
| `AxisDragAnchorBuilderTests.cs` | 110 | 4 个 ScreenProjection + 2 个 DragPlane 测试 | ✅ 测试通过 |

### 3.2 测试锁定清单

| 行为 | 测试文件 | 测试数 | 锁定 |
|---|---|---|---|
| Z 轴 ScreenProjection 正常路径 | `AxisDragAnchorBuilderTests` | 3 | ✅ |
| X 轴 ScreenProjection 正常路径 | `AxisDragAnchorBuilderTests` | 1 | ✅ |
| ScreenProjection 上下拖动可逆 | `AxisDragAnchorBuilderTests` | 1 | ✅ |
| **DragPlane 退化路径** | `AxisDragAnchorBuilderTests` | **1** | ✅ **9.0Y-2 新增** |
| **DragPlane 上下拖动可逆** | `AxisDragAnchorBuilderTests` | **1** | ✅ **9.0Y-2 新增** |
| **EndDrag 清 ActiveElement + HoveredElement** | `MoveGizmoInteractionTests` | **8** | ✅ **9.0Y-2 新增** |
| **Gizmo 可见性 OR→AND** | `MoveGizmoFrameSourceTests` | **5** | ✅ **9.0Y-2 新增** |

### 3.3 未进入 9.0Y 的 stash 内容

| 类别 | 文件 | 不进 9.0Y 理由 |
|---|---|---|
| B（延后） | `EditorShellPickingRoute.cs` | 地面十字标记退役，独立处理 |
| C（不进） | `GizmoDragProbe.cs` / `EditorProbe.cs` / `EditorConsoleOutput.cs` | Probe 基础设施 |
| C（不进） | `EditorShell.axaml` / `.axaml.cs` / `.Performance.cs` | 性能监视器 + UI |

---

## 四、当前剩余风险

| 优先级 | 风险 | 来源 | 说明 |
|---|---|---|---|
| **P2** | MoveToolActive + NoEntity 时 Gizmo 仍渲染 | AND 条件自然结果 | 需产品确认是否预期。`!MoveToolActive && !SelectedEntityId.IsValid` 在工具激活无实体时返回 false（不 Empty），Gizmo 会以 EntityId.None 渲染。如果预期"无选中实体时永远不显示"，需额外守卫。 |
| **P2** | CameraRight/CameraUp 硬编码 | `AxisDragAnchorBuilder.cs:60-61` | 死数据（求解器不使用），不影响行为。建议后续清理。 |
| P3 | 本地路径含 Kimi 配置 | `.vscode/settings.json` | 已加入 `.git/info/exclude`，不会误提交。 |
| 非 9.0Y | 已有测试 WorldHierarchy 排序失败 | — | 与 Gizmo 无关。 |

**未发现 P0/P1 风险。**

---

## 五、Build / Test 结果

```bash
git status
# 干净（2 个未跟踪文件，不属 9.0Y）

dotnet build XuanYu.Engine.sln
# Build succeeded. 0 Warning(s) 0 Error(s)

dotnet test XuanYu.Engine.Tests
# Passed: 712, Failed: 1, Skipped: 0, Total: 713
# 失败项：WorldHierarchyTreeBuilderTests（中文字符排序，与 9.0Y 无关）
```

### 测试数量变化历程

| 阶段 | 测试总数 | 变化 | 说明 |
|---|---|---|---|
| 9.0X 基线 | 698 | — | 输入生命周期封版 |
| 9.0Y-1 | 698 | +0 | 审计阶段，未新增测试 |
| 9.0Y-2 | **713** | **+15** | DragPlane(2)+Interaction(8)+FrameSource(5) |

---

## 六、封版结论

### 9.0Y 是否可以封版？

> **✅ 可以封版。**

### 封版依据

1. **类别 A 的 4 个文件已全部纳入**并经过审计，数学正确、状态机完整。
2. **15 个新增测试全部通过**，覆盖：
   - DragPlane 退化路径
   - EndDrag 状态清理
   - Gizmo 可见性条件
3. **stash 边界清楚**：类别 C 5 文件仍保留，未混入 9.0Y。
4. **工作树干净**：仅 2 个类别 C 未跟踪文件。
5. **Build 0 error，Tests 712/713**（仅已有排序失败）。
6. **无 P0/P1 风险**。P2 项（NoEntity 渲染、CameraRight/CameraUp 死数据）已知但不阻塞封版。

### 封版表述

> **Gizmo 拖动链路已通过审计、测试补强和封版验证。DragPlane 退化降级数学正确，EndDrag 状态清理完整，Gizmo 可见性条件已锁定。9.0Y 可封版。**

---

## 七、下一阶段建议

| 顺序 | 阶段 | 建议内容 |
|---|---|---|
| **1** | **9.1A** | **EditorShellV2 布局重建**。输入生命周期和 Gizmo 链路均已封版，适合推进 UI 布局。建议新 Shell 并行、先只接 Viewport。 |
| **2** | 9.1B | Transform 工具补全：旋转/缩放/局部-世界切换/吸附/撤销。 |
| **3** | 独立 | 地面十字标记退役（stash B 类）。 |
| **4** | 延后 | 性能监视器 + Probe 基础设施（stash C 类）。 |

## 八、变更清单

| 文件 | 行数 | 变更类型 |
|---|---|---|
| `docs/audit-gizmo-chain-9.0Y-3.md` | 新文件 | 封版验证报告 |

未修改任何源代码。

---

## 九、禁止项确认

- [x] 未修改 Vulkan
- [x] 未修改 EditorShell UI 布局
- [x] 未恢复性能监视器 / Probe
- [x] 未 stash pop
- [x] 未修改 Gizmo 数学逻辑
- [x] 未修改 MoveGizmoFrameSource 可见性
- [x] 未改 P2 产品确认项
- [x] 纯文档提交
