# XYC-UI-V0.1-P0 Preflight

Date: 2026-10-18 (Asia/Shanghai)
Owner: Parent / current task
WriteScope: `XuanYu.Editor.Control.Cli/`; new XYC UI adapter files under `XuanYu.Editor.UI/Control/`; clean XYC startup/dispatch integration points only; `XuanYu.World.Tests/Control/`; `docs/qa/XYC-UI-V0.1-P0/`.

## Repository and execution baseline

| Check | Result | Evidence |
|---|---|---|
| Repository | PRESENT | `E:\MyDoc\project-VSCode\XuanYuEngine` |
| Branch / HEAD | PRESENT | `recovery/editor-center-navigation-r1` / `1ca3b515a1d4f7b86cad2d9add0cbd45686d6c8c` |
| Upstream | PRESENT | `origin/recovery/editor-center-navigation-r1`; Ahead 0 / Behind 0 |
| Dirty state | PRESENT | Worktree is dirty with unrelated XYUI, renderer, map, terrain and test changes. Preserve all existing dirty paths. XYC overlap `XuanYu.Editor.UI/Control/Silent/SilentCaptureEvidence.cs` is foreign to this task and excluded. |
| .NET resolver | PRESENT | `scripts/xye-bootstrap.ps1` resolved `E:\MyApp\sdk-dotnet\sdk\10.0.400\dotnet.exe`, SDK `10.0.400`. |
| Scoped baseline build | PRESENT | `scripts/xye-dotnet.ps1 build XuanYu.Editor.Control.Cli/XuanYu.Editor.Control.Cli.csproj --no-restore -m 1 -nr false -p:BuildInParallel=false -p:UseSharedCompilation=false`: 0 warnings / 0 errors. |
| Runtime platform | PRESENT | Windows 11 Enterprise, build `10.0.26300`; primary display 1920×1080, work area 1920×1032; reported adapter `GameViewer Virtual Display Adapter`. |
| Existing Editor process | PRESENT | No `XuanYu.Editor.App` process at preflight. Debug app artifact exists. |

## P0-A capability audit

| Capability | State | Finding |
|---|---|---|
| XYC CLI / stdio / session lifecycle | PRESENT | CLI launches the real `XuanYu.Editor.App.exe`; Editor uses JSON-lines `XYC-API/1`, allowlisted commands and `session.close`. |
| Production Editor entry / UiWin creation | PRESENT | `XuanYu.Editor.UI/Bootstrap/App.axaml.cs` creates production `UiWin` and `UiVm`; `XuanYu.Editor.App` is the real executable entry. |
| Exact HWND ownership | PRESENT | Child process PID is available in CLI; Avalonia window platform handle and Win32 `GetWindowThreadProcessId` can bind the HWND to that PID. No XYC UI inspect command exists yet. |
| Window size / DPI inspection and control | MISSING | No XYC window command exists. Avalonia window size and native client rect/DPI APIs are available. |
| Production theme inspect / set | PRESENT | Production TOP has a `ThemeVariantScope`; existing production theme toggle changes its `RequestedThemeVariant`. No XYC theme command exists. |
| TOP production bounds | PRESENT | Production TOP is hosted under `UiRoot` in `UiWin`; its visual bounds and transform are available after layout. No XYC region command exists. |
| Real production TOP PNG capture | MISSING | Existing `capture.arm`/`capture.release` capture Vulkan viewport readback only. No TOP UI capture command exists. Avalonia can render the live production TOP visual to PNG. |
| Build identity and capture metadata | PARTIAL | Existing `runtime.info` reports build, branch, dirty, process ID and frame. No UI capture manifest links window/TOP geometry and PNG hash. |
| Silent Host isolation | PRESENT | `--xyc-silent` has a separate hidden HWND startup path. Keep it unchanged and run existing silent smoke after implementation. |
| Runtime automation prerequisites | PARTIAL | Display dimensions match requested widths; adapter is virtual and must be treated as an environment limit if the actual Editor cannot render/capture. |

## Active knowledge loaded

Planning and Execution Knowledge Preflight matched `K-XYC-001`, `K-XYC-002`, `K-REN-005`, `K-VAL-001`, `K-VAL-002`, `K-SPA-004` and `K-SPA-002`. Applied constraints: use production state ownership and read back changes; distinguish command state, presented pixels and capture provenance; report a window/visual capture honestly; do not equate diagnostic screenshots with XYUI acceptance; preserve build/source identity. No P0 Knowledge conflict found.

## Ownership and isolation

This task is the sole writer for the declared XYC UI control files and this evidence directory. Existing dirty files are protected, including all TOP/XYUI, renderer, map, terrain, version and unrelated test paths. `XuanYu.Editor.UI/Control/Silent/SilentCaptureEvidence.cs` and Silent Host code are excluded. No second worktree is created. Any required overlap with an existing dirty path stops only that operation and is reported.
