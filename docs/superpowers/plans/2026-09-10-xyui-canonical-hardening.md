# XYUI Canonical Hardening Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make XYTabs sizing, XYButton/XYToggleButton content alignment, XYIconButton viewport alignment, and XYBadge contrast follow reusable XYUI contracts.

**Architecture:** Add an opt-in canonical `XyuiTabSizingMode` property to `XYTabs`, preserving `Equal` as the default and using `Content` for audited Engine/Gallery consumers. Keep button geometry in the shared `XyuiButtonChrome`; keep stateful layer actions as `XYToggleButton` and correct their shared content alignment without changing command semantics.

**Tech Stack:** C#, Avalonia, XYUI.Avalonia headless tests, XYUI.Avalonia Gallery, PowerShell gates.

**Spec:** `docs/superpowers/specs/2026-09-10-area-d-xyui-canonical-layout-design.md`

## Global Constraints

- Do not modify or stage `XuanYu.Editor.UI/Win/UnsavedChangesConfirmationWindow.axaml`.
- Do not add Engine-specific sizing/alignment workarounds for canonical controls.
- Keep every hand-written `.cs` and `.axaml` at or below 100 physical lines.
- Preserve XYTabBar ScrollViewer, previous/next, overflow menu, keyboard navigation, and selected/modified/close behavior.
- Automated visual tests assert layout/token contracts only; do not claim user visual acceptance.

### Task 1: Add failing XYUI sizing and alignment contracts

**Files:**
- Create: `xyui/avalonia/tests/XYUI.Avalonia.Tests/XYUI3TabSizingContractTests.cs`
- Create: `xyui/avalonia/tests/XYUI.Avalonia.Tests/XYUI2AlignmentContractTests.cs`
- Modify: `xyui/avalonia/tests/XYUI.Avalonia.Tests/XYUI3CompactNavigationStructureTests.cs`

**Interfaces:**
- Consumes the existing `XyuiBatchTestHost`, `XYTabs`, `XYTabBar`, `XYButton`, `XYIconButton`, `XYToggleButton`, `XYBadge` APIs.
- Produces red tests that define `XyuiTabSizingMode.Content`, `XyuiTabSizingMode.Equal`, centered ContentPresenter bounds, centered icon/toggle content, and Light/Dark badge state token resolution.

- [ ] **Step 1: Write the failing sizing tests**

  Create a headless window with two labels of materially different lengths. Assert `Content` mode gives different tab widths and `Equal` mode gives equal widths. Create a narrow `XYTabBar` with long tabs, select the last tab, and assert the existing horizontal offset/overflow action remains usable.

- [ ] **Step 2: Write the failing alignment tests**

  Show a text-only `XYButton`, an icon-plus-text `XYButton`, an `XYIconButton`, and an `XYToggleButton` containing a fixed-size icon host. Assert the content host center is within 0.5 DIP of the control center in both axes. Assert Primary, Secondary, Danger and Disabled classes/states remain present. Assert `XYBadge` resolves distinct Light/Dark Default and Accent brushes and preserves its 22 DIP geometry.

- [ ] **Step 3: Run the focused XYUI tests and record the expected failures**

  Run the focused test filters with the existing test project after the current build state is known. Expected failures are missing sizing API and the current left-aligned button/toggle presenter contracts; no production file is changed in this step.

### Task 2: Implement canonical tab sizing and shared alignment

**Files:**
- Create: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-08-Tabs/UI/XyuiTabSizingMode.cs`
- Modify: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-08-Tabs/UI/XYTabs.cs`
- Modify: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-08-Tabs/UI/XYTab.cs`
- Modify: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI2/_Shared/Styles/XyuiControlStyles.ButtonFamily.cs`
- Modify: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI2/_Shared/Styles/XyuiControlStyles.GhostAndToggle.cs`
- Modify: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI2/_Shared/ButtonFamily/XyuiButtonChrome.cs`

**Interfaces:**
- Produces `public enum XyuiTabSizingMode { Equal, Content }` and `XYTabs.SizingMode`, defaulting to `Equal`.
- `XYTabs.Build()` applies the mode to its panel/tab arrangement without changing selection events.
- Shared button Chrome centers XYButton content and centers XYToggleButton content while preserving full-width control bounds and existing `HorizontalContentAlignment` overrides for specialized controls.

