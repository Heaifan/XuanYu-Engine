# WAVE-2 Closeout / WAVE-2.5 Entry Freeze

状态：WAVE-2 readiness closed；WAVE-2.5 input convergence opened

## Accepted WAVE-2 evidence

### A — A1.5-FIX1 GPU Composition

Branch: `spike/viewport-vulkan-avalonia-gpu-composition-fix1`

Verdict commit:
`f67210debb3a4f201662933b0d549a3939d8850d`

结论：PASS。

已证明：

`Vulkan GPU Image -> Avalonia Vulkan Backend -> ImportImage -> CompositionDrawingSurface.UpdateWithSemaphoresAsync -> CompositionSurfaceVisual -> Avalonia Overlay`

因此 GPU Composition 技术门解除。

注意：Spike 代码保持独立，不在此 readiness integration 中合并为生产实现。

### B — Input Consumer Readiness

Commit:
`a2933d931f59b13f3a753c8890fcc93e1e1b1e6f`

结论：PARTIAL。

- Consumers: 25
- READY: 7
- GAP: 18
- BLOCKED: 0

此结果定义 WAVE-2.5 的工作范围。

### C — Migration Boundary Guard

Commit:
`15fc7503d28b99168a20ba550423843aae779774`

结论：PASS。

### D — Migration Regression Baseline

Commit:
`78c78fa65d7841d77e9b960a23c194590d3d0d69`

结论：PASS。

Known Baseline failures 保留登记；New Regression 必须保持 0。

## WAVE-2.5 Goal

将 Viewport Input Readiness 从：

`7 / 25 READY`

收口到：

`25 / 25 READY, GAP=0, BLOCKED=0`

在达到该 Gate 前禁止：

- XYViewportControl Production Migration
- 删除 NativeControlHost
- 删除 Win32ViewportHost
- 正式切换 Avalonia Viewport 输入入口
- 以 GPU Composition PASS 为理由跳过输入收口

## WAVE-2.5 Phase 1

三路并行：

A. Unified ViewportInputRouter + Gesture Owner arbitration
B. Cancellation Lifecycle
C. Native/Avalonia Platform Input Parity

三路都从同一 WAVE-2.5 integration baseline 开工，文件范围必须互斥。

## Global gates

- Solution Build: 0W/0E
- Targeted tests
- ARCH-A
- ARCH-VIEWPORT-R1
- 5+100
- git diff --check
- Viewport migration regression baseline: New Regression = 0

WAVE-2.5 Phase 1 完成后再开 Consumer migration（Camera/Picking/Gizmo 与 Map Editing）。