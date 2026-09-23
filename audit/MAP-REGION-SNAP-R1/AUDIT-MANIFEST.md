# MAP-REGION-SNAP-R1 Audit Manifest

## Identity

- Task: MAP-REGION-SNAP-R1
- Pre-commit base: `01a3f99e`
- Implementation commit: `d46e8d20fbdbdf830333a05161fce8b6c449851d`
- Branch: `feat/XYUI-ENGINE-AREA-A-CD`
- Package contents: complete copies of every source, test, UI/config, and plan file changed in this round.
- Original changed files: 17
- Original additions/deletions/net: 332 / 24 / +308 lines. Counts use `git diff --numstat`; new files use their complete physical line count.
- Production files: 12
- Test files: 3
- Plan/governance files: 1
- UI/config files: 1

## File inventory

| Path | Type | Production | Purpose | Add | Delete | Net |
|---|---|---:|---|---:|---:|---:|
| `XuanYu.Editor.UI/Left/RegionPanel.axaml` | AXAML | Yes | Expose existing-panel snap status | 2 | 0 | +2 |
| `XuanYu.Editor.UI/Viewport/Vulkan/NativePointerMessage.cs` | C# | Yes | Decode Native MK_ALT | 1 | 0 | +1 |
| `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.AvaloniaPointer.cs` | C# | Yes | Forward Avalonia Alt and remember pointer | 5 | 2 | +3 |
| `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Picking.cs` | C# | Yes | Forward snap suppression through draw routes | 15 | 8 | +7 |
| `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Pointer.cs` | C# | Yes | Forward Native Alt during drawing preview/press | 4 | 3 | +1 |
| `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.RegionSnapInput.cs` | C# | Yes | Immediate Alt key refresh using last pointer | 33 | 0 | +33 |
| `XuanYu.Editor.UI/Vm/Map/UiVm.RegionDrawing.Commit.cs` | C# | Yes | Clear snap state at commit | 1 | 1 | 0 |
| `XuanYu.Editor.UI/Vm/Map/UiVm.RegionDrawing.DraftHistory.cs` | C# | Yes | Publish snap properties and clear on history | 8 | 0 | +8 |
| `XuanYu.Editor.UI/Vm/Map/UiVm.RegionDrawing.cs` | C# | Yes | Resolve snap point for drawing preview/click | 7 | 3 | +4 |
| `XuanYu.Editor.UI/Vm/Map/UiVm.RegionDrawing.Snap.cs` | C# | Yes | Drawing snap state and result projection | 42 | 0 | +42 |
| `XuanYu.Editor.UI/Vm/Transform/UiVm.Tool.cs` | C# | Yes | Clear snap state on tool exit | 1 | 0 | +1 |
| `XuanYu.Editor/MapEditing/RegionVertexSnapResolver.cs` | C# | Yes | Expose stable screen distance in result | 9 | 5 | +4 |
| `XuanYu.Editor/MapEditing/RegionVertexSnapResult.cs` | C# | Yes | Add ScreenDistance diagnostic field | 3 | 2 | +1 |
| `XuanYu.World.Tests/MapEditing/RegionVertexSnapResolverTests.cs` | C# | No | Assert screen-distance contract | 1 | 0 | +1 |
| `XuanYu.World.Tests/UiRuntime/RegionDrawingSnapRuntimeTests.cs` | C# | No | Drawing snap, Alt, exact coordinate, status regression | 72 | 0 | +72 |
| `XuanYu.World.Tests/Viewport/RegionDrawingInputModifierTests.cs` | C# | No | Avalonia/Native modifier forwarding contract | 24 | 0 | +24 |
| `docs/superpowers/plans/2026-09-23-map-region-snap-r1.md` | Markdown | No | Approved execution plan and scope ledger | 104 | 0 | +104 |

## Verification

- Snap focused tests: PASS, 21/21.
- Snap result self-review tests after `ScreenDistance`: PASS, 8/8.
- World test suite: 1618 passed, 17 failed, 0 skipped. The 17 failures are pre-existing XYUI/Inspector/Workspace/Diagnostic contract baseline failures; no failure names the Region Snap implementation.
- Solution Build: PASS, 0 warnings, 0 errors.
- ARCH-A / 5+100: PASS.
- `git diff --check`: PASS.
- Editor visual/real-device acceptance: pending user IPO acceptance.

## Known issues

- Full World baseline is not green at this checkout; unrelated failures were not modified.
- No real-window evidence yet proves Native HWND visual placement or user-observed Alt timing.

## Unresolved follow-up

- User IPO: normal snap, Alt cancel, release recovery, nearest candidate, zoom behavior, Undo/Redo, Save/Reload.
- Diagnostic follow-up only if the user reports the existing snap status cannot be resolved by the current probe.
- MAP-REGION-SNAP-R2 Vertex→Edge remains out of scope.
