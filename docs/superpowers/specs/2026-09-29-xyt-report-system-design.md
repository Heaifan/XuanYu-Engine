# XYT-F 报告体系设计

## 目标

让每轮测试产生可追踪、可巡检、可审计的正式 T 报告，并能按月、季、年聚合；报告身份必须绑定版本号、Commit、分支、测试模式、受影响能力、测试集版本和时间。

## 方案

仓库根 `xyt.ps1` 作为唯一入口，使用 Git 与 `scripts/resolve-version.ps1` 采集身份。每次 `report` 生成同名 JSON/Markdown 文件到 `docs/reports/xyt/runs/YYYY/MM/`，JSON 是机器事实源，Markdown 是巡检视图。`aggregate` 只读取已生成的 JSON，按 UTC 时间窗口输出周期 JSON/Markdown；不补造缺失字段，不把失败改写成通过。`upload` 只上传已存在且身份完整的报告，默认使用当前 GitHub remote 的普通 Git push，失败保留本地文件并返回可复核错误。

## 命令契约

- `xyt.ps1`：保持现有 QUICK_VALIDATION 空运行兼容。
- `xyt.ps1 report -TestMode <mode> -TestSetVersion <version> -AffectedCapability <capability> -Status <PASS|FAIL|BLOCKED> [-Evidence <path>] [-StartedAt <ISO8601>]`：生成正式 T 报告。
- `xyt.ps1 aggregate -Period <month|quarter|year> [-At <ISO8601>]`：聚合窗口内所有 JSON 报告。
- `xyt.ps1 upload -ReportPath <path> [-AggregatePath <path>]`：校验报告后将精确文件提交并推送到当前 upstream；不使用全量 stage。

## 不变量

1. `Version`, `Commit`, `Branch`, `TestMode`, `AffectedCapabilities`, `TestSetVersion`, `Timestamp` 必须存在且非空。
2. Commit 使用完整 SHA；Dirty 工作区只能标记为 `DIRTY_RUNTIME_PROBE`，不能伪装正式清洁身份。
3. 聚合只统计窗口内报告，保留 PASS/FAIL/BLOCKED 原状，并记录每个报告路径与身份。
4. 上传前执行 JSON schema 级校验、`git diff --check` 和精确路径检查；任何失败不 push。

## 验证

PowerShell 自测覆盖：缺字段拒绝、身份采集、PASS/FAIL 报告生成、月/季/年边界、聚合保留失败、上传预检拒绝 dirty/缺报告，以及既有 `XYT/tests/xyt.selftest.ps1` 启动兼容性。