- [ ] **Step 1: Add the sizing enum and property with the compatibility default**

  Register `SizingMode` as a styled property on `XYTabs` with `XyuiTabSizingMode.Equal`. Rebuild the panel when it changes, just as item changes already rebuild it.

- [ ] **Step 2: Implement Content and Equal layout paths**

  In `XYTabs`, keep the existing StackPanel as the single tab host. For `Content`, arrange tabs at natural measured width and set each tab to left-aligned sizing; for `Equal`, retain the existing available-width distribution. Do not add a second overflow control to `XYTabs`; leave `XYTabBar`’s existing ScrollViewer and action logic as the overflow implementation.

- [ ] **Step 3: Make the shared presenter center the whole content**

  Change the XYButton canonical style/template call from the current left presenter alignment to centered alignment. Keep the icon-plus-text stack as one content object so its combined bounds, not only its text child, are centered. Set the toggle shared content alignment to center without removing its checked-state edge or changing `IsChecked` behavior.

- [ ] **Step 4: Run the focused tests and correct only canonical implementation defects**

  Run the two new test classes and the existing XYUI2 button, icon, badge, and compact navigation tests with `--no-build` after a successful serial build. Fix only failures in the new shared contracts; do not add Engine margins or per-icon offsets.

### Task 3: Harden badge contrast and update XYUI Gallery

**Files:**
- Modify: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI1/XYUI1-09-Badge/XYBadge.cs`
- Modify: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI1/_Shared/Styles/XyuiComponentStyles.Semantic.cs`
- Inspect: `xyui/avalonia/src/XYUI.Avalonia/Foundation/XyuiColorTokens.Accent.cs` and retain its existing values unless the new contrast assertion demonstrates a canonical token defect
- Create: `xyui/avalonia/gallery/XYUI.Avalonia.Gallery/XYUI2LiveExamplesFactory.Alignment.cs`
- Create: `xyui/avalonia/gallery/XYUI.Avalonia.Gallery/XYUI3LiveExamplesFactory.TabSizing.cs`
- Modify: `xyui/avalonia/gallery/XYUI.Avalonia.Gallery/XYUI3GalleryCatalog.cs`

**Interfaces:**
- Gallery exposes live Content vs Equal tabs, centered text/icon buttons, centered icon/toggle actions, and badge normal/accent/disabled states.
- Badge changes continue to use Foundation tokens; no new business color literals are introduced.

- [ ] **Step 1: Add the live examples**

  Add side-by-side Content and Equal tab groups, text-only and Icon+Text buttons, Eye/Lock-style icon actions at 16/20/24 DIP, and Light-theme badge Default/Accent/Disabled examples with state labels.

- [ ] **Step 2: Make the canonical Light badge state readable**

  Trace the actual `XYBadge` visual descendants and update only the responsible canonical foreground/shape state style or existing `XY.Tag.Accent` token mapping. Keep the badge border contract consistent with `XYUI-1.canonical.md` and keep Disabled visibly subordinate.

- [ ] **Step 3: Run Gallery and contract tests**

  Verify every new live example is reachable through its canonical Gallery ID, and run the focused runtime tests in both Light and Dark theme variants.

### Task 4: Apply Content sizing to audited Engine tabs

**Files:**
- Modify: `XuanYu.Editor.UI/Left/Left.axaml`
- Modify: `XuanYu.Editor.UI/Right/EditorRightTabs.axaml`
- Modify: `XuanYu.Editor.UI/Right/MapEditorPanel.axaml`

**Interfaces:**
- These three consumers explicitly set `SizingMode="Content"` while retaining their existing compact XY size/density and event handlers.

- [ ] **Step 1: Add the one canonical sizing setting to each audited host**

  Add the same XYUI property to Left `XYTabs`, Right `XYTabs`, and Map `XYTabBar`’s exposed sizing path. Do not add Width, Margin, TextAlignment, or per-tab sizing values.

- [ ] **Step 2: Run the Engine source-contract tests**

  Assert the three hosts select `Content` and still route project/file, inspector/hierarchy/debug, and map secondary selection events to their existing handlers.
