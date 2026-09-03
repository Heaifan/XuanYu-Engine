# 玄域引擎 docs 索引

> 本文档只回答「哪类文档在哪里」；完整文件职责见 `file-tree.md`。分类依据：SHR-2026-08-R2 文档治理（2026-08-03）。

## 治理文档（docs/ 根目录 + governance/）

- `docs/玄域引擎_AI开发宪法.md`：最高长期治理规则（唯一宪法事实源）
- `docs/dev-rules.md`：开发硬规则执行手册
- `docs/CODE_CONSTITUTION.md`：代码与架构硬规则
- `docs/governance/版本号规范与历史映射.md`：版本格式与历史编号映射
- `docs/governance/dev-rules-understanding.md`：dev-rules 解释
- `docs/governance/diagnostic-safety.md`：诊断日志与 UI 调度安全规范
- `docs/governance/NAMING_RULES.md`、`naming-XuanYu-Engine.md`：命名与品牌规范
- `docs/governance/ui-spec.md`：UI 规范 1.0 讨论初稿（强约束 UI 默认标准与受控例外机制，待审订）
- `docs/governance/xyui/`：XYUI Codex + Gemini 双 Agent 开发规范与越权监督入口
- `docs/governance/debts/arch-world-debts.md`：受控债务登记
- `docs/governance/shr-2026-08-closure.svg`：SHR-2026-08 考核收口图

## 开发知识库（knowledge/）

- `docs/knowledge/README.md`：知识库目的、字段、证据、生命周期与使用说明
- `docs/knowledge/knowledge-index.md`：Knowledge / Lesson 类型化总索引
- `docs/knowledge/engineering.md`：验证、治理与工程流程知识
- `docs/knowledge/architecture.md`：空间、架构与组合根知识
- `docs/knowledge/rendering.md`：Vulkan、Depth、Overlay 与 Native 渲染知识
- `docs/knowledge/input.md`：Pointer 与 Mouse Capture 输入生命周期知识
- `docs/knowledge/ui.md`：Avalonia 布局、命中区与 Runtime UI 知识
- `docs/knowledge/data.md`：数据保存、加载事务与资产处理知识
- `docs/knowledge/performance.md`：Preview/Commit 高频路径性能知识
- `docs/knowledge/incidents.md`：代表性事故复盘与 Knowledge 映射
- `docs/knowledge/lessons.md`：错误前提、停止条件与可复用教训

## 当前阶段（milestones/current/）

- `docs/milestones/current/EDITOR-A/EDITOR-A-R1-workspace-contract.md`：EDITOR-A-R1 的纯 Editor Workspace Contract、范围和验收证据
- `docs/milestones/current/EDITOR-A/editor-a-r1-workspace-contract.svg`：EDITOR-A-R1 Workspace Owner 与切换不变量图
- `docs/milestones/current/EDITOR-A/EDITOR-A-R2-workspace-switch.md`：EDITOR-A-R2 可见 Workspace 切换、组合不变量、门禁与真机 IPO
- `docs/milestones/current/EDITOR-A/editor-a-r2-workspace-switch.svg`：EDITOR-A-R2 选择器、左右上下文宿主与唯一 Viewport 的浅色结构图
- `docs/milestones/current/EDITOR-A/EDITOR-A-R3-mode-shell.md`：EDITOR-A-R3 Manage/Edit Mode、默认 Shell、门禁与真机 IPO
- `docs/milestones/current/EDITOR-A/EDITOR-A-R3-F1-shell-compact.md`：EDITOR-A-R3-F1 紧凑 Shell、统一 Mode 控件、门禁与最终真机 IPO
- `docs/milestones/current/EDITOR-A/editor-a-r3-mode-shell.svg`：EDITOR-A-R3 Mode、Edit Workspace 和常驻 Shell 的浅色结构图
- `docs/milestones/current/MAP-A/`：MAP-A 地图合同与当前轮验收材料
- `docs/milestones/current/MAP-A/R3-backlog.md`：R2 关闭后的 MAP-A-R3 候选方向与冻结前约束
- `docs/milestones/current/MAP-A/R3-F1-closeout.md`：F1 FINAL 15 项真机 IPO 收口清单
- `docs/milestones/current/MAP-A/MAP-A-strategic-closeout.md`：MAP-A 战略终止、知识审计和 EDITOR-A 迁移边界
- `docs/milestones/current/MAP-A/MAP-A-CLOSE-plan.md`：MAP-A-CLOSE 的 C1～C4 收口计划与里程碑知识沉淀门禁
- `docs/milestones/current/MAP-A/viewport-overlay-development-plan.md`：OVL-R0～R3 比例尺架构整改开发计划
- `docs/milestones/current/MAP-A/viewport-overlay-roadmap.svg`：Viewport Overlay / Scale Indicator 浅色路线图
- `docs/milestones/current/MAP-DOC-A/`：MAP-DOC-A-R1 Map Content Navigation、Manifest 合同与真机验收材料
- `docs/milestones/current/MAP-DOC-A/MAP-DOC-A-R1-F1-carryover.md`：R1 M07 身份同步失败事实、F1 修复合同与待补验记录
- `docs/milestones/current/MAP-DOC-A/MAP-DOC-A-R2-plan.md`：R2 Dataset Registry 分段合同、范围与验收边界
- `docs/milestones/current/MAP-DOC-A/MAP-DOC-A-R1-F1-acceptance.md`：R1-F1 Manifest ID 同步与复制布局真机 IPO 清单
- `docs/milestones/current/MAP-DOC-A/MAP-DOC-A-R2-acceptance.md`：R2 Dataset Registry 真机 IPO 清单与未 CLOSED 边界
- `docs/milestones/current/MAP-DOC-A/MAP-DOC-A-R2-F1-root-cause.md`：R2-F1 Create/Register 取证矩阵、根因与修复证据
- `docs/milestones/current/MAP-DOC-A/MAP-DOC-A-R2-F2-root-cause.md`：R2-F2 列表状态同步、中文展示与自动 ID 取证矩阵
- `docs/milestones/current/MAP-DOC-A/MAP-DOC-A-R2-F2-acceptance.md`：R2-F2 真机 IPO 验收模板与 R2-M02 补验路径
- `docs/milestones/current/MAP-DOC-A/MAP-DOC-A-R2-F3-root-cause.md`：R2-F3 Dataset 选择态与右侧 Layer Projection 取证矩阵
- `docs/milestones/current/MAP-DOC-A/MAP-DOC-A-R2-F3-acceptance.md`：R2-F3 Dataset/Layer 双向选择与解除注册真机 IPO 模板

