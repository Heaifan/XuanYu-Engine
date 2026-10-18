# XYC-UI-V0.1-P0 Closeout Audit

Date: 2026-10-18 (Asia/Shanghai)
Scope: read-only closeout verification for P1-A entry gate. No P1 product code changes.

## Evidence checks

| Gate | Result | Evidence |
|---|---|---|
| Four original PNG hashes | PASS | `36e279fdeda64487869f1c56d225167f/runtime-evidence.json`; recalculated SHA-256 for all four files; zero mismatches. |
| Real UI session cleanup | PASS | Manifest: `sessionClose.closed=true`, `cliExited=true`, `cliExitCode=0`; foreground remained false for each capture. |
| Scoped XYC tests | PASS | Reran `XuanYu.World.Tests` with `FullyQualifiedName~Xyc`: 25 passed, 0 failed, 0 skipped. |
| Silent Host regression | PASS | Reran `tools/xyc-ui/Invoke-XYC-Silent-Smoke.ps1`: hidden HWND, foreground not taken, 776×650, Vulkan attach and Present, close, process exit and HWND cleanup all PASS. `readbackCaptured=false`; this smoke does not assert readback. |
| Whitespace check | PASS | `git diff --check`; only pre-existing line-ending warnings were emitted. |

## P0 ownership inventory

P0-owned product changes are the XYC UI command/runtime files (`XycUi*.cs`), the CLI and app-session integration (`XuanYu.Editor.Control.Cli/Program.cs`, `Silent/XycCliSessionRunner.cs`, `Silent/XycCliStartConfiguration.cs`, `XuanYu.Editor.UI/Bootstrap/App.axaml.cs`, `Control/XycAppSession.cs`, `Control/XycCommandProcessor.cs`), the XYC command tests, `tools/xyc-ui/`, P0 QA documents, and P0 diagnostic evidence.

The preflight recorded extensive pre-existing foreign Dirty changes, including the overlapping `XuanYu.Editor.UI/Control/Silent/SilentCaptureEvidence.cs`, XYUI TOP, map, terrain, renderer and unrelated tests. Those were not changed by P0. Current branch remains `recovery/editor-center-navigation-r1`; P0 has no formal commit.

## Version, changelog, and candidate discipline

`Directory.Build.props` currently reports `0.3.0.25-fix`. The dirty version change and `version-events.tsv` row are attributable to `WP2-P4-VISUAL-FIX-R1`; `changelog.md` is also dirty and was pre-existing/foreign at P0 preflight. P0 did not edit or stage these files and did not add its own Version Event or Changelog entry. The P0 report proposes `XYK CANDIDATE: NONE`; no dirty dependency is proposed as candidate evidence.

The P0 runtime evidence and tests pass, but formal repository closeout is **PARTIAL / NOT CLOSED**: P0 remains uncommitted, and the shared dirty version/changelog state prevents an attributable P0 release/commit record. Keep the current ownership boundary; resolve formal version and commit attribution through the Parent/coordinator process before treating P0 as formally closed. No P1 product code has been started.

## R1 closeout coordination update

- After `git fetch origin`, local `HEAD` and `origin/recovery/editor-center-navigation-r1` remain `1ca3b515a1d4f7b86cad2d9add0cbd45686d6c8c` (`0/0`). Remote `Directory.Build.props` is still `0.3.0.24-fix`; no WP2-P4-VISUAL-FIX-R1 commit/event is present in fetched refs. Local dirty `Directory.Build.props` is `0.3.0.25-fix` and its event row is still uncommitted.
- The WP2 Owner was contacted as directed and replied that the WP2 runtime recovery is PARTIAL and they will not commit or push its dirty version/changelog files in that task. Ownership conflict remains unresolved; no XYC version may be allocated by inference.
- Scoped App and CLI Debug builds were rerun after the XYC tests: PASS, each 0 warnings / 0 errors. XYC tests were rerun: 25/25. Silent smoke was rerun: all eight checks PASS. These were built/run in the shared dirty workspace and are not an isolated Candidate build.
- The successful runtime screenshot build identity records UI DLL SHA-256 `618A2AB8...B537C`; the current shared-workspace build output is `9A4EB2B3...FC3423` while the App executable hash is unchanged. Other dirty Editor.UI and project changes are excluded from XYC ownership. Therefore the current shared output is not evidence for a pure XYC Candidate; if the final Candidate identity differs, capture runtime evidence from that exact Candidate build.
- The four original PNGs, their original JSON sidecars, and the original manifest were copied byte-for-byte into the non-ignored `evidence/36e279fdeda64487869f1c56d225167f/` folder and all image hashes were reverified. Runtime evidence links now target these persistent copies.
- P0-owned C# files inspected for the 100-line limit are all at or below 100 lines. A repository-wide Candidate 5+100/ownership gate was not claimed because the shared tree is not a purified XYC Candidate.

