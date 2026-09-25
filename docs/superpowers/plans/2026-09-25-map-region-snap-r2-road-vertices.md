# MAP-REGION-SNAP-R2 and linear-geometry vertex migration

## Goal

Implement Vertex → Existing Edge snap for region drawing while preserving the frozen R1 Vertex → Vertex behavior. Add the smallest compatible vertex add/delete workflow for road-like linear geometry without introducing shared topology or schema changes.

## Scope and non-goals

- In scope: committed-region edge candidates, exact segment projection, vertex-over-edge priority, 8 px enter / 12 px release hysteresis, immediate Alt suppression/recovery through the existing input chain, distinct vertex/edge UI state, R1 regression coverage, and road vertex insertion/deletion.
- Out of scope: edge splitting of old regions, shared topology, boundary tracing, schema redesign, router redesign, diagnostic/XYUI changes, and unrelated navigation work already present in the working tree.
- Road migration assumption: add inserts a vertex on the selected road segment at the current valid pointer position (or the nearest legal segment projection); delete removes the selected road vertex only when the remaining line stays valid. Existing undo/redo and persistence paths must be reused.

## Architecture constraints

Pointer and modifier state continue through the existing Unified Pointer Model → Viewport Router → Map Editing Consumer → Region/Road consumer chain. Screen space decides candidate eligibility; world geometry produces the exact projected coordinate. Existing committed geometry is read-only during R2.

## Plan

1. Freeze R1: inspect and separate R1 files from unrelated working-tree changes; run the final targeted gates; update the R1 acceptance document; commit only R1; push and verify the remote tip.
2. Map existing snap and geometry-editing seams: inspect resolver/state/result, committed geometry enumeration, render status, command history, road editing, and persistence before changing production code.
3. TDD edge candidate model: add failing tests for vertex priority, edge acquisition, exact projection, deterministic nearest edge, degenerate/draft/self exclusion, hysteresis, Alt behavior, and Native/Avalonia paths.
4. Implement the smallest R2 resolver/state integration and UI status projection; keep R1 APIs/behavior compatible.
5. Run focused R2/R1 tests, then solution build and architecture/5+100/diff gates; create `MAP-REGION-SNAP-R2-AUDIT.zip` containing complete changed production/test files and `AUDIT-MANIFEST.md`.
6. TDD road vertex migration: add failing tests for insert/delete, minimum valid line, undo/redo, persistence, and input ownership; implement through existing map geometry edit/history seams.
7. Re-run all affected gates, document residual manual acceptance cases, and stop at `WAITING FOR USER ACCEPTANCE` for the official `run.bat` check.

## Verification gates

- R1 targeted regression remains green.
- R2 region snap and input-chain tests, including Native and Avalonia paths.
- Road vertex add/delete tests and persistence/history tests.
- Official SDK solution build: 0 warnings / 0 errors.
- ARCH-A, 5+100, `git diff --check`, and applicable World tests.
- Manual acceptance only through `D:/MyDoc/project-vsCode/XuanyuEngine/run.bat`.

## Review focus

Candidate ownership, exact world projection, hysteresis identity, Alt refresh without pointer movement, committed-only edge enumeration, and ensuring road edits do not alter region topology or unrelated viewport behavior.
