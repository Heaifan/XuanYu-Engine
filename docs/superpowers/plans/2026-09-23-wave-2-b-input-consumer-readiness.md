# WAVE-2 B — Unified Input Consumer Readiness R2

状态：READY / 与 Composition 解耦

基线：`2189e7f4a45d0bbd2b0388e6f26bed110aa2918a`

## Goal

在不切换生产 Viewport host 的前提下，证明 WAVE-1 B1 的 `EditorPointerEvent` 足以承载现有 Viewport 全部输入消费者，为未来 XYViewportControl 迁移建立可执行清单和契约测试。

## Required inventory

逐项审计并建立 Source -> Adapter -> Owner -> Consumer -> Cancel/Capture 路径：

- Camera navigation
- Navigation Gizmo
- Transform Gizmo
- Picking / selection
- Map geometry
- Region
- Road
- Marker
- Wheel
- Pointer capture
- FocusLost / Cancel / CaptureLost
- Alt / Shift / Ctrl modifiers
- DPI logical-coordinate conversion

## Allowed changes

- tests
- test fixtures
- docs / migration matrix
- 必要的纯输入 abstraction 小补强，但不得切换生产 runtime route

## Forbidden

- 不修改 Vulkan rendering/composition
- 不删除 NativeControlHost / WS_CHILD
- 不把任一 consumer 切到新的生产 host
- 不改变 Camera/Picking/Gizmo/Region/Road/Marker/Snap 用户行为
- 不与 A1.5-FIX1 共改 Spike 文件

## Acceptance

1. 每个 consumer 都有明确输入 owner。
2. 一次 gesture 只能有一个 owner。
3. Native 与 Avalonia 等价输入进入统一契约后语义一致。
4. DPI 只转换一次。
5. Cancel/CaptureLost/FocusLost 都有可验证终止路径。
6. 输出一份 Migration Readiness Matrix，明确 READY / GAP / BLOCKED。
7. 对发现的 GAP 只补契约/测试，不做生产迁移。

## Verification

- 定向输入测试
- World/Editor 相关回归
- ARCH-A
- ARCH-VIEWPORT-R1
- 5+100
- git diff --check
- Build 0W0E

## Delivery

提交审计矩阵、测试证据、所有 GAP、Commit SHA；不得宣称 Production Migration 已开始。