# MAP-A → EDITOR-A · 单轮过渡计划

> 计划状态：OPEN · 用户已于 2026-08-11 明确裁定：本轮不得只做 MAP-A 收口，必须在同一个 Round 内实际进入 `EDITOR-A`。
>
> 当前事实：`MAP-A-R3-D2-F1` 仍为 `OPEN · FINAL ACCEPTANCE FAILED · 5 ITEMS REMAIN`。该失败不得改写为 PASS；旧 Region Drawing 产品路径允许在战略收口中被冻结为非成功终态，并迁移到 REGION-A。
>
> 本计划取代“MAP-A-CLOSE 完成后停止、等待下一轮再启动 EDITOR-A-R1”的旧节奏。收口和下一阶段 Bootstrap 现在属于同一个 Transition Round。

## 一、单轮目标冻结

本轮只允许三个大目标，不再继续向下拆成等待轮：

1. **MAP-A 战略收口**：冻结真实状态、架构边界、知识与遗留项；
2. **立即进入 EDITOR-A**：从收口后的远端 HEAD 创建并切换 `feat/EDITOR-A-workspace`；
3. **完成 EDITOR-A-R1 最小 Workspace Contract**：提交第一批正式 Workspace 代码和测试，并执行完整门禁。

本轮结束时必须达到：

```text
MAP-A 已完成战略收口事实记录
+
旧 F1 失败事实未被伪造成 PASS
+
feat/EDITOR-A-workspace 已成为当前开发线
+
EDITOR-A-R1 已有正式代码落库
+
完整门禁通过
+
Commit + Push + Remote HEAD Verify 完成
```

只完成 MAP-A 文档收口、没有进入 EDITOR-A：**本轮失败。**

---

## 二、阶段 A · MAP-A 战略收口（同轮前半段）

### A1 · Fresh Baseline

执行前 fresh 确认：

- 当前 Branch / HEAD / upstream；
- ahead / behind；
- worktree / staged / untracked / stash；
- 当前版本；
- 当前 MAP-A 状态；
- 现有 Knowledge / Lessons / Backlog；
- 当前生效治理文件。

原有未跟踪 `_tmp_blind_rows/` 若确认属于既有本地残留，本轮继续不触碰，不得为了“干净”擅自删除。

### A2 · 状态冻结

必须保留真实历史：

```text
MAP-A-R3-D2-F1
FINAL ACCEPTANCE FAILED
5 ITEMS REMAIN
```

禁止改写成：

```text
PASS
ACCEPTED
FUNCTIONALLY CLOSED
```

旧 Region Drawing 路径在本轮可以冻结为类似：

```text
SUPERSEDED · NOT ACCEPTED
Migration Target: REGION-A
```

若仓库已有等价状态词，优先复用；核心语义必须同时满足：**非成功、终态、保留失败事实、有明确迁移目标。**

### A3 · 架构收口

只记录已经有证据支持的边界：

- `MapRegion` / `MapRegionDraft` / `MapPoint` / Validator / History：保留；
- Picking / World Viewport / Camera / RenderVectorOverlay：共享基础设施；
- Stroke / Marker / Fill / Ear Clipping / 动态 Buffer / latest-state-wins：复用，不因新 Workspace 重写；
- 旧 Map Editor Region Drawing UI：战略终止，作为 REGION-A Migration Source；
- Project Browser / Debug：后续转 Editor Shell 全局能力；
- Map Editor / Region Editor：后续作为独立 Workspace；
- Map Document / Dataset / Asset：只冻结方向，本轮不实现 JSON。

### A4 · Milestone Knowledge Review

本轮必须完成第八十六条要求的 MAP-A 知识沉淀审计，但不得把它发展成独立开发项目。

候选只允许：

```text
KNOWLEDGE
LESSON
CHANGELOG_ONLY
BACKLOG
REJECTED
CONSTITUTION_CANDIDATE
```

重点审计：

- 为什么 Region Drawing 一次跨 Input / Picking / Draft / Render / Commit / History 会显著增加定位难度；
- 自动测试 PASS 与 Viewport / Input / UI 真机可用之间的差异；
- 已验证 Renderer / Domain 能力在产品架构变化后为什么应优先复用；
- Map Editor Tool 职责膨胀暴露出的 Workspace 边界问题；
- 失败方案、错误根因、临时 workaround 和未来迁移债务。

知识条目必须基于真实仓库、Commit、验收和失败记录，不得凭 AI 记忆编造。

### A5 · MAP-A 收口自动门禁

MAP-A 收口文档、Knowledge、Lessons、Backlog 完成后 fresh 执行：

```text
Solution Build: 0 Warning / 0 Error
Core.Tests: all PASS
World.Tests: all PASS
WarCore.Tests: all PASS
专项: 按实际范围
ARCH-A: PASS
5+100: PASS
版本一致性: PASS
git diff --check: PASS
```

门禁通过后 Commit + Push，并验证远端 HEAD。

**这里不停止、不等待下一轮。**

---

## 三、阶段 B · 同轮切入 EDITOR-A

MAP-A 收口自动门禁和远端 HEAD 验证通过后，立即：

```text
从刚刚验证的 MAP-A 收口远端 HEAD
创建并切换：feat/EDITOR-A-workspace
```

禁止：

