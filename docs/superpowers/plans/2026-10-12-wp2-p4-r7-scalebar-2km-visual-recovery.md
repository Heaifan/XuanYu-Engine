# WP2 P4 R7 Scale-Bar 2 km Visual Recovery Plan

> **For agentic workers:** execute inline in the canonical checkout. Parent is the sole writer, test/build/runtime scheduler, and Commit/Push owner.

**Goal:** Reproduce the user's independent 2 km scale-bar Wheel and Orbit blur triggers, prove the relevant mechanism, and deliver the smallest bounded fix with RED→GREEN and 1 m navigation nonregression when evidence permits.

**Architecture:** Preserve and audit the three R6 diagnostic edits. Extend the existing opt-in Windows Vulkan capture so one presented frame ties native HWND geometry, DPI, VM viewport, Swapchain extent, scale-bar metrics, camera and terrain LOD/render data together. Capture the two native trajectories separately; only a valid first-blur frame permits frozen-pose one-variable A/B tests. CUT-001 remains independent.

**Tech Stack:** C#/.NET 10, Avalonia, Windows HWND/SendInput, Vulkan replay, XYT, repository governance scripts.

**Spec:** GitHub Issue #17, latest R7 authorization comment `issuecomment-6116464327`.

## Global Constraints

- Keep `WP2=ACTIVE_FIX`, `WP3–WP8=WAITING`, `CUT-001=USER FAIL`, and `.17-fix` until a product root cause is proven and an individual fix is verified.
- Use the actual `ScaleIndicatorText == "2 km"` trigger; never substitute camera-to-pivot distance.
- Preserve the three pre-existing uncommitted diagnostic files and their original content; no reset, clean, stash, checkout, or new worktree.
- Diagnostic switches remain opt-in; no permanent global LOD0, whole-tile 3601² mesh, unbounded FarPlane, or disabled culling workaround.
- Preserve center-wheel anchor, Frozen Pivot, Reverse-Z, and the prior 1 m navigation recovery.
- Parent alone stages, commits, and pushes. Use AI Constitution, XYT, and XYK only; no retired Version Event Gate.

## Review Focus

- Scale-bar 1–2–5 quantization/hysteresis: log entry/exit frame and real ground metric.
- Native HWND focus and event routing: refuse SendInput unless target editor is foreground.
- Frame identity: HWND, Swapchain, VM viewport, camera and image must share frame/window generation.
- LOD history: retain previous/selected/draw LOD and projected density at both first-blur poses.
- 1 m zoom recovery and CUT-001 status remain separately classified.

## Execution Tasks

### Task 1: Preserve and audit R6 candidate

- Record SHA-256 and exact diffs for the three dirty files; confirm index remains clean and identify all consumers and runtime gates.
- Keep `.17-fix`; inspect existing test registry, XYT routes, architecture guards, and diagnostic opt-in defaults.

### Task 2: Add R7 atomic observability

- Extend opt-in same-frame capture with `ScaleIndicatorText`, width, meters-per-DIP, measured ground endpoint distance, camera-pivot distance, logical/physical VM viewport, DPI, HWND parent/child client rect, Swapchain extent, frame/generation/revision, projection aspect, and last resize source/time.
- Capture warnings from `EditorLogBuffer.All` at generation with time, source, category, message, detail, and correlation ID.
- Add narrowly scoped tests for telemetry semantics and preserve ≤100 lines plus SRP.

### Task 3: Native scale-triggered reproduction

- Run `ViewAll → native Wheel until displayed label is exactly 2 km`; record `PRE_2KM`, scale entry/exit and incremental `PRE_ZOOM_BLUR` / `FIRST_ZOOM_BLUR` / reverse recovery.
- Independently return to displayed `2 km` and run incremental native Orbit across directions, capturing `PRE_ORBIT` / `FIRST_ORBIT_BLUR` / recovery.
- Require target foreground PID/HWND, native WndProc event IDs, successful Vulkan Present linkage, and screenshot hashes; otherwise record INPUT BLOCKED / NOT REPRODUCED.

### Task 4: Causal A/B and RED→GREEN

- Only for a valid first-blur pose, compare auto LOD vs fixed LOD0, 513 grid vs bounded high-resolution local patch, baseline vs expanded Far, and normal vs CPU cull-off, one variable at a time.
- Quantify source/render height roughness, projected triangle density, normals/shading and pixel masks; check same-frame viewport authority before linking viewport to GPU LOD.
- Implement one bounded product fix only after mechanism is proven. Write and observe a failing regression test before implementation, then GREEN plus 1 m navigation nonregression.

### Task 5: Governance and delivery

- Run applicable XYT, Architecture Guards, 5+100+SRP, Release Build and affected/full suites serially; retain exact exit codes and residual failures.
- Advance product version and Changelog only for a proven product FIX; verify DLL/runtime identity and candidate hashes.
- Keep CUT-001 separate, WP2 ACTIVE_FIX and WP3–WP8 WAITING until user P4. Parent stages/commits/pushes only after all required gates; provide full report and XYK CANDIDATE text without writing XYK.
