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
- `docs/governance/debts/arch-world-debts.md`：受控债务登记
- `docs/governance/shr-2026-08-closure.svg`：SHR-2026-08 考核收口图

## 当前阶段（milestones/current/）

- `docs/milestones/current/MAP-A/`：MAP-A 地图合同与当前轮验收材料

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

## 归档（archive/）

- `docs/archive/changelog/`：changelog 月度归档（changelog-YYYY-MM.md，索引见 changelog.md）
- `docs/archive/superseded/`：已被新文档取代但仍保留审计历史（旧规则、旧仓库审计）

## 查找旧阶段证据

- changelog 历史：`docs/archive/changelog/` 按自然月，或 `changelog.md`「历史归档索引」
- 已关闭阶段文档：按大里程碑在 `milestones/closed/` 下查找
- SHR-2026-08-R2 迁移前（2026-08-03 之前）文档全部平铺于 docs 根目录；迁移采用 `git mv` 保留历史，任意旧路径可用 `git log --follow -- <旧路径>` 追溯，无需记忆旧路径
