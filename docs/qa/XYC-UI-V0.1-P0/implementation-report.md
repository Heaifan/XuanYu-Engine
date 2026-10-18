# XYC-UI-V0.1-P0 Implementation Report

Date: 2026-10-18 (Asia/Shanghai)
Result: **PASS for the scoped P0 capability gates; product visual acceptance remains with the user.**

## Root Cause

Before this work XYC launched the real Editor only through the existing silent/render-oriented path. Its command allowlist covered runtime and Vulkan capture behavior but had no production window, theme, TOP region, or real UI pixel inspection commands. Existing viewport readback could not prove the actual production TOP.

## Changed Files

- `XuanYu.Editor.Control.Cli/Program.cs`
- `XuanYu.Editor.Control.Cli/Silent/XycCliSessionRunner.cs`
- `XuanYu.Editor.Control.Cli/Silent/XycCliStartConfiguration.cs`
- `XuanYu.Editor.UI/Bootstrap/App.axaml.cs`
- `XuanYu.Editor.UI/Control/XycAppSession.cs`
- `XuanYu.Editor.UI/Control/XycCommandProcessor.cs`
- New XYC UI contracts, runtime, native window/capture, and command implementation files under `XuanYu.Editor.UI/Control/` (`XycUi*.cs`)
- `XuanYu.World.Tests/Control/XycUiCommandProcessorTests.cs`
- `tools/xyc-ui/Invoke-XYC-UI-P0.ps1`
- `tools/xyc-ui/Invoke-XYC-Silent-Smoke.ps1`
- This task's four files under `docs/qa/XYC-UI-V0.1-P0/`
- Runtime evidence under `artifacts/diagnostics/xyc-ui-v0.1-p0/`

Pre-existing unrelated dirty paths were preserved. In particular, the overlapping `XuanYu.Editor.UI/Control/Silent/SilentCaptureEvidence.cs` was not changed. No commit or push was made.

## Tests

- Scoped Editor App build: PASS, 0 warnings / 0 errors.
- Scoped XYC CLI build: PASS, 0 warnings / 0 errors.
- XYC-filtered World tests: PASS, 25/25, 0 skipped.
- Real UI host launch, owned HWND/PID inspection, 1360×820 and 1920×820 resize/readback, actual Light/Dark state inspection and pixel check, four TOP PNG captures: PASS.
- Normal `session.close`, clean CLI exit and process/window cleanup: PASS.
- Silent hidden-HWND Vulkan Present regression smoke: PASS.
- `git diff --check`: PASS.

## Evidence

See [runtime-evidence.md](runtime-evidence.md), [command-contract.md](command-contract.md), `preflight.md`, and the persisted copies in [`evidence/36e279fdeda64487869f1c56d225167f/`](evidence/36e279fdeda64487869f1c56d225167f/), which preserve the original manifest, PNGs, and sidecars byte-for-byte. The images are pixels from the actual DWM-composed production Editor window, cropped to production TOP geometry. The Editor remained non-foreground; capture temporarily changed z-order without activation and restored it.

## Known Risk

- Capture depends on DWM desktop composition and the target window being capturable on the desktop; this is not an offscreen or guaranteed invisible capture path. The window is visible, though foreground remained false in this run.
- UI capture verifies pixel provenance and theme background samples; it does not constitute XYUI TOP visual design acceptance.
- Silent smoke proves hidden HWND, Vulkan attach/Present, and cleanup; it did not request a new readback in this run.
- The worktree contains extensive unrelated dirty changes. The task did not edit those paths, and no formal version/changelog event or commit was created.

## Candidate Lessons

NONE. Existing K-XYC-002 already covers separating state changes from pixel evidence and requiring capture provenance; this run corroborates it. No new XYK candidate is proposed.

==================================================
KNOWLEDGE / EXPERIENCE AUDIT HANDOFF
==================================================

请 ChatGPT 审计本任务结果，并判断是否存在可沉淀的知识/经验。
如有，必须仅在本报告的 `XYK CANDIDATE` 区块中以纯文本输出，
禁止写入任何知识库、经验库、Codex memory、ad-hoc note 或其它文件；
正式入库由 ChatGPT / XYK Coordinator 后续决定。

XYK CANDIDATE:
NONE

若存在候选，替换上面的 `NONE`，并仅在本报告中按下列字段输出：
- ID/Category: <可选候选标识>
- Problem: <发生了什么问题>
- Root Cause: <根因>
- Rule / Knowledge: <可复用知识>
- Applies To: <适用范围>
- Evidence: <本任务证据>
- Expiry / Revalidation: <如适用>
- Suggested Action: <后续治理/优化建议>

AI AGENTS DO NOT HAVE DEFAULT XYK WRITE AUTHORITY.
候选不得被宣称为已沉淀、已写入或已更新；只有明确授予 XYK WRITE AUTHORITY
时，ChatGPT / XYK Coordinator 才能另行审计并维护正式 XYK。

CHATGPT KNOWLEDGE AUDIT REQUIRED
