# ARCH-WORLD R6 架构退出门禁

版本：v0.2.19.9-rz
基线：`9113361` / `refactor/ARCH-WORLD-R5-render-contract`
目标：判断 ARCH-WORLD 是否可以退出纯架构治理，进入一个士兵 WarCore 闭环。

## 一、最终裁定

**ARCH-WORLD 可以退出。**

R6 未发现阻断项。测试程序集分层存在 D3 历史债务，但不阻挡从引擎架构治理转入游戏领域闭环；本轮不新增 `Render.Tests`，不大规模迁移既有 171 个测试，不修改生产架构。

## 二、测试程序集审计

当前只有 `XuanYu.Core.Tests` 与 `XuanYu.World.Tests` 两个测试程序集。两者均引用 `Editor` / `Editor.UI`，这是历史测试承载债务，而非 R5 新问题。`SceneRenderProjectionAdapterTests` 真实被测对象是 Editor/UI 组合边界到 Render.Abstractions 的适配器，放在 `Core.Tests` 不理想；但新增 `Render.Tests` 不正确，因为它并不测试 Render 后端。若后续清理测试分层，优先新增 `XuanYu.Editor.Tests` 承载 Editor / Editor.UI 组合测试。

| 测试文件 | 真实被测层 |
|---|---|
| CoreSmokeTests.cs | Core |
| EditorTool/EditorTransformCapturePolicyTests.cs | Editor.UI |
| Gizmo/MoveGizmoDragConstraintTests.cs | Core |
| Gizmo/MoveGizmoLayoutG1Tests.cs | Core |
| Gizmo/MoveGizmoLayoutTests.cs | Core |
| Gizmo/MoveGizmoLayoutVulkanTests.cs | Core |
| History/EditorHistoryOwnerTests.cs | Core |
| History/EditorHistoryRedoTests.cs | Core |
| History/TransformHistoryIntegrationTests.cs | Editor |
| History/TransformHistoryRedoIntegrationTests.cs | Editor |
| Picking/ViewportPickingServiceTests.cs | World-backed Picking integration |
| Render/SceneRenderProjectionAdapterTests.cs | Editor.UI + Render.Abstractions |
| Space/* | Core |
| Spatial/* in Core.Tests | Core |
| World.Tests/Spatial/* | World |
| World.Tests/Transform/TransformSessionTests.cs | Editor |
| World.Tests/World/EntityRegistryTests.cs | World |
| World.Tests/World/GlobalWorldTests.cs | World |
| World.Tests/World/WorldCameraFramingTests.cs | Editor.UI |
| World.Tests/World/WorldEntityBoundsSemanticsTests.cs | World |
| World.Tests/World/WorldPartition* | Mostly World, R1 variants include Editor Transform |
| World.Tests/World/WorldPartitionUiTests.cs | Editor.UI |
| World.Tests/World/WorldR1FinalSceneTests.cs | Editor |
| World.Tests/World/WorldR1FinalSelectionTests.cs | Editor.UI |
| World.Tests/World/WorldSceneConsumptionTests.cs | World |
| World.Tests/World/WorldSceneIsolationTests.cs | World |
| World.Tests/World/WorldSceneMultiEntityGateTests.cs | World |
| World.Tests/World/WorldSceneSelectionReentryTests.cs | Editor.UI |
| World.Tests/World/WorldSceneSingleAuthorityTests.cs | World |
| World.Tests/World/WorldSpatial* | World |
| World.Tests/World/WorldUi* | Editor.UI |

### R6 判断

- 放错程序集：存在，但范围广，属 D3 历史债务；不适合在退出门禁中局部移动一两个文件制造假干净。
- Render Projection 测试：确实不该长期留在 Core.Tests；但正确归宿是未来 `Editor.Tests`，不是 `Render.Tests`。
- 架构守卫：项目引用边界使用 csproj 解析；源代码禁用引用使用字符串扫描，是当前仓库轻量守卫策略，能防回退但不是完整类型系统边界。不构成退出阻断。
- 最小校正：不移动测试、不新增测试项目；将 D3 更新为非阻断退出后债务。

## 三、WarCore 一个士兵闭环入口

下一阶段最小链路：

```text
World Entity
  -> WarCore MilitaryIdentity
  -> FactionId / 最小组织归属
  -> 士兵可观察状态
  -> Editor / Render 显示投影
```

### 依赖边界

- `XuanYu.Core`：只保留通用数学、身份、空间、日志等基础机制；不写战争语义。
- `XuanYu.World`：只回答实体在哪里、是否存在、空间与分区状态；不回答军事身份。
- `XuanYu.WarCore`：回答实体属于谁、是什么军事身份、最小组织归属和可观察士兵状态。
- `XuanYu.Editor.UI`：只显示 WarCore 给出的状态投影，不成为军事事实源。
- `XuanYu.Render.*`：只消费最终可渲染投影，不认识 WarCore 规则。

### 第一轮只允许

- `MilitaryIdentity`
- `FactionId`
- 最小组织归属
- 一个士兵可观察状态投影

### 第一轮禁止

战斗结算、接触面、命令系统、后勤、国家系统、AI、完整编制树、通用 ECS 大重构。

## 四、R6 结论

ARCH-WORLD 主阶段可以关闭。后续不继续制造 R7/R8 架构阶段；剩余非阻断债务进入债务表，等真实功能需要时再处理。下一步建议从当前 clean HEAD 创建 `feat/WARCORE-one-soldier-loop`，进入一个士兵游戏领域闭环。
