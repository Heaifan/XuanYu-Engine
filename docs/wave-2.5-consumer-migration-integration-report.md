# WAVE-2.5 Consumer Migration Integration Report

## Conclusion

`PASS / CONSUMER-INTEGRATED`. D1 and D2 Consumer layers coexist under one
Router with explicit eligibility/priority arbitration and the existing single
`ViewportGestureLifecycle.Current` source. Production Input Route was not
switched; that remains WAVE-2.5-E.

## Git

- Branch: `feat/wave-2.5-consumer-migration-integration`
- Start HEAD: `00d8f147b1ffda5eabab2ae3a308f32c00b16258`
- D1: `01a84edfd38244e95f0d9d2966178ad189140b2b`
- D2 chain tip: `924ab09e0dac4a559958fbe70109e05530d8c34a`
- Final HEAD: this integration commit
- D1 and D2 source branches: unchanged

The branch proved baseline ancestry, merged D1 first, then the complete D2
chain through the exact stated tip. No forbidden host, renderer, production
route, composition, or WAVE-3 paths changed.

## Consumer integration

Camera, Picking, Gizmo, Map Geometry, Region, Road, and Marker are valid unique
Router owners. D1 handlers now participate in the Router eligibility phase;
only the selected candidate receives `Handle`. Cross-domain priorities are
explicit: Gizmo 400, Camera 300, Map consumers 200, Picking 100. Snap remains
a helper/constraint and has no independent owner.

## Lifecycle and arbitration

The Router projects `ViewportGestureState` from `ViewportGestureLifecycle.Current`.
Begin/Update/Commit/Cancel and capture release stay in the existing lifecycle.
Escape, CaptureLost, FocusLost, WindowDeactivated, ToolChanged, ModeChanged,
and ViewportDisposed use one exactly-once cancellation path. Repeated cancel is
a no-op, does not commit, clears temporary map state, and permits the next D1
or D2 gesture.

Integration coverage includes Camera/Picking, Gizmo/Picking, Gizmo/Camera,
Camera/Map, Picking/Marker, MapGeometry/Region, MapGeometry/Road,
MapGeometry/Marker, Snap non-preemption, both D1↔D2 cancellation handoffs,
consecutive gestures, CaptureLost recovery, and lifecycle terminal sources.

## Gates

- Solution Build: PASS, 0 warnings / 0 errors.
- D1 targeted: PASS, 9/9.
- D2 and consumer integration targeted: PASS, 34/34.
- Phase-1 Router/Cancellation/Platform Parity: PASS, 54/54.
- Viewport Migration Baseline: PASS, 2080 passed, 22 known baseline failures,
  0 new regressions.
- ARCH-A: PASS.
- ARCH-VIEWPORT-R1: PASS (included by ARCH-A).
- 5+100: PASS (included by ARCH-A).
- `git diff --check`: PASS.

## C-route gaps

Delta, X1/X2, Horizontal Wheel, Avalonia Capture Source, Timestamp, and Device
Type remain unchanged and registered. They do not block this Consumer
Integration result; they remain dependencies for Production Input Route Wiring
and therefore block WAVE-2.5-E/Final production-entry closure as previously
specified. No C-route expansion was made in this task.

## Boundaries

Production Input Route: not switched. VulkanNativeHost, Win32ViewportHost,
Production Viewport, Vulkan Renderer, Composition, and WAVE-3: untouched.
