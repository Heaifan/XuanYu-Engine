# MAP-REGION-SNAP-R1-FIX1 Audit Manifest

## Identity

- Implementation Base: `51b81251` (`test(map): cover snapped region persistence`)
- FIX1 Commit: `5e758f63e29aca4ec30890b0999ac93778e967e5`
- Final Audit HEAD: `pending audit packaging commit`
- Branch: `feat/XYUI-ENGINE-AREA-A-CD`
- Scope: Native Alt state, Native RegionPreview propagation, RegionPanel row layout, and Undo/Redo exact-coordinate regression.

## Files in this package

| Path | Type | Purpose | Production | Test |
|---|---|---|---:|---:|
| `XuanYu.Editor.UI/Left/RegionPanel.axaml` | modified | Add the missing third Grid row for snap status. | yes | no |
| `XuanYu.Editor.UI/Viewport/Vulkan/NativePointerMessage.cs` | modified | Store Alt independently from Win32 mouse-button flags. | yes | no |
| `XuanYu.Editor.UI/Viewport/Vulkan/NativePointerRoutePolicy.cs` | modified | Preserve Alt suppression for Native RegionPreview. | yes | no |
| `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Pointer.cs` | modified | Pass Native Alt suppression into RegionPreview. | yes | no |
| `XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.Input.cs` | modified | Read `VK_MENU` with `GetKeyState` when creating messages. | yes | no |
| `XuanYu.World.Tests/Viewport/RegionDrawingInputModifierTests.cs` | modified | Cover XBUTTON1 independence and Native RegionPreview suppression. | no | yes |
| `XuanYu.World.Tests/UiRuntime/RegionDrawingSnapRuntimeTests.History.cs` | added | Cover exact snapped coordinate across Undo/Redo. | no | yes |

Modified project file count: 7

## Verification

- Snap-focused tests: PASS, 25 passed, 0 failed, 0 skipped.
- World full test suite: 1622 passed, 17 failed, 0 skipped, 1639 total. The 17 failures match the pre-existing baseline; no FIX1 test failed.
- ARCH-A: PASS.
- 5+100: PASS.
- `git diff --check`: PASS.
- Full Solution Build: blocked by an existing `XYUI.Avalonia.Gallery` process (PID 35084) locking its output DLLs; MSB3027/MSB3021. This was not terminated or modified by FIX1.

## Known issues

- Manual Native/Vulkan editor acceptance is still pending.
- Full Solution Build must be rerun after the existing Gallery process releases its locked files.
- An unrelated parallel modification remains outside this task: `xyui/avalonia/tests/XYUI.Avalonia.Tests/XYUI3ContextToolbarBoardTests.cs`; it is not included in this package or FIX1 commit.

## Follow-up

- Close/release the existing Gallery process, rerun the full Solution Build, and then perform the user-owned Avalonia/Native manual acceptance matrix.
- Do not expand R1 into Vertex-to-Edge snapping or shared topology.