## UI 规范（docs/ui/，ARCH-UI-SPEC-R1 治理产物）

- `docs/ui/玄域引擎_UI规范_1.0.md`：**UI 规范 1.0 正式规范（唯一 UI 规范事实源，UI Spec 1.0）**
- `docs/ui/玄域引擎_旧UI审计矩阵.md`：旧 UI 全量审计矩阵（违规清零追踪）
- `docs/ui/玄域引擎_UI真机基线清单.md`：真机验收共用 IPO 清单与 D0 基线
- 讨论初稿决策原文：`docs/governance/ui-spec.md`（**历史讨论决策记录，不再作为实施合同**；正式规则以 UI 规范 1.0 为准）

## XYUI Runtime 合同

- `xyui/specs/XYUI0.09/XYUI-0.09-surface-runtime-contract.md`：Surface Runtime 定义、Canonical 成员、Facade、继承覆盖与 Popup/Tooltip 边界。
- `xyui/specs/XYUI0.10/XYUI-0.10-runtime-contract.md`：States Runtime/Public API 真值、编号纠正、测试数量口径与 Gemini 交接事实。

## 架构文档（architecture/）

- `docs/architecture/ENGINE_ARCHITECTURE.md`：引擎总体架构
- `docs/architecture/world-a-r0-coordinate-contract.md`：官方坐标合同（Z-Up、XY 水平、X×Y=Z）

## 已关闭阶段（milestones/closed/）

按大里程碑归档，同一阶段内不再按 R 分目录：

- `ARCH-A/`、`ARCH-B/`：早期规划
- `ARCH-C/`：真实场景编辑交互闭环（R2–R8）
- `ARCH-WORLD/`：物理分层与状态所有权治理（R0–R6）
- `WORLD-A/`、`WORLD-B/`、`WORLD-C/`：世界实体、编辑交互、场景文档里程碑
- `RZ-VK/`：M2 早期 Vulkan 生命周期与日志 UX（VK1–VK5、LOG-UX）
- `M1/`：首版引擎构建阶段审计与计划（9.0X/9.1A 系列、项目章程）
- `MAP-A/`：MAP-A-R2 收口报告与交付证据

## 归档（archive/）

- `docs/archive/changelog/`：changelog 月度归档（changelog-YYYY-MM.md，索引见 changelog.md）
- `docs/archive/superseded/`：已被新文档取代但仍保留审计历史（旧规则、旧仓库审计）

## 查找旧阶段证据

- changelog 历史：`docs/archive/changelog/` 按自然月，或 `changelog.md`「历史归档索引」
- 已关闭阶段文档：按大里程碑在 `milestones/closed/` 下查找
- SHR-2026-08-R2 迁移前（2026-08-03 之前）文档全部平铺于 docs 根目录；迁移采用 `git mv` 保留历史，任意旧路径可用 `git log --follow -- <旧路径>` 追溯，无需记忆旧路径
