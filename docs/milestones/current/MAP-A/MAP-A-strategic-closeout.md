# MAP-A 战略收口与 EDITOR-A 迁移裁定

**状态**：IMPLEMENTED · AUTOMATED GATES PASS · PUSH PENDING

**范围**：本文件只记录 MAP-A 的战略收口、知识沉淀审计和向 EDITOR-A 的迁移边界；不实现 Region Drawing、Workspace UI、Schema、Renderer 或 Picking 修改。

## 1. Fresh Baseline

- 收口计划基线：`feat/MAP-A-R3` / `1da9902`；
- 当前旧路径：`MAP-A-R3-D2-F1 = FINAL ACCEPTANCE FAILED · 5 ITEMS REMAIN`；
- 既有未跟踪 `_tmp_blind_rows/`：本轮不触碰；
- 生效治理：宪法 2.2 第八十六条、`dev-rules.md` 第 18 节；
- 证据来源：`R3-F1-closeout.md`、`R3-backlog.md`、MAP-A changelog、Knowledge/Incidents、可核验 Commit。

## 2. 战略终止状态

`MAP-A-R3-D2-F1` 不得改写为 `PASS`、`ACCEPTED` 或 `CLOSED`。用户批准其旧 Region Drawing 产品路径进入：

```text
SUPERSEDED · NOT ACCEPTED
Migration Target: REGION-A
```

这只终止旧产品路径，不删除已经验证的基础能力：Map/Region Domain、MapPoint/Validator/History、Picking、Camera、`RenderVectorOverlay`、Depth Policy、Ear Clipping、动态 Buffer 与 latest-state-wins。后续 Workspace 必须迁移复用这些合同，不得借战略终止重写它们。

## 3. 架构边界

| 归属 | 战略裁定 |
|---|---|
| MapRegion / MapRegionDraft / MapPoint / Validator / History | 保留领域与编辑会话事实；不由 Workspace 复制。 |
| Picking / World Viewport / Camera / RenderVectorOverlay | 共享基础设施；Workspace 只消费/协调，不拥有第二份状态。 |
| Stroke / Marker / Fill / Ear Clipping / 动态 Buffer / latest-state-wins | 已验证渲染合同；后续优先复用。 |
| 旧 Map Editor Region Drawing UI | 战略终止；作为 `REGION-A` Migration Source。 |
| Project Browser / Debug | 后续 Editor Shell 全局能力，不属于本轮 Workspace UI。 |
| Map Editor / Region Editor | 独立 Workspace；本轮只建立 R1 纯合同。 |
| Map Document / Dataset / Asset | 仅冻结方向；本轮不实现 JSON 或 Registry。 |

## 4. Milestone Knowledge Review

| ID | 候选 | 真实证据 | 分类 | 处理 |
|---|---|---|---|---|
| K01 | 产品模式膨胀时先建立 Workspace 边界 | F1 输入竞争、渲染返工、未验收 Region 产品路径、过渡计划 | KNOWLEDGE | 新增 K-ARCH-002 |
| L01 | 跨完整交互链的产品切片需要先拆清边界 | F1-M06 FAIL、INC-2026-08-10-001/002、F1 多轮返工 | LESSON | 新增 L-ARCH-001 |
| C01 | F1-FAR-SAFE/RECOVERY 修复细节 | F1 closeout / changelog | CHANGELOG_ONLY | 保留既有历史，不复制进 Knowledge |
| B01 | 旧 Region Drawing 未验收行为 | F1-M03～M06、M15 与 R3 backlog | BACKLOG | 新增 REGION-A-MIG-001 |
| X01 | 自动门禁足以判定旧 Viewport/Region UX 成功 | 真机 IPO 5 项未通过 | REJECTED | 不进入长期知识 |
| P01 | 新宪法条款 | 第八十六条已覆盖 | REJECTED | 无 Constitution Candidate |

审计结果：新增 Knowledge 1；更新 Knowledge 0；新增 Lesson 1；新增 Backlog 1；`CHANGELOG_ONLY` 1；`REJECTED` 2；Constitution Candidate 0。

## 5. 过渡约束

MAP-A 自动门禁、Commit、Push 和远端 HEAD 复核完成后，同一 Transition Round 立即从该远端 tip 创建 `feat/EDITOR-A-workspace`。该动作不等于 F1 通过；`EDITOR-A-R1` 只允许 Workspace Identity、Definition、Manager、切换不变量和自动回归，禁止 Region Drawing、Workspace UI、Renderer/Picking 重写和 Schema。

## 6. 禁止项确认

- [x] 未将旧 F1 FAIL 改写为 PASS/CLOSED。
- [x] 未删除事故、验收或失败历史。
- [x] 未把未经验证猜想写成 Knowledge。
- [x] 未自动创建 Constitution Candidate 或修改宪法规则体系。
- [x] 未修改产品代码、Renderer、Picking、Schema 或 Region Drawing。

## 7. MAP-A 收口自动门禁

- Solution Build：0 Warning / 0 Error；
- Core.Tests：339/339 PASS；
- World.Tests：1115/1115 PASS；
- WarCore.Tests：22/22 PASS；
- ARCH-A、5+100、版本一致性、`git diff --check`：PASS；
- 执行日期：2026-08-11（UTC+08:00）；Commit / Push / Remote HEAD：待本阶段收口提交补证。
