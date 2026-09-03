# XYUI 双 Agent 开发规范入口

本目录存放 XYUI 的 Codex + Gemini 双 Agent 协作规范。附件原文以
`XYUI_Codex_Gemini双Agent开发与代码封装规范_v1.0.md` 原样保存，SHA-256 为
`31FA7827975FF7444B9A7DA44BEE5954B7D6ECAE719253A1AD4862408B3F6843`。

## 规则优先级

本目录规范是 XYUI 专项 Current Working Standard，不替代仓库入口
`AGENTS.md`、`docs/dev-rules.md` 或 `docs/玄域引擎_AI开发宪法.md`。发生冲突时，先按更高优先级规则执行，并立即向用户报告冲突与证据；未经用户明确冻结，不把本目录内容自动升格为宪法条款。

优先级从高到低为：

1. 用户当前明确请求与批准。
2. `docs/玄域引擎_AI开发宪法.md`。
3. `AGENTS.md` 与 `docs/dev-rules.md`。
4. 本目录中的 XYUI 双 Agent 协作规范。
5. 具体任务书、Gallery 截图、口述和历史日志。

## 固定所有权

| 参与方 | 默认所有权 |
| --- | --- |
| 用户 | 需求、设计方向、Public API / Runtime Contract / Canonical 变更批准、视觉与交互最终验收、冻结与最终拍板 |
| Gemini | Gallery View/Section XAML、页面布局、响应式 Presentation、中文说明、示例组合、SVG 与 Gallery 视觉装修 |
| Codex | Runtime、组件类型与组合、Attached Property、Facade、Canonical Resolver、Sizing/Density、Interaction、State、Template Contract、Runtime/Architecture Tests、Build/ARCH-A/5+100 |

## 每轮互相监督

编辑前先列出 `CODEX OWNERSHIP`、`GEMINI OWNERSHIP`、允许文件和禁止项。收口前双方按实际 diff 审查：

- Codex 检查 Gemini：不得改 Runtime/API/Tests/ARCH-A，不得复制 Resolver/Token/Geometry，不得用假控件、手写尺寸或 Gallery-only 规则冒充 Runtime。
- Gemini 检查 Codex：不得擅自重做 Presentation、改变既定视觉方案、顺手修改无关 Gallery，不得以 Runtime 正确为由忽略可读性。
- 任一方发现 Runtime 缺能力、所有权越界、第二真值、Hidden Border、Margin 冒充 Gap、手写 Height 冒充 Size 或跨界改动，必须立即 STOP 并报告，不得用临时补丁掩盖。

越权报告至少包含：参与方、发现时间、分支与 commit/工作树、文件和行号、违反的条款、可复现证据、影响、是否已停止以及等待的用户裁决。未完成报告前，不得继续扩大修改范围。

## 证据状态不得混用

- `TECHNICAL PASS`：Build、Tests、ARCH-A、5+100、diff-check 等自动/技术证据。
- `PRESENTATION IMPLEMENTED`：Gallery Presentation 已完成。
- `READY FOR USER VISUAL REVIEW`：具备人工复核条件。
- `USER VISUAL ACCEPTED`：仅用户可以给出。
- `FINAL CLOSEOUT`：只有 Runtime、Presentation、技术门禁和用户验收全部满足后才可使用。

标准执行顺序：明确目标 → 对照所有权 → Runtime/Presentation 分工 → 技术门禁 → Gallery 真实复核 → 用户验收 → Codex 收口 → changelog/file-tree 同步。
