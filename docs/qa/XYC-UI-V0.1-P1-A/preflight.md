# P1-A preflight

Status: completed before implementation.

| Check | Result | Evidence |
|---|---|---|
| Official baseline | PRESENT | `6b1979899d7421ee63d75833ebc7ceb0648f1505` (P0), branch `recovery/editor-center-navigation-r1`; fetched origin branch matches, Ahead/Behind `0/0`. |
| Worktree and ownership | PRESENT with foreign Dirty | XYC-owned control source/test paths separated from WP2/map/terrain/render and XYUI TOP Dirty; no foreign path was added to candidate. |
| Production Editor path | PRESENT | Existing `--xyc-ui` launches production Editor; process ownership ties target HWND to spawned Editor PID. |
| Production TOP identifiers | PRESENT | Old TOP at official baseline has DebugId values for View All/Pan/Orbit, Select/Box Select and Save. No newer five-module TOP assumed. |
| Interaction path | MISSING before task | No XYC control discovery/invoke/status APIs; implementation uses Avalonia AutomationPeer `IInvokeProvider`. |
| Safe state effect | PRESENT | `VIEW.ALL` invokes real Avalonia button; isolated camera test pose makes resulting camera-position change independently observable and is restored. |
| Theme/capture | PRESENT from P0 | P0 theme, window sizing and TOP capture APIs reused. |
| Runtime platform | PRESENT | Windows runtime and Vulkan-capable environment available; four target profiles executed. |
| Current version source | PRESENT, numbering rule insufficient | Constitution Article 34 forbids independent event ledgers and uses product version source. Existing official HEAD says `0.3.0.24-fix`; dirty `0.3.0.25-fix` is WP2-owned and excluded. |

Candidate uses official HEAD source snapshot plus this task overlay. It does not include dirty XYUI TOP or business/render changes.
