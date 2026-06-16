# Codex Takeover Audit

Date: 2026-06-15

## Repository fact source

- Working directory: `E:\MyDoc\project-VSCode\fluidwarfare`
- Current branch: `main`
- HEAD: `1ef139c0052380ea2cf7efaed4179896a2d26715`
- Latest commit: `1ef139c 8.7.4.0B: Move projection extraction -- anchor, relative delta, safety limits, fallback, math removed from EditorShell`
- Worktree status: dirty before this audit started.

Dirty files already present before Phase 0:

- Modified: `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportHostControl.cs`
- Modified: `FluidWarfare.Editor.Windows/Panels/Viewport/VulkanViewportHostPanel.axaml.cs`
- Modified: `FluidWarfare.Editor.Windows/Shell/EditorShell.axaml.cs`
- Modified: `FluidWarfare.Editor/Transform/Move/EntityMoveSession.cs`
- Modified: `FluidWarfare.Editor/Transform/Move/Projection/EntityMoveSafetyLimits.cs`
- Modified: `FluidWarfare.Editor/Transform/Move/Projection/GroundMoveAnchor.cs`
- Modified: `FluidWarfare.Editor/Transform/Move/Projection/GroundMoveProjection.cs`
- Modified: `FluidWarfare.Editor/Transform/Move/Projection/VerticalMoveProjection.cs`
- Modified: `FluidWarfare.Tests/Editor/Transform/Move/EntityMoveSessionTests.cs`
- Untracked: `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/ViewportSceneToolPressResult.cs`
- Untracked: `FluidWarfare.Editor/Transform/Move/Projection/VerticalMoveAnchor.cs`
- Untracked: `FluidWarfare.Editor/Transform/Move/Projection/WorldAxisScreenProjection.cs`
- Untracked: `FluidWarfare.Tests/Editor/Transform/Move/GroundMoveProjectionTests.cs`

## Environment

- .NET SDK: `10.0.103`
- MSBuild: `18.0.11+c2435c3e0`
- Host runtime: `10.0.3`
- OS: Windows 10.0.26200 x64
- `global.json`: not present

## Build and test

### `dotnet build FluidWarfare.sln`

First sandboxed run failed at restore:

- `NU1301`: sandbox blocked `https://api.nuget.org/v3/index.json`.

Escalated run restored packages, compiled project dependencies, then failed because a running editor process locked output DLLs:

- Locking process: `FluidWarfare.Editor.Windows (8168)`
- Final result: failed
- Reported summary: 82 warnings, 14 errors
- Blocking errors: `MSB3027` and `MSB3021` copying project DLLs into `FluidWarfare.Editor.Windows/bin/Debug/net10.0`

Notable compile warnings before copy failure:

- `FluidWarfare.Render/ViewportNavigation/ViewportNavigationLayout.cs`: nullable initialization warnings.
- `FluidWarfare.Editor/Transform/Move/EntityMoveSession.cs`: obsolete warning for `VerticalMoveProjection.ApplyToPosition`.
- `FluidWarfare.Tests`: nullable dereference warnings.
- `FluidWarfare.Editor.Windows/Shell/EditorShell.axaml.cs`: possible null `SceneRay` arguments.

### `dotnet test FluidWarfare.sln --no-build`

The existing test binary ran:

- Total: 629
- Passed: 626
- Failed: 3
- Skipped: 0

Failing tests:

- `EntityMoveSessionTests.LegacyUpdateVertical_EventCountDoesNotAffectResult`
- `EntityMoveSessionTests.LegacyUpdateVertical_NoPointerMove_KeepsPosition`
- `EntityMoveSessionTests.LegacyUpdateVertical_XYJumpOnConstraintSwitch`

Observed failure pattern:

- Expected X/Y remain at the current preview position after switching to Z.
- Actual X/Y revert to the original session position.
- This confirms the current legacy Z path does not preserve the current preview position when Z is entered through `SetAxisConstraint(EntityMoveAxis.Z)` plus `UpdateVertical`.

## EditorShell size

- `FluidWarfare.Editor.Windows/Shell/EditorShell.axaml.cs`: 3012 lines

## Files over 100 lines

These are source and test `.cs` files outside `bin` and `obj`:

