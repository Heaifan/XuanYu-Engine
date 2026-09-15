# FIX2 T1 Report: Probe Resolver

## Status

GREEN. This is the first FIX2 commit after the clean FIX1 baseline `60a86ae`; no FIX1 production files or `xyui/` files were changed.

## TDD evidence

- RED: the new resolver tests initially failed to compile because Avalonia `Control.GetBaseValue` returns `Optional<string>`; after that correction, locator/ancestor assertions exposed the wrong visual-chain root behavior.
- GREEN: targeted build completed with 0 warnings and 0 errors; filtered resolver tests passed 8/8.

## Coverage

- Button child TextBlock resolves to Button.
- ComboBox child ContentPresenter resolves to ComboBox.
- Named interactable control wins semantic resolution.
- DeepVisual mode returns the raw hit visual.
- Nearest ancestor DebugId is reported as ParentDebugId.
- Named locator uses the stable Name.
- Unnamed locator uses type plus zero-based sibling index.
- Missing fields return `N/A` without throwing.

## Scope and concerns

- Added only pure resolver/result types and resolver tests; no Overlay, input, clipboard, or business UI changes.
- RuntimeLocator remains diagnostic output and is not registered as a DebugId.
- T2 Probe Overlay and T3 interaction remain unimplemented and blocked on this task.
