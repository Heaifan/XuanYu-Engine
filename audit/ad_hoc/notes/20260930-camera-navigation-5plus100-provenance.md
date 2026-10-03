# C-5PLUS100 CameraNavigation provenance audit

- In the specified `E:\MyDoc\project-VSCode\XuanYuEngine` checkout, `XuanYu.Editor/Camera/CameraNavigation.Try.cs` was 78 physical lines at audit time; `HEAD` was 99 lines and the worktree delta was `0 additions / 21 deletions`.
- The file history contains no committed version above 100 lines: `e435286e` reached exactly 100, while later relevant committed versions were 99. Therefore a claimed current count of 139 must be revalidated against the exact checkout before classifying it.
- `scripts/arch-a-guard.ps1` scans all current tracked and untracked handwritten files and only applies a physical `>100` test; it has no baseline/delta provenance classification. A minimal governance improvement is to report historical-overlimit unchanged files separately from new or expanded violations.
