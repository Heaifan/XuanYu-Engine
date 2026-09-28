# XYK Task Domain Map

> 施工 Agent 先读取 `xyk/index/knowledge-index.md`；这里只提供远端预检入口映射。

| Domain | 优先读取 |
|---|---|
| Validation / Delivery | K-VAL-001、K-VAL-002、K-GOV-*、EXP-GOVERNANCE-* |
| Architecture / Ownership | K-ARCH-*、L-ARCH-*、相关 DEC、EXP-ARCH-* |
| Rendering / Native | K-REN-*、K-NATIVE-*、L-REN-*、L-NATIVE-* |
| Input / Pointer | K-INP-*、EXP-TEST-*、相关 Native Knowledge |
| UI / Inspector / Diagnostic | K-UI-*、K-DIAG-*、EXP-UI-* |
| Data / Save / Asset | K-DATA-*、K-ASSET-* |
| Performance | K-PERF-* |
| Global Migration | 相关 K/L/DEC + EXP-GOVERNANCE-* |
| Agent Error Pattern | `xyk/incidents/agent-error-log.md` + `xyk/experience/rules.md` |

Planning 与 Execution 两阶段都必须从远端最新 `xyk/main` 获取 Knowledge SHA。
