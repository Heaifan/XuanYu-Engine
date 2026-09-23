# Viewport Architecture D1-FIX1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Correct D1 governance so the existing NativeControlHost/Win32/Diagnostic Native Popup routes are explicitly transitional and cannot be expanded.

**Architecture:** Keep production runtime unchanged. Separate CURRENT transitional composition from TARGET Avalonia-owned composited Viewport, record A1 as a failed composition spike, and enforce an exact three-file temporary allowlist plus renderer/UI boundary checks.

**Tech Stack:** Markdown, PowerShell architecture guard, existing ARCH-A and 5+100 gates.

**Spec:** User-provided `XYE-VIEWPORT-W1-D1-FIX1` specification.

## Global Constraints

- Modify only docs, architecture Guard, and this plan document.
- Do not modify any production runtime `.cs` or `.axaml` file.
- Do not add dependencies or alter Camera, Picking, Gizmo, Region, Road, Marker, Snap, or Renderer runtime behavior.
- Preserve canonical checkout dirty/untracked material.

### Task 1: Correct CURRENT/TARGET governance

**Files:**
- Modify: `docs/architecture/viewport-native-architecture-governance.md`

- [ ] Mark NativeControlHost, Win32 child HWND, Diagnostic Native Popup, and native probes as transitional/migration-only.
- [ ] Freeze the `XYViewportControl` composited target.
- [ ] Record A1 commit `2687c917b04cce396c065ee3c85999d8d51dcbf8` as `FAIL` and route follow-up to A1.5.

### Task 2: Prevent legacy-route expansion

**Files:**
- Modify: `scripts/arch-a-guard-viewport.ps1`
- Modify: `scripts/arch-a-guard.ps1` only if guard wiring changes are required.

- [ ] Permit only the exact three-file migration allowlist.
- [ ] Reject a second Viewport `NativeControlHost` implementation.
- [ ] Reject new Viewport child HWND creators.
- [ ] Reject new Diagnostic Native Popup/owner/z-order expansion and renderer UI leakage.
- [ ] Continue allowing legitimate Vulkan/platform interop.

### Task 3: Verify and deliver

- [ ] Run relevant Guard, Build, ARCH-A, 5+100, and `git diff --check`.
- [ ] Confirm no production runtime files changed.
- [ ] Commit and push the documentation/Guard correction, then verify remote tip and clean worktree.
