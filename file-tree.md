# 玄域引擎文件树（自动重建）

> 由 `git ls-files` 全量重建；每个目录和 tracked 文件仅保留一句当前职责。

```text
├─ .gitattributes  # 保存对应模块的正式工程内容。
├─ .gitignore  # 保存对应模块的正式工程内容。
├─ AGENTS.md  # 记录对应主题的当前有效说明。
├─ changelog.md  # 已发生有效变化日志（版本+日期+验证，月度归档）
├─ docs/  # 集中项目治理、架构与阶段文档。
│  ├─ 玄域引擎_AI开发宪法.md  # 最高长期治理规则（唯一宪法事实源）
│  ├─ architecture/  # 组织该模块下的正式文件。
│  │  ├─ ENGINE_ARCHITECTURE.md  # 引擎总体架构说明
│  │  └─ world-a-r0-coordinate-contract.md  # 官方坐标合同（Z-Up、XY 水平、X×Y=Z）
│  ├─ archive/  # 组织该模块下的正式文件。
│  │  └─ changelog/  # 组织该模块下的正式文件。
│  │     ├─ changelog-2026-05.md  # 2026-05 changelog 月度归档
│  │     ├─ changelog-2026-06.md  # 2026-06 changelog 月度归档
│  │     └─ changelog-2026-07.md  # 2026-07 changelog 月度归档
│  ├─ CODE_CONSTITUTION.md  # 代码与架构硬规则
│  ├─ dev-rules.md  # 开发硬规则执行手册（接手红线清单）
│  ├─ docs-index.md  # docs 目录分类索引（哪类文档在哪里）
│  ├─ governance/  # 组织该模块下的正式文件。
│  │  ├─ 版本号规范与历史映射.md  # 版本格式与历史编号映射
│  │  ├─ debts/  # 组织该模块下的正式文件。
│  │  │  ├─ arch-ui-spec-debts.md  # 记录对应主题的当前规范、计划或审计事实。
│  │  │  └─ arch-world-debts.md  # 架构受控债务登记
│  │  ├─ dev-rules-understanding.md  # dev-rules 规则动机解释
│  │  ├─ diagnostic-safety.md  # 诊断日志与 UI 调度安全规范
│  │  ├─ NAMING_RULES.md  # 命名与品牌规范
│  │  ├─ naming-XuanYu-Engine.md  # 玄域引擎命名与品牌规范
│  │  ├─ ui-spec.md  # 记录对应主题的当前规范、计划或审计事实。
│  │  └─ xyui/  # XYUI 双 Agent 开发规范与越权监督入口。
│  │     ├─ README.md  # XYUI 双 Agent 规范优先级、所有权与交叉监督流程。
│  │     └─ XYUI_Codex_Gemini双Agent开发与代码封装规范_v1.0.md  # 用户提供的 XYUI 双 Agent Current Working Standard 原文。
│  ├─ knowledge/  # 集中正式工程知识与事故经验。
│  │  ├─ architecture.md  # 记录对应工程知识的当前有效内容。
│  │  ├─ data.md  # 记录对应工程知识的当前有效内容。
│  │  ├─ engineering.md  # 记录对应工程知识的当前有效内容。
│  │  ├─ incidents.md  # 记录对应工程知识的当前有效内容。
│  │  ├─ input.md  # 记录对应工程知识的当前有效内容。
│  │  ├─ knowledge-index.md  # 记录对应工程知识的当前有效内容。
│  │  ├─ lessons.md  # 类型化 Lesson、停止条件与错误前提复盘。
│  │  ├─ performance.md  # 记录对应工程知识的当前有效内容。
│  │  ├─ README.md  # 记录对应工程知识的当前有效内容。
│  │  ├─ rendering.md  # 记录对应工程知识的当前有效内容。
│  │  ├─ ui/  # 组织该模块下的正式文件。
│  │  │  └─ viewport-ui-control-development-guide.md  # Viewport UI 控件承载层与开发验收知识库。
│  │  └─ ui.md  # 记录对应工程知识的当前有效内容。
│  ├─ milestones/  # 组织该模块下的正式文件。
│  │  ├─ closed/  # 组织该模块下的正式文件。
│  │  │  ├─ MAP-A/  # 组织该模块下的正式文件。
│  │  │  │  └─ R2-closeout.md  # 记录对应主题的当前规范、计划或审计事实。
│  │  │  ├─ MAP-DATA-A/  # 组织该模块下的正式文件。
│  │  │  │  └─ R1-closeout.md  # 记录对应主题的当前规范、计划或审计事实。
│  │  │  └─ MAP-DOC-A/  # 组织该模块下的正式文件。
│  │  │     └─ R3-closeout.md  # 记录对应主题的当前规范、计划或审计事实。
│  │  └─ current/  # 组织该模块下的正式文件。
│  │     ├─ EDITOR-A/  # 组织该模块下的正式文件。
│  │     │  ├─ EDITOR-A-R1-workspace-contract.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ editor-a-r1-workspace-contract.svg  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ EDITOR-A-R2-workspace-switch.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ editor-a-r2-workspace-switch.svg  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ EDITOR-A-R3-F1-closeout.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ EDITOR-A-R3-F1-shell-compact.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ EDITOR-A-R3-mode-shell.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ editor-a-r3-mode-shell.svg  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  └─ XYUI-backlog.md  # 非阻塞 XYUI/UI 债务登记，记录 RegionPanel Binding 文本显示异常。
│  │     ├─ LAYER-A/  # 组织该模块下的正式文件。
│  │     │  └─ LAYER-A-R1-layer-shell.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     ├─ MAP-A/  # 组织该模块下的正式文件。
│  │     │  ├─ MAP-A-CLOSE-plan.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ MAP-A-strategic-closeout.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ map-contract.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ R3-backlog.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ R3-C2-closure.md  # C2 RF-M01～RF-M03 真机 IPO 收口记录。
│  │     │  ├─ R3-F1-closeout.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ viewport-overlay-development-plan.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  └─ viewport-overlay-roadmap.svg  # Viewport Overlay / Scale Indicator 浅色路线图。
│  │     ├─ MAP-DATA-A/  # 组织该模块下的正式文件。
│  │     │  ├─ MAP-DATA-A-R1-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ MAP-DATA-A-R1-F1-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ MAP-DATA-A-R1-F2-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ MAP-DATA-A-R1-F3-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ MAP-DATA-A-R2-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ MAP-DATA-A-R2-F1-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ MAP-DATA-A-R2-F1-plan.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ MAP-DATA-A-R2-F2-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ MAP-DATA-A-R2-F2-F2-F1-acceptance.md  # Visible Delete Dialog 中文 IPO 真机验收清单。
│  │     │  ├─ MAP-DATA-A-R2-F2-F2-F1-visible-delete-dialog.md  # Native HWND airspace 根因、最小修复和范围冻结记录。
│  │     │  ├─ MAP-DATA-A-R2-F2-F2-layer-delete-ui-lock-recovery-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ MAP-DATA-A-R2-F2-F2-layer-delete-ui-lock-recovery-plan.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ MAP-DATA-A-R2-F2-plan.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ MAP-DATA-A-R2-F2-region-pointer-safety-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ MAP-DATA-A-R2-F2-region-pointer-safety-plan.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  ├─ MAP-DATA-A-R2-F2-region-pointer-safety.svg  # 记录对应主题的当前规范、计划或审计事实。
│  │     │  └─ MAP-DATA-A-R2-plan.md  # 记录对应主题的当前规范、计划或审计事实。
│  │     └─ MAP-DOC-A/  # 组织该模块下的正式文件。
│  │        ├─ MAP-DOC-A-R1-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R1-F1-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R1-F1-carryover.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R1-plan.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R2-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R2-closeout.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R2-F1-root-cause.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R2-F2-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R2-F2-root-cause.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R2-F3-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R2-F3-root-cause.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R2-F4-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R2-F4-root-cause.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R2-plan.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R3-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R3-F2-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R3-F2-ui-closeout.svg  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R3-F3-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R3-F3-ui-spec-rework.svg  # 记录对应主题的当前规范、计划或审计事实。
│  │        ├─ MAP-DOC-A-R3-F4-acceptance.md  # 记录对应主题的当前规范、计划或审计事实。
│  │        └─ MAP-DOC-A-R3-plan.md  # Dataset Layer Editing 的冻结范围与验收边界。
│  └─ ui/  # 组织该模块下的正式文件。
│     ├─ 玄域引擎_旧UI审计矩阵.md  # 旧 UI 全量审计矩阵：违规 71 项 W01~W71 与结构性缺口 G01~G08 及清零追踪
│     ├─ 玄域引擎_UI规范_1.0.md  # 记录对应主题的当前规范、计划或审计事实。
│     ├─ 玄域引擎_UI真机基线清单.md  # 记录对应主题的当前规范、计划或审计事实。
│     ├─ ARCH-UI-SPEC-R1-D3_主窗口外壳与顶层页签.svg  # 记录对应主题的当前规范、计划或审计事实。
│     ├─ ARCH-UI-SPEC-R1-D4_工作面板治理.svg  # 记录对应主题的当前规范、计划或审计事实。
│     ├─ ARCH-UI-SPEC-R1-D4-F1_单行属性行修复.svg  # 记录对应主题的当前规范、计划或审计事实。
│     └─ ARCH-UI-SPEC-R1-D5_控件状态与弹窗通知治理.svg  # 记录对应主题的当前规范、计划或审计事实。
├─ file-tree.md  # 记录对应主题的当前有效说明。
├─ NuGet.Config  # api.nuget.org/v3/index.json" />
├─ run.bat  # 提供对应 Windows 启动或工具入口。
├─ samples/  # 组织该模块下的正式文件。
│  └─ world-c-r1-ten-triangles.xyscene  # 保存对应模块的正式工程内容。
├─ scripts/  # 集中工程治理、验证与生成脚本。
│  ├─ arch-a-guard-editor.ps1  # 提供对应工程治理、验证或生成流程。
│  ├─ arch-a-guard-render.ps1  # 提供对应工程治理、验证或生成流程。
│  ├─ arch-a-guard-warcore.ps1  # 提供对应工程治理、验证或生成流程。
│  ├─ arch-a-guard-world.ps1  # 提供对应工程治理、验证或生成流程。
│  ├─ arch-a-guard.ps1  # 提供对应工程治理、验证或生成流程。
│  └─ generate-ui-tokens.py  # 提供对应工程治理、验证或生成流程。
├─ XuanYu.Core/  # 组织该模块下的正式文件。
│  ├─ .gitkeep  # 保存对应模块的正式工程内容。
│  ├─ Diagnostics/  # 组织该模块下的正式文件。
│  │  └─ CoreSelfTest.cs  # static class CoreSelfTest
│  ├─ Gizmo/  # 组织该模块下的正式文件。
│  │  ├─ Common/  # 组织该模块下的正式文件。
│  │  │  └─ ScreenPoint.cs  # 实现对应模块的 C# 职责。
│  │  ├─ Move/  # 组织该模块下的正式文件。
│  │  │  ├─ MoveGizmoAxis.cs  # enum MoveGizmoAxis
│  │  │  ├─ MoveGizmoDragConstraint.Axes.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ MoveGizmoDragConstraint.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ MoveGizmoLayout.cs  # 可见轴杆线宽（DIP）。与 Vulkan 顶点着色器生成的 Gizmo 几何同尺度（审计实测约 2–3px）。
│  │  │  ├─ MoveGizmoLayout.Hit.cs  # sealed partial class MoveGizmoLayout
│  │  │  ├─ MoveGizmoLayout.Plane.cs  # sealed partial class MoveGizmoLayout
│  │  │  ├─ MoveGizmoPlane.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ MoveGizmoScreenSize.cs  # Move Gizmo 的屏幕恒定尺寸真源。CPU 布局与 Vulkan 绘制共用同一世界轴长。
│  │  │  └─ MoveGizmoSegment.cs  # 实现对应模块的 C# 职责。
│  │  ├─ Rotate/  # 组织该模块下的正式文件。
│  │  │  ├─ RotateGizmoAxis.cs  # enum RotateGizmoAxis
│  │  │  ├─ RotateGizmoDrag.cs  # 旋转拖拽解算：将指针在"垂直于旋转轴的平面"上的投影角度变化，映射为
│  │  │  ├─ RotateGizmoDrag.Math.cs  # 旋转解算的纯静态数学辅助，与实例状态分离的 partial。
│  │  │  ├─ RotateGizmoLayout.cs  # 旋转环世界半径默认值（与 MoveGizmo AxisLength=1.2 同尺度）。
│  │  │  ├─ RotateGizmoRing.cs  # 一条旋转环的屏幕折线几何。命中以"指针到折线最近距离"为唯一真源，
│  │  │  └─ RotateGizmoScreenRadius.cs  # 旋转环屏幕空间恒定尺寸换算：将目标 DIP 半径按相机深度与视口逻辑高度换算为世界半径。
│  │  └─ Scale/  # 组织该模块下的正式文件。
│  │     ├─ ScaleGizmoAxis.cs  # 单轴缩放手柄：修改实体自身 TRS 的局部 X / Y / Z 分量。
│  │     ├─ ScaleGizmoDrag.cs  # Scale Gizmo 拖拽解算：指数映射，倍率恒为正、不穿过零，且不逐帧累乘。
│  │     ├─ ScaleGizmoHitTester.cs  # CPU 命中布局与 Vulkan 绘制共用 ScaleGizmoLayout，保证“看见的位置 = 实际命中位置”。
│  │     ├─ ScaleGizmoLayout.cs  # Scale Gizmo 屏幕空间布局：三轴末端控制柄 + 中心等比控制柄。
│  │     └─ ScaleGizmoScreenSize.cs  # Scale Gizmo 屏幕空间恒定尺寸换算（与 RotateGizmoScreenRadius 同思路）。
│  ├─ History/  # 组织该模块下的正式文件。
│  │  ├─ EditorHistoryOwner.cs  # sealed class EditorHistoryOwner
│  │  └─ TransformHistoryEntry.cs  # 实现对应模块的 C# 职责。
│  ├─ Identity/  # 组织该模块下的正式文件。
│  │  └─ EntityId.cs  # 实现对应模块的 C# 职责。
│  ├─ Logging/  # 组织该模块下的正式文件。
│  │  ├─ EngineLogEntry.cs  # 实现对应模块的 C# 职责。
│  │  └─ EngineLogLevel.cs  # enum EngineLogLevel
│  ├─ Map/  # 组织该模块下的正式文件。
│  │  ├─ MapSurfaceKind.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapSurfaceSampler.cs  # 实现对应模块的 C# 职责。
│  │  └─ MapTerrainVertex.cs  # 实现对应模块的 C# 职责。
│  ├─ Math/  # 组织该模块下的正式文件。
│  │  ├─ Vector3d.cs  # 实现对应模块的 C# 职责。
│  │  └─ YawRotation.cs  # 实现对应模块的 C# 职责。
│  ├─ Picking/  # 组织该模块下的正式文件。
│  │  ├─ ViewportPickingRequest.cs  # 实现对应模块的 C# 职责。
│  │  ├─ ViewportPickingResult.cs  # 实现对应模块的 C# 职责。
│  │  └─ ViewportPickingService.cs  # static class ViewportPickingService
│  ├─ Properties/  # 组织该模块下的正式文件。
│  │  └─ AssemblyInfo.cs  # 实现对应模块的 C# 职责。
│  ├─ Results/  # 组织该模块下的正式文件。
│  │  ├─ EngineError.cs  # 实现对应模块的 C# 职责。
│  │  └─ EngineResult.cs  # 实现对应模块的 C# 职责。
│  ├─ Scene/  # 组织该模块下的正式文件。
│  │  ├─ CommittedTransform.cs  # 实现对应模块的 C# 职责。
│  │  ├─ ISceneRenderSnapshotSource.cs  # interface ISceneRenderSnapshotSource
│  │  ├─ SceneEntitySnapshot.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SceneRenderSnapshot.cs  # 实现对应模块的 C# 职责。
│  │  └─ SceneTransformCommitResult.cs  # 实现对应模块的 C# 职责。
│  ├─ Space/  # 组织该模块下的正式文件。
│  │  ├─ CameraState.cs  # 实现对应模块的 C# 职责。
│  │  ├─ DefaultEditorCamera.cs  # static class DefaultEditorCamera
│  │  ├─ ProjectionMode.cs  # 实现对应模块的 C# 职责。
│  │  ├─ ViewportState.cs  # 实现对应模块的 C# 职责。
│  │  ├─ ViewProjectionState.cs  # sealed class ViewProjectionState
│  │  ├─ ViewProjectionState.Projection.cs  # 世界点严格投影与失败安全 Try 投影 API。
│  │  ├─ WorldRay.cs  # 实现对应模块的 C# 职责。
│  │  └─ WorldRayFactory.cs  # 基于 CameraState 与 ViewportState 的双精度世界射线构造。
│  ├─ Spatial/  # 组织该模块下的正式文件。
│  │  ├─ RayAabbHit.cs  # 实现对应模块的 C# 职责。
│  │  ├─ RayAabbIntersection.cs  # static class RayAabbIntersection
│  │  ├─ SpatialAabb.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SpatialBounds.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SpatialQueryCategory.cs  # enum SpatialQueryCategory
│  │  ├─ SpatialQueryResult.cs  # sealed class SpatialQueryResult
│  │  ├─ SpatialQueryStats.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SpatialRayAabb.cs  # static class SpatialRayAabb
│  │  ├─ SpatialRaycastHit.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SpatialRaycastResult.cs  # sealed class SpatialRaycastResult
│  │  ├─ SpatialRaycastStats.cs  # 实现对应模块的 C# 职责。
│  │  └─ SpatialRayQuery.cs  # 实现对应模块的 C# 职责。
│  ├─ Time/  # 组织该模块下的正式文件。
│  │  ├─ SimulationTime.cs  # 实现对应模块的 C# 职责。
│  │  └─ TimeStep.cs  # 实现对应模块的 C# 职责。
│  ├─ Transform/  # 组织该模块下的正式文件。
│  │  ├─ PreviewTransform.cs  # 实现对应模块的 C# 职责。
│  │  └─ TransformStartSnapshot.cs  # 实现对应模块的 C# 职责。
│  └─ XuanYu.Core.csproj  # 配置对应 .NET 项目的构建与依赖。
├─ XuanYu.Core.Tests/  # 组织该模块下的正式文件。
│  ├─ Camera/  # 组织该模块下的正式文件。
│  │  ├─ CameraBasisTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ CameraFarRecoveryTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ CameraNavigationRollTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ CameraNavigationSequenceTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ CameraNavigationStressTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ CameraNavigationTests.cs  # sealed class CameraNavigationTests
│  │  ├─ CameraNavigationUiSequenceTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ CameraNavigationUiSequenceTests.Safety.cs  # 实现对应模块的 C# 职责。
│  │  ├─ CameraOrthographicNavigationTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  └─ FarProjectionSafetyTests.cs  # 验证对应模块的自动化行为与回归合同。
│  ├─ CoreSmokeTests.cs  # sealed class CoreSmokeTests
│  ├─ EditorTool/  # 组织该模块下的正式文件。
│  │  └─ EditorTransformCapturePolicyTests.cs  # sealed class EditorTransformCapturePolicyTests
│  ├─ Gizmo/  # 组织该模块下的正式文件。
│  │  ├─ MoveGizmoDragConstraintTests.cs  # sealed class MoveGizmoDragConstraintTests
│  │  ├─ MoveGizmoLayoutG1Tests.cs  # sealed partial class MoveGizmoLayoutTests
│  │  ├─ MoveGizmoLayoutPlaneTests.cs  # sealed partial class MoveGizmoLayoutTests
│  │  ├─ MoveGizmoLayoutTests.cs  # 命中半径必须由“可见几何 + 显式容差”派生，禁止再开大半径
│  │  ├─ MoveGizmoLayoutVulkanTests.cs  # sealed partial class MoveGizmoLayoutTests
│  │  ├─ MoveGizmoScreenSizeTests.cs  # sealed class MoveGizmoScreenSizeTests
│  │  ├─ RotateGizmoLayoutTests.cs  # 命中半径必须由“可见环几何 + 显式容差”派生，禁止再开大半径
│  │  ├─ ScaleGizmoTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ ScaleGizmoTests.Drag.cs  # sealed partial class ScaleGizmoTests
│  │  ├─ ScaleGizmoTests.DragSafety.cs  # sealed partial class ScaleGizmoTests
│  │  ├─ ScaleGizmoTests.Helpers.cs  # sealed partial class ScaleGizmoTests
│  │  └─ ScaleGizmoTests.R5R1.cs  # sealed partial class ScaleGizmoTests
│  ├─ History/  # 组织该模块下的正式文件。
│  │  ├─ EditorHistoryOwnerTests.cs  # sealed class EditorHistoryOwnerTests
│  │  ├─ EditorHistoryRedoTests.cs  # sealed class EditorHistoryRedoTests
│  │  ├─ TransformHistoryIntegrationTests.cs  # sealed class TransformHistoryIntegrationTests
│  │  └─ TransformHistoryRedoIntegrationTests.cs  # sealed class TransformHistoryRedoIntegrationTests
│  ├─ Picking/  # 组织该模块下的正式文件。
│  │  └─ ViewportPickingServiceTests.cs  # sealed class ViewportPickingServiceTests
│  ├─ Render/  # 组织该模块下的正式文件。
│  │  ├─ Camera/  # 组织该模块下的正式文件。
│  │  │  └─ StandardViewResolverTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ Diagnostics/  # 组织该模块下的正式文件。
│  │  │  └─ RenderLogNoiseContractTests.cs  # 日志高频噪声边界合同测试。
│  │  ├─ DrawPlan/  # 组织该模块下的正式文件。
│  │  │  ├─ CubeRenderDrawPlanTests.cs  # sealed class CubeRenderDrawPlanTests
│  │  │  ├─ FrameExecutionPolicyTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ RenderDrawPlanTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ SceneRenderProjectionAdapterTests.cs  # sealed partial class SceneRenderProjectionAdapterTests
│  │  │  ├─ SceneRenderProjectionAdapterTests.Rotation.cs  # sealed partial class SceneRenderProjectionAdapterTests
│  │  │  ├─ SceneRenderProjectionAdapterTests.Selection.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ ViewportAssistDrawPlanTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ ViewportChromeContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  └─ ViewportScaleIndicatorContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ Grid/  # 组织该模块下的正式文件。
│  │  │  ├─ ReferenceGridDrawPlanTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ ReferenceGridFrameStateTests.cs  # GRID-RW-2B：1/2/5 全帧 Step 与 24~80 DIP 回滞合同。
│  │  │  ├─ ReferenceGridShaderContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ ScaleIndicatorMetricTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  └─ ViewportMetricScaleTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ LatestRenderProjectionQueueTests.cs  # PointerMoved 多次发布时只消费最新渲染投影的合同测试。
│  │  ├─ Map/  # 组织该模块下的正式文件。
│  │  │  ├─ MapRegionDrawPlanTests.cs  # 区域渲染资源进入帧绘制计划的合同测试。
│  │  │  ├─ MapRenderDrawPlanTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ MapSurfaceGeometryTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ MapSurfaceLayerVisibilityTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ MapSurfaceResourceKeyTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  └─ MapSurfaceResourceUpdatePolicyTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ NavigationGizmo/  # 组织该模块下的正式文件。
│  │  │  ├─ NavigationGizmoDipContractTests.cs  # 导航 Gizmo DIP 尺寸与 DPI 缩放合同测试。
│  │  │  ├─ NavigationGizmoInputIsolationTests.cs  # STAB-1：可见 Gizmo 端点/轴线命中，空白区域不消费 Region 输入。
│  │  │  ├─ NavigationGizmoLayoutTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ NavigationGizmoLayoutTests.Facing.cs  # 实现对应模块的 C# 职责。
│  │  │  └─ NavigationGizmoOverlayContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ Overlay/  # 组织该模块下的正式文件。
│  │  │  ├─ ScaleIndicatorGlyphLiteTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  └─ ViewportOverlayLayoutTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  └─ StaticModels/  # 组织该模块下的正式文件。
│  │     ├─ RegionModelTransformContractTests.cs  # 实现对应模块的 C# 职责。
│  │     ├─ StaticModelDepthRegressionTests.cs  # sealed class StaticModelDepthRegressionTests
│  │     └─ StaticModelRenderContractTests.cs  # sealed class StaticModelRenderContractTests
│  ├─ Space/  # 组织该模块下的正式文件。
│  │  ├─ CameraOrthographicTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ CameraStateTests.cs  # sealed class CameraStateTests
│  │  ├─ DefaultEditorCameraTests.cs  # sealed class DefaultEditorCameraTests
│  │  ├─ SpaceAssert.cs  # 实现对应模块的 C# 职责。
│  │  ├─ ViewportStateTests.cs  # sealed class ViewportStateTests
│  │  ├─ ViewProjectionStateTests.cs  # sealed class ViewProjectionStateTests
│  │  ├─ WorldRayFactoryTests.cs  # sealed class WorldRayFactoryTests
│  │  └─ WorldRayTests.cs  # sealed class WorldRayTests
│  ├─ Spatial/  # 组织该模块下的正式文件。
│  │  ├─ RayAabbIntersectionTests.cs  # sealed class RayAabbIntersectionTests
│  │  ├─ SpatialBoundsTests.cs  # sealed class SpatialBoundsTests
│  │  └─ SpatialTestData.cs  # 实现对应模块的 C# 职责。
│  └─ XuanYu.Core.Tests.csproj  # 配置对应 .NET 项目的构建与依赖。
├─ XuanYu.Editor/  # 组织该模块下的正式文件。
│  ├─ Assets/  # 组织该模块下的正式文件。
│  │  ├─ Catalog/  # 组织该模块下的正式文件。
│  │  │  └─ SceneStaticModelCatalog.cs  # 实现对应模块的 C# 职责。
│  │  ├─ Hosting/  # 组织该模块下的正式文件。
│  │  │  ├─ HostedSceneAsset.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ ModelAssetRuntimeState.cs  # enum ModelAssetRuntimeState
│  │  │  ├─ Planning/  # 组织该模块下的正式文件。
│  │  │  │  ├─ SceneAssetHostingPlan.cs  # 实现对应模块的 C# 职责。
│  │  │  │  └─ SceneAssetHostingPlanner.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ SceneAssetHostingError.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ SceneAssetHostingState.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ SceneAssetPathPolicy.cs  # static class SceneAssetPathPolicy
│  │  │  └─ Transactions/  # 组织该模块下的正式文件。
│  │  │     ├─ SceneAssetHostingTransaction.Activate.cs  # 实现对应模块的 C# 职责。
│  │  │     ├─ SceneAssetHostingTransaction.Complete.cs  # 实现对应模块的 C# 职责。
│  │  │     ├─ SceneAssetHostingTransaction.cs  # 实现对应模块的 C# 职责。
│  │  │     └─ SceneAssetHostingTransaction.Rollback.cs  # 实现对应模块的 C# 职责。
│  │  ├─ Identity/  # 组织该模块下的正式文件。
│  │  │  └─ AssetId.cs  # 实现对应模块的 C# 职责。
│  │  ├─ Import/  # 组织该模块下的正式文件。
│  │  │  └─ Gltf/  # 组织该模块下的正式文件。
│  │  │     ├─ GlbContainer.cs  # 实现对应模块的 C# 职责。
│  │  │     ├─ GlbImportService.cs  # sealed class GlbImportService
│  │  │     ├─ GltfAccessorReader.cs  # 实现对应模块的 C# 职责。
│  │  │     ├─ GltfCoordinatePolicy.cs  # static class GltfCoordinatePolicy
│  │  │     ├─ GltfJsonAccess.cs  # 实现对应模块的 C# 职责。
│  │  │     ├─ GltfNodeTransform.cs  # 实现对应模块的 C# 职责。
│  │  │     ├─ GltfStaticModelImporter.cs  # 实现对应模块的 C# 职责。
│  │  │     └─ ImportStop.cs  # 实现对应模块的 C# 职责。
│  │  └─ StaticModels/  # 组织该模块下的正式文件。
│  │     ├─ SceneStaticModelBinding.cs  # 实现对应模块的 C# 职责。
│  │     ├─ StaticModelAuthoringService.cs  # sealed record StaticModelAuthorResult
│  │     ├─ StaticModelBuilder.cs  # 实现对应模块的 C# 职责。
│  │     ├─ StaticModelColor.cs  # 实现对应模块的 C# 职责。
│  │     ├─ StaticModelData.cs  # sealed record StaticModelData
│  │     ├─ StaticModelImportCodes.cs  # enum StaticModelImportErrorCode
│  │     ├─ StaticModelImportResult.cs  # sealed record StaticModelImportResult
│  │     ├─ StaticModelImportWarning.cs  # sealed record StaticModelImportWarning
│  │     ├─ StaticModelPrimitive.cs  # 实现对应模块的 C# 职责。
│  │     └─ StaticModelVertex.cs  # 实现对应模块的 C# 职责。
│  ├─ Camera/  # 组织该模块下的正式文件。
│  │  ├─ CameraBasis.cs  # 实现对应模块的 C# 职责。
│  │  ├─ CameraFarProjectionDiagnostic.cs  # 实现对应模块的 C# 职责。
│  │  ├─ CameraFrameResult.cs  # 实现对应模块的 C# 职责。
│  │  ├─ CameraNavigation.cs  # 实现对应模块的 C# 职责。
│  │  ├─ CameraNavigation.Far.cs  # 实现对应模块的 C# 职责。
│  │  ├─ CameraNavigation.Try.cs  # 实现对应模块的 C# 职责。
│  │  ├─ EditorCameraFraming.cs  # 实现对应模块的 C# 职责。
│  │  ├─ EditorCameraFraming.Draft.cs  # 实现对应模块的 C# 职责。
│  │  ├─ EditorCameraFraming.MapOrthographic.cs  # 实现对应模块的 C# 职责。
│  │  ├─ EditorCameraFraming.Orthographic.cs  # 实现对应模块的 C# 职责。
│  │  └─ OrthographicViewFactory.cs  # 实现对应模块的 C# 职责。
│  ├─ Layering/  # 组织该模块下的正式文件。
│  │  ├─ EditorLayerItem.cs  # 通用编辑图层项目与无领域语义的命令结果。
│  │  └─ IEditorLayerProvider.cs  # 编辑模式图层提供器通用合同：读取、选择、组织与状态操作。
│  ├─ MapDocument/  # 组织该模块下的正式文件。
│  │  ├─ DatasetLayerState.cs  # Dataset 图层显隐、锁定和连续顺序的唯一状态模型。
│  │  ├─ MapDatasetDescriptor.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDatasetDocument.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDatasetDocumentJson.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDatasetDocumentSerializer.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDatasetDocumentValidator.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDatasetFeatureBinding.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDatasetIdGenerator.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDatasetLayerIdProjection.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDatasetPathPolicy.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDatasetRegionBinding.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDatasetRegistry.Commands.cs  # Dataset Create/Register 生命周期命令。
│  │  ├─ MapDatasetRegistry.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDatasetRegistry.FeatureQuery.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDatasetRegistry.LayerStates.cs  # Dataset Layer State 的内存更新与连续顺序归一化。
│  │  ├─ MapDatasetRegistry.Query.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDatasetRegistry.RegionTransaction.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDatasetRegistry.Rename.cs  # Dataset 名称的内存更新与 Manifest 合同校验。
│  │  ├─ MapDatasetRegistry.Transaction.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDatasetRegistry.Unregister.cs  # Dataset 解除注册、锁定保护和状态归一化。
│  │  ├─ MapDatasetRuntimeProjection.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDatasetStorageService.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDocument.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDocumentAggregateBridge.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDocumentJson.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDocumentOwner.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDocumentResult.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDocumentValidator.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapEnvironmentDefinition.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapJsonMapper.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapJsonSerializer.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapManifest.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapManifestJson.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapManifestMapper.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapManifestOwner.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapManifestSerializer.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapManifestStorageService.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapManifestValidator.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapRegionDatasetCodec.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapRegionDatasetFeature.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapRoadDatasetCodec.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapRoadDatasetFeature.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapStorageService.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapWorkingStorage.cs  # 未保存地图的内部工作 Manifest 生命周期。
│  │  └─ MapWorkingStorage.Promotion.cs  # Working Dataset 到正式地图目录的提升事务。
│  ├─ MapEditing/  # 组织该模块下的正式文件。
│  │  ├─ MapEditEvents.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapEditReason.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapEditSession.ActiveLayer.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapEditSession.Commands.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapEditSession.Commit.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapEditSession.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapEditSession.Document.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapEditSession.Geometry.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapEditSession.History.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapEditSession.Layers.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapEditSession.Regions.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapEditSession.Roads.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapEditSession.RuntimeProjection.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapEditSession.Selection.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapGeometryEditTypes.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapGeometryHitTester.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapHistoryEntry.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapSelection.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapSelectionKind.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapSurfacePicker.cs  # 复用 ViewProjection 与 WorldRayFactory，按中心原点合同拾取地图平面 MapPoint。
│  │  ├─ RegionDrawingState.cs  # 区域绘制临时草稿、光标与首点闭合候选状态。
│  │  └─ RoadDrawingState.cs  # 实现对应模块的 C# 职责。
│  ├─ Mode/  # 组织该模块下的正式文件。
│  │  ├─ EditorModeId.cs  # 编辑器顶层模式标识：管理或编辑。
│  │  ├─ EditorModeManager.cs  # Manage/Edit Mode 的纯状态 Owner，不持有 Workspace 或渲染状态。
│  │  └─ EditorModeTransition.cs  # Mode 转换不可变结果与状态保留合同。
│  ├─ SceneDocument/  # 组织该模块下的正式文件。
│  │  ├─ MapReference.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SceneDocumentAsset.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SceneDocumentEntity.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SceneDocumentJson.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SceneDocumentLoadTransaction.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SceneDocumentMapper.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SceneDocumentResult.cs  # sealed record SceneDocumentResult
│  │  ├─ SceneDocumentSaveTransaction.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SceneDocumentSession.cs  # sealed class SceneDocumentSession
│  │  ├─ SceneDocumentSnapshot.cs  # sealed record SceneDocumentSnapshot
│  │  ├─ SceneDocumentValidator.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SceneDocumentValidator.MapReference.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SceneDocumentWorldBridge.cs  # static class SceneDocumentWorldBridge
│  │  ├─ SceneLoadCandidate.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SceneSaveOutcome.cs  # 实现对应模块的 C# 职责。
│  │  └─ SceneStorageService.cs  # sealed class SceneStorageService
│  ├─ Transform/  # 组织该模块下的正式文件。
│  │  ├─ TransformSession.cs  # sealed partial class TransformSession
│  │  ├─ TransformSession.Rotate.cs  # 旋转起始：与 Begin（移动）互斥，复用同一会话生命周期与提交/取消路径。
│  │  └─ TransformSession.Scale.cs  # 缩放起始：与 Begin（移动）/ BeginRotate（旋转）互斥，复用同一会话生命周期与提交/取消路径。
│  ├─ Workspace/  # 组织该模块下的正式文件。
│  │  ├─ EditorWorkspaceDefinition.cs  # 实现对应模块的 C# 职责。
│  │  ├─ EditorWorkspaceDefinitions.cs  # 实现对应模块的 C# 职责。
│  │  ├─ EditorWorkspaceId.cs  # 实现对应模块的 C# 职责。
│  │  ├─ EditorWorkspaceManager.cs  # 实现对应模块的 C# 职责。
│  │  ├─ EditorWorkspaceTool.cs  # 实现对应模块的 C# 职责。
│  │  ├─ EditorWorkspaceTransition.cs  # 实现对应模块的 C# 职责。
│  │  └─ RegionAuthoringMode.cs  # 实现对应模块的 C# 职责。
│  └─ XuanYu.Editor.csproj  # 配置对应 .NET 项目的构建与依赖。
├─ XuanYu.Editor.App/  # 组织该模块下的正式文件。
│  ├─ EditorCompositionRoot.cs  # static class EditorCompositionRoot
│  ├─ Program.cs  # 实现对应模块的 C# 职责。
│  └─ XuanYu.Editor.App.csproj  # 配置对应 .NET 项目的构建与依赖。
├─ XuanYu.Editor.UI/  # 组织该模块下的正式文件。
│  ├─ Accessibility/  # 组织该模块下的正式文件。
│  │  ├─ UiAutomationNamer.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiDpiContract.cs  # 实现对应模块的 C# 职责。
│  │  └─ UiMotionPreference.cs  # 实现对应模块的 C# 职责。
│  ├─ app.manifest  # 保存对应模块的正式工程内容。
│  ├─ Bootstrap/  # 组织该模块下的正式文件。
│  │  ├─ App.axaml  # github.com/avaloniaui"
│  │  ├─ App.axaml.cs  # sealed class App
│  │  └─ Program.cs  # WinExe 进程默认无控制台；AttachConsole(-1) 继承父终端（dotnet run 控制台），
│  ├─ Design/  # 组织该模块下的正式文件。
│  │  ├─ UiStyles.D4F1.axaml  # 定义对应 Avalonia 界面与资源。
│  │  ├─ UiStyles.D5.axaml  # 定义对应 Avalonia 界面与资源。
│  │  ├─ UiTokenManifest.json  # 保存对应模块的结构化数据。
│  │  ├─ UiTokens.axaml  # UI Token 聚合入口（合并 7 个 Token 文件；由 UiTokenManifest.json 生成，禁手改）
│  │  ├─ UiTokens.Colors.Components.axaml  # UI Token 组件色（日志/文档状态/图层，UI Spec 1.0 §4.3/§4.4/§12.2）
│  │  ├─ UiTokens.Colors.Core.axaml  # UI Token 核心语义色（四级背景/文字/强调/状态/对象，§4.1/§4.2）
│  │  ├─ UiTokens.Controls.axaml  # UI Token 控件尺寸（高度/宽度等级/热区/边框/焦点/阴影/日志列宽，§5.3/§6/§9/§13）
│  │  ├─ UiTokens.Fonts.axaml  # UI Token 字体（回退链/8 级字号行高/字重，§3.1/§3.2/§3.4）
│  │  ├─ UiTokens.Icons.axaml  # UI Token 图标（视口/笔画，§8.1）
│  │  ├─ UiTokens.Motion.axaml  # UI Token 动效时长（悬停/展开，§15.3）
│  │  └─ UiTokens.Spacing.axaml  # UI Token 间距/内边距/圆角（§5.1/§5.2/§5.4）
│  ├─ Dialogs/  # 组织该模块下的正式文件。
│  │  ├─ IEditorDialogService.cs  # 实现对应模块的 C# 职责。
│  │  └─ NullEditorDialogService.cs  # 实现对应模块的 C# 职责。
│  ├─ EditorState/  # 组织该模块下的正式文件。
│  │  ├─ EditorInteractionChangedResult.cs  # enum EditorInteractionChangeKind
│  │  ├─ EditorInteractionCommand.cs  # sealed record BeginInteractionCommand
│  │  ├─ EditorInteractionPointerSnapshot.cs  # 实现对应模块的 C# 职责。
│  │  ├─ EditorInteractionSnapshot.cs  # enum EditorInteractionPhase
│  │  ├─ EditorSelectionCommand.cs  # sealed record SelectEditorItemCommand
│  │  ├─ EditorSelectionSnapshot.cs  # sealed record EditorSelectionSnapshot
│  │  ├─ EditorStateChangedResult.cs  # enum EditorStateChangeKind
│  │  ├─ EditorStateOwner.cs  # sealed partial class EditorStateOwner
│  │  ├─ EditorStateOwner.Interaction.cs  # sealed partial class EditorStateOwner
│  │  ├─ EditorStateOwner.Tool.cs  # sealed partial class EditorStateOwner
│  │  ├─ EditorToolChangedResult.cs  # sealed record EditorToolChangedResult
│  │  ├─ EditorToolCommand.cs  # sealed record ChangeEditorToolCommand
│  │  ├─ EditorToolId.cs  # enum EditorToolId
│  │  ├─ EditorToolSnapshot.cs  # sealed record EditorToolSnapshot
│  │  ├─ EditorToolText.cs  # static class EditorToolText
│  │  └─ EditorTransformCapturePolicy.cs  # static class EditorTransformCapturePolicy
│  ├─ Foot/  # 组织该模块下的正式文件。
│  │  ├─ Foot.axaml  # github.com/avaloniaui"
│  │  ├─ Foot.axaml.cs  # LOG-UX-2：Foot.axaml.cs 只做接线——自动滚动 controller、日志选中、Ctrl+A/Ctrl+C。
│  │  ├─ Foot.States.axaml  # Foot 日志选中状态的模板 Presenter 样式覆盖。
│  │  ├─ LogAutoScrollPolicy.cs  # 实现对应模块的 C# 职责。
│  │  ├─ LogDetailPanel.axaml  # github.com/avaloniaui"
│  │  ├─ LogDetailPanel.axaml.cs  # partial class LogDetailPanel
│  │  ├─ LogListAutoScrollController.cs  # 实现对应模块的 C# 职责。
│  │  ├─ LogListAutoScrollController.Follow.cs  # 实现对应模块的 C# 职责。
│  │  ├─ LogListAutoScrollController.Layout.cs  # 实现对应模块的 C# 职责。
│  │  ├─ NotificationBar.axaml  # 定义对应 Avalonia 界面与资源。
│  │  └─ NotificationBar.axaml.cs  # 实现对应模块的 C# 职责。
│  ├─ Icons/  # 组织该模块下的正式文件。
│  │  └─ EditorIcons.axaml  # github.com/avaloniaui"
│  ├─ Left/  # 组织该模块下的正式文件。
│  │  ├─ InlineRenameActivation.cs  # static class InlineRenameActivation
│  │  ├─ Left.axaml  # github.com/avaloniaui"
│  │  ├─ Left.axaml.cs  # partial class Left
│  │  ├─ Left.EntityCommands.cs  # partial class Left
│  │  ├─ Left.Styles.axaml  # github.com/avaloniaui"
│  │  ├─ RegionalAuthoringPanel.axaml  # 定义对应 Avalonia 界面与资源。
│  │  ├─ RegionalAuthoringPanel.axaml.cs  # 实现对应模块的 C# 职责。
│  │  ├─ RegionPanel.axaml  # 定义对应 Avalonia 界面与资源。
│  │  ├─ RegionPanel.axaml.cs  # 实现对应模块的 C# 职责。
│  │  ├─ RoadPanel.axaml  # 定义对应 Avalonia 界面与资源。
│  │  └─ RoadPanel.axaml.cs  # 实现对应模块的 C# 职责。
│  ├─ Main/  # 组织该模块下的正式文件。
│  │  ├─ Main.axaml  # github.com/avaloniaui"
│  │  └─ Main.axaml.cs  # partial class Main
│  ├─ NativeHostResizeCoalescer.cs  # / <summary>
│  ├─ NativeHostResizeSnapshot.cs  # / <summary>
│  ├─ NativeHostSurfaceContract.cs  # VK3-A：把现有 NativeHost 生命周期快照映射为渲染层交接句柄。
│  ├─ RelayCommand.cs  # sealed class RelayCommand
│  ├─ Right/  # 组织该模块下的正式文件。
│  │  ├─ DatasetLayerPanel.axaml  # Dataset Layer Dock 的满宽行、状态操作、插入线与拖动热区。
│  │  ├─ DatasetLayerPanel.axaml.cs  # Dataset Layer 行选择和显隐/锁定命令转发。
│  │  ├─ DatasetLayerPanel.Drag.cs  # 右侧 Dataset Layer 的阈值拖拽、预览和插入目标计算。
│  │  ├─ DatasetPanel.axaml  # Dataset 左侧满宽列表、名称编辑、新建与解除注册入口。
│  │  ├─ DatasetPanel.axaml.cs  # Dataset 左侧选择与名称应用事件转发。
│  │  ├─ EditableFormLayoutModel.cs  # 实现对应模块的 C# 职责。
│  │  ├─ EditorLayerDock.axaml  # 定义对应 Avalonia 界面与资源。
│  │  ├─ EditorLayerDock.axaml.cs  # 通用图层 Dock 的展开/折叠 UI 状态。
│  │  ├─ EditorRightTabs.axaml  # 检查器/调试页签的复用宿主，供管理模式与编辑模式上下分栏。
│  │  ├─ EditorRightTabs.axaml.cs  # 复用页签宿主的 TopTabStripController 接线。
│  │  ├─ InspectorPanel.axaml  # 定义对应 Avalonia 界面与资源。
│  │  ├─ InspectorPanel.axaml.cs  # 实现对应模块的 C# 职责。
│  │  ├─ LayerInspectorPanel.axaml  # github.com/avaloniaui"
│  │  ├─ LayerInspectorPanel.axaml.cs  # 实现对应模块的 C# 职责。
│  │  ├─ LayerPanel.axaml  # github.com/avaloniaui"
│  │  ├─ LayerPanel.axaml.cs  # 实现对应模块的 C# 职责。
│  │  ├─ LayerPanel.DragDrop.cs  # 实现对应模块的 C# 职责。
│  │  ├─ LayerPanel.Rename.cs  # 实现对应模块的 C# 职责。
│  │  ├─ LayerPanel.States.axaml  # 图层行选中、可见与锁定状态的最终渲染样式。
│  │  ├─ MapEditorLayoutModel.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapEditorPanel.axaml  # github.com/avaloniaui"
│  │  ├─ MapEditorPanel.axaml.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapFormPanel.axaml  # 定义对应 Avalonia 界面与资源。
│  │  ├─ MapFormPanel.axaml.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapIdDisplayFormat.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapPagePanel.axaml  # 地图编辑器地图页及内部地图工具入口，含 Region Drawing 归属与 Selected 状态样式。
│  │  ├─ MapPagePanel.axaml.cs  # 实现对应模块的 C# 职责。
│  │  ├─ Right.axaml  # 顶层右侧页签，向 UiVm 同步当前页签以区分地图编辑模式。
│  │  ├─ Right.axaml.cs  # 实现对应模块的 C# 职责。
│  │  ├─ TopTabStripController.AllTabs.cs  # 实现对应模块的 C# 职责。
│  │  ├─ TopTabStripController.cs  # 实现对应模块的 C# 职责。
│  │  ├─ TopTabStripController.Hint.cs  # 实现对应模块的 C# 职责。
│  │  ├─ TopTabStripController.Visible.cs  # 实现对应模块的 C# 职责。
│  │  ├─ TopTabStripModel.cs  # 实现对应模块的 C# 职责。
│  │  └─ TopTabStripTemplate.axaml  # 定义对应 Avalonia 界面与资源。
│  ├─ Root/  # 组织该模块下的正式文件。
│  │  ├─ UiRoot.axaml  # 全局 Shell 布局，承载唯一 Main、常驻左右栏、资源底栏和日志。
│  │  └─ UiRoot.axaml.cs  # Row1 主工作区最低高度（与 axaml MinHeight 一致）
│  ├─ Top/  # 组织该模块下的正式文件。
│  │  ├─ Top.axaml  # 顶部命令、Manage/Edit Mode、编辑目标与上下文工具栏。
│  │  ├─ Top.axaml.cs  # partial class Top
│  │  └─ Top.States.axaml  # 顶部工具 ToggleButton 状态的模板 Presenter 样式覆盖。
│  ├─ TreeGuide.cs  # sealed class TreeGuide
│  ├─ TreeGuideSegment.cs  # enum TreeGuideSegmentKind
│  ├─ Ui.axaml  # github.com/avaloniaui"
│  ├─ Viewport/  # 组织该模块下的正式文件。
│  │  ├─ ViewNavigationGizmo.HitTest.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ ViewNavigationGizmo.Layout.cs  # 实现对应模块的 C# 职责。
│  │  └─ Vulkan/  # 组织该模块下的正式文件。
│  │     ├─ NativePointerMessage.cs  # 实现对应模块的 C# 职责。
│  │     ├─ NativePointerRoutePolicy.cs  # 实现对应模块的 C# 职责。
│  │     ├─ VulkanNativeHost.AvaloniaCamera.cs  # sealed partial class VulkanNativeHost
│  │     ├─ VulkanNativeHost.AvaloniaPointer.cs  # STAB-1：Avalonia 指针路径先交给 Navigation Gizmo，再进入 Region/Picking，并捕获/释放手势。
│  │     ├─ VulkanNativeHost.Bridge.cs  # sealed partial class VulkanNativeHost
│  │     ├─ VulkanNativeHost.CameraPointer.cs  # sealed partial class VulkanNativeHost
│  │     ├─ VulkanNativeHost.cs  # sealed partial class VulkanNativeHost
│  │     ├─ VulkanNativeHost.Dpi.cs  # sealed partial class VulkanNativeHost
│  │     ├─ VulkanNativeHost.Gizmo.cs  # sealed partial class VulkanNativeHost
│  │     ├─ VulkanNativeHost.LayoutSync.cs  # 实现对应模块的 C# 职责。
│  │     ├─ VulkanNativeHost.Log.cs  # 实现对应模块的 C# 职责。
│  │     ├─ VulkanNativeHost.NavGizmo.cs  # 实现对应模块的 C# 职责。
│  │     ├─ VulkanNativeHost.Picking.cs  # sealed partial class VulkanNativeHost
│  │     ├─ VulkanNativeHost.Pointer.Cancel.cs  # 实现对应模块的 C# 职责。
│  │     ├─ VulkanNativeHost.Pointer.cs  # sealed partial class VulkanNativeHost
│  │     ├─ VulkanViewport.axaml  # 定义对应 Avalonia 界面与资源。
│  │     ├─ VulkanViewport.axaml.cs  # partial class VulkanViewport
│  │     ├─ Win32ViewportHost.cs  # 通用 Vulkan 子 HWND 生命周期、尺寸与窗口过程，不承载 Viewport Overlay。
│  │     └─ Win32ViewportHost.Input.cs  # 实现对应模块的 C# 职责。
│  ├─ ViewportNativeHostRoute.cs  # static class ViewportNativeHostRoute
│  ├─ Vm/  # 组织该模块下的正式文件。
│  │  ├─ Camera/  # 组织该模块下的正式文件。
│  │  │  ├─ CameraSessionMode.cs  # enum CameraSessionMode
│  │  │  ├─ CameraSessionSnapshot.cs  # sealed record CameraSessionSnapshot
│  │  │  ├─ StandardViewResolver.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.Camera.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.Camera.Framing.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.Camera.Framing.Draft.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.CameraDolly.cs  # 地图编辑 Dolly 入口，在候选相机阶段触发极远安全诊断。
│  │  │  ├─ UiVm.CameraNavigation.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.FarProjectionDiagnostic.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.ScaleIndicator.cs  # 实现对应模块的 C# 职责。
│  │  │  └─ UiVm.ViewGizmo.cs  # 实现对应模块的 C# 职责。
│  │  ├─ History/  # 组织该模块下的正式文件。
│  │  │  ├─ UiVm.EntityCommands.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.History.cs  # sealed partial class UiVm
│  │  │  └─ UiVm.History.Entities.cs  # sealed partial class UiVm
│  │  ├─ Inspector/  # 组织该模块下的正式文件。
│  │  │  ├─ InspectorFieldRow.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.Inspector.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.InspectorInput.cs  # sealed partial class UiVm
│  │  │  └─ UiVm.InspectorInput.Parse.cs  # sealed partial class UiVm
│  │  ├─ Layer/  # 组织该模块下的正式文件。
│  │  │  ├─ EditorLayerProviderAdapter.cs  # 将当前 UiVm Region 图层会话适配为通用图层提供器。
│  │  │  └─ UiVm.LayerContext.cs  # 编辑模式当前图层提供器、Map 空状态与 Region 可见项目绑定。
│  │  ├─ Logging/  # 组织该模块下的正式文件。
│  │  │  ├─ DebugText.cs  # static class DebugText
│  │  │  ├─ EditorDisplayText.cs  # static class EditorDisplayText
│  │  │  ├─ EditorLogBuffer.cs  # sealed class EditorLogBuffer
│  │  │  ├─ EditorLogBus.cs  # sealed class EditorLogBus
│  │  │  ├─ EditorLogCategory.cs  # enum EditorLogCategory
│  │  │  ├─ EditorLogClipboardText.cs  # static class EditorLogClipboardText
│  │  │  ├─ EditorLogFilter.cs  # enum EditorLogFilter
│  │  │  ├─ EditorLogFilterQuery.cs  # static class EditorLogFilterQuery
│  │  │  ├─ EditorLogLevel.cs  # enum EditorLogLevel
│  │  │  ├─ EditorLogNoiseFilter.cs  # static class EditorLogNoiseFilter
│  │  │  ├─ EditorLogRepeatKey.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ EditorLogSource.cs  # enum EditorLogSource
│  │  │  ├─ EditorLogSummary.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ LogEntry.cs  # sealed record LogEntry
│  │  │  ├─ SampleLogEntries.cs  # static class SampleLogEntries
│  │  │  ├─ UiText.cs  # static class UiText
│  │  │  ├─ UiVm.Logging.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.Logging.Refresh.cs  # 实现对应模块的 C# 职责。
│  │  │  └─ UiVm.Logging.State.cs  # 实现对应模块的 C# 职责。
│  │  ├─ Map/  # 组织该模块下的正式文件。
│  │  │  ├─ MapDatasetRow.cs  # Dataset Layer/Inspector 投影使用的 Dataset 行快照与中文类型显示映射。
│  │  │  ├─ MapDatasetTypePresentation.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ MapLayerRowViewModel.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ MapLayerRowViewModel.Rename.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ MapRegionRenderProjection.cs  # 将正式区域和绘制草稿投影为静态模型渲染资源。
│  │  │  ├─ MapRenderSnapshotProjection.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ MapVectorOverlayBuilder.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ MapVectorOverlayBuilder.Finalize.cs  # Vector Overlay AABB 与稳定 revision 计算。
│  │  │  ├─ MapVectorOverlayBuilder.Road.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ MapVectorOverlayTriangulation.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapCommandRouting.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapCommandRouting.Danger.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapDanger.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapDataset.Commands.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapDataset.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapDataset.DrawingBootstrap.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapDataset.DrawingTarget.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapDataset.Inspector.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapDataset.LayerBridge.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapDataset.Logging.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapDataset.Name.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapDataset.RegionPresentation.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapDataset.RoadBootstrap.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapDataset.RoadPresentation.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapDataset.Routing.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapDataset.Selection.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapDiagnostics.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapDiagnostics.Format.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapEditor.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapEditor.Display.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapEditor.Validation.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapEditor.Validation.Rules.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapGeometryEditing.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapGeometryEditing.Helpers.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapHistory.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapLayerDiagnostics.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapLayerDrag.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapLayerInspector.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapLayers.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapLayerSelection.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapManifest.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapRender.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.MapWorld.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.RegionDrawing.Commit.cs  # 区域 Draft 闭合、提交成功与错误反馈。
│  │  │  ├─ UiVm.RegionDrawing.cs  # 区域绘制地面命中、Draft 顶点与失败安全预览输入。
│  │  │  ├─ UiVm.RegionDrawing.DraftHistory.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.RegionDrawing.Input.cs  # 区域绘制视口边界判断与地图表面拾取。
│  │  │  ├─ UiVm.RegionDrawing.Logging.cs  # 区域绘制开始、成功、取消与错误的低频中文日志。
│  │  │  ├─ UiVm.RoadDrawing.Commit.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.RoadDrawing.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.RoadDrawing.History.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.RoadDrawing.Logging.cs  # 实现对应模块的 C# 职责。
│  │  │  └─ UiVm.RoadTool.cs  # 实现对应模块的 C# 职责。
│  │  ├─ Mode/  # 组织该模块下的正式文件。
│  │  │  └─ UiVm.Mode.cs  # Manage/Edit Mode 的 UiVm 桥接、统一显示文字、输入取消与上下文保留。
│  │  ├─ Scene/  # 组织该模块下的正式文件。
│  │  │  ├─ D2StaticModelDemo.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ SceneHistoryEntry.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ SceneRenderProjectionAdapter.cs  # static class SceneRenderProjectionAdapter
│  │  │  ├─ StaticModelRenderAdapter.cs  # static class StaticModelRenderAdapter
│  │  │  ├─ UiVm.DocumentStatus.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.RenderProjection.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.Scene.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.SceneDocument.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.SceneDocument.New.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.SceneDocumentLog.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.SceneDocumentMapRef.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiVm.SceneDocumentSave.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.StaticModelImport.cs  # sealed partial class UiVm
│  │  │  └─ UiVm.WorldProjection.cs  # sealed partial class UiVm
│  │  ├─ Selection/  # 组织该模块下的正式文件。
│  │  │  ├─ UiVm.Picking.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.Selection.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.SelectionProjection.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.SelectionTrace.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.SelectionValidity.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.ViewportSelection.cs  # sealed partial class UiVm
│  │  │  └─ ViewportPickingLogFormatter.cs  # static class ViewportPickingLogFormatter
│  │  ├─ Transform/  # 组织该模块下的正式文件。
│  │  │  ├─ Move/  # 组织该模块下的正式文件。
│  │  │  │  ├─ UiVm.MoveGizmo.cs  # sealed partial class UiVm
│  │  │  │  ├─ UiVm.MoveGizmoLogging.cs  # sealed partial class UiVm
│  │  │  │  └─ UiVm.MoveGizmoScreenSize.cs  # sealed partial class UiVm
│  │  │  ├─ Rotate/  # 组织该模块下的正式文件。
│  │  │  │  └─ UiVm.RotateGizmo.cs  # sealed partial class UiVm
│  │  │  ├─ Scale/  # 组织该模块下的正式文件。
│  │  │  │  └─ UiVm.ScaleGizmo.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.InputGuards.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.Interaction.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.InteractionCancel.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.InteractionPointer.cs  # sealed partial class UiVm
│  │  │  ├─ UiVm.Tool.cs  # sealed partial class UiVm
│  │  │  └─ UiVm.ViewportAssist.cs  # sealed partial class UiVm
│  │  ├─ Tree/  # 组织该模块下的正式文件。
│  │  │  ├─ EditorTreeNode.cs  # sealed class EditorTreeNode
│  │  │  ├─ TreeGuideBuilder.cs  # static class TreeGuideBuilder
│  │  │  └─ UiVm.TreeCommands.cs  # sealed partial class UiVm
│  │  ├─ UiVm.cs  # sealed partial class UiVm
│  │  ├─ UiVm.NativeHostLifecycle.cs  # sealed partial class UiVm
│  │  ├─ UiVm.Notification.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiVm.NotificationLifetime.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiVm.RightPanel.cs  # 实现对应模块的 C# 职责。
│  │  └─ Workspace/  # 组织该模块下的正式文件。
│  │     ├─ UiVm.RegionAuthoring.cs  # 实现对应模块的 C# 职责。
│  │     └─ UiVm.Workspace.cs  # 编辑目标与活动 Workspace 切换桥接。
│  ├─ Win/  # 组织该模块下的正式文件。
│  │  ├─ DialogFocusTrap.cs  # 实现对应模块的 C# 职责。
│  │  ├─ LayerDeleteConfirmationWindow.axaml  # 图层删除的独立可见确认窗口视图。
│  │  ├─ LayerDeleteConfirmationWindow.axaml.cs  # 删除确认窗口的 Owner 模态结果、键盘取消与幂等完成行为。
│  │  ├─ UiWin.Accessibility.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiWin.axaml  # github.com/avaloniaui"
│  │  ├─ UiWin.axaml.cs  # partial class UiWin
│  │  ├─ UiWin.DialogHost.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiWin.DialogHost.Danger.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiWin.DialogHost.Input.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiWin.Dialogs.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiWin.EntityShortcuts.cs  # partial class UiWin
│  │  ├─ UiWin.MapCommands.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiWin.SceneCommands.cs  # partial class UiWin
│  │  ├─ UiWin.Shortcuts.cs  # 窗口快捷键路由，包含区域绘制 Enter 闭合与 Esc 取消入口。
│  │  └─ UiWin.UnsavedDialog.cs  # partial class UiWin
│  ├─ Workspace/  # 组织该模块下的正式文件。
│  │  ├─ WorkspaceSelector.axaml  # Manage 的唯一 Mode 控件，以及 Edit 的 Map/Region Chevron 菜单。
│  │  └─ WorkspaceSelector.axaml.cs  # Mode 主区域的双击路由；状态仍由 UiVm 管理。
│  └─ XuanYu.Editor.UI.csproj  # 配置对应 .NET 项目的构建与依赖。
├─ XuanYu.Editor.Win/  # 组织该模块下的正式文件。
│  ├─ MainForm.cs  # 实现对应模块的 C# 职责。
│  └─ XuanYu.Editor.Win.csproj  # 配置对应 .NET 项目的构建与依赖。
├─ XuanYu.Engine.slnx  # 登记解决方案中的正式项目。
├─ XuanYu.Render.Abstractions/  # 组织该模块下的正式文件。
│  ├─ EditorViewPlaneGridKind.cs  # 实现对应模块的 C# 职责。
│  ├─ EditorViewportAssistState.cs  # 实现对应模块的 C# 职责。
│  ├─ FrameExecutionPolicy.cs  # 实现对应模块的 C# 职责。
│  ├─ INativeHostSurfaceBridge.cs  # NativeHost 生命周期到 Surface 生命周期的交接契约。
│  ├─ INativeHostSurfaceBridgeFactory.cs  # 实现对应模块的 C# 职责。
│  ├─ IRenderProjectionSource.cs  # interface IRenderProjectionSource
│  ├─ LatestRenderProjectionQueue.cs  # PointerMoved 高频发布只保留最新 RenderProjection 的线程安全邮箱。
│  ├─ MapBoundsGeometry.cs  # 实现对应模块的 C# 职责。
│  ├─ MapRenderSnapshot.cs  # 实现对应模块的 C# 职责。
│  ├─ MapSurfaceGeometry.cs  # 实现对应模块的 C# 职责。
│  ├─ MapSurfaceResourceKey.cs  # 实现对应模块的 C# 职责。
│  ├─ MapSurfaceResourceUpdatePolicy.cs  # 实现对应模块的 C# 职责。
│  ├─ MapSurfaceResourceUpdateText.cs  # 实现对应模块的 C# 职责。
│  ├─ NativeHostHandleSnapshot.cs  # 实现对应模块的 C# 职责。
│  ├─ NativeHostLifecycleLogFormatter.cs  # 实现对应模块的 C# 职责。
│  ├─ NativeHostLifecycleProbe.cs  # 实现对应模块的 C# 职责。
│  ├─ NativeHostLifecycleState.cs  # 实现对应模块的 C# 职责。
│  ├─ NativeHostSurfaceHandle.cs  # NativeHost 交给渲染层的窗口交接句柄。
│  ├─ ReferenceGridFrameState.cs  # GRID-RW-2B：按 1/2/5 和 24~80 DIP 回滞选择的每帧唯一 World Grid Step。
│  ├─ ReferenceGridScale.cs  # 实现对应模块的 C# 职责。
│  ├─ RenderCameraProjection.cs  # 实现对应模块的 C# 职责。
│  ├─ RenderDrawPlan.cs  # 实现对应模块的 C# 职责。
│  ├─ RenderDrawPlan.Typed.cs  # 实现对应模块的 C# 职责。
│  ├─ RenderEntityProjection.cs  # 实现对应模块的 C# 职责。
│  ├─ RenderEntityType.cs  # enum RenderEntityType
│  ├─ RenderProjection.cs  # 渲染帧投影快照，携带相机、观察中心与各类渲染资源。
│  ├─ RenderProjectionResult.cs  # 实现对应模块的 C# 职责。
│  ├─ RenderStaticModelKey.cs  # 实现对应模块的 C# 职责。
│  ├─ RenderStaticModelPrimitive.cs  # 实现对应模块的 C# 职责。
│  ├─ RenderStaticModelResource.cs  # sealed record RenderStaticModelResource
│  ├─ RenderStaticModelTransform.cs  # 静态模型位置、旋转与缩放变换合同，提供单位变换。
│  ├─ RenderStaticModelVertex.cs  # 实现对应模块的 C# 职责。
│  ├─ RenderVectorOverlayKey.cs  # 实现对应模块的 C# 职责。
│  ├─ RenderVectorOverlayPrimitive.cs  # 实现对应模块的 C# 职责。
│  ├─ RenderVectorOverlayResource.cs  # 实现对应模块的 C# 职责。
│  ├─ RenderVectorOverlayVertex.cs  # 实现对应模块的 C# 职责。
│  ├─ ScaleIndicatorGlyphLite.cs  # 实现对应模块的 C# 职责。
│  ├─ ScaleIndicatorMetric.cs  # 实现对应模块的 C# 职责。
│  ├─ ScaleIndicatorOverlayProjection.cs  # 实现对应模块的 C# 职责。
│  ├─ ViewportMetricScale.cs  # 计算视口 X/Y 方向公制尺度；不可逆 VP 时返回失败而不抛异常。
│  ├─ ViewportOverlayAnchor.cs  # 实现对应模块的 C# 职责。
│  ├─ ViewportOverlayLayoutResolver.cs  # 实现对应模块的 C# 职责。
│  └─ XuanYu.Render.Abstractions.csproj  # 配置对应 .NET 项目的构建与依赖。
├─ XuanYu.Render.Vulkan/  # 组织该模块下的正式文件。
│  ├─ Bridge/  # 组织该模块下的正式文件。
│  │  ├─ VulkanBridgeDeviceAttachStep.cs  # VK4-B：在 VK4-A 物理设备选择成功后，基于其选择结果创建 LogicalDevice（VkDevice + 队列）。
│  │  ├─ VulkanBridgePhysicalDeviceAttachStep.cs  # 实现对应模块的 C# 职责。
│  │  ├─ VulkanBridgeRenderSessionAttachStep.cs  # VK4-D：把 RenderSession 创建从 Bridge 抽离，Bridge 只委托，不内联 VK4-D 细节。
│  │  └─ VulkanBridgeSwapchainAttachStep.cs  # VK4-C：在设备 step 之后链式驱动 Swapchain 创建（Swapchain + Images + ImageViews）。
│  ├─ Device/  # 组织该模块下的正式文件。
│  │  ├─ VulkanDeviceOwner.cs  # VK4-B：LogicalDevice 持有者。基于 VK4-A 的 VulkanPhysicalDeviceSelection 创建 VkDevice 与队列。
│  │  ├─ VulkanDeviceOwner.Physical.cs  # 实现对应模块的 C# 职责。
│  │  ├─ VulkanPhysicalDeviceInfo.cs  # VK4-A：纯数据物理设备信息。仅描述候选设备，不持有任何 Vulkan 句柄（VkPhysicalDevice 不外露）。
│  │  ├─ VulkanPhysicalDeviceSelection.cs  # VK4-A：物理设备选择结果（纯数据，渲染层）。Success 为 true 时 Handle / Device / Queue 非空。
│  │  ├─ VulkanPhysicalDeviceSelector.cs  # VK4-A：物理设备选择器。在已有 Instance + Surface 前提下枚举并选择可用于渲染/呈现的设备。
│  │  └─ VulkanQueueFamilySelection.cs  # VK4-A：纯数据队列族选择结果。索引为 -1 表示未找到对应能力。
│  ├─ Diagnostic/  # 组织该模块下的正式文件。
│  │  └─ VulkanResizeTracer.cs  # 实现对应模块的 C# 职责。
│  ├─ Pipeline/  # 组织该模块下的正式文件。
│  │  ├─ ShaderBytecode.Frag.cs  # 实现对应模块的 C# 职责。
│  │  ├─ ShaderBytecode.GridLineFrag.cs  # GRID-RW-1：由世界线片元 GLSL 生成的 SPIR-V 字节码。
│  │  ├─ ShaderBytecode.GridLineVert.cs  # GRID-RW-1：由世界线顶点 GLSL 生成的 SPIR-V 字节码。
│  │  ├─ ShaderBytecode.GridVert.cs  # 实现对应模块的 C# 职责。
│  │  ├─ ShaderBytecode.NavGizmoFrag.cs  # 实现对应模块的 C# 职责。
│  │  ├─ ShaderBytecode.NavGizmoVert.cs  # AUTO-GENERATED from editor_nav_gizmo.vert / editor_nav_gizmo.frag / editor_world_origin.frag (glslc -O)
│  │  ├─ ShaderBytecode.ScaleIndicatorFrag.cs  # 实现对应模块的 C# 职责。
│  │  ├─ ShaderBytecode.Vert.cs  # STAB-4C：由 glslc -O 从 scene.vert 生成的直接 ViewProjection 字节码。
│  │  ├─ ShaderBytecode.ViewPlaneGridFrag.cs  # 实现对应模块的 C# 职责。
│  │  ├─ ShaderBytecode.WorldAxesFrag.cs  # 实现对应模块的 C# 职责。
│  │  ├─ ShaderBytecode.WorldOriginFrag.cs  # AUTO-GENERATED from editor_nav_gizmo.vert / editor_nav_gizmo.frag / editor_world_origin.frag (glslc -O)
│  │  ├─ ShaderBytecode.WorldReferenceGridFrag.cs  # GRID-RW-2A：独立 World XY 固定网格片元 GLSL 的 SPIR-V 字节码。
│  │  ├─ VulkanGraphicsPipelineOwner.cs  # 实现对应模块的 C# 职责。
│  │  ├─ VulkanGraphicsPipelineOwner.Depth.cs  # STAB-3：主场景与 Vector Overlay 可分别配置深度测试/写入策略。
│  │  ├─ VulkanGraphicsPipelineOwner.Fullscreen.cs  # 实现对应模块的 C# 职责。
│  │  ├─ VulkanGraphicsPipelineOwner.Grid.cs  # 实现对应模块的 C# 职责。
│  │  ├─ VulkanGraphicsPipelineOwner.GridLine.cs  # GRID-RW-1-CORR2：参考网格专用 Empty-input LineList 管线（无顶点绑定、负 Depth Bias）。
│  │  ├─ VulkanGraphicsPipelineOwner.Sky.cs  # 实现对应模块的 C# 职责。
│  │  ├─ VulkanGraphicsPipelineOwner.StaticModelInput.cs  # 实现对应模块的 C# 职责。
│  │  ├─ VulkanPipelineLogFormatter.cs  # VK5-A：GraphicsPipeline 资源中文日志格式器。仅生成字符串，经注入的 Action<string> log 回调输出（日志单出口）。
│  │  ├─ VulkanScenePushConstants.cs  # std140 布局：
│  │  └─ VulkanShaderModuleOwner.cs  # VK5-A：ShaderModule 创建助手。创建后由 GraphicsPipelineOwner 在管道建好后立即释放（短生命周期，不持有到会话结束）。
│  ├─ Render/  # 组织该模块下的正式文件。
│  │  ├─ ClearFrame/  # 组织该模块下的正式文件。
│  │  │  ├─ VulkanClearFrameLogFormatter.cs  # VK4-D：单色清屏日志格式化（统一经 Bridge 的 Emit 单出口）。
│  │  │  ├─ VulkanClearFrameOwner.Commands.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanClearFrameOwner.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanClearFrameOwner.Lifecycle.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanClearFrameOwner.Matrix.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanClearFrameOwner.PipelineBind.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanClearFrameOwner.PushConstants.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanClearFrameOwner.Resources.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanClearFrameOwner.Trace.cs  # 实现对应模块的 C# 职责。
│  │  │  └─ VulkanClearFrameOwner.VectorOverlayPipeline.cs  # STAB-3：持有独立 Vector Overlay 管线并在命令缓冲重录时注入。
│  │  ├─ Grid/  # 组织该模块下的正式文件。
│  │  │  ├─ VulkanClearFrameOwner.Grid.cs  # GRID-RW-2B：以全屏三角形绘制帧级统一 Step 的 World XY 网格。
│  │  │  ├─ VulkanClearFrameOwner.GridScale.cs  # GRID-RW-2A：网格公制计算固定消费 World XY 的 Z=0 平面。
│  │  │  ├─ VulkanClearFrameOwner.NavGizmo.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanClearFrameOwner.ScaleIndicator.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanClearFrameOwner.ViewPlaneGrid.cs  # 实现对应模块的 C# 职责。
│  │  │  └─ VulkanClearFrameOwner.WorldAxes.cs  # 实现对应模块的 C# 职责。
│  │  ├─ Map/  # 组织该模块下的正式文件。
│  │  │  └─ VulkanClearFrameOwner.MapSurface.cs  # 实现对应模块的 C# 职责。
│  │  ├─ Present/  # 组织该模块下的正式文件。
│  │  │  ├─ VulkanPresentLoop.cs  # VK-LIFE-1：Present 泵必须确认停止成功后，才允许释放同步对象。
│  │  │  ├─ VulkanPresentLoop.Frame.cs  # 实现对应模块的 C# 职责。
│  │  │  └─ VulkanPresentLoop.Lifecycle.cs  # 实现对应模块的 C# 职责。
│  │  ├─ Scene/  # 组织该模块下的正式文件。
│  │  │  ├─ VulkanClearFrameOwner.Draw.cs  # GRID-DIAG-GROUND-01：在管线绑定前暂时跳过 MapGround，隔离地面绘制与深度写入供真机诊断。
│  │  │  ├─ VulkanClearFrameOwner.DrawAssist.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanClearFrameOwner.DrawGizmo.cs  # 实现对应模块的 C# 职责。
│  │  │  └─ VulkanClearFrameOwner.Scene.cs  # 实现对应模块的 C# 职责。
│  │  ├─ StaticModels/  # 组织该模块下的正式文件。
│  │  │  ├─ VulkanClearFrameOwner.DrawStaticBounds.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanClearFrameOwner.DrawStaticModel.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanStaticModelBuffer.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanStaticModelCache.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanStaticModelFailureTracker.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanStaticModelLog.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanStaticModelResource.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanStaticModelValidator.cs  # 实现对应模块的 C# 职责。
│  │  │  └─ VulkanStaticModelVertex.cs  # 实现对应模块的 C# 职责。
│  │  ├─ VectorOverlay/  # 组织该模块下的正式文件。
│  │  │  ├─ VulkanClearFrameOwner.DrawVectorOverlay.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanVectorOverlayBufferReusePolicy.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanVectorOverlayCache.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanVectorOverlayResource.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ VulkanVectorOverlayValidator.cs  # 实现对应模块的 C# 职责。
│  │  │  └─ VulkanVectorOverlayVertex.cs  # 实现对应模块的 C# 职责。
│  │  └─ VulkanDepthAttachment.cs  # 实现对应模块的 C# 职责。
│  ├─ Session/  # 组织该模块下的正式文件。
│  │  ├─ GridPipelineSet.cs  # 实现对应模块的 C# 职责。
│  │  ├─ VulkanRenderSession.cs  # VK-LIFE-1：组合根负责失败回滚，不把半初始化资源留给 Bridge。
│  │  ├─ VulkanRenderSession.Lifecycle.cs  # sealed partial class VulkanRenderSession
│  │  ├─ VulkanRenderSession.Recover.cs  # sealed partial class VulkanRenderSession
│  │  ├─ VulkanRenderSession.Resize.cs  # sealed partial class VulkanRenderSession
│  │  └─ VulkanRenderSession.VectorOverlay.cs  # STAB-3：创建并挂接无深度测试/无深度写入的 Vector Overlay 管线。
│  ├─ Shaders/  # 组织该模块下的正式文件。
│  │  ├─ editor_nav_gizmo.frag  # 玄域编辑器：Blender 风格导航 Gizmo
│  │  ├─ editor_nav_gizmo.vert  # 保存对应模块的正式工程内容。
│  │  ├─ editor_reference_grid_line.frag  # GRID-RW-1：固定颜色与 Alpha 的世界线片元 Shader。
│  │  ├─ editor_reference_grid_line.vert  # GRID-RW-1：按 gl_VertexIndex 生成相机吸附世界线的顶点 Shader。
│  │  ├─ editor_reference_grid.vert  # 保存对应模块的正式工程内容。
│  │  ├─ editor_scale_indicator.frag  # 保存对应模块的正式工程内容。
│  │  ├─ editor_view_plane_grid.frag  # 保存对应模块的正式工程内容。
│  │  ├─ editor_world_axes.frag  # 保存对应模块的正式工程内容。
│  │  ├─ editor_world_origin.frag  # 保存对应模块的正式工程内容。
│  │  ├─ editor_world_reference_grid.frag  # GRID-RW-2B：世界射线与 Z=0 平面求交、CPU 全帧 Step、fwidth 仅抗锯齿。
│  │  ├─ scene.frag  # 保存对应模块的正式工程内容。
│  │  └─ scene.vert  # 保存对应模块的正式工程内容。
│  ├─ Swapchain/  # 组织该模块下的正式文件。
│  │  ├─ VulkanSwapchainBuilder.cs  # VK4-C：Swapchain 构建细节（创建 Swapchain + 取 Images + 建 ImageViews）。纯逻辑，不持有状态。
│  │  ├─ VulkanSwapchainCapabilities.cs  # VK4-C：Swapchain 能力查询（纯数据，不创建 Swapchain）。
│  │  ├─ VulkanSwapchainLogFormatter.cs  # VK4-C：Swapchain 中文生命周期日志格式器。纯文本，无副作用。
│  │  ├─ VulkanSwapchainOwner.Accessors.cs  # 实现对应模块的 C# 职责。
│  │  └─ VulkanSwapchainOwner.cs  # 实现对应模块的 C# 职责。
│  ├─ VulkanApiProbe.cs  # 实现对应模块的 C# 职责。
│  ├─ VulkanBridgeLogFormatter.cs  # 实现对应模块的 C# 职责。
│  ├─ VulkanDeviceInfo.cs  # sealed record VulkanDeviceInfo
│  ├─ VulkanInstanceCreateInfoBuilder.cs  # VK3-B1：Instance 创建信息构造辅助。仅构造 InstanceCreateInfo（含最小扩展集），不直接调用 Vulkan。
│  ├─ VulkanInstanceExtensions.cs  # VK3-B1：Instance 启用的最小扩展名集合（仅 surface 相关，以 null 结尾字节序列）。
│  ├─ VulkanInstanceLogFormatter.cs  # VK3-B1：Vulkan Instance 生命周期中文日志格式器。纯文本生成，无副作用。
│  ├─ VulkanInstanceOwner.cs  # 实现对应模块的 C# 职责。
│  ├─ VulkanInstanceResult.cs  # VK3-B1：Vulkan Instance 创建结果。Owner 非空表示创建成功。
│  ├─ VulkanNativeHostSurfaceBridge.Attach.cs  # sealed partial class VulkanNativeHostSurfaceBridge
│  ├─ VulkanNativeHostSurfaceBridge.cs  # VK-LIFE-1：Attach 全成功后才写入字段；失败按现有释放顺序回滚。
│  ├─ VulkanNativeHostSurfaceBridge.Lifecycle.cs  # sealed partial class VulkanNativeHostSurfaceBridge
│  ├─ VulkanNativeHostSurfaceBridge.Resize.cs  # sealed partial class VulkanNativeHostSurfaceBridge
│  ├─ VulkanNativeHostSurfaceBridge.Scene.cs  # sealed partial class VulkanNativeHostSurfaceBridge
│  ├─ VulkanNativeHostSurfaceBridgeFactory.cs  # 实现对应模块的 C# 职责。
│  ├─ VulkanProbeLogFormatter.cs  # static class VulkanProbeLogFormatter
│  ├─ VulkanProbeResult.cs  # sealed record VulkanProbeResult
│  ├─ VulkanSurfaceLogFormatter.cs  # VK3-B2：Vulkan Surface 生命周期中文日志格式器。纯文本生成，无副作用。
│  ├─ VulkanSurfaceOwner.cs  # 实现对应模块的 C# 职责。
│  ├─ VulkanSurfaceResult.cs  # VK3-B2：Vulkan Surface 创建结果。Owner 非空表示创建成功。
│  └─ XuanYu.Render.Vulkan.csproj  # 配置对应 .NET 项目的构建与依赖。
├─ XuanYu.WarCore/  # 组织该模块下的正式文件。
│  ├─ Identity/  # 组织该模块下的正式文件。
│  │  ├─ FactionId.cs  # / <summary>
│  │  ├─ MilitaryIdentity.cs  # / <summary>
│  │  ├─ OrganizationId.cs  # / <summary>
│  │  ├─ UnitId.cs  # / <summary>
│  │  └─ UnitKind.cs  # / <summary>
│  ├─ State/  # 组织该模块下的正式文件。
│  │  └─ SoldierState.cs  # / <summary>
│  └─ XuanYu.WarCore.csproj  # 配置对应 .NET 项目的构建与依赖。
├─ XuanYu.WarCore.Tests/  # 组织该模块下的正式文件。
│  ├─ Identity/  # 组织该模块下的正式文件。
│  │  └─ MilitaryIdentityTests.cs  # 验证对应模块的自动化行为与回归合同。
│  ├─ State/  # 组织该模块下的正式文件。
│  │  └─ SoldierStateTests.cs  # 验证对应模块的自动化行为与回归合同。
│  ├─ WarCoreDependencyTests.cs  # 验证对应模块的自动化行为与回归合同。
│  └─ XuanYu.WarCore.Tests.csproj  # 配置对应 .NET 项目的构建与依赖。
├─ XuanYu.World/  # 组织该模块下的正式文件。
│  ├─ EntityRegistry.Authoring.cs  # sealed partial class EntityRegistry
│  ├─ EntityRegistry.cs  # sealed partial class EntityRegistry
│  ├─ EntityRegistry.Replace.cs  # sealed partial class EntityRegistry
│  ├─ GlobalWorld.Authoring.cs  # sealed partial class GlobalWorld
│  ├─ GlobalWorld.cs  # sealed partial class GlobalWorld
│  ├─ GlobalWorld.Query.cs  # sealed partial class GlobalWorld
│  ├─ GlobalWorld.Snapshot.cs  # sealed partial class GlobalWorld
│  ├─ GridWorldPartitionStrategy.cs  # sealed class GridWorldPartitionStrategy
│  ├─ IWorldPartitionStrategy.cs  # interface IWorldPartitionStrategy
│  ├─ Map/  # 组织该模块下的正式文件。
│  │  ├─ MapBounds.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapCoordinateContract.cs  # MapPoint 与世界 XY 的唯一直接映射合同。
│  │  ├─ MapDefaultDefinition.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDefinition.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapDefinitionValidator.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapGeometry.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapId.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapLayer.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapLayerId.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapLayerKind.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapLayerRules.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapLayerStack.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapLayerValidator.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapRegion.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapRegionDraft.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapRegionId.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapRegionIntersection.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapRegionKind.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapRegionValidator.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapRoad.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapRoadDraft.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapRoadId.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapRoadValidator.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapSurfaceDefinition.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapValidationResult.cs  # 实现对应模块的 C# 职责。
│  │  ├─ WorldMapState.cs  # 实现对应模块的 C# 职责。
│  │  └─ WorldMapStateOwner.cs  # 实现对应模块的 C# 职责。
│  ├─ RegionKey.cs  # 实现对应模块的 C# 职责。
│  ├─ Scene/  # 组织该模块下的正式文件。
│  │  ├─ SceneSpatialBoundsProjection.cs  # static class SceneSpatialBoundsProjection
│  │  ├─ SceneStateOwner.cs  # Placeholder scene entities declare their OWN spatial extent (1
│  │  ├─ SceneStateOwner.Lifecycle.cs  # sealed partial class SceneStateOwner
│  │  ├─ SceneStateOwner.Seeding.cs  # sealed partial class SceneStateOwner
│  │  ├─ SceneStateOwner.StaticModel.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SceneStateOwner.Transform.cs  # sealed partial class SceneStateOwner
│  │  └─ SceneWorldProjection.cs  # static class SceneWorldProjection
│  ├─ Spatial/  # 组织该模块下的正式文件。
│  │  ├─ DynamicAabbTree.cs  # sealed partial class DynamicAabbTree
│  │  ├─ DynamicAabbTree.Insert.cs  # sealed partial class DynamicAabbTree
│  │  ├─ DynamicAabbTree.Node.cs  # sealed partial class DynamicAabbTree
│  │  ├─ DynamicAabbTree.Query.cs  # sealed partial class DynamicAabbTree
│  │  ├─ DynamicAabbTree.Refit.cs  # sealed partial class DynamicAabbTree
│  │  ├─ DynamicAabbTree.Remove.cs  # sealed partial class DynamicAabbTree
│  │  ├─ ISpatialIndex.cs  # interface ISpatialIndex
│  │  ├─ SpatialIndexOwner.cs  # sealed class SpatialIndexOwner
│  │  └─ SpatialRaycastResolver.cs  # sealed class SpatialRaycastResolver
│  ├─ WorldEntityActivity.cs  # enum WorldEntityActivity
│  ├─ WorldEntityName.cs  # static class WorldEntityName
│  ├─ WorldEntitySnapshot.cs  # 实现对应模块的 C# 职责。
│  ├─ WorldEntityType.cs  # enum WorldEntityType
│  ├─ WorldPartitionEntry.cs  # 实现对应模块的 C# 职责。
│  ├─ WorldPartitionMembership.cs  # sealed class WorldPartitionMembership
│  ├─ WorldQuery.cs  # Mutation is reserved
│  └─ XuanYu.World.csproj  # 配置对应 .NET 项目的构建与依赖。
├─ XuanYu.World.Tests/  # 组织该模块下的正式文件。
│  ├─ Assets/  # 组织该模块下的正式文件。
│  │  ├─ AssetContractTests.cs  # sealed class AssetContractTests
│  │  ├─ AssetDialogTests.cs  # sealed class AssetDialogTests
│  │  ├─ GlbFactory.cs  # 实现对应模块的 C# 职责。
│  │  ├─ GlbImportTests.cs  # sealed class GlbImportTests
│  │  ├─ GlbMultiPrimitiveFactory.cs  # 实现对应模块的 C# 职责。
│  │  ├─ HostingCompleteTests.cs  # sealed class HostingCompleteTests
│  │  ├─ HostingPlannerRejectTests.cs  # sealed class HostingPlannerRejectTests
│  │  ├─ HostingPlannerTests.cs  # sealed class HostingPlannerTests
│  │  ├─ HostingRollbackTests.cs  # sealed class HostingRollbackTests
│  │  ├─ HostingSaveAsTests.cs  # sealed class HostingSaveAsTests
│  │  ├─ HostingTestEnv.cs  # 实现对应模块的 C# 职责。
│  │  ├─ HostingTransactionTests.cs  # sealed class HostingTransactionTests
│  │  ├─ LoadStructureErrorTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ LoadTransactionTests.cs  # sealed class LoadTransactionTests
│  │  ├─ SaveAsTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ SaveTransactionTests.cs  # sealed class SaveTransactionTests
│  │  ├─ ScenePersistenceEnv.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SchemaCompatibilityTests.cs  # sealed class SchemaCompatibilityTests
│  │  ├─ StaticModelAuthoringServiceTests.cs  # sealed class StaticModelAuthoringServiceTests
│  │  ├─ StaticModelBaseVertexTests.cs  # sealed class StaticModelBaseVertexTests
│  │  ├─ StaticModelCatalogTests.cs  # 确定性 AssetId，保证字典序固定：…00 < …01。
│  │  ├─ StaticModelFailureTrackerTests.cs  # sealed class StaticModelFailureTrackerTests
│  │  ├─ StaticModelProjectionTests.cs  # sealed class StaticModelProjectionTests
│  │  ├─ StaticModelUiTests.cs  # sealed class StaticModelUiTests
│  │  └─ StaticModelValidatorTests.cs  # SharpGLTF 边界会先拒绝索引越界 GLB（ParserFailure）；
│  ├─ Camera/  # 组织该模块下的正式文件。
│  │  ├─ CameraC2DraftFramingTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ CameraC2MapFramingTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ CameraC2MapFramingTests.Helpers.cs  # 实现对应模块的 C# 职责。
│  │  ├─ CameraDocumentTests.cs  # sealed class CameraDocumentTests
│  │  ├─ CameraFramingOccupancyTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ CameraFramingTests.cs  # sealed class CameraFramingTests
│  │  ├─ CameraNavigationUiTests.cs  # sealed class CameraNavigationUiTests
│  │  ├─ CameraNavigationUiTests.Focus.cs  # 无选中实体时聚焦保持相机与观察中心不变的回归测试
│  │  └─ UiViewGizmoTests.cs  # 验证对应模块的自动化行为与回归合同。
│  ├─ Logging/  # 组织该模块下的正式文件。
│  │  ├─ FootAxamlTailContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ LogAutoScrollPolicyTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ LogListAutoScrollControllerContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiMapLogChineseTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  └─ UiRootLogRowContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  ├─ Map/  # 组织该模块下的正式文件。
│  │  ├─ Editing/  # 组织该模块下的正式文件。
│  │  │  ├─ MapLayerSessionTests.Behavior.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ MapLayerSessionTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ MapLayerSessionTests.Drag.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ MapLayerSessionTests.Drag.History.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiLayerStateFeedbackTests.cs  # 状态图标消费与插入线反馈合同（A-D/E）
│  │  │  ├─ UiLayerVisualContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiLogSummaryPriorityTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiLogSummaryTimingTests.cs  # 通知时序（F/G/H）
│  │  │  ├─ UiMapCommandRoutingTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapDatasetContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapDatasetF1AcceptanceTests.cs  # Dataset Name、左侧满宽和拖拽投影稳定性回归测试。
│  │  │  ├─ UiMapDatasetF1Tests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapDatasetF2Tests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapDatasetF3ContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapDatasetF3Tests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapDatasetLayerR3Tests.cs  # Dataset Layer 显隐、锁定、顺序、选择稳定和保存重开测试。
│  │  │  ├─ UiMapDatasetRegionBootstrapPersistenceTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapDatasetRegionBootstrapTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapDatasetRegionLayerF3Tests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapDatasetRegionRuntimeTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapDatasetRegionToolActivationTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapDatasetRegionToolInvalidTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapEditorTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapHistoryTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapInitialProjectionTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapLayerDeleteLockRecoveryTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapLayerDragTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapLayerLockLogTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapLayerPanelTests.Behavior.cs  # 实现对应模块的 C# 职责。
│  │  │  ├─ UiMapLayerPanelTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapLayoutContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ UiMapManifestIdentityTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  └─ UiMapManifestNavigationTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapBoundsTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapCoordinateValidationTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapDatasetContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapDatasetDocumentTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapDatasetLayerStateTests.cs  # Dataset Layer 旧 Manifest 兼容、状态校验、Promotion 与底层锁定保护测试。
│  │  ├─ MapDatasetRegistryF1FailureTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapDatasetRegistryF2Tests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapDatasetRegistryFailureTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapDatasetRegistryLifecycleTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapDatasetStorageContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapDefaultMapTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapDefinitionTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapDocumentAggregateBridgeTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapDocumentOwnerChainTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapDocumentOwnerTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapEnvironmentValidationTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapIdTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapJsonRoundTripTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapJsonStrictnessTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapLayerRulesTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapLayerStackTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapLayerStackTests.Drag.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapLayerStackTests.Order.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapLayerTests.Base.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapLayerTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapManifestCreationTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapManifestSerializationTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapManifestStorageTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapManifestValidationTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapRegionDatasetContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapRegionDatasetRuntimeTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapRegionDraftTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapRegionTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapRegionTests.Geometry.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapRegionTests.Helpers.cs  # sealed partial class MapRegionTests
│  │  ├─ MapRegionTests.Strictness.cs  # 实现对应模块的 C# 职责。
│  │  ├─ MapRoadDatasetContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapSizeValidationTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapStorageFailureTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapStorageTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapSurfaceSamplerTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapSurfaceValidationTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapWorkingStorageTests.cs  # 工作区创建、提升、孤儿排除和碰撞失败回归。
│  │  ├─ SceneMapReferenceTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ WorldMapStateOwnerTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  └─ WorldMapStateTests.cs  # 验证对应模块的自动化行为与回归合同。
│  ├─ MapEditing/  # 组织该模块下的正式文件。
│  │  ├─ MapCoordinateContractTests.cs  # MapPoint 与世界坐标直接映射往返测试。
│  │  ├─ MapEditSessionCommandTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapEditSessionCreationTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapEditSessionDirtyTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapEditSessionGeometryTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapEditSessionHistoryTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapEditSessionMapPropertiesTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapEditSessionRegionTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapEditSessionSelectionTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapEditSessionThreadTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapEditSessionValidationTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapGeometryHitTesterTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapPickingRoundTripTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapRenderSnapshotProjectionTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapSurfacePickerTests.cs  # 地图表面拾取边界与中心命中测试。
│  │  ├─ RegionDrawingF3HistoryTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  └─ RegionDrawingStateTests.cs  # 绘制草稿顶点、闭合候选与取消测试。
│  ├─ Mode/  # 组织该模块下的正式文件。
│  │  ├─ EditorModeManagerTests.cs  # Manage/Edit Mode 纯合同测试。
│  │  ├─ EditorModeUiCompositionTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  └─ EditorModeUiTests.cs  # Mode/Workspace 直接切换、Esc/Tab、状态保留与 Region 隔离回归。
│  ├─ RegionDrawingTestVm.cs  # Region Drawing 回归测试的合法 Dataset/Workspace 上下文构造辅助。
│  ├─ Render/  # 组织该模块下的正式文件。
│  │  ├─ VulkanPresentLoopContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ VulkanPresentModeSelectionTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  └─ WorldGridIndependenceContractTests.cs  # GRID-RW-2A：锁定 MapGround 恢复与 World Grid 的 Z=0 独立性。
│  ├─ Scene/  # 组织该模块下的正式文件。
│  │  ├─ CommandSmokeTests.cs  # sealed class CommandSmokeTests
│  │  ├─ EditorEnvironmentTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ EntityBoundsSemanticsTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ EntityRegistryTests.cs  # sealed class EntityRegistryTests
│  │  ├─ EntityTests.cs  # sealed class EntityTests
│  │  ├─ FinalSceneTests.cs  # sealed class FinalSceneTests
│  │  ├─ GlobalWorldTests.cs  # sealed class GlobalWorldTests
│  │  ├─ SceneConsumptionTests.cs  # sealed class SceneConsumptionTests
│  │  ├─ SceneDocumentPersistenceTests.cs  # sealed class SceneDocumentPersistenceTests
│  │  ├─ SceneDocumentTests.cs  # sealed partial class SceneDocumentTests
│  │  ├─ SceneDocumentTests.Opening.cs  # sealed partial class SceneDocumentTests
│  │  ├─ SceneDocumentTests.SaveFeedback.cs  # sealed partial class SceneDocumentTests
│  │  ├─ SceneIsolationTests.cs  # sealed class SceneIsolationTests
│  │  ├─ SceneMultiEntityGateTests.cs  # sealed class SceneMultiEntityGateTests
│  │  ├─ SceneSelectionReentryTests.cs  # sealed class SceneSelectionReentryTests
│  │  ├─ SceneSingleAuthorityTests.cs  # sealed class SceneSingleAuthorityTests
│  │  ├─ UiHistoryTests.cs  # sealed partial class UiHistoryTests
│  │  └─ UiHistoryTests.InlineRename.cs  # sealed partial class UiHistoryTests
│  ├─ Selection/  # 组织该模块下的正式文件。
│  │  ├─ FinalSelectionTests.cs  # sealed class FinalSelectionTests
│  │  ├─ SelectionToolStateUiTests.cs  # sealed class SelectionToolStateUiTests
│  │  ├─ ToolStateHighlightUiTests.cs  # sealed partial class ToolStateHighlightUiTests
│  │  └─ ToolStateHighlightUiTests.Selection.cs  # sealed partial class ToolStateHighlightUiTests
│  ├─ Spatial/  # 组织该模块下的正式文件。
│  │  ├─ SceneStateOwnerSpatialTests.cs  # sealed class SceneStateOwnerSpatialTests
│  │  ├─ SpatialIndexEditLifecycleTests.cs  # sealed class SpatialIndexEditLifecycleTests
│  │  ├─ SpatialIndexOwnerLifecycleTests.cs  # sealed class SpatialIndexOwnerLifecycleTests
│  │  ├─ SpatialIndexOwnerRevisionTests.cs  # sealed class SpatialIndexOwnerRevisionTests
│  │  ├─ SpatialIndexRebuildTests.cs  # sealed class SpatialIndexRebuildTests
│  │  ├─ SpatialIndexScaleTests.cs  # sealed class SpatialIndexScaleTests
│  │  ├─ SpatialQueryGovernanceTests.cs  # sealed class SpatialQueryGovernanceTests
│  │  ├─ SpatialQueryOracle.cs  # 实现对应模块的 C# 职责。
│  │  ├─ SpatialQueryTests.cs  # sealed partial class SpatialQueryTests
│  │  ├─ SpatialQueryTests.Geometry.cs  # sealed partial class SpatialQueryTests
│  │  ├─ SpatialRaycastNearestTests.cs  # sealed class SpatialRaycastNearestTests
│  │  ├─ SpatialRaycastRevisionTests.cs  # sealed class SpatialRaycastRevisionTests
│  │  ├─ SpatialRaycastScaleTests.cs  # sealed class SpatialRaycastScaleTests
│  │  ├─ SpatialRayQueryLifecycleTests.cs  # sealed class SpatialRayQueryLifecycleTests
│  │  ├─ SpatialRayQueryTests.cs  # sealed class SpatialRayQueryTests
│  │  └─ SpatialTestData.cs  # 实现对应模块的 C# 职责。
│  ├─ Transform/  # 组织该模块下的正式文件。
│  │  ├─ Move/  # 组织该模块下的正式文件。
│  │  │  ├─ MoveTransformUiTests.cs  # sealed partial class MoveTransformUiTests
│  │  │  ├─ MoveTransformUiTests.Plane.cs  # sealed partial class MoveTransformUiTests
│  │  │  ├─ MoveTransformUiTests.Region.cs  # sealed partial class MoveTransformUiTests
│  │  │  └─ MoveTransformUiTests.Session.cs  # sealed partial class MoveTransformUiTests
│  │  ├─ Rotate/  # 组织该模块下的正式文件。
│  │  │  ├─ RotateTransformUiTests.cs  # sealed partial class RotateTransformUiTests
│  │  │  ├─ RotateTransformUiTests.DragState.cs  # sealed partial class RotateTransformUiTests
│  │  │  ├─ RotateTransformUiTests.Helpers.cs  # sealed partial class RotateTransformUiTests
│  │  │  ├─ RotateTransformUiTests.Preview.cs  # 实现对应模块的 C# 职责。
│  │  │  └─ RotateTransformUiTests.ToolSwitch.cs  # 实现对应模块的 C# 职责。
│  │  ├─ Scale/  # 组织该模块下的正式文件。
│  │  │  ├─ ScaleGizmoGlobalModeTests.cs  # sealed class ScaleGizmoGlobalModeTests
│  │  │  ├─ ScaleTransformUiTests.AxisUniform.cs  # sealed partial class ScaleTransformUiTests
│  │  │  ├─ ScaleTransformUiTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  │  ├─ ScaleTransformUiTests.Helpers.cs  # sealed partial class ScaleTransformUiTests
│  │  │  ├─ ScaleTransformUiTests.History.cs  # sealed partial class ScaleTransformUiTests
│  │  │  ├─ ScaleTransformUiTests.Pointer.cs  # sealed partial class ScaleTransformUiTests
│  │  │  └─ ScaleTransformUiTests.Target.cs  # sealed partial class ScaleTransformUiTests
│  │  ├─ TransformFoundationTests.cs  # sealed partial class TransformFoundationTests
│  │  ├─ TransformFoundationTests.Input.cs  # sealed partial class TransformFoundationTests
│  │  ├─ TransformFoundationTests.Inspector.cs  # sealed partial class TransformFoundationTests
│  │  ├─ TransformSessionTests.cs  # sealed class TransformSessionTests
│  │  └─ ViewportAssistTests.cs  # sealed class ViewportAssistTests
│  ├─ Tree/  # 组织该模块下的正式文件。
│  │  ├─ UiHierarchyConnectorTests.cs  # sealed class UiHierarchyConnectorTests
│  │  ├─ UiTreeGuideTests.cs  # sealed class UiTreeGuideTests
│  │  └─ UiTreeToggleTests.cs  # sealed class UiTreeToggleTests
│  ├─ UiRuntime/  # 组织该模块下的正式文件。
│  │  ├─ DatasetLayerPanelRuntimeLayoutTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ LayerARuntimeTests.cs  # 实现对应模块的 C# 职责。
│  │  ├─ LayerPanelRuntimeLayoutTests.cs  # LayerPanel 冷启动与增层布局运行时门禁。
│  │  ├─ LayerPanelRuntimeStateTests.cs  # LayerPanel 选中、可见和锁定状态运行时门禁。
│  │  ├─ MapVectorOverlayAnchorContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapVectorOverlayDepthPolicyTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ MapVectorOverlayV1Tests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ RegionDrawingF1ActivationRuntimeTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ RegionDrawingF1BTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ RegionDrawingF1CStabilityTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ RegionDrawingF1FullRuntimeTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ RegionDrawingF1RenderContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ RegionDrawingF1ResizeTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ RegionDrawingF1RuntimeRedTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ RegionDrawingF2PolygonTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ RegionPointerSafetyF2Tests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ ScaleIndicatorVisibilityRuntimeTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiHeadlessFixture.cs  # 可复用 Avalonia Headless 会话与 UI 线程调度夹具。
│  │  ├─ UiRuntimeCollection.cs  # Headless UI 测试串行集合定义。
│  │  ├─ UiRuntimeRiskTests.cs  # Top/Foot Fluent 状态覆盖风险运行时门禁。
│  │  ├─ UiRuntimeTestHost.cs  # Headless Window、布局和 Visual 树查询辅助。
│  │  └─ UiTestAppBuilder.cs  # 正式 Editor.UI App 的 Headless AppBuilder 配置。
│  ├─ UiTokens/  # 组织该模块下的正式文件。
│  │  ├─ LayerAUiCompositionTests.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiCsColorRulesTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD2F1RegionToolActivationContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD2F1RegionToolContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD3DebtClearedTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD4DebtClearedTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD4F1ButtonContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD4F1LayoutModelTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD4F1TextOverflowContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD4F1TypographyContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD4InspectorContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD4LayerContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD4LayoutModelTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD4MapEditorContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD5ButtonContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD5CorrectionBehaviorTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD5CorrectionNotifyTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD5CorrectionStructureTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD5DangerFlowTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD5DialogAndLogContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD5FormContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD5InputValidationTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD5MapStatusTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD5NotificationTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD5UnsavedDialogBehaviorTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD5UnsavedDialogTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD5UnsavedFlowTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD6AccessibilityContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD6DpiContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD6LogPerformanceTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiD6MotionContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiDebtBaseline.Colors.Axaml1.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiDebtBaseline.Colors.Axaml2.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiDebtBaseline.Colors.Cs.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiDebtBaseline.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiDebtBaseline.Typography.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiDebtBaselineBypassF2Tests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiDebtBaselineBypassTests.cs  # 基线绕过反例 10 项（换位/换选择器/换 x:Name/换属性/注释漂移/增长禁止）
│  │  ├─ UiDebtBaselineTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiF3LayerRowContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiLayerDeleteDialogContractTests.cs  # 独立删除确认窗口、Owner 模态与安全默认值源码合同。
│  │  ├─ UiSourceContractAnalyzer.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiSourceContractAnalyzer.CsRules.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiSourceContractAnalyzer.Icon.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiSourceContractAnalyzer.Inline.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiSourceContractAnalyzer.Structure.cs  # 实现对应模块的 C# 职责。
│  │  ├─ UiSourceContractAnalyzerTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiSourceContractAnalyzerTokenRefTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiTokenManifestGraphTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiTokenManifestTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiTopTabStripContractTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ UiTopTabStripModelHintAndListTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  └─ UiTopTabStripModelTests.cs  # 验证对应模块的自动化行为与回归合同。
│  ├─ Viewport/  # 组织该模块下的正式文件。
│  │  └─ NativePointerRoutePolicyTests.cs  # 验证对应模块的自动化行为与回归合同。
│  ├─ Workspace/  # 组织该模块下的正式文件。
│  │  ├─ EditorWorkspaceManagerTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ EditorWorkspaceUiCompositionTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  ├─ EditorWorkspaceUiTests.cs  # 验证对应模块的自动化行为与回归合同。
│  │  └─ RegionAuthoringHierarchyTests.cs  # 验证对应模块的自动化行为与回归合同。
│  ├─ WorldPartition/  # 组织该模块下的正式文件。
│  │  ├─ WorldPartitionInvariantTests.cs  # sealed class WorldPartitionInvariantTests
│  │  ├─ WorldPartitionMigrationTests.Activity.cs  # sealed partial class WorldPartitionMigrationTests
│  │  ├─ WorldPartitionMigrationTests.cs  # sealed partial class WorldPartitionMigrationTests
│  │  ├─ WorldPartitionTests.cs  # sealed partial class WorldPartitionTests
│  │  ├─ WorldPartitionTests.PartitionStrategy.cs  # sealed partial class WorldPartitionTests
│  │  └─ WorldPartitionUiTests.cs  # sealed class WorldPartitionUiTests
│  └─ XuanYu.World.Tests.csproj  # 配置对应 .NET 项目的构建与依赖。
└─ xyui/  # 集中 XYUI 规范、实现、审计与 Gallery。
   ├─ audit/  # 组织该模块下的正式文件。
   │  ├─ cross-audit.md  # 记录对应主题的当前有效说明。
   │  ├─ XYUI0/  # 组织该模块下的正式文件。
   │  │  ├─ decision-classification.json  # 保存对应模块的结构化数据。
   │  │  ├─ decision-classification.md  # 记录对应主题的当前有效说明。
   │  │  ├─ evidence-index.json  # 保存对应模块的结构化数据。
   │  │  ├─ source-audit.md  # 记录对应主题的当前有效说明。
   │  │  └─ text-input-interaction-audit.md  # 记录对应主题的当前规范、计划或审计事实。
   │  ├─ XYUI1/  # 组织该模块下的正式文件。
   │  │  └─ R5-F4-fidelity-matrix.md  # 记录对应主题的当前规范、计划或审计事实。
   │  ├─ XYUI4/  # 组织该模块下的正式文件。
   │  │  ├─ conflict-matrix.md  # 记录对应主题的当前有效说明。
   │  │  ├─ reconciliation.md  # 记录对应主题的当前有效说明。
   │  │  └─ source-audit.md  # 记录对应主题的当前有效说明。
   │  ├─ XYUI5/  # 组织该模块下的正式文件。
   │  │  ├─ reconciliation.md  # 记录对应主题的当前有效说明。
   │  │  └─ source-audit.md  # 记录对应主题的当前有效说明。
   │  ├─ XYUI6/  # 组织该模块下的正式文件。
   │  │  ├─ reconciliation.md  # 记录对应主题的当前有效说明。
   │  │  └─ source-audit.md  # 记录对应主题的当前有效说明。
   │  ├─ XYUI7/  # 组织该模块下的正式文件。
   │  │  ├─ reconciliation.md  # 记录对应主题的当前有效说明。
   │  │  └─ source-audit.md  # 记录对应主题的当前有效说明。
   │  └─ XYUI8/  # 组织该模块下的正式文件。
   │     ├─ reconciliation.md  # 记录对应主题的当前有效说明。
   │     └─ source-audit.md  # 记录对应主题的当前有效说明。
   ├─ avalonia/  # 组织该模块下的正式文件。
   │  ├─ gallery/  # 集中可运行 Gallery 与组件文档。
   │  │  ├─ CATALOG-COVERAGE.md  # 提供 Gallery 对应页面、数据或运行时预览。
   │  │  ├─ README.md  # 提供 Gallery 对应页面、数据或运行时预览。
   │  │  ├─ XYUI-1-COMPONENT-INVENTORY.md  # 记录对应主题的当前规范、计划或审计事实。
   │  │  └─ XYUI.Avalonia.Gallery/  # 组织该模块下的正式文件。
   │  │     ├─ App.axaml  # Gallery 应用样式根（FluentTheme）。
   │  │     ├─ App.axaml.cs  # 提供 Gallery 对应页面、数据或运行时预览。
   │  │     ├─ MainWindow.axaml  # 定义对应 Avalonia 界面与资源。
   │  │     ├─ MainWindow.axaml.cs  # 主窗口数据模型（具名 MainWindowModel，x:DataType 编译绑定）。
   │  │     ├─ PaletteCatalog.cs  # 色板数据模型（家族分组 + swatch 项）。
   │  │     ├─ PaletteViewModel.cs  # Foundation 色彩页面的分组数据模型。
   │  │     ├─ Program.cs  # Gallery 入口（平台检测启动）。
   │  │     ├─ ShapeCatalog.cs  # Shape 规范页数据（Spacing/Radius/Border/Elevation 分区）。
   │  │     ├─ ShapeViewModel.cs  # Shape 规范页数据模型（x:DataType 编译绑定）。
   │  │     ├─ TypographyCatalog.cs  # Typography 规范页数据（FontFamily/Size/LineHeight/Weight 分区）。
   │  │     ├─ TypographyViewModel.cs  # Typography 规范页数据模型（x:DataType 编译绑定）。
   │  │     ├─ Views/  # 组织该模块下的正式文件。
   │  │     │  ├─ CatalogView.axaml  # 提供 Gallery 对应页面、数据或运行时预览。
   │  │     │  ├─ CatalogView.axaml.cs  # 提供 Gallery 对应页面、数据或运行时预览。
   │  │     │  ├─ ComponentSamplesView.axaml  # 提供 Gallery 对应页面、数据或运行时预览。
   │  │     │  ├─ ComponentSamplesView.axaml.cs  # 提供 Gallery 对应页面、数据或运行时预览。
   │  │     │  ├─ Density/  # 信息密度 Gallery 页面子视图与实验室。
   │  │     │  │  ├─ DensityCoreRulesView.axaml  # 核心规则与第一屏 3 秒规则视图。
   │  │     │  │  ├─ DensityCoreRulesView.axaml.cs  # 核心规则视图代码隐藏。
   │  │     │  │  ├─ DensityGuardrailsView.axaml  # 推荐原则与 6 大禁止护栏视图。
   │  │     │  │  ├─ DensityGuardrailsView.axaml.cs  # 推荐与护栏代码隐藏。
   │  │     │  │  ├─ DensityLabView.axaml  # 实时信息密度实验室主界面。
   │  │     │  │  ├─ DensityLabView.axaml.cs  # 实验室切换与作用域处理。
   │  │     │  │  ├─ DensityLabView.Rows.cs  # 实验室工作台真实数据行构建。
   │  │     │  │  ├─ DensityMatrixView.axaml  # 信息层级压缩矩阵表格视图。
   │  │     │  │  └─ DensityMatrixView.axaml.cs  # 层级矩阵视图代码隐藏。
   │  │     │  ├─ DensitySamplesView.axaml  # 信息密度主页面（4 大区聚合与滚动呈现）。
   │  │     │  ├─ DensitySamplesView.axaml.cs  # 信息密度主页面代码隐藏。
   │  │     │  ├─ FoundationSamplesView.axaml  # 消费示例：Surface/Text/Border/Accent 的 DynamicResource 用法。
   │  │     │  ├─ FoundationSamplesView.axaml.cs  # 消费示例视图代码隐藏。
   │  │     │  ├─ FoundationStatesView.axaml  # 消费示例：State/Semantic/Disabled 三态的 DynamicResource 用法。
   │  │     │  ├─ FoundationStatesView.axaml.cs  # 状态示例视图代码隐藏。
   │  │     │  ├─ InteractionStatesView.axaml  # Token-compliant、单 Scroll ownership、结构化高密度状态示例。
   │  │     │  ├─ InteractionStatesView.axaml.cs  # 提供 Gallery 对应页面、数据或运行时预览。
   │  │     │  ├─ PaletteView.axaml  # Foundation 色彩页面视图。
   │  │     │  ├─ PaletteView.axaml.cs  # Foundation 色彩页面代码隐藏与数据初始化。
   │  │     │  ├─ ResponsiveCodePatternsSection.axaml  # Responsive 真实可复制代码/算法与规范视图。
   │  │     │  ├─ ResponsiveCodePatternsSection.axaml.cs  # Responsive 代码规范代码隐藏。
   │  │     │  ├─ ResponsiveCoreRulesSection.axaml  # Responsive 四大核心规则卡片视图。
   │  │     │  ├─ ResponsiveCoreRulesSection.axaml.cs  # Responsive 四大核心规则代码隐藏。
   │  │     │  ├─ ResponsiveDecisionMatrixSection.axaml  # Responsive 宽度决策矩阵视图。
   │  │     │  ├─ ResponsiveDecisionMatrixSection.axaml.cs  # Responsive 决策矩阵代码隐藏。
   │  │     │  ├─ ResponsiveLiveLabSection.axaml  # Responsive 真实 AdaptiveLayout 容器重排实验室视图。
   │  │     │  ├─ ResponsiveLiveLabSection.axaml.cs  # Responsive 重排实验室代码隐藏。
   │  │     │  ├─ ResponsiveView.axaml  # Responsive 响应式规范页主视图（四段式结构）。
   │  │     │  ├─ ResponsiveView.axaml.cs  # Responsive 响应式规范页代码隐藏。
   │  │     │  ├─ ShapeCodePatternsSection.axaml  # Shape 规范使用写法/高级建议/禁止反例视图。
   │  │     │  ├─ ShapeCodePatternsSection.axaml.cs  # Shape 规范使用代码隐藏。
   │  │     │  ├─ ShapeCompositionSection.axaml  # Shape 与 Radius/Border/Surface 四通道正交矩阵视图。
   │  │     │  ├─ ShapeCompositionSection.axaml.cs  # Shape 正交矩阵视图代码隐藏。
   │  │     │  ├─ ShapeCoreRulesSection.axaml  # Shape 四大核心规则卡片视图。
   │  │     │  ├─ ShapeCoreRulesSection.axaml.cs  # Shape 四大核心规则代码隐藏。
   │  │     │  ├─ ShapeMatrixSection.axaml  # Shape 五大基础几何形态卡片矩阵视图。
   │  │     │  ├─ ShapeMatrixSection.axaml.cs  # Shape 基础几何形态卡片代码隐藏。
   │  │     │  ├─ ShapeSamplesView.axaml  # 静态组合示例：Panel 结构/Border 五档/Elevation 卡片。
   │  │     │  ├─ ShapeSamplesView.axaml.cs  # 静态组合示例代码隐藏。
   │  │     │  ├─ ShapeScenariosSection.axaml  # Shape 场景决策判断卡片视图。
   │  │     │  ├─ ShapeScenariosSection.axaml.cs  # Shape 场景决策判断代码隐藏。
   │  │     │  ├─ ShapeView.axaml  # Shape 规范页主视图（四段式：核心规则/主矩阵/四通道正交/代码与守卫）。
   │  │     │  ├─ ShapeView.axaml.cs  # Shape 规范页主代码隐藏。
   │  │     │  ├─ SurfaceCodePatternsSection.axaml  # Surface 规范使用写法/高级建议/禁止反例视图。
   │  │     │  ├─ SurfaceCodePatternsSection.axaml.cs  # Surface 规范使用代码隐藏。
   │  │     │  ├─ SurfaceCompositionSection.axaml  # Surface 与 Border/Radius/Popup 四通道正交矩阵视图。
   │  │     │  ├─ SurfaceCompositionSection.axaml.cs  # Surface 正交矩阵视图代码隐藏。
   │  │     │  ├─ SurfaceCoreRulesSection.axaml  # Surface 四大核心规则卡片视图。
   │  │     │  ├─ SurfaceCoreRulesSection.axaml.cs  # Surface 四大核心规则代码隐藏。
   │  │     │  ├─ SurfaceMapSection.axaml  # Surface 承载层级地图（工作区与浮层分离）主视觉视图。
   │  │     │  ├─ SurfaceMapSection.axaml.cs  # Surface 承载层级地图代码隐藏。
   │  │     │  ├─ SurfaceScenariosSection.axaml  # Surface 场景决策判断卡片视图。
   │  │     │  ├─ SurfaceScenariosSection.axaml.cs  # Surface 场景决策判断代码隐藏。
   │  │     │  ├─ SurfaceView.axaml  # Surface 表面层级规范页主视图（四段式结构）。
   │  │     │  ├─ SurfaceView.axaml.cs  # Surface 表面层级规范页代码隐藏。
   │  │     │  ├─ StatesCodePatternsSection.axaml  # States 规范使用写法/高级建议/禁止反例视图。
   │  │     │  ├─ StatesCodePatternsSection.axaml.cs  # States 规范使用代码隐藏。
   │  │     │  ├─ StatesCoreRulesSection.axaml  # States 四大核心规则卡片视图。
   │  │     │  ├─ StatesCoreRulesSection.axaml.cs  # States 四大核心规则代码隐藏。
   │  │     │  ├─ StatesLiveLabSection.axaml  # States 实时操作/可用性/持续身份实验室视图。
   │  │     │  ├─ StatesLiveLabSection.axaml.cs  # States 实时实验室代码隐藏。
   │  │     │  ├─ StatesResolutionSection.axaml  # States 状态来源 Source Map 与单解析矩阵视图。
   │  │     │  ├─ StatesResolutionSection.axaml.cs  # States 状态来源与解析代码隐藏。
   │  │     │  ├─ StatesView.axaml  # States 交互状态规范页主视图（四段式结构）。
   │  │     │  ├─ StatesView.axaml.cs  # States 交互状态规范页代码隐藏。
   │  │     │  ├─ TypographySamplesView.axaml  # Typography 消费示例（Heading/Body/Label/Caption/Mono/信息等级/高密度对照）。
   │  │     │  ├─ TypographySamplesView.axaml.cs  # Typography 消费示例代码隐藏。
   │  │     │  ├─ TypographyView.axaml  # Typography 规范页视图（token 表数据驱动 + 滚动）。
   │  │     │  ├─ TypographyView.axaml.cs  # Typography 规范页代码隐藏。
   │  │     │  ├─ XYUI1ComponentDocumentView.axaml  # 单组件中文文档模板（Preview/Usage/API/Token）。
   │  │     │  ├─ XYUI1ComponentDocumentView.axaml.cs  # 单组件文档视图代码隐藏。
   │  │     │  ├─ XYUI1DocumentationView.axaml  # 定义对应 Avalonia 界面与资源。
   │  │     │  ├─ XYUI1DocumentationView.axaml.cs  # 文档导航视图代码隐藏与模型初始化。
   │  │     │  ├─ XYUI1GalleryView.axaml  # 定义对应 Avalonia 界面与资源。
   │  │     │  ├─ XYUI1GalleryView.axaml.cs  # 实现对应模块的 C# 职责。
   │  │     │  ├─ XYUI1ModuleOverviewView.axaml  # 定义对应 Avalonia 界面与资源。
   │  │     │  ├─ XYUI1ModuleOverviewView.axaml.cs  # 组件索引点击导航处理。
   │  │     │  ├─ XYUI2ModuleOverviewView.axaml  # 定义对应 Avalonia 界面与资源。
   │  │     │  └─ XYUI2ModuleOverviewView.axaml.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYBadgePreviewFactory.cs  # Badge Default/Accent 左指针标签的真实 Gallery Preview 工厂。
   │  │     ├─ XYIconButtonNamingExtensions.cs  # IconButton Gallery 自动化名称扩展。
   │  │     ├─ XYMonoPreviewFactory.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYSelectableTextPreviewFactory.cs  # SelectableText 默认/Technical 变体与独立 Copy Mark Preview 工厂。
   │  │     ├─ XYSubMenuHierarchyDebugPreview.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI.Avalonia.Gallery.csproj  # Gallery 可执行项目文件。
   │  │     ├─ XYUI1DocumentationCatalog.Api.cs  # 真实 Avalonia 属性与 Foundation Token 文档表。
   │  │     ├─ XYUI1DocumentationCatalog.Content.cs  # 24 个组件的基础用法、变体和状态文案。
   │  │     ├─ XYUI1DocumentationCatalog.Phase1A.Content.cs  # 提供 Phase 1A 01～06 的 QuickStart、CoreRules 与 HowToUse 文案。
   │  │     ├─ XYUI1DocumentationCatalog.Phase1A.Foundation.cs  # 提供 Phase 1A 01～06 的 Foundation Token 映射与状态合同说明。
   │  │     ├─ XYUI1DocumentationCatalog.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI1DocumentationModels.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI1DocumentationViewModel.cs  # 左侧导航选择与模块/组件文档视图切换模型。
   │  │     ├─ XYUI1DocumentationViewModel.XYUI2.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI1DocumentationViewModel.XYUI3.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI1GalleryCatalog.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI1LiveExamplesFactory.cs  # 提供 Phase 1A 01～03（Text/Label/Caption）真实业务场景 Live Examples 工厂。
   │  │     ├─ XYUI1LiveExamplesFactory.TitlesAndLink.cs  # 提供 Phase 1A 04～06（Heading/SectionTitle/Link）真实业务场景 Live Examples 工厂。
   │  │     ├─ XYUI2DocumentationCatalog.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2DocumentationCatalog.Phase2A.Anatomy.cs  # 定义 Phase 2A (01～06) 控件变体与属性文档模型。
   │  │     ├─ XYUI2DocumentationCatalog.Phase2A.Content.cs  # 定义 Phase 2A (01～06) 控件极简 QuickStart 与核心规则文档。
   │  │     ├─ XYUI2DocumentationCatalog.Phase2A.Foundation.cs  # 定义 Phase 2A (01～06) Foundation Token 映射与状态伪类文档。
   │  │     ├─ XYUI2DocumentationCatalog.Phase2A.HowToUse.cs  # 定义 Phase 2A (01～06) 使用场景、禁用场景与交互规范。
   │  │     ├─ XYUI2DocumentationCatalog.Properties.cs  # 提供 Gallery 对应页面、数据或运行时预览。
   │  │     ├─ XYUI2DocumentationCatalog.Usages.cs  # 提供 Gallery 对应页面、数据或运行时预览。
   │  │     ├─ XYUI2LiveExamplesFactory.Actions.cs  # SplitButton、DropDownButton、Checkbox 真实业务场景 Live Examples。
   │  │     ├─ XYUI2LiveExamplesFactory.Buttons.cs  # Button、IconButton、ToggleButton 真实业务场景 Live Examples。
   │  │     ├─ XYUI2LiveExamplesFactory.Commands.cs  # Phase 2A Live Examples 交互测试辅助命令。
   │  │     ├─ XYUI2LiveExamplesFactory.cs  # Phase 2A 真实业务场景 Live Examples 门面分发。
   │  │     ├─ XYUI2GalleryCatalog.Buttons.cs  # 提供 Gallery 对应页面、数据或运行时预览。
   │  │     ├─ XYUI2GalleryCatalog.Choices.cs  # Checkbox、RadioButton、Switch 真实场景样例工厂。
   │  │     ├─ XYUI2GalleryCatalog.ColorBool.cs  # ColorPicker 与 BoolProperty 的中文真实场景样例工厂。
   │  │     ├─ XYUI2GalleryCatalog.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2GalleryCatalog.DateTime.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2GalleryCatalog.DropDown.cs  # DropDownButton 导出/筛选/排序等真实场景样例工厂。
   │  │     ├─ XYUI2GalleryCatalog.Inputs.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2GalleryCatalog.Properties.cs  # Number、Vector、Enum、Reference Property 的中文真实场景样例工厂。
   │  │     ├─ XYUI2GalleryCatalog.SearchPassword.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI3DocumentationCatalog.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI3DocumentationCatalog.Api.cs  # 提供 XYUI-3-23 基础用法与完整 API 文档。
   │  │     ├─ XYUI3GalleryCatalog.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI3GalleryCatalog.Navigation.cs  # 提供紧凑导航四组件的真实 Gallery Preview。
   │  │     └─ XYUI3GalleryCatalog.Final.cs  # 提供 XYUI-3-21~24 最终导航组件真实预览。
   │  ├─ src/  # 组织该模块下的正式文件。
   │  │  └─ XYUI.Avalonia/  # 组织该模块下的正式文件。
   │  │     ├─ Catalog/  # 组织该模块下的正式文件。
   │  │     │  ├─ XyuiCatalogEntry.cs  # Catalog 条目及 READY/READY WITH GAP 状态文本。
   │  │     │  ├─ XyuiCatalogPaths.cs  # 实现对应模块的 C# 职责。
   │  │     │  ├─ XyuiCatalogSource.cs  # 实现对应模块的 C# 职责。
   │  │     │  ├─ XyuiCatalogSpecReader.cs  # 从 canonical spec 提取用途、变体、状态和场景文案。
   │  │     │  ├─ XyuiCatalogTruth.cs  # 实现对应模块的 C# 职责。
   │  │     │  └─ XyuiCatalogTypeMap.cs  # Canonical ID 到稳定 Avalonia 类型名及 Gallery 覆盖映射。
    │  │     ├─ Controls/  # 集中公开 XYUI 控件实现。
    │  │     │  ├─ README.md  # 实现对应 XYUI 控件的视觉、状态或交互职责。
    │  │     │  ├─ AdaptiveLayout.cs  # 根据自身可用宽度将直接子项重排为一至多列。
   │  │     │  ├─ XYUI1/  # 组织该模块下的正式文件。
   │  │     │  │  ├─ _Shared/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Base/  # 组织该模块下的正式文件。
   │  │     │  │  │  │  ├─ XyuiTextComponent.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  └─ XyuiVectorTextSurface.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  ├─ Geometry/  # 组织该模块下的正式文件。
   │  │     │  │  │  │  └─ XyuiBadgeTagPath.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │     ├─ XyuiComponentStyles.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  │     ├─ XyuiComponentStyles.ResultText.cs  # 定义搜索结果与截断文本的禁用态语义样式。
   │  │     │  │  │     ├─ XyuiComponentStyles.Selection.cs  # 定义可选择文本与空状态文本的禁用态语义样式。
   │  │     │  │  │     ├─ XyuiComponentStyles.Semantic.cs  # 定义状态点、状态徽章及禁用态语义样式。
   │  │     │  │  │     ├─ XyuiComponentStyles.Surfaces.cs  # 定义 CodeText/Badge 的 Foundation 表面几何与间距。
   │  │     │  │  │     └─ XyuiComponentStyles.Typography.cs  # 定义 XYUI-1 文本、图标和标记的排版样式。
   │  │     │  │  │  └─ XyuiStatusStateTokens.cs  # 集中 StatusBadge/StatusDot 的五态与禁用态语义资源映射。
   │  │     │  │  ├─ XYUI1-01-Text/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYText.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI1-02-Label/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYLabel.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI1-03-Caption/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYCaption.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI1-04-Heading/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYHeading.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI1-05-SectionTitle/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYSectionTitle.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI1-06-Link/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYLink.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI1-07-CodeText/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYCodeText.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI1-08-MonoText/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYMonoText.Layout.cs  # MonoText 共享三列布局、响应宽度与合法列间距实现。
   │  │     │  │  │  ├─ XYMonoDataRow.cs  # MonoText 的 Label/Value/Unit 结构化数据行模型。
   │  │     │  │  │  └─ XYMonoText.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI1-09-Badge/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYBadge.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  ├─ XYUI1-10-StatusBadge/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYStatusBadge.cs  # 定义五态状态徽章及 8 DIP 标记几何。
   │  │     │  │  ├─ XYUI1-11-StatusDot/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYStatusDot.cs  # 定义五态状态点及 8 DIP 圆形几何。
   │  │     │  │  ├─ XYUI1-12-Icon/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYIcon.Rendering.cs  # XYIcon 逻辑视口缩放与最终 DIP Stroke 绘制。
   │  │     │  │  │  └─ XYIcon.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  ├─ XYUI1-13-IconLabel/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYIconLabel.cs  # 复用公开 XYIcon 与文本呈现器并保持 Space1/垂直居中。
   │  │     │  │  ├─ XYUI1-14-Separator/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYSeparator.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  ├─ XYUI1-15-HelpText/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYHelpText.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  ├─ XYUI1-16-ErrorText/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYErrorText.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  ├─ XYUI1-17-WarningText/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYWarningText.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  ├─ XYUI1-18-ShortcutHint/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYShortcutHint.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  ├─ XYUI1-19-Tooltip/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYTooltip.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  ├─ XYUI1-20-RichText/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYRichText.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  ├─ XYUI1-21-SelectableText/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYSelectableText.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  ├─ XYUI1-22-EmptyText/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYEmptyText.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  ├─ XYUI1-23-SearchHighlight/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYSearchHighlight.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  └─ XYUI1-24-TruncatedText/  # 组织该模块下的正式文件。
   │  │     │  │     └─ XYTruncatedText.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  ├─ XYUI2/  # 组织该模块下的正式文件。
   │  │     │  │  ├─ _Shared/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Base/  # 组织该模块下的正式文件。
   │  │     │  │  │  │  └─ XyuiEditableTextBox.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  ├─ ButtonFamily/  # 组织该模块下的正式文件。
   │  │     │  │  │  │  ├─ XyuiActionEdge.cs  # Button 家族底部 Action Edge 元素（内部实现构件，非公开组件）。
   │  │     │  │  │  │  ├─ XyuiButtonChrome.cs  # Batch 01 三按钮共享 Chrome 模板（Border+内容+Edge 覆盖层）。
   │  │     │  │  │  │  └─ XyuiButtonVariant.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  │  ├─ Property/  # 组织该模块下的正式文件。
   │  │     │  │  │  │  └─ XYPropertyLayoutMetrics.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  ├─ XyuiControlStyles.ButtonFamily.cs  # Button 样式：变体 Edge 语言、Focus Ring、Disabled 衰减。
   │  │     │  │  │  │  ├─ XyuiControlStyles.ChoiceControls.cs  # Checkbox、Radio、Switch 状态样式与 token 消费。
   │  │     │  │  │  │  ├─ XyuiControlStyles.ColorBool.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  ├─ XyuiControlStyles.ComboBox.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  ├─ XyuiControlStyles.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  │  │  ├─ XyuiControlStyles.DateTime.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  ├─ XyuiControlStyles.DropDownButton.cs  # DropDownButton Chevron Track 样式与控件级状态映射。
   │  │     │  │  │  │  ├─ XyuiControlStyles.Edges.cs  # Action Edge 填色/显隐/Hover 抬升样式辅助。
   │  │     │  │  │  │  ├─ XyuiControlStyles.GhostAndToggle.cs  # IconButton Ghost Reveal 与 ToggleButton Persistent Edge 样式。
   │  │     │  │  │  │  ├─ XyuiControlStyles.InputFamily.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  ├─ XyuiControlStyles.NumberField.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  │  │  ├─ XyuiControlStyles.PropertyControls.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  │  │  ├─ XyuiControlStyles.SearchPassword.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  ├─ XyuiControlStyles.Select.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  ├─ XyuiControlStyles.Slider.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  ├─ XyuiControlStyles.SplitButton.cs  # SplitButton Compact Icon Well 样式与状态映射。
   │  │     │  │  │  │  └─ XyuiControlStyles.TextArea.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  └─ Tokens/  # 集中共享尺寸与语义常量。
   │  │     │  │  │     └─ XyuiComponentTokens.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-01-Button/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYButton.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  ├─ XYUI2-02-IconButton/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYIconButton.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  ├─ XYUI2-03-ToggleButton/  # 组织该模块下的正式文件。
   │  │     │  │  │  └─ XYToggleButton.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  ├─ XYUI2-04-SplitButton/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYSplitButton.Template.cs  # SplitButton 单 Chrome、主区、Divider 与 Icon Well 模板。
   │  │     │  │  │  └─ XYSplitButton.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-05-DropDownButton/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYDropDownButton.Template.cs  # DropDownButton 双列模板：装饰槽不可命中、无 Divider。
   │  │     │  │  │  └─ XYDropDownButton.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-06-Checkbox/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYCheckbox.Template.cs  # Checkbox 方形视觉盒、勾选符号与 Mixed 横线模板。
   │  │     │  │  │  └─ XYCheckbox.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │  ├─ XYUI2-07-RadioButton/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYRadioButton.Template.cs  # Radio Halo、圆环、中心点与标签模板。
   │  │     │  │  │  └─ XYRadioButton.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-08-Switch/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYSwitch.Template.cs  # Compact Track + Thumb 固定尺寸模板。
   │  │     │  │  │  └─ XYSwitch.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-09-TextField/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYTextField.Template.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  └─ XYTextField.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-10-NumberField/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │  │  │  │  ├─ XYNumberField.Keyboard.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  ├─ XYNumberField.Scrub.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  └─ XYNumberField.Value.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYNumberField.Template.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  └─ XYNumberField.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-11-Slider/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYSlider.Template.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  ├─ XYSlider.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  └─ XYSliderTrack.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-12-ComboBox/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │  │  │  │  ├─ XYComboBox.Filter.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  ├─ XYComboBox.Keyboard.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  └─ XYComboBox.Lifecycle.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYComboBox.Template.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  └─ XYComboBox.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-13-Select/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │  │  │  │  ├─ XYSelect.Keyboard.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  ├─ XYSelect.Lifecycle.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  └─ XYSelect.Popup.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYSelect.Template.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  └─ XYSelect.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-14-TextArea/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYTextArea.Template.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  └─ XYTextArea.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-15-SearchField/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │  │  │  │  └─ XYSearchField.Keyboard.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYSearchField.Template.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  └─ XYSearchField.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-16-PasswordField/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │  │  │  │  ├─ XYPasswordField.Keyboard.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  └─ XYPasswordField.Reveal.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYPasswordField.Template.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  └─ XYPasswordField.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-17-DatePicker/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │  │  │  │  ├─ XYDatePicker.Calendar.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  ├─ XYDatePicker.Keyboard.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  └─ XYDatePicker.Popup.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYDatePicker.Template.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  └─ XYDatePicker.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-18-TimePicker/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │  │  │  │  ├─ XYTimePicker.Keyboard.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  ├─ XYTimePicker.Popup.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  └─ XYTimePicker.Scrub.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYTimePicker.Template.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  └─ XYTimePicker.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-19-ColorPicker/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │  │  │  │  ├─ XYColorPicker.Color.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  ├─ XYColorPicker.Input.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  ├─ XYColorPicker.Lifecycle.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  ├─ XYColorPicker.Panel.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  │  └─ XYColorPicker.Popup.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYColorPicker.Template.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  └─ XYColorPicker.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-20-BoolProperty/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYBoolProperty.Template.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  └─ XYBoolProperty.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-21-NumberProperty/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │  │  │  │  └─ XYNumberProperty.Scrub.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYNumberProperty.Template.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  └─ XYNumberProperty.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-22-VectorProperty/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYVectorProperty.Template.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  ├─ XYVectorProperty.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  └─ XYVectorProperty.Layout.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  ├─ XYUI2-23-EnumProperty/  # 组织该模块下的正式文件。
   │  │     │  │  │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │  │  │  └─ XYEnumProperty.Template.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  │  └─ XYEnumProperty.cs  # 实现对应模块的 C# 职责。
   │  │     │  │  └─ XYUI2-24-ReferenceProperty/  # 组织该模块下的正式文件。
   │  │     │  │     ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │  │     │  ├─ XYReferenceProperty.DragDrop.cs  # 实现对应 XYUI 控件的视觉、状态或交互职责。
   │  │     │  │     │  └─ XYReferenceProperty.Popup.cs  # 实现对应模块的 C# 职责。
   │  │     │  │     ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │  │     │  └─ XYReferenceProperty.Template.cs  # 实现对应模块的 C# 职责。
   │  │     │  │     ├─ XYReferenceProperty.cs  # 实现对应模块的 C# 职责。
   │  │     │  │     └─ XYReferenceProperty.Layout.cs  # 实现对应模块的 C# 职责。
   │  │     │  └─ XYUI3/  # 组织该模块下的正式文件。
   │  │     │     ├─ _Shared/  # 组织该模块下的正式文件。
   │  │     │     │  ├─ Models/  # 组织该模块下的正式文件。
   │  │     │     │  │  ├─ XYMenuItemModel.cs  # 实现对应模块的 C# 职责。
   │  │     │     │  │  └─ XYNavigationState.cs  # 实现对应模块的 C# 职责。
   │  │     │     │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │     │  │  ├─ XyuiComponentStyles.Batch04.cs  # 定义分页、步骤、工具栏与工具组语义样式。
   │  │     │     │  │  ├─ XyuiComponentStyles.Batch05.cs  # 定义命令栏、命令面板、历史导航与工作区紧凑样式。
   │  │     │     │  │  ├─ XyuiComponentStyles.BottomNavigation.cs  # 定义移动端底部导航的等宽槽位与主动作视觉样式。
   │  │     │     │  │  └─ XyuiComponentStyles.XYUI3.cs  # 实现对应模块的 C# 职责。
   │  │     │     │  └─ Tokens/  # 集中共享尺寸与语义常量。
   │  │     │     │     └─ XyuiCompactNavigationTokens.cs  # 集中保存紧凑导航组件的几何常量。
   │  │     │     ├─ XYUI3-01-MenuBar/  # 组织该模块下的正式文件。
   │  │     │     │  ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │     │  │  ├─ XYMenuBar.Interaction.cs  # MenuBar Popup、顶层切换、键盘与外部点击交互。
   │  │     │     │  │  └─ XYMenuBarItem.Interaction.cs  # MenuBarItem 指针/键盘激活交互。
   │  │     │     │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │     │  │  └─ XYMenuBarItem.Style.cs  # MenuBarItem 文本、活动指示线与垂直居中视觉构建。
   │  │     │     │  ├─ XYMenuBar.cs  # 实现对应模块的 C# 职责。
   │  │     │     │  └─ XYMenuBarItem.cs  # 实现对应模块的 C# 职责。
   │  │     │     ├─ XYUI3-02-Menu/  # 组织该模块下的正式文件。
   │  │     │     │  ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │     │  │  ├─ XYMenu.Interaction.cs  # 菜单打开、焦点导航、Enter/Esc 与关闭交互。
   │  │     │     │  │  └─ XYMenuItem.Interaction.cs  # 菜单项指针、键盘、命令与子菜单触发交互。
   │  │     │     │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │     │  │  └─ XYMenuItem.Visual.cs  # 菜单项勾选、单选、快捷键与 Chevron 视觉构建。
   │  │     │     │  ├─ XYMenu.cs  # 实现对应模块的 C# 职责。
   │  │     │     │  └─ XYMenuItem.cs  # 实现对应模块的 C# 职责。
   │  │     │     ├─ XYUI3-03-ContextMenu/  # 组织该模块下的正式文件。
   │  │     │     │  ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │     │  │  └─ XYContextMenu.Interaction.cs  # 右键目标、Pointer Popup、轻量关闭与 Esc 交互。
   │  │     │     │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │     │  │  └─ XYContextMenu.Style.cs  # ContextMenu 头部文本垂直居中视觉构建。
   │  │     │     │  └─ XYContextMenu.cs  # 实现对应模块的 C# 职责。
   │  │     │     ├─ XYUI3-04-SubMenu/  # 组织该模块下的正式文件。
   │  │     │     │  ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │     │  │  └─ XYSubMenu.Interaction.cs  # 子菜单层级可见性、递归关闭、兄弟互斥、连接列与方向键交互。
   │  │     │     │  ├─ Styles/  # 集中控件视觉构建与语义样式。
   │  │     │     │  │  └─ XYSubMenuConnector.cs  # 子菜单连接线与锚点视觉。
   │  │     │     │  └─ XYSubMenu.cs  # 实现对应模块的 C# 职责。
   │  │     │     ├─ XYUI3-05-NavigationMenu/  # 组织该模块下的正式文件。
   │  │     │     │  ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │     │  │  ├─ XYNavigationItem.Interaction.cs  # 实现对应模块的 C# 职责。
   │  │     │     │  │  └─ XYNavigationMenu.Interaction.cs  # 实现对应模块的 C# 职责。
   │  │     │     │  └─ UI/  # 组织该模块下的正式文件。
   │  │     │     │     ├─ XYNavigationItem.cs  # 实现对应模块的 C# 职责。
   │  │     │     │     ├─ XYNavigationMenu.cs  # 实现对应模块的 C# 职责。
   │  │     │     │     └─ XYNavigationMenuStyles.cs  # 实现对应模块的 C# 职责。
   │  │     │     ├─ XYUI3-06-Sidebar/  # 组织该模块下的正式文件。
   │  │     │     │  ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │     │  │  └─ XYSidebar.Interaction.cs  # 实现对应模块的 C# 职责。
   │  │     │     │  └─ UI/  # 组织该模块下的正式文件。
   │  │     │     │     ├─ XYSidebar.cs  # 实现对应模块的 C# 职责。
   │  │     │     │     └─ XYUI3SidebarStyles.cs  # 实现对应模块的 C# 职责。
   │  │     │     ├─ XYUI3-07-NavigationRail/  # 组织该模块下的正式文件。
   │  │     │     │  ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │     │  │  └─ XYNavigationRail.Interaction.cs  # 实现对应模块的 C# 职责。
   │  │     │     │  └─ UI/  # 组织该模块下的正式文件。
   │  │     │     │     └─ XYNavigationRail.cs  # 实现对应模块的 C# 职责。
   │  │     │     ├─ XYUI3-08-Tabs/  # 组织该模块下的正式文件。
   │  │     │     │  ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │     │  │  ├─ XYTab.Interaction.cs  # 实现对应模块的 C# 职责。
   │  │     │     │  │  └─ XYTabs.Selection.cs  # 实现对应模块的 C# 职责。
   │  │     │     │  └─ UI/  # 组织该模块下的正式文件。
   │  │     │     │     ├─ XYTab.cs  # 实现对应模块的 C# 职责。
   │  │     │     │     └─ XYTabs.cs  # 实现对应模块的 C# 职责。
   │  │     │     ├─ XYUI3-09-TabBar/  # 组织该模块下的正式文件。
   │  │     │     │  ├─ Interaction/  # 集中页签栏滚动、溢出与新增请求交互。
   │  │     │     │  │  └─ XYTabBar.Interaction.cs  # 实现按钮和滚轮滚动、溢出选页及新增请求。
   │  │     │     │  └─ UI/  # 组织该模块下的正式文件。
   │  │     │     │     ├─ XYTabBar.cs  # 组合真实页签视口、溢出 Popup 与固定操作槽。
   │  │     │     │     └─ XYTabBarStyles.cs  # 定义单底边页签栏与停靠页签语义样式。
   │  │     │     ├─ XYUI3-10-DockTabs/  # 组织该模块下的正式文件。
   │  │     │     │  ├─ Interaction/  # 集中停靠页签选择、关闭与同栏排序交互。
   │  │     │     │  │  ├─ XYDockTab.Drag.cs  # 将 Drag Grip 指针释放位置转换为排序请求。
   │  │     │     │  │  └─ XYDockTabs.Interaction.cs  # 管理单选、关闭、排序及对应事件。
   │  │     │     │  └─ UI/  # 组织该模块下的正式文件。
   │  │     │     │     ├─ XYDockTab.cs  # 为真实页签组合停靠握柄、分隔线与唯一选中强调。
   │  │     │     │     └─ XYDockTabs.cs  # 承载并重建可排序的紧凑停靠页签集合。
   │  │     │     ├─ XYUI3-11-Breadcrumb/  # 组织该模块下的正式文件。
   │  │     │     │  ├─ Interaction/  # 集中路径导航和下拉请求交互。
   │  │     │     │  │  ├─ XYBreadcrumb.Interaction.cs  # 管理 Current 单选与 Dropdown 请求转发。
   │  │     │     │  │  └─ XYBreadcrumbItem.Interaction.cs  # 提供鼠标及 Enter/Space 调用入口。
   │  │     │     │  └─ UI/  # 组织该模块下的正式文件。
   │  │     │     │     ├─ XYBreadcrumb.cs  # 组合文字路径项与矢量层级分隔符。
   │  │     │     │     ├─ XYBreadcrumbItem.cs  # 表达普通、折叠与当前位置路径项视觉。
   │  │     │     │     └─ XYBreadcrumbTreeStyles.cs  # 定义面包屑与树形导航的紧凑语义样式。
   │  │     │     └─ XYUI3-12-TreeNavigation/  # 组织该模块下的正式文件。
   │  │     │        ├─ Interaction/  # 集中树节点展开、选择与键盘导航交互。
   │  │     │        │  ├─ XYTreeNavigation.Interaction.cs  # 管理单选及方向键父子和同级导航。
   │  │     │        │  └─ XYTreeNode.Interaction.cs  # 提供 Chevron、行点击及键盘请求入口。
   │  │     │        └─ UI/  # 组织该模块下的正式文件。
   │  │     │           ├─ XYTreeNavigation.cs  # 按展开状态投影紧凑树形导航可见节点。
   │  │     │           └─ XYTreeNode.cs  # 绘制树节点、矢量图标与默认或活动祖先 Guide。
   │  │     ├─ XYUI3-13-Pagination/  # 组织紧凑分页与数据 Footer。
   │  │     │  ├─ Interaction/XYPagination.Navigation.cs  # 管理页码跳转事件。
   │  │     │  └─ UI/  # 实现分页视觉与真实输入复用。
   │  │     │     ├─ XYPagination.cs  # 邻近页、前后页与 Jump 输入分页控件。
   │  │     │     └─ XYPaginationFooter.cs  # 复用分页与 Select 的数据 Footer。
   │  │     ├─ XYUI3-14-Steps/  # 组织横向与纵向步骤导航。
   │  │     │  └─ UI/  # 实现步骤状态节点与连接线。
   │  │     │     ├─ XYStepNode.cs  # 表达 Completed、Current、Pending 等步骤状态。
   │  │     │     └─ XYSteps.cs  # 以 Orientation 自适应排列步骤节点。
   │  │     ├─ XYUI3-15-Toolbar/UI/  # 实现连续紧凑工具栏与基础工具复用。
   │  │     │  ├─ XYToolbar.cs  # 排列 Toolbar 工具。
   │  │     │  └─ XYToolbarTool.cs  # 以 XYIconButton 承载工具语义。
   │  │     ├─ XYUI3-16-ToolGroup/UI/  # 实现 Toolbar 内工具组与静态折叠。
   │  │     │  └─ XYToolGroup.cs  # 提供组间距、Hover 区域和折叠触发器。
   │  │     ├─ XYUI3-17-CommandBar/UI/  # 实现紧凑一次性命令栏与 More 菜单。
   │  │     │  └─ XYCommandBar.cs  # 提供主命令、命令项和真实 XYMenu Popup。
   │  │     ├─ XYUI3-18-CommandPalette/Interaction/  # 组织命令面板搜索、Scope 与 Popup 生命周期。
   │  │     │  ├─ XYCommandPalette.Interaction.cs  # 提供过滤、Recent、Scope、键盘与执行关闭。
   │  │     │  └─ XYCommandPalette.Lifecycle.cs  # 提供打开、关闭、焦点恢复和失活收口。
   │  │     ├─ XYUI3-18-CommandPalette/UI/  # 实现紧凑搜索命令面板。
   │  │     │  ├─ XYCommandPalette.cs  # 定义命令模型、公共状态和面板宿主。
   │  │     │  ├─ XYCommandPalette.Layout.cs  # 构造搜索、结果、分隔线和详情的 Grid 布局。
   │  │     │  └─ XYCommandPaletteItem.cs  # 提供整行 Stretch 的结果项交互。
   │  │     ├─ XYUI3-19-BackForwardNavigation/Interaction/  # 组织历史菜单、快捷键与生命周期收口。
   │  │     │  ├─ XYBackForwardNavigation.Interaction.cs  # 提供历史菜单跳转与 Alt 快捷键。
   │  │     │  └─ XYBackForwardNavigation.Lifecycle.cs  # 提供失活与卸载时关闭历史弹层。
   │  │     ├─ XYUI3-19-BackForwardNavigation/UI/  # 实现独立导航历史。
   │  │     │  └─ XYBackForwardNavigation.cs  # 提供 34 DIP Surface、前进、后退和 Forward 截断。
   │  │     ├─ XYUI3-20-WorkspaceSwitcher/Interaction/  # 组织工作区选择、请求提交与 Popup 生命周期。
   │  │     │  ├─ XYWorkspaceSwitcher.Interaction.cs  # 提供同宽菜单、键盘选择、管理入口与请求后提交。
   │  │     │  └─ XYWorkspaceSwitcher.Lifecycle.cs  # 提供失活、关闭与卸载时关闭工作区弹层。
   │  │     └─ XYUI3-20-WorkspaceSwitcher/UI/  # 实现紧凑工作区切换器。
   │  │        └─ XYWorkspaceSwitcher.cs  # 定义 XYButton Trigger、共享 State 与工作区模型。
   │  │     ├─ XYUI3-21-ViewSwitcher/  # 实现共享视图状态与三种切换变体。
   │  │     │  └─ XYViewSwitcher.cs  # 提供 Segmented、Dropdown、Primary+More 与 request→commit。
   │  │     ├─ XYUI3-22-TableOfContents/  # 实现限深两级章节目录。
   │  │     │  └─ XYTableOfContents.cs  # 提供层级/紧凑变体与共享章节状态。
   │  │     ├─ XYUI3-23-BottomNavigation/  # 实现移动端底部目的地导航。
   │  │     │  └─ XYBottomNavigation.cs  # 提供等宽目的地槽与独立 Primary Action。
   │  │     └─ XYUI3-24-NavigationDrawer/  # 实现响应式临时导航抽屉。
   │  │        └─ XYNavigationDrawer.cs  # 提供共享导航状态、遮罩、Esc 与卸载关闭。
   │  │     ├─ Foundation/  # 组织该模块下的正式文件。
   │  │     │  ├─ XyuiColorToken.cs  # Canonical 颜色 token 记录（id + Light/Dark 成对解析与 Color 转换）。
   │  │     │  ├─ XyuiColorTokens.Accent.cs  # XY.Accent.*/Tool/Button/Tag 6 色。
   │  │     │  ├─ XyuiColorTokens.Border.cs  # XY.Border.Color.* 与 XY.Divider.* 6 色。
   │  │     │  ├─ XyuiColorTokens.Core.cs  # XY.Color.* CorePalette 母版 10 色。
   │  │     │  ├─ XyuiColorTokens.cs  # 颜色 token 权威表聚合（83 唯一 id、BrushKey、TryFind）。
   │  │     │  ├─ XyuiColorTokens.Editor.cs  # XY.Editor.* 编辑器专用 16 色。
   │  │     │  ├─ XyuiColorTokens.Icon.cs  # XYUI 图标与辅助标记的 Light/Dark 语义色 token。
   │  │     │  ├─ XyuiColorTokens.Semantic.cs  # XY.Semantic.* 语义四态三通道 12 色。
   │  │     │  ├─ XyuiColorTokens.State.cs  # XY.State.* 交互状态与 Disabled/ReadOnly/Locked 三态 17 色。
   │  │     │  ├─ XyuiColorTokens.Surface.cs  # XY.Surface.* 十档背景层级。
   │  │     │  └─ XyuiColorTokens.Text.cs  # XY.Text.* 文本色 6 色。
   │  │     ├─ Interaction/  # 集中控件交互与生命周期职责。
   │  │     │  ├─ XyuiFocusStyles.cs  # 焦点边框环两条样式（xyui-focusable，与 Hover/Selected 视觉分离）。
    │  │     │  ├─ XyuiInteractionState.cs  # 交互状态 selector、canonical token 与资源键唯一真值。
    │  │     │  ├─ XyuiInteractionStyles.cs  # Foundation 状态类样式与 Hover/Pressed/Selected/Focus/Disabled 解析顺序。
    │  │     │  ├─ XyuiStateResolver.cs  # States Foundation 的单通道视觉解析与独立 Focus/Selection 输出。
    │  │     │  └─ XyuiStateSnapshot.cs  # 可共存交互事实、独立 FocusVisible 与 Semantic 状态模型。
   │  │     ├─ Density/  # 信息组织密度 Runtime 与继承范围。
   │  │     │  ├─ XyuiDensity.cs  # Compact/Default/Comfortable 档位与既有 Spacing 组合指标。
   │  │     │  └─ XyuiDensityScope.cs  # 可继承 Density AttachedProperty 与子树查询入口。
   │  │     ├─ Spatial/  # 组织该模块下的正式文件。
   │  │     │  ├─ XyuiShapeStyles.cs  # 语义形状样式类（代码构建 9 类 xyui-border-*/xyui-surface-*/xyui-shadow-*）。
   │  │     │  ├─ XyuiSpatial.cs  # Spatial 基础资源字典构建（Space/Radius/BorderWidth/Shadow，含 BoxShadow 解析）。
   │  │     │  └─ XyuiSpatialTokens.cs  # Spatial/Shape token 权威常量表（Spacing/Radius/Border 宽度/Elevation，转录 token-canonical-map.json）。
   │  │     ├─ Theme/  # 组织该模块下的正式文件。
   │  │     │  ├─ XyuiSectionTitleResources.cs  # SectionTitle S-05 左侧短竖线与标题布局主题资源。
   │  │     │  └─ XyuiTheme.cs  # Light/Dark 双主题 ResourceDictionary 构建器（canonical 成对值）。
   │  │     ├─ Typography/  # 组织该模块下的正式文件。
   │  │     │  ├─ XyuiTextStyles.cs  # 实现对应模块的 C# 职责。
   │  │     │  ├─ XyuiTypography.cs  # Typography 基础资源字典构建（31 个 XY.Font*/XY.FontSize*/XY.FontWeight*/XY.LineHeight*/XY.LetterSpacing* 资源）。
   │  │     │  └─ XyuiTypographyTokens.cs  # Typography token 权威常量表（字体/字号/字重/行高/字距，转录 token-canonical-map.json）。
   │  │     ├─ Vector/  # 组织该模块下的正式文件。
   │  │     │  └─ XyuiVectorIcons.cs  # 实现对应模块的 C# 职责。
   │  │     └─ XYUI.Avalonia.csproj  # XYUI.Avalonia 库项目文件（Avalonia 12.0.4）。
   │  ├─ tests/  # 集中对应模块的自动化测试。
    │  │  └─ XYUI.Avalonia.Tests/  # 组织该模块下的正式文件。
    │  │     ├─ AdaptiveLayoutRuntimeTests.cs  # AdaptiveLayout 容器列数、Gap 与 Size/Density 正交合同。
   │  │     ├─ DensityRuntimeTests.cs  # Density 默认值、继承、覆盖、档位与 Spacing 组合合同。
   │  │     ├─ BadgeRuntimeTests.cs  # Badge 高度、Auto 宽度、左对齐与左指针几何运行时回归。
   │  │     ├─ BrushRuntimeTests.cs  # 主题字典 key/类型/值/重复/缺失测试。
   │  │     ├─ CanonicalAlignmentTests.cs  # token 表与 token-canonical-map.json 逐条对照。
   │  │     ├─ CatalogSourceTests.cs  # Catalog 注册数量与类型映射源同步合同。
   │  │     ├─ CodeTextRuntimeTests.cs  # CodeText 正文、右下 Vector Code Mark 与禁用态资源回归。
   │  │     ├─ Phase1CFeedbackRuntimeTests.cs  # XYUI-1 15～17 反馈文本共用语义、标记结构与禁用态主题合同。
   │  │     ├─ Phase1CSeparatorRuntimeTests.cs  # XYUI-1-14 Separator 六 Variant、方向、厚度、间距与双主题合同。
   │  │     ├─ Phase1CShortcutHintRuntimeTests.cs  # XYUI-1-18 ShortcutHint 分离键帽结构、Foundation 几何、禁用态与双主题合同。
   │  │     ├─ ControlSurfaceTests.cs  # 验证对应模块的自动化行为与回归合同。
   │  │     ├─ GalleryInteractionContractTests.cs  # 验证对应模块的自动化行为与回归合同。
   │  │     ├─ GallerySmokeTests.cs  # App 资源、窗口标题、色板覆盖 Headless 冒烟。
   │  │     ├─ GalleryThemeConstructionTests.cs  # Gallery Light/Dark 主题构造与切换资源一致性测试。
   │  │     ├─ InteractionCombinationTests.cs  # 验证对应模块的自动化行为与回归合同。
   │  │     ├─ InteractionStateTests.cs  # 验证对应模块的自动化行为与回归合同。
   │  │     ├─ XYUI10StateResolverTests.cs  # XYUI-0-10 状态共存、单通道解析、焦点与语义分离合同。
   │  │     ├─ MonoTextResponsiveTests.cs  # MonoText 共享 Label/Value/Unit 列在宽度变化下的稳定对齐回归。
   │  │     ├─ MonoTextRuntimeTests.cs  # MonoText 三列字体、字重、间距、对齐与禁用态合同测试。
   │  │     ├─ NavigationCollapseTests.cs  # 验证对应模块的自动化行为与回归合同。
   │  │     ├─ R5F4F1AlignmentTests.cs  # 验证对应模块的自动化行为与回归合同。
   │  │     ├─ R5F4FidelityTests.cs  # 验证对应模块的自动化行为与回归合同。
   │  │     ├─ Phase1DSearchTruncatedRuntimeTests.cs  # SearchHighlight 结果呈现与 TruncatedText 截断合同回归。
   │  │     ├─ Phase1DSelectableEmptyRuntimeTests.cs  # SelectableText 复制选择与 EmptyText 语义回归。
   │  │     ├─ Phase1DTooltipRichTextRuntimeTests.cs  # Tooltip Foundation 与 RichText 行结构、禁用态回归。
   │  │     ├─ SearchHighlightRuntimeTests.cs  # SearchHighlight 高亮正文与 8 DIP 搜索标记间距、色调和几何回归。
   │  │     ├─ SecondTruthTests.cs  # 防回潮：未登记 hex 扫描 + AXAML 资源引用可解析。
   │  │     ├─ SelectableTextRuntimeTests.cs  # SelectableText 选择能力、Technical 字体及独立 Copy Mark 回归。
   │  │     ├─ ShapeRuntimeTests.cs  # Spatial 资源/语义形状类在真实 Border 上的应用测试。
   │  │     ├─ XYUI08ShapeContractTests.cs  # XYUI-0-08 Shape 三通道组合、轮廓与尺寸稳定性合同测试。
   │  │     ├─ SkeletonTests.cs  # 骨架引用链与 BrushKey 命名测试。
   │  │     ├─ SpatialTokenTests.cs  # Spatial/Shape 常量与 token-canonical-map.json 逐条对照。
   │  │     ├─ SplitButtonCountingCommand.cs  # 验证对应模块的自动化行为与回归合同。
   │  │     ├─ ThemeRuntimeTests.cs  # XYUI 控件 Light/Dark 主题资源解析与运行时切换回归。
   │  │     ├─ TypographyRuntimeTests.cs  # Typography 资源/语义样式类在真实 TextBlock 上的应用测试。
   │  │     ├─ TypographyTokenTests.cs  # Typography 常量与 token-canonical-map.json 逐条对照。
   │  │     ├─ XYSubMenuHierarchyTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI.Avalonia.Tests.csproj  # 测试项目文件（xunit + Avalonia.Headless 12.0.4）。
   │  │     ├─ XYUI1CoverageTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI1DocumentationTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI1TextRuntimeTests.cs  # 验证 XYUI-1 01～06 文本控件 Runtime 与 Link 状态契约。
   │  │     ├─ XYUI1FidelityTests.cs  # 验证对应模块的自动化行为与回归合同。
   │  │     ├─ XYUI2Batch01ReconcileTests.cs  # Batch 01 文档/预览对账回归（计数与真实状态一致）。
   │  │     ├─ XYUI2BoolPropertyTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2ButtonRuntimeTests.cs  # Button Variant→class 与 Action Edge 存在性/弱化/语义/衰减合同。
   │  │     ├─ XYUI2ButtonVisualStateTests.cs  # Button 高度、Hover 与 Pressed 状态回归。
   │  │     ├─ XYUI2ChoiceControlsTests.cs  # Checkbox 三态、Radio 分组、Switch 几何与 Gallery 接线回归。
   │  │     ├─ XYUI2ColorPickerTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2ComboBoxTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2ComponentReconcileTests.cs  # 组件文档登记与 Gallery 预览最小样本对账（含 05 待验收锁）。
   │  │     ├─ XYUI2DatePickerInteractionReworkTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2DatePickerTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2DropDownButtonRuntimeTests.cs  # DropDownButton 单命中区结构与点击语义（含槽区无第二行为）回归。
   │  │     ├─ XYUI2DropDownButtonVisualStateTests.cs  # DropDownButton 五状态视觉合同（含 Chevron 衰减与聚焦环）。
   │  │     ├─ XYUI2GhostToggleRuntimeTests.cs  # IconButton Selected≠Checked 解耦与 ToggleButton Persistent Edge 合同。
   │  │     ├─ XYUI2GhostToggleVisualStateTests.cs  # IconButton 与 ToggleButton 视觉状态回归。
   │  │     ├─ XYUI2InkAlignmentAuditTests.cs  # 家族文字着墨等线与左对齐内距测量合同（BuildGeometry 实测）。
   │  │     ├─ XYUI2InputControlsTests.cs  # TextField、NumberField、Slider、ComboBox、Select、TextArea 运行时合同测试。
   │  │     ├─ XYUI2NumberFieldTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2PasswordFieldTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2Phase2AContractTests.cs  # 验证 Phase 2A (01～06) 触发器契约、双命令区、无障碍与三态回归。
   │  │     ├─ XYUI2PropertyControlsTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2SearchFieldTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2SelectTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2SliderTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2SplitButtonRuntimeTests.cs  # 验证对应模块的自动化行为与回归合同。
   │  │     ├─ XYUI2SplitButtonVisualStateTests.cs  # 验证对应模块的自动化行为与回归合同。
   │  │     ├─ XYUI2TextAreaFocusTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2TextAreaTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2TextInputInteractionTests.cs  # 可编辑文本宿主焦点/鼠标激活全选与占位层防重叠回归。
   │  │     ├─ XYUI2TimePickerInteractionReworkTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2TimePickerTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI2VectorPropertyLayoutTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI3CompactInteractionTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI3CompactNavigationInteractionTests.cs  # 验证紧凑导航新增页签、Popup 菜单与焦点/选择分离。
   │  │     ├─ XYUI3Batch04StructureTests.cs  # 验证分页、步骤、工具栏与工具组结构复用。
   │  │     ├─ XYUI3Batch05StructureTests.cs  # 验证 17～20 紧凑控件与核心交互。
   │  │     ├─ XYUI3BackForwardNavigationTests.cs  # 验证 19 的紧凑结构、历史跳转与 Popup 生命周期。
   │  │     ├─ XYUI3CommandPaletteTests.cs  # 验证 18 的整行结果、详情、Scope、Recent 与 Popup 执行。
   │  │     ├─ XYUI3GalleryNavigationTests.cs  # 验证 XYUI-3 计数与最新组件默认落点。
   │  │     ├─ XYUI3WorkspaceSwitcherTests.cs  # 验证 20 的同宽菜单、整行项、共享 State、请求提交与生命周期。
   │  │     ├─ XYUI3ViewSwitcherTests.cs  # 验证 21 的单一 Surface、分段尺寸、选中态、请求提交、More 与 Popup 生命周期。
   │  │     ├─ XYUI3TableOfContentsTests.cs  # 验证 22 的文本优先层级、Guide、Compact 路径、状态提交与 Popup 生命周期。
   │  │     ├─ XYUI3FinalNavigationTests.cs  # 验证 21~24 状态提交、层级限制、主操作隔离、抽屉生命周期与 Gallery。
   │  │     ├─ XYUI3CompactNavigationStructureTests.cs  # 验证紧凑导航复用、单底边、垂直居中及交互状态机。
   │  │     ├─ XYUI3InteractionTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUI3StructureTests.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XyuiBatchTestHost.cs  # 实现对应模块的 C# 职责。
   │  │     ├─ XYUICompositionReuseTests.cs  # 验证 XYIconLabel 等组件复用公开 XYUI 控件而非重复绘制。
   │  │     ├─ XyuiHeadlessCollection.cs  # Headless 串行 collection 定义（禁并行）。
   │  │     ├─ XyuiHeadlessFixture.cs  # Headless 会话夹具（独立 UI 线程 dispatch）。
   │  │     ├─ XyuiTestAppBuilder.cs  # Headless App 构建器（复用 Gallery App）。
   │  │     ├─ StatusAndIconLabelRuntimeTests.cs  # StatusBadge/StatusDot 五态语义与 IconLabel 复用、对齐、禁用态回归。
   │  │     └─ XYUIVectorViewportTests.cs  # XYIcon 24×24 logical viewport、尺寸与 Stroke 合同。
   │  └─ XYUI.Avalonia.slnx  # XYUI.Avalonia 独立解决方案（库/Gallery/Tests 三项目）。
   ├─ governance/  # 组织该模块下的正式文件。
   │  ├─ amendments.md  # 记录对应主题的当前有效说明。
   │  └─ XYUI-A-plan.md  # 记录对应主题的当前有效说明。
   ├─ packs/  # 组织该模块下的正式文件。
   │  └─ core-0.1/  # 组织该模块下的正式文件。
   │     ├─ AGENT-GUIDE.md  # 记录对应主题的当前有效说明。
   │     ├─ gaps.json  # 保存对应模块的结构化数据。
   │     ├─ manifest.json  # 保存对应模块的结构化数据。
   │     └─ README.md  # 记录对应主题的当前有效说明。
   ├─ registry/  # 组织该模块下的正式文件。
   │  ├─ examples/  # 组织该模块下的正式文件。
   │  │  └─ foundation-registry.example.json  # 保存对应模块的结构化数据。
   │  ├─ foundation/  # 组织该模块下的正式文件。
   │  │  ├─ foundation-registry.json  # 保存对应模块的结构化数据。
   │  │  ├─ foundation-registry.manifest.json  # 保存对应模块的结构化数据。
   │  │  ├─ identity-map.json  # 保存对应模块的结构化数据。
   │  │  ├─ README.md  # 记录对应主题的当前有效说明。
   │  │  ├─ relationship-map.json  # 保存对应模块的结构化数据。
   │  │  └─ validation-report.md  # 记录对应主题的当前有效说明。
   │  └─ schema/  # 组织该模块下的正式文件。
   │     ├─ foundation-registry.schema.json  # 保存对应模块的结构化数据。
   │     └─ README.md  # 记录对应主题的当前有效说明。
   ├─ source/  # 组织该模块下的正式文件。
   │  ├─ XYUI0/  # 组织该模块下的正式文件。
   │  │  └─ XYUI-0.md  # 保存对应 XYUI 规范的原始源文本。
   │  ├─ XYUI1/  # 组织该模块下的正式文件。
   │  │  └─ XYUI-1.md  # 保存对应 XYUI 规范的原始源文本。
   │  ├─ XYUI2/  # 组织该模块下的正式文件。
   │  │  └─ XYUI-2.md  # 保存对应 XYUI 规范的原始源文本。
   │  ├─ XYUI3/  # 组织该模块下的正式文件。
   │  │  └─ XYUI-3.md  # 保存对应 XYUI 规范的原始源文本。
   │  ├─ XYUI4/  # 组织该模块下的正式文件。
   │  │  └─ XYUI-4.md  # 保存对应 XYUI 规范的原始源文本。
   │  ├─ XYUI5/  # 组织该模块下的正式文件。
   │  │  └─ XYUI-5.md  # 保存对应 XYUI 规范的原始源文本。
   │  ├─ XYUI6/  # 组织该模块下的正式文件。
   │  │  └─ XYUI-6.md  # 保存对应 XYUI 规范的原始源文本。
   │  ├─ XYUI7/  # 组织该模块下的正式文件。
   │  │  └─ XYUI-7.md  # 保存对应 XYUI 规范的原始源文本。
   │  └─ XYUI8/  # 组织该模块下的正式文件。
   │     └─ XYUI-8.md  # 保存对应 XYUI 规范的原始源文本。
   ├─ specs/  # 组织该模块下的正式文件。
   │  ├─ XYUI1/  # 组织该模块下的正式文件。
   │  │  ├─ XYUI-1.canonical.md  # 保存对应 XYUI 规范的规范化数据与映射。
   │  │  ├─ XYUI-1.gaps.json  # 保存对应模块的结构化数据。
   │  │  ├─ XYUI-1.identity.json  # 保存对应模块的结构化数据。
   │  │  └─ XYUI-1.mapping.json  # 保存对应 XYUI 规范的规范化数据与映射。
   │  ├─ XYUI2/  # 组织该模块下的正式文件。
   │  │  ├─ XYUI-2.canonical.md  # 保存对应 XYUI 规范的规范化数据与映射。
   │  │  ├─ XYUI-2.gaps.json  # 保存对应 XYUI 规范的规范化数据与映射。
   │  │  ├─ XYUI-2.identity.json  # 保存对应模块的结构化数据。
   │  │  └─ XYUI-2.mapping.json  # 保存对应模块的结构化数据。
   │  ├─ XYUI3/  # 组织该模块下的正式文件。
   │  │  ├─ XYUI-3.canonical.md  # 保存对应 XYUI 规范的规范化数据与映射。
   │  │  ├─ XYUI-3.gaps.json  # 保存对应 XYUI 规范的规范化数据与映射。
   │  │  └─ XYUI-3.mapping.json  # 保存对应 XYUI 规范的规范化数据与映射。
   │  ├─ XYUI4/  # 组织该模块下的正式文件。
   │  │  ├─ XYUI-4.canonical.md  # 保存对应 XYUI 规范的规范化数据与映射。
   │  │  ├─ XYUI-4.gaps.json  # 保存对应 XYUI 规范的规范化数据与映射。
   │  │  └─ XYUI-4.mapping.json  # 保存对应 XYUI 规范的规范化数据与映射。
   │  ├─ XYUI5/  # 组织该模块下的正式文件。
   │  │  ├─ XYUI-5.canonical.md  # 保存对应 XYUI 规范的规范化数据与映射。
   │  │  ├─ XYUI-5.gaps.json  # 保存对应 XYUI 规范的规范化数据与映射。
   │  │  └─ XYUI-5.mapping.json  # 保存对应 XYUI 规范的规范化数据与映射。
    │  ├─ XYUI0.05/  # XYUI-0.05 Density Runtime API 合同。
    │  │  └─ XYUI-0.05-density-runtime-contract.md  # Density / Spacing / Sizing 边界与三档继承 API。
    │  ├─ XYUI0.08/  # XYUI-0.08 Shape Runtime 合同。
    │  │  └─ XYUI-0.08-shape-runtime-contract.md  # Shape 几何边界、组合规则、真实入口与无通用 API 决策。
    │  ├─ XYUI0.09/  # XYUI-0.09 Surface Runtime 合同。
    │  │  └─ XYUI-0.09-surface-runtime-contract.md  # Surface 语义、Canonical 成员、Facade 继承覆盖与浮层边界。
    │  ├─ XYUI0.10/  # XYUI-0.10 States Runtime/Public API 真值合同。
    │  │  └─ XYUI-0.10-runtime-contract.md  # 编号纠正、Public API 真值、测试数量口径与 Gemini 交接。
    │  ├─ XYUI0.11/  # XYUI-0.11 AdaptiveLayout Container-first Runtime 合同。
    │  │  └─ XYUI-0.11-responsive-runtime-contract.md  # 容器宽度列数计算、Reflow、正交边界与限制。
   │  ├─ XYUI6/  # 组织该模块下的正式文件。
   │  │  ├─ XYUI-6.canonical.md  # 保存对应 XYUI 规范的规范化数据与映射。
   │  │  ├─ XYUI-6.gaps.json  # 保存对应 XYUI 规范的规范化数据与映射。
   │  │  └─ XYUI-6.mapping.json  # 保存对应 XYUI 规范的规范化数据与映射。
   │  ├─ XYUI7/  # 组织该模块下的正式文件。
   │  │  ├─ XYUI-7.canonical.md  # 保存对应 XYUI 规范的规范化数据与映射。
   │  │  ├─ XYUI-7.gaps.json  # 保存对应 XYUI 规范的规范化数据与映射。
   │  │  └─ XYUI-7.mapping.json  # 保存对应 XYUI 规范的规范化数据与映射。
   │  └─ XYUI8/  # 组织该模块下的正式文件。
   │     ├─ XYUI-8.canonical.md  # 保存对应 XYUI 规范的规范化数据与映射。
   │     ├─ XYUI-8.gaps.json  # 保存对应 XYUI 规范的规范化数据与映射。
   │     └─ XYUI-8.mapping.json  # 保存对应 XYUI 规范的规范化数据与映射。
   └─ tokens/  # 集中共享尺寸与语义常量。
      ├─ architecture/  # 组织该模块下的正式文件。
      │  ├─ token-architecture.json  # 保存对应模块的结构化数据。
      │  ├─ token-architecture.md  # 记录对应主题的当前有效说明。
      │  └─ token-canonical-map.json  # 保存对应模块的结构化数据。
      └─ audit/  # 组织该模块下的正式文件。
         ├─ token-audit.md  # 记录对应主题的当前有效说明。
         ├─ token-collision-matrix.json  # 保存对应模块的结构化数据。
         └─ token-occurrences.json  # 保存对应模块的结构化数据。
```