| Lines | File |
| ---: | --- |
| 3012 | `FluidWarfare.Editor.Windows/Shell/EditorShell.axaml.cs` |
| 1243 | `FluidWarfare.Render.Vulkan/Scene3D/Session/VulkanScene3dSession.cs` |
| 613 | `FluidWarfare.Tests/Project/Loading/GameProjectLoaderTests.cs` |
| 601 | `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportHostControl.cs` |
| 594 | `FluidWarfare.Tests/Editor/Transform/Move/EntityMoveSessionTests.cs` |
| 585 | `FluidWarfare.Editor.Windows/Preferences/EditorPreferencesWindow.axaml.cs` |
| 474 | `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dRenderer.cs` |
| 467 | `FluidWarfare.Render.Vulkan/Context/VulkanRenderContext.cs` |
| 458 | `FluidWarfare.Tests/Editor/Transform/Move/GroundMoveProjectionTests.cs` |
| 416 | `FluidWarfare.Render.Vulkan/Scene3D/Session/VulkanScene3dSwapchainResources.cs` |
| 412 | `FluidWarfare.Render.Vulkan/Clear/VulkanClearProbe.cs` |
| 389 | `FluidWarfare.Project/Loading/GameProjectLoader.cs` |
| 379 | `FluidWarfare.Editor.Windows/Panels/Inspector/InspectorPanel.axaml.cs` |
| 350 | `FluidWarfare.Tests/Render/Vulkan/Camera/PerspectiveOrthographicPickingTests.cs` |
| 315 | `FluidWarfare.Tests/Editor/Input/Runtime/EditorInputBindingSnapshotTests.cs` |
| 302 | `FluidWarfare.Render.Vulkan/Scene3D/Overlay/VulkanNavigationOverlayGeometry.cs` |
| 298 | `FluidWarfare.Render.Vulkan/Swapchain/VulkanSwapchainProbe.cs` |
| 290 | `FluidWarfare.Tests/Project/Content/GameContentFileScannerTests.cs` |
| 289 | `FluidWarfare.Tests/Render/ViewportNavigation/ViewportNavigationLayoutTests.cs` |
| 288 | `FluidWarfare.Render/ViewportNavigation/ViewportNavigationLayout.cs` |
| 287 | `FluidWarfare.Render.Vulkan/Device/VulkanDeviceProbe.cs` |
| 259 | `FluidWarfare.Editor.Windows/Panels/Viewport/Input/WindowsViewportInputTranslator.cs` |
| 256 | `FluidWarfare.Editor/Transform/Move/EntityMoveSession.cs` |
| 233 | `FluidWarfare.Render.Vulkan/Scene3D/Overlay/VulkanOverlayResources.cs` |
| 229 | `FluidWarfare.Editor.Windows/Panels/WorldHierarchy/WorldHierarchyTreePanel.axaml.cs` |
| 216 | `FluidWarfare.Editor.Windows/Panels/LeftDock/ProjectWorldDockPanel.axaml.cs` |
| 206 | `FluidWarfare.Tests/Engine/World/WorldStateTests.cs` |
| 202 | `FluidWarfare.Render.Vulkan/Surface/VulkanSurfaceProbe.cs` |
| 200 | `FluidWarfare.Tests/Render/Camera/Navigation/SceneNavigationCameraMotionTests.cs` |
| 195 | `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dCommandRecorder.cs` |
| 188 | `FluidWarfare.Render/Camera/SceneOrbitCameraMotion.cs` |
| 181 | `FluidWarfare.Editor.Windows/Panels/Viewport/ViewportPlaceholderPanel.axaml.cs` |
| 176 | `FluidWarfare.Tests/Editor/WorldHierarchy/WorldHierarchyTreeBuilderTests.cs` |
| 176 | `FluidWarfare.Render.Vulkan/Camera/VulkanCameraMatrices.cs` |
| 168 | `FluidWarfare.Editor.Windows/Panels/ProjectContentTree/ProjectContentTreePanel.axaml.cs` |
| 167 | `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dVertexBuffers.cs` |
| 165 | `FluidWarfare.Editor/Input/Runtime/EditorInputBindingSnapshot.cs` |
| 160 | `FluidWarfare.Render/Camera/Navigation/SceneNavigationCameraMotion.cs` |
| 156 | `FluidWarfare.Render.Vulkan/Camera/VulkanSceneRayBuilder.cs` |
| 155 | `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dVertex.cs` |
| 152 | `FluidWarfare.Editor.Windows/Panels/Viewport/VulkanViewportHostPanel.axaml.cs` |
| 151 | `FluidWarfare.Tests/Core/Logging/EngineLogEntryTests.cs` |
| 150 | `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dPipelines.cs` |
| 147 | `FluidWarfare.Tests/Architecture/ProjectDependencyDirectionTests.cs` |
| 147 | `FluidWarfare.Tests/Render/Camera/SceneCameraMotionTests.cs` |
| 147 | `FluidWarfare.Editor/Input/Actions/EditorInputActionCatalog.cs` |
| 143 | `FluidWarfare.Tests/Render/Vulkan/Scene3D/VulkanScene3dRunGateTests.cs` |
| 143 | `FluidWarfare.Editor.Windows/Panels/DebugDock/DebugDockPanel.axaml.cs` |
| 137 | `FluidWarfare.Tests/Render/Vulkan/Scene3D/VulkanScene3dVertexTests.cs` |
| 134 | `FluidWarfare.Tests/Render/Selection/Ground/SceneRayGroundIntersectionTests.cs` |
| 134 | `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dRenderResources.cs` |
| 133 | `FluidWarfare.Tests/Editor/Input/Bindings/EditorInputConflictDetectorTests.cs` |
| 132 | `FluidWarfare.Tests/Render/Scene/Position/RenderSceneObjectPositionWriterTests.cs` |
| 130 | `FluidWarfare.Tests/Bridge/ProjectEngine/World/ProjectContentWorldSeederTests.cs` |
| 128 | `FluidWarfare.Tests/Render/Vulkan/Camera/ProjectionUnprojectionRoundTripTests.cs` |
| 128 | `FluidWarfare.Render.Vulkan/Scene3D/Overlay/VulkanOverlayPipeline.cs` |
| 127 | `FluidWarfare.Render.Vulkan/Validation/VulkanDebugMessengerScope.cs` |
| 122 | `FluidWarfare.Render.Vulkan/Scene3D/Depth/VulkanScene3dDepthAttachments.cs` |
| 121 | `FluidWarfare.Editor/WorldHierarchy/WorldHierarchyTreeBuilder.cs` |
| 121 | `FluidWarfare.Project/Content/GameContentFileScanner.cs` |
| 121 | `FluidWarfare.Render.Vulkan/Instance/VulkanInstanceProbe.cs` |
| 120 | `FluidWarfare.Editor/Transform/Move/Projection/WorldAxisScreenProjection.cs` |
| 119 | `FluidWarfare.Editor/Transform/Move/Projection/GroundMoveProjection.cs` |
| 119 | `FluidWarfare.Tests/Render/Camera/SceneOrbitCameraMotionTests.cs` |
| 115 | `FluidWarfare.Tests/Render/Vulkan/Device/VulkanDeviceInfoTests.cs` |
| 115 | `FluidWarfare.Render.Vulkan/Validation/VulkanValidationAvailabilityProbe.cs` |
| 114 | `FluidWarfare.Editor.Windows/Panels/ProjectContentTree/ProjectContentNodeView.cs` |
| 112 | `FluidWarfare.Editor.Windows/Panels/WorldHierarchy/WorldHierarchyTreeIndex.cs` |
| 112 | `FluidWarfare.Engine/World/WorldState.cs` |
| 111 | `FluidWarfare.Tests/Core/Results/EngineResultTests.cs` |
| 108 | `FluidWarfare.Tests/Render/World/WorldToRenderSceneBuilderTests.cs` |
| 106 | `FluidWarfare.Tests/Editor/Input/Runtime/EditorInputServiceTests.cs` |
| 105 | `FluidWarfare.Tests/Render/Selection/Pointer/ScenePointerPickerTests.cs` |
| 101 | `FluidWarfare.Tests/Render/Vulkan/Scene3D/VulkanScene3dInfoTests.cs` |