- rebase；
- force push；
- merge；
- 删除旧 MAP-A 分支；
- 为切分支创建额外 worktree；
- 把未知本地修改带入新分支。

用户已明确批准本次 Transition Round 跨越里程碑边界，因此**不得以“MAP-A 还需要另一个对话轮等待用户确认”为理由停下。**

MAP-A 的旧失败项仍保持真实；这次跨阶段不是把旧 F1 变成 PASS，而是用户批准的 Strategic Closeout → New Milestone Bootstrap。

---

## 四、阶段 C · EDITOR-A-R1 · Workspace Contract

本轮必须真正修改产品代码，不能只创建分支或计划文件。

### C1 · 最小 Workspace Contract

建立最小 Workspace 架构，具体类名可根据仓库现状调整，但职责必须清楚：

```text
Editor Workspace Identity
Workspace Definition / Contract
Workspace Manager / Owner
```

至少表达：

```text
WorkspaceId
DisplayName
Toolbar identity / slot
Left panel identity / slot
Main content identity / slot
Right panel identity / slot
Enter
Leave
Current Workspace
Switch
```

第一批正式 Workspace 至少注册：

```text
Map Editor
Region Editor
```

Region Editor 本轮只作为 Workspace 身份/占位合同存在，不实现 Region Drawing。

### C2 · 切换不变量

Workspace 切换合同必须冻结：

```text
1. 结束当前临时 Tool 状态
2. 保留 World Context
3. 保留 Camera Context
4. 保留兼容 Selection
5. 切换 Toolbar 上下文
6. 切换 Left Panel 上下文
7. 切换 Right Panel 上下文
8. 新 Workspace 回到默认 Select Tool
```

R1 重点是合同和状态所有权，不要求完整视觉 UI。

### C3 · 本轮必须证明的事情

自动测试至少覆盖：

- 默认 Workspace 为 Map Editor；
- Map → Region 切换成功；
- Region → Map 切换成功；
- 重复切换不产生重复状态；
- Current Workspace 只有一个权威所有者；
- Workspace 切换不创建第二份 World / Camera 权威状态；
- Region Workspace 默认 Tool 为 Select；
- 未开始 Region Drawing；
- Workspace 层不依赖 Vulkan 实现。

### C4 · 明确禁止

EDITOR-A-R1 本轮禁止：

- 完整 Workspace 下拉 UI；
- 左右面板正式重排；
- 删除旧 MapEditorPanel；
- Region Drawing 新实现；
- map.json / Dataset Registry；
- Renderer 重写；
- Picking 重写；
- Unit / Road / Terrain Editor。

这些不是为了继续拆 Round，而是防止本轮再次膨胀成 UI + Input + Picking + Render 的大包。

---

## 五、阶段 D · 本轮最终门禁与 Git

EDITOR-A-R1 最后一处代码/测试/文档修改完成后，重新 fresh 执行一次完整正式门禁：

```text
Solution Build: 0 Warning / 0 Error
Core.Tests: all PASS
World.Tests: all PASS
WarCore.Tests: all PASS
EDITOR-A-R1 focused tests: all PASS
ARCH-A: PASS
5+100: PASS
版本一致性: PASS
git diff --check: PASS
```

随后必须同轮：

```text
Commit
Push
Remote HEAD Verify
```

最终要求：

```text
Branch = feat/EDITOR-A-workspace
Local HEAD == Remote HEAD
Ahead = 0
Behind = 0
```

若本地只有已知既有 `_tmp_blind_rows/` 未跟踪残留，必须继续明确标注其来源和“本轮未触碰”，不得伪报绝对 clean。

---

## 六、本轮完成状态

### 工程执行完成时

应报告：

```text
MAP-A Strategic Closeout: IMPLEMENTED
EDITOR-A-R1 Workspace Contract: IMPLEMENTED
Branch: feat/EDITOR-A-workspace
READY FOR USER ACCEPTANCE
```

这时已经**实际进入 EDITOR-A 开发阶段**，不能再报告“下一步才进入 EDITOR-A”。

### 用户验收对象

用户验收集中到本轮末尾，不在 MAP-A 与 EDITOR-A 之间再插一个等待轮。

必须验收：

1. MAP-A 旧失败事实是否真实保留；
2. Knowledge / Lessons / Backlog 分类是否合理；
3. MAP-A → EDITOR-A 架构边界是否正确；
4. 当前开发分支是否已经是 `feat/EDITOR-A-workspace`；
5. Workspace Contract 是否真实落库；
6. Map / Region Workspace 切换合同是否符合预期；
7. World / Camera 没有复制第二份权威状态；
8. GitHub 与本地 HEAD 是否一致。

如果 Workspace R1 仅涉及纯合同、无可见 UI，本轮用户验收以状态、测试、架构与 Git 证据为主；真正的可见切换 UI 留到后续 Workspace UI 实现时真机验收。

---

## 七、最终项目状态目标

```text
MAP-A
  └─ Strategic Closeout / Superseded legacy path
          ↓
同一个 Transition Round
          ↓
feat/EDITOR-A-workspace
          ↓
EDITOR-A-R1 Workspace Contract IMPLEMENTED
          ↓
READY FOR USER ACCEPTANCE
```

**硬要求：本轮不得停在“MAP-A-CLOSE 完成，EDITOR-A 尚未开始”。**