R1 final disposition: **BLOCKED at version ownership coordination**. No files were staged, no version/changelog files were edited, no commit/push was attempted, and P1-A remains out of scope.

## R2 independent Candidate attempt

- Built an independent source snapshot from remote-authoritative HEAD `1ca3b515a1d4f7b86cad2d9add0cbd45686d6c8c` under the system Temp directory; no Git Worktree was created. Foreign WP2/XYUI/map/terrain/render Dirty files were excluded. Candidate App and CLI builds passed with 0 warnings / 0 errors; XYC-filtered tests passed 26/26 after supplying the already hash-verified test DEM fixture outside the Candidate file set.
- Isolated Candidate Silent Host smoke passed all eight checks: hidden HWND, no foreground activation, 776x650, Vulkan attach/Present, close, process exit and HWND cleanup. `readbackCaptured=false`; this run verifies the lifecycle/Present smoke only.
- The first clean-HEAD runtime exposed that XYC theme control depended on an uncommitted XYUI BF3 control type. P0-owned `XycUiRuntime.Theme.cs` now sets the production Avalonia theme scope when present and falls back to `Application.Current.RequestedThemeVariant` when the clean-HEAD TOP has no scope. Two focused theme tests passed. The TOP visual/component tree was not changed.
- The runtime runner's old theme pixel probe sampled the rounded capture corner, which is equal in Light and Dark. The probe now samples interior TOP coordinate (100,10). A fresh real Editor run passed all four combinations and closed normally: 1360/1920 DIP × Light/Dark, 96 DPI, foreground=false, HWND `0x1B0F72`, PID 2288, CLI exit 0. Pixel samples were Light `255,255,255` and Dark `42,56,66`. App SHA-256 `DE13C9CF9C40C876B9F62C49D864F0FD665E77FE73EA5CCC1EE8896E0538B74C`; UI DLL SHA-256 `E22502C8898EC1E60B4AC31BC15047B08D313C9F828BCB3D5F4E3ABE549DCF01`.
- New real PNGs, sidecars and manifest are persisted under `evidence/9dc755d7872c4906b41bcf8d7287c82a/`. PNG SHA-256: Light-1360 `63F3CB8EFBB1B39F0F8C349B4489C7F152B657AACFEBC109800358CE90FA6D75`; Dark-1360 `0169B9C44BD3636B50154A21B7B3D4CCD4F5F3F0EFD78A09F933EACE0E79BD6B`; Light-1920 `3AA62D280800626BC3C8B4F985E6009A06764F161A0B348FBE386943A39993BD`; Dark-1920 `8E3943EEDA53D4E2D0EA3B9AF0502DE0ADE09254E94C2FE28D00FF228286EF54`.
- Candidate handwritten C# files are each <=100 physical lines. Full `arch-a-guard.ps1` could not establish a Candidate pass in the `.git`-free source snapshot: it emitted no-repository diagnostics and failed on `XuanYu.Render.Abstractions.csproj` referencing Vulkan. This is not reported as a Candidate 5+100/SRP PASS; that repository-wide guard finding is outside the isolated XYC change set and needs exact Candidate-context triage.
- Version authority conflict is now explicit: the repository entrypoint names `docs/玄域引擎_AI开发宪法.md` as the sole authority, and its Article 34 prohibits separate version-event ledgers/gates. Older `docs/governance/agent-experience-rules.md` still requires a Version Preflight/Event and references missing `tools/governance/version-next.ps1` and related gates. The official remote version is `0.3.0.24-fix`; WP2's `0.3.0.25-fix` row/changelog remains uncommitted Dirty and its owner declined submission. No lawful XYC FEATURE number or event format can be derived without resolving this policy conflict. No shared version file was modified.