## Transform and move files

Production files directly involved in current move behavior:

- `FluidWarfare.Editor.Windows/Shell/EditorShell.axaml.cs`
- `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportHostControl.cs`
- `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/ViewportSceneToolPressResult.cs`
- `FluidWarfare.Editor.Windows/Panels/Viewport/Input/WindowsViewportInputTranslator.cs`
- `FluidWarfare.Editor/Transform/Move/EntityMoveSession.cs`
- `FluidWarfare.Editor/Transform/Move/EntityMoveAxis.cs`
- `FluidWarfare.Editor/Transform/Move/EntityMoveResult.cs`
- `FluidWarfare.Editor/Transform/Move/Projection/GroundMoveAnchor.cs`
- `FluidWarfare.Editor/Transform/Move/Projection/GroundMoveProjection.cs`
- `FluidWarfare.Editor/Transform/Move/Projection/VerticalMoveAnchor.cs`
- `FluidWarfare.Editor/Transform/Move/Projection/VerticalMoveProjection.cs`
- `FluidWarfare.Editor/Transform/Move/Projection/WorldAxisScreenProjection.cs`
- `FluidWarfare.Editor/Transform/Move/Projection/EntityMoveSafetyLimits.cs`
- `FluidWarfare.Render.Vulkan/Camera/PresentedCameraSnapshot.cs`
- `FluidWarfare.Render.Vulkan/Camera/VulkanSceneRayBuilder.cs`
- `FluidWarfare.Render/Selection/Pointer/ScenePointerPicker.cs`
- `FluidWarfare.Render/Selection/Ground/SceneRayGroundIntersection.cs`
- `FluidWarfare.Render/Scene/Position/RenderSceneObjectPositionWriter.cs`

