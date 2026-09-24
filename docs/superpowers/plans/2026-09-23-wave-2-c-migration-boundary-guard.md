# WAVE-2 C — Viewport Migration Boundary Guard R2

状态：READY / Governance-only

基线：`2189e7f4a45d0bbd2b0388e6f26bed110aa2918a`

## Goal

在 A1.5 未 PASS 前把“可实验”和“可进入生产”的边界写死，防止任何 Agent 因 Spike 代码存在而提前把 GPU Composition 或新的 Native workaround 混入生产 Viewport。

## Guard rules

1. 现有三文件 Temporary Legacy Allowlist 不扩张：
   - VulkanNativeHost.cs
   - Win32ViewportHost.cs
   - DiagnosticOverlayHost.NativeOverlay.cs
2. 禁止新增 Viewport NativeControlHost。
3. 禁止新增 WS_CHILD / CreateWindowEx Viewport 路线。
4. 禁止新增 Native Popup workaround。
5. 禁止 CPU Readback 作为 Viewport composition route。
6. A1.5 未 PASS 前，生产 `XuanYu.Editor.UI` 不得引入 Spike-only GPU import/composition 实现。
7. 独立 Composition Spike 允许使用 GPU interop API，但不得被生产项目引用。
8. Render 层继续禁止 Avalonia UI ownership。

## Scope

优先修改：

- `scripts/arch-a-guard-viewport.ps1`
- Guard 定向测试/fixture
- 架构治理文档

除非 Guard wiring 必需，不修改生产 runtime。

## Acceptance

- 构造违规 fixture 时 Guard 必须失败。
- 当前统一 HEAD 合法路径必须 PASS。
- Spike 项目允许独立存在。
- Production -> Spike project reference 必须被拒绝。
- legacy allowlist 仍为精确三文件，无目录级豁免。

## Verification

- ARCH-VIEWPORT-R1
- ARCH-A
- Guard negative fixtures
- 5+100
- git diff --check

## Delivery

给出新增 Guard 规则、正反例证据、Commit SHA。不得修改 Composition 技术结论。