# Ownership、Candidate 与提交状态

## Candidate Tree Purity

候选基线是正式 HEAD `d0335c52b0c9938277f571cc58d11c4d038f3b43` 的 `git archive`，再覆盖本任务明确拥有的 8 个源/测试文件。Candidate 没有 Git Worktree、没有工作树里的共享 Dirty，也不包含 `Directory.Build.props`、WP2、XYUI TOP、地图、地形、渲染或 Silent Host 文件。

物化快照的 8 个目标文件 SHA-256 与工作树 Owner 清单逐一相同（0 mismatch）；归档 SHA-256、源码哈希、候选身份、Release 二进制哈希见 `candidate-identity.json`。Candidate Release Build/Test 与 Runtime 均在该快照完成。

## 5+100 与 SRP

8 个 Candidate 源码/测试文件均 ≤100 行（最大 97 行）。职责拆分为：命令路由、场景 API 契约、场景命令校验、UI Dispatcher Runtime 适配、UiVm 操作编排、状态/历史守卫、Fake 扩展与命令测试。World 与 Editor History 的权威 Owner 保持不变。

## 版本与正式提交

- 正式 HEAD 的 `Directory.Build.props` 版本是 `0.3.0.24-fix`。
- 当前 Dirty `Directory.Build.props` 改为 `0.3.0.25-fix`，它不属于 XYC，且与 WP2 版本施工一致。
- 开发宪法第34条禁止独立 Version Event 台账；本轮没有获授权的新版本号，不能将未提交 WP2 版本视作 XYC 版本，也不能自行猜 FEATURE 编号。
- XYC Candidate 代码、测试和 QA 报告未暂存、未 Commit、未 Push。`git status` 显示 191 条其他开发线 Dirty，清单在 `foreign-dirty-preserved.txt`；均保留。
- Remote branch 在核验时仍为基线 HEAD；Ahead/Behind 为 `0/0`。没有正式 P2-A Commit 或 Push，因此提交验收项未满足，状态 PARTIAL。

正式收口需要协调共享产品版本源归属与合法版本处理，再由 Parent 作为唯一 Commit/Push Owner 完成精确路径提交与远端复核。不得恢复独立 Event 台账。

## R2 纯 Candidate、5+100 与最终归属

- 最终 Candidate 以正式 HEAD `d0335c52b0c9938277f571cc58d11c4d038f3b43` 为父树，只叠加 9 个 XYC 源码/测试路径、`docs/qa/XYC-P2-A-R1/` 和授权的 Changelog 条目。外部 NASADEM fixture 仅用于临时验证，不进入 Candidate。
- Candidate Source Tree Identity：`32e5276330117b7112fedad4e2e15f038b63577c`；所有源码哈希与 `candidate-identity-r2.json` 一致。正式 Git Candidate Tree 在精确临时 Index 构造后核验。
- 9 个 `.cs` 文件均 ≤100 行，最大 100 行。SRP：API 契约、命令识别/校验、UI Dispatcher 适配、UiVm 场景编排、事务守卫、Fake 与真实 UiVm 故障测试分责清晰；World 与 History Owner 未改变。
- 正式版本保持 `0.3.0.24-fix`。`Directory.Build.props` 不入候选；WP2 的 0.3.0.25-fix Dirty 不覆盖、不提交。
- Parent 独占精确 Index 提交与 Push；所有其它 Dirty 在提交前记录并在提交后逐路径复核保全。
