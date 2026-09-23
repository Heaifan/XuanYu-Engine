# WAVE-2 Readiness Integration Freeze

状态：OPEN / Production Viewport Migration 仍 BLOCKED

统一基线：
`2189e7f4a45d0bbd2b0388e6f26bed110aa2918a`

A1.5-R0：
`0fdeb2f3c2b8e50997d2dfe77d3ea57d88b8f511` = FAIL

## WAVE-2 并行任务

### A — A1.5-FIX1
Branch: `spike/viewport-vulkan-avalonia-gpu-composition-fix1`

继续独立 Spike，修正实验条件：
- Avalonia 12.1.3（Spike-only）
- Windows 强制 Vulkan backend
- Vulkan image layout
- GPU Composition / Overlay / Resize 重新裁决

A PASS 才允许解除 Production Viewport Migration blocker。

### B — Unified Input Consumer Readiness R2
Branch: `audit/viewport-input-consumer-readiness-r2`

盘点 Camera/Picking/Gizmo/Region/Road/Marker/Snap 等现有输入消费者与统一 `EditorPointerEvent` 契约的迁移准备度。只做审计、契约和测试，不切生产 route。

### C — Viewport Migration Boundary Guard R2
Branch: `guard/viewport-migration-boundary-r2`

强化 Guard：A1.5 未 PASS 前禁止 GPU Composition Spike 实现进入生产 Viewport，同时禁止扩张 NativeControlHost / WS_CHILD / Native Popup / CPU Readback workaround。

### D — Viewport Migration Regression Baseline R1
Branch: `test/viewport-migration-regression-baseline-r1`

冻结正式迁移前的行为回归基线与一键入口，不改生产行为。

## Merge policy

- B/C/D 允许在互斥文件前提下并行。
- A 不与 B/C/D 互相 cherry-pick。
- A FAIL：B/C/D 可完成，但 Production Migration 继续 BLOCKED。
- A PASS：先审计 A 证据，再决定是否进入 WAVE-3 / XYViewportControl Production Migration。
- 禁止任何任务直接 merge 回旧 A1/B1/C1/D1 独立分支。
- 禁止通过 Native workaround 绕过 A 的技术门。

## Global gates

- ARCH-A
- ARCH-VIEWPORT-R1
- 5+100
- git diff --check
- 各任务定向 Build/Test

所有任务完成后先合入 `feat/wave-2-readiness-integration`，再做统一集成审计。