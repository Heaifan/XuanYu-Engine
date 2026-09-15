# XYENGINE-DIAG-V1 Diagnostic Mode Design

## Goal

Create a stable human-to-agent UI coordinate system for real-device acceptance. A user can identify a UI region with a semantic ID such as `XYE.INSPECTOR.ROAD.STATE`, copy that ID, or copy a bounded diagnostic snapshot without exposing user data or changing the observed layout.

## Frozen identity contract

- IDs use `XYE.<DOMAIN>.<FEATURE>.<SECTION>[.<CONTROL>]` with uppercase semantic segments.
- `DebugId` and `AreaId` are editor-owned attached properties; neither is an EntityId.
- Visual-tree paths, generated names, numeric temporary names, and physical position are not identities.
- Moving a UI element does not rename its semantic identity.
- V1 registers only existing UI. It never creates placeholder sections solely to carry an ID.

## Architecture

`XuanYu.Editor.UI/Diagnostic` owns validation, attached properties, registry, overlay labels, snapshot formatting, and clipboard routing. XYUI remains unchanged and supplies only its existing controls.

The overlay host owns zero layout space and opens one anchored `Popup` per visible registered target. Each popup is only as large as its badge, so normal editor controls remain hit-testable. Native popups also avoid the Vulkan `NativeControlHost` airspace limitation. Popup content is not inserted into business `StackPanel` or `ScrollViewer` content.

The registry is rebuilt when diagnostic mode or inspector identity changes. It rejects invalid IDs and duplicate visible registrations before badges are shown. The feature inspector uses one shared production view; an Editor-side resolver maps its current Road or Region identity to semantic module and section IDs.

## Interaction

- Entry: `视图 → 诊断模式`; V1 adds no shortcut.
- Hover: the badge tooltip presents the DebugId and a short copy hint.
- Click: copy the DebugId.
- Shift+click: copy the full V1 snapshot.
- Turning the mode off closes and removes every diagnostic popup.

## Snapshot contract

The snapshot contains only DebugId, nearest AreaId, editor version, selection type, entity ID or `N/A`, effective visibility, effective enabled state, target bounds in window coordinates, window size, render scale, and theme. Missing context becomes `N/A`; capture must not throw because selection or entity data is unavailable.

V1 explicitly excludes user data, full visual-tree paths, arbitrary control properties, call stacks, GPU data, performance sampling, network diagnostics, automatic reporting, screenshots, and uploads.

## Initial registrations

- Areas: `XYE.AREA.TOP`, `LEFT`, `CENTER`, `RIGHT`, `BOTTOM`.
- Modules: `XYE.MENU`, `CONTEXT_TOOLBAR`, `PROJECT_TREE`, `VIEWPORT`, `INSPECTOR`, `LAYER_DOCK`, `LOG_PANEL`.
- Inspectors: `XYE.INSPECTOR.MAP`, `MARKER`, `ROAD`, `REGION`.
- Existing Road and Region sections: `BASIC`, `GEOMETRY`, `STATE`, `RELATIONS`.

`OTHER` is not registered because the current production feature inspector has no Other section.

## Verification

- Identity tests cover valid IDs, invalid IDs, independent AreaId, and duplicate detection.
- Snapshot tests cover field formatting and missing EntityId.
- Headless runtime tests compare target bounds and scroll extent with diagnostic mode off and on.
- Pointer tests prove only a badge handles badge input while an underlying normal control remains operable.
- Integration tests prove Road and Region expose their correct module and section IDs.
- Repository gates remain serial: one full solution build, related tests with `--no-build`, architecture guard, and `git diff --check`.
- Automated gates do not close the visual acceptance state; the user performs the final IPO real-device check.

