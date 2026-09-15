# FIX2 T3 report — Probe interaction and clipboard

## Status

DONE_WITH_CONCERNS. T3 interaction is implemented locally on top of the existing T1 resolver and T2 internal overlay. No push was performed.

## Scope and files

- Added `XuanYu.Editor.UI/Diagnostic/DiagnosticElementFormatter.cs`.
- Added `XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.Interaction.cs`.
- Added `XuanYu.World.Tests/UiRuntime/DiagnosticProbeInteractionTests.cs`.
- Minimal command/state and host event wiring changes in the existing diagnostic files and `UiVm.Scene.cs`.
- No `xyui/` files, desktop-level Window/Popup, business UI, or FIX1 registration behavior changed.
- All changed handwritten `.cs/.axaml` files are at most 100 lines.

## TDD evidence

- RED: focused build failed because the new interaction API was absent (`IsDiagnosticProbeMode`, `ProbeHover`, `CurrentProbeResult`, `ProbeClick`, `ExitProbe`); this was the expected pre-implementation failure.
- GREEN build: `dotnet build XuanYu.World.Tests\\XuanYu.World.Tests.csproj --no-restore -m:1 -nr:false -p:BuildInParallel=false -p:UseSharedCompilation=false -v:minimal` — 0 warnings, 0 errors.
- GREEN tests: `dotnet test XuanYu.World.Tests\\XuanYu.World.Tests.csproj --no-build --no-restore --filter FullyQualifiedName~DiagnosticProbeInteractionTests` — 4 passed, 0 failed.

## Coverage

- Probe command toggles only when diagnostic mode is enabled.
- Semantic hover and Alt/deep-visual hover select the expected result modes.
- Click formatting includes the frozen element diagnostic fields and uses the injected clipboard.
- Exit clears probe visuals and leaves probe mode disabled.
- Probe rendering preserves the target bounds.

## Concerns

The focused tests exercise the host interaction methods directly; they do not yet dispatch a real headless pointer-move/press and Escape event through the full `Window` input pipeline. The production host registers tunnel handlers for those events, but end-to-end event routing remains a follow-up verification concern for T3 acceptance.

## Commit

Implementation commit before this report correction: `ae9d01a79e2ad84ec18cb5fe8b51725318aba709`; report is committed locally with the final change set. Not pushed.