NativeHost input files:

- `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportHostControl.cs`
- `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportPickInput.cs`
- `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportHostState.cs`
- `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportHostInfo.cs`
- `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/ViewportSceneToolPressResult.cs`
- `FluidWarfare.Editor.Windows/Panels/Viewport/Input/WindowsViewportInputTranslator.cs`

## Search results

Searched with `Select-String` because `rg` did not return reliable matches against the current mixed-encoding files in this workspace.

Important matches:

- `EntityMoveSession.cs`: holds `_verticalAnchor`, `_hasVerticalAnchor`, `BeginVertical`, `ReanchorVertical`, `TryUpdateVertical`, and legacy `UpdateVertical`.
- `VerticalMoveProjection.cs`: contains obsolete `ApplyToPosition` and anchor-based `TryMap`.
- `WorldAxisScreenProjection.cs`: projects `pivot` and `pivot + Vector3d.UnitZ` to create a Z anchor.
- `GroundMoveProjection.cs`: explicitly returns zero movement for `EntityMoveAxis.Z` in ground paths.
- `EditorShell.axaml.cs`: routes active Z move through `TryUpdateVertical` only if `_moveSession.Axis == EntityMoveAxis.Z` and `_moveSession.HasVerticalAnchor`.
- `EditorShell.axaml.cs`: keyboard `Z` is explicitly disabled while moving.
- `WindowsVulkanViewportHostControl.cs`: scene tool consumption blocks legacy picking after move-tool drag begins.

No direct `currentZ` production variable was found.

## Current confirmed risks

- Build is blocked by a live editor process locking output assemblies.
- Test suite is red: 3 existing Z-axis tests fail.
- `EditorShell.axaml.cs` is 3012 lines and still owns input routing, picking, move session orchestration, transform application, logging, dirty state, and rendering synchronization.
- Z movement has a separate path from X/Y: `VerticalMoveAnchor`, `VerticalMoveProjection`, `WorldAxisScreenProjection`, and `BeginVertical/ReanchorVertical`.
- The Z-key path is disabled in `HandleRawKeyDown`; the existing Z anchor path appears present but not connected from keyboard axis switching.
- Preview writes through `ApplyEntityTransform`, which marks dirty and updates render state on every preview frame. This does not satisfy the desired Begin/Preview/Confirm/Cancel transaction boundary.
- Existing tests mostly exercise `EntityMoveSession`, `GroundMoveProjection`, and `VerticalMoveProjection` directly. They do not yet prove the full production chain from camera snapshot to ray builder to input route to solver to preview.

## Current test gaps

- No confirmed production-chain test covering `PresentedCameraSnapshot -> VulkanSceneRayBuilder -> NativeHost/InputRoute -> solver/session -> preview`.
- No test proving camera snapshot is frozen for the duration of a drag.
- No test proving X/Y/Z all share one axis translation solver.
- No test proving 1 pointer move and 100 pointer moves produce the same result through the real input route.
- No test proving active move-tool consumption cannot trigger entity picking or ground picking.
- Existing Z tests are session-level and legacy-specific; they do not model real viewport input or camera projection.

## Recommended next files for Phase 1 and Phase 2

- `FluidWarfare.Editor.Windows/Shell/EditorShell.axaml.cs`
- `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportHostControl.cs`
- `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/ViewportSceneToolPressResult.cs`
- `FluidWarfare.Editor/Transform/Move/EntityMoveSession.cs`
- `FluidWarfare.Editor/Transform/Move/Projection/VerticalMoveProjection.cs`
- `FluidWarfare.Editor/Transform/Move/Projection/WorldAxisScreenProjection.cs`
- `FluidWarfare.Editor/Transform/Move/Projection/GroundMoveProjection.cs`
- `FluidWarfare.Render.Vulkan/Camera/VulkanSceneRayBuilder.cs`
- `FluidWarfare.Render.Vulkan/Camera/PresentedCameraSnapshot.cs`
- `FluidWarfare.Tests/Editor/Transform/Move/EntityMoveSessionTests.cs`
- `FluidWarfare.Tests/Editor/Transform/Move/GroundMoveProjectionTests.cs`

Phase 1 should reproduce the editor symptom with debug trace only. Phase 2 should add failing tests before any production rewrite.
