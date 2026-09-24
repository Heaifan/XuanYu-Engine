# WAVE-2 D — Viewport Migration Regression Baseline R1

状态：READY / Test-only

基线：`2189e7f4a45d0bbd2b0388e6f26bed110aa2918a`

## Goal

在任何 Production Viewport migration 之前冻结“不能被迁移破坏”的行为基线，形成一键回归入口。此任务不实现新 Viewport。

## Baseline coverage

至少覆盖：

- Camera pointer navigation
- Wheel dolly
- Picking / selection
- Navigation Gizmo
- Transform Gizmo
- Region preview/edit
- Road preview/edit
- Marker interaction
- Snap / native Alt semantics
- Pointer capture
- Cancel / FocusLost / CaptureLost
- DPI coordinate semantics
- Diagnostic placement contract

## Required output

1. 盘点现有测试，不重复造已有测试。
2. 为缺失但可无 UI 验证的关键 invariant 增加最小测试。
3. 新增一个独立回归入口脚本，串联相关 suites/guards。
4. 记录当前已知 baseline failures；不得顺手清理范围外历史失败。
5. 输出 before-migration regression manifest。

## Forbidden

- 不改生产交互行为
- 不改 renderer
- 不做 Composition migration
- 不改 Native host ownership
- 不做 UI 美化
- 不以删除/放宽测试来获得 PASS

## Acceptance

- 一条命令可执行 Viewport migration 核心回归。
- 新入口能区分 NEW failure 与已登记 baseline failure。
- ARCH-A 与 ARCH-VIEWPORT-R1 纳入结果。
- 所有新增手写文件满足 5+100。
- git diff --check PASS。

## Delivery

给出回归入口、测试清单、baseline failure 清单、运行结果、Commit SHA。