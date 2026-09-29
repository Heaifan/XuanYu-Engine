# XYT-I / P4 IPO Generator

## 目的

`xyt-ipo.ps1` 根据受影响 Capability 生成最小的人工产品验收清单。它只负责 P4 用户验收，不执行产品操作、不读取运行时判定，也不能自行把结果写成 P4 PASS。

## 调用

```powershell
pwsh -File scripts/governance/xyt-ipo.ps1 `
  -Capability mode-switch,region-editing `
  -ChangeSet context-transition,region-drag `
  -RequiredP4Scope mode-switching,region-editing `
  -Version v0.3.0.0-r1 -Commit <sha> -Branch <branch> `
  -OutputPath .artifacts/p4-ipo.json
```

Capability 映射位于 `scripts/governance/xyt-ipo.mapping.ps1`。相同 Capability 或映射到同一 IPO 的输入只产生一项；未知 Capability 直接失败，禁止猜测验收内容。

## 输出合同

每项固定包含：`序号`、`路径`、`输入 I`、`过程 P`、`输出 O`、`判定`。过程至少包含带 `①`、`②`、`③` 的可观察步骤，不接受一句模糊描述。文档顶层同时保留 Capability、Change Set、Required P4 Scope、Version、Commit 和 Branch。

默认判定为 `P4 PENDING`。只有用户显式传入 `-UserVerdict PASS` 或 `-UserVerdict FAIL` 时，生成结果才记录对应用户判定；自动化 Selftest 只能验证字段保持 Pending，不能产生产品验收 PASS。

## 验证

```powershell
pwsh -File XYT/Acceptance/xyt-ipo.selftest.ps1
```

Selftest 覆盖单 Capability、多 Capability、去重、过程编号、最小集合及用户判定字段保持 Pending。

## KNOWLEDGE / EXPERIENCE AUDIT HANDOFF

Candidate Lesson：P4 必须是 Capability 驱动的最小人工 IPO 投影；自动化生成器只能生成步骤与 Pending 状态，不能把 P0-P3 证据升级为 P4 PASS。请 ChatGPT 审计该规则是否值得长期沉淀，并按 SEARCH EXISTING 后的 CREATE / UPDATE / STRENGTHEN / RETIRE / NO DEPOSIT 决策处理。