R2 disposition: **PARTIAL / BLOCKED before formal versioned commit**. Candidate functional builds/tests/runtime pass on the isolated `.24-fix` source snapshot, but Candidate-wide governance purity/5+100 and final version binding are not established. No files were staged; no commit or push was attempted; main-worktree foreign Dirty remains preserved; P1-A remains out of scope.

## R3 version ruling and final Candidate gates

- User ruled that Constitution Article 34 is the sole version authority: do not restore the old FEATURE Event ledger. The same article does not require a version increment for every feature commit or define a FEATURE numbering format. Candidate therefore preserves the official source version `0.3.0.24-fix`; the WP2 `0.3.0.25-fix` source, ledger row and Changelog block remain excluded and untouched in the main worktree. An unversioned XYC P0 Changelog note is included in the Candidate to record the delivered product change.
- The exact Candidate is isolated in a temporary Git Index against HEAD `1ca3b515a1d4f7b86cad2d9add0cbd45686d6c8c`; it contains only XYC source/integration, XYC tests, XYC scripts, P0 QA/evidence and the XYC Changelog note. It excludes `Directory.Build.props`, the WP2 ledger row and WP2 Changelog block. `git diff --cached --check` passes. Candidate Tree Identity is recorded in the final delivery report.
- Candidate-wide 5+100 line scan completed with no files over 100 lines. Manual SRP review found distinct runtime responsibilities for window control, theme control, TOP geometry, capture and protocol dispatch. `arch-a-guard.ps1` completed those checks but overall returned FAIL solely for the unchanged base-project reference `XuanYu.Render.Abstractions -> XuanYu.Render.Vulkan`; neither project is in the Candidate. Record the repository ARCH-A result as a baseline blocker, not as an XYC code failure.
- The R2 temporary-index attempt originally reported excess blank lines at EOF in the copied old manifest and runtime-evidence Markdown. The original raw artifact bundle is retained unchanged in `evidence/36e279fdeda64487869f1c56d225167f.zip`; only the persistent manifest/Markdown copies had terminal blank lines normalized. PNG and sidecar evidence values remain unchanged; all JSON parses and screenshot hashes still match.
- Candidate App/CLI builds (0W/0E), XYC tests (26/26), Silent Host smoke (8 checks), and exact-Candidate four-configuration real Editor capture passed. The new capture manifest ties the PNGs to the build identity and records session close/exit. No Editor process remains after the run.

R3 disposition: **READY FOR FORMAL COMMIT / PUSH** under Constitution Article 34 with product version unchanged at `0.3.0.24-fix`. No independent Version Event is created. P1-A remains out of scope.

## XYK audit candidate

`STRENGTHEN K-XYC-002`: Runtime evidence intended for a formal Candidate must live in a non-ignored, reviewable repository path; keeping screenshots only beneath ignored `artifacts/` makes them disappear from the commit even when hashes and manifests are valid. Copy original PNGs and sidecars byte-for-byte into the task QA evidence directory, retain the manifest, and verify copied image hashes against it. Evidence: this audit found `artifacts/` ignored, copied the four original captures and sidecars into `evidence/36e279fdeda64487869f1c56d225167f/`, and verified all four SHA-256 values. Since the Candidate is not committed yet, tracked/remote persistence remains to be confirmed after ownership conflict resolves. This is a candidate only; no XYK knowledge file was written.
