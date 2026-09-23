# MAP-REGION-SNAP-R1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** 将现有 Vertex→Vertex 屏幕空间吸附核心接入 Region 绘制预览与提交，并支持 Alt 临时抑制、即时恢复和可观察反馈。

**Architecture:** 复用 `RegionVertexSnapResolver`、`RegionVertexSnapState` 和 `MapEditSession.QueryLocalRegions`，不创建第二套空间索引或拓扑模型。UiVm 在绘制 PointerMoved/Pressed 时统一解析 raw 地面点，绘制状态保存解析后的 Cursor；Avalonia/Native 两条输入宿主只负责传递当前 Alt 状态，核心不依赖 UI。

**Tech Stack:** C# / .NET / Avalonia / xUnit / ImmutableArray / existing map projection and region spatial index.

**Spec:** 用户提供的 MAP-REGION-SNAP-R1 开发任务书（本对话）。

## Global Constraints

- 仅实现新区域顶点 → 已有区域顶点；禁止 Vertex→Edge、共享拓扑、合并、布尔和 Schema 改造。
- 吸附半径使用屏幕空间；采用现有 8 px 进入、12 px 释放稳定范围，集中于 `RegionVertexSnapSettings.Default`。
- 吸附成功必须复用目标 `MapPoint` 实例值；Alt 为按住抑制，松开自动恢复，不得做 Toggle。
- Preview 只更新 Draft/Cursor/Overlay，不写正式 Map/History/Inspector；提交继续走既有 Region Create/Undo/Save 链。
- 保持 `Editor.UI` 不依赖 Vulkan 实现；不修改 XYUI 或 Diagnostic，除非测试证明阻塞。
- 所有手写 `.cs` / `.axaml` / `.js` 文件不超过 100 行。

## Review Focus

- Alt 从按下到松开必须立即改变同一 Pointer 预览状态：由绘制输入宿主传入并由测试验证。
- 吸附预览与点击提交必须消费同一个解析点：由 UiVm 绘制测试验证，防止只移动视觉指示器。
- 没有已提交区域或局部查询失败时必须保留 raw 点且不抛异常：由核心已有/新增测试验证。
- 相同距离候选和临时 Draft 顶点不得造成不稳定或自吸附：由核心测试验证。
- Undo/Redo、Close、Cancel 和 Save/Reload 不得残留 Snap 状态：由区域工作流回归验证。

### Task 1: Extend the core contract tests

**Files:**
- Modify: `XuanYu.World.Tests/MapEditing/RegionVertexSnapResolverTests.cs`
- Create: `XuanYu.World.Tests/MapEditing/RegionDrawingSnapContractTests.cs`

**Interfaces:**
- Consumes existing `RegionVertexSnapResolver.Resolve`, `RegionVertexSnapState`, `RegionVertexSnapSettings`.
- Produces executable expectations for raw/suppressed/recovered resolution and exact target coordinates.

- [ ] Write tests for suppression as a caller-controlled bypass, exact target point reuse, empty candidates, and stable equal-distance selection.
- [ ] Run `dotnet test XuanYu.World.Tests/XuanYu.World.Tests.csproj --filter FullyQualifiedName~RegionVertexSnap` and verify the new integration expectations fail because Region drawing does not yet resolve snap.
- [ ] Keep existing core tests green and do not alter the resolver to add UI concerns.

### Task 2: Add Region drawing snap state and resolve preview/commit points

**Files:**
- Modify: `XuanYu.Editor.UI/Vm/Map/UiVm.RegionDrawing.cs`
- Modify: `XuanYu.Editor.UI/Vm/Map/UiVm.RegionDrawing.DraftHistory.cs`
- Modify: `XuanYu.Editor.UI/Vm/Map/MapRegionRenderProjection.cs` only if the existing overlay contract needs the snap marker state.
- Test: `XuanYu.World.Tests/UiRuntime/RegionDrawingSnapRuntimeTests.cs`

**Interfaces:**
- Consumes `RegionVertexSnapResolver`, `MapSession.QueryLocalRegions`, `MapSession.TryGetRegion`, current projection, raw Pointer point, and `snapSuppressed`.
- Produces `IsRegionDrawingSnapActive`, target identity/point diagnostics, and a resolved `_regionDrawing.Cursor` used by both click and preview.

- [ ] Write a failing runtime test that creates an existing region, starts a second draft, moves near an existing vertex, and asserts the cursor/next committed vertex equals the existing `MapPoint` exactly.
- [ ] Add the minimal UiVm resolver call with explicit source identity for the active draft; before the draft has an identity, exclude all temporary draft vertices by querying only committed map regions.
- [ ] Add an Alt-suppressed branch that clears the held snap state and preserves the raw picked point; release must call the same preview method and reacquire without requiring pointer motion.
- [ ] Clear snap state on start, commit, cancel, tool exit, undo/redo where appropriate; keep existing history and persistence methods unchanged.
- [ ] Run the focused runtime tests after each red/green cycle.

### Task 3: Route Alt state through existing viewport input owners

**Files:**
- Modify: `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.AvaloniaPointer.cs`
- Modify: `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Picking.cs`
- Modify: `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Pointer.cs`
- Test: `XuanYu.World.Tests/Viewport/RegionDrawingInputModifierTests.cs`

**Interfaces:**
- Consumes Avalonia `KeyModifiers.Alt` and existing Native pointer message Alt state.
- Produces one explicit `snapSuppressed` value for Region drawing Preview and Pressed; no parallel pointer owner or new mode.

- [ ] Write failing contract tests for pressed Alt, released Alt, and Native/Avalonia route parity.
- [ ] Change only the existing Region drawing input forwarding signatures/overloads needed to pass the modifier; leave camera/gizmo arbitration order intact.
- [ ] Verify Alt press/release causes an immediate preview refresh through the existing pointer route; do not implement a persistent toggle.

### Task 4: Add concise feedback and integration coverage

**Files:**
- Modify: `XuanYu.Editor.UI/Vm/Map/UiVm.RegionDrawing.cs`
- Modify: `XuanYu.Editor.UI/Vm/Map/UiVm.RegionDrawing.DraftHistory.cs`
- Modify: existing region drawing UI contract file selected by the current test layout.

**Interfaces:**
- Consumes `IsRegionDrawingSnapActive` and target identity from UiVm.
- Produces a simple “吸附中/未吸附/Alt 已取消吸附” status for current existing UI bindings; no new XYUI component.

- [ ] Write failing assertions for status transitions during normal preview, Alt suppression, release recovery, and close/cancel cleanup.
- [ ] Bind or expose the status through the existing Region drawing/context toolbar path; keep the visual treatment concise and diagnostic-friendly.
- [ ] Run Region drawing, Undo/Redo, Save/Reload, and focused Snap tests.

### Task 5: Gates, audit package, and Git delivery

**Files:**
- Create: `audit/MAP-REGION-SNAP-R1/AUDIT-MANIFEST.md`
- Create: `audit/MAP-REGION-SNAP-R1/<every changed file at repository-relative path>`
- Create: `audit/MAP-REGION-SNAP-R1-AUDIT-20260923.zip`

- [ ] Run focused Snap/Region tests, complete solution build, ARCH-A, 5+100, and `git diff --check`; record exact totals and unrelated baseline failures separately.
- [ ] Verify all modified/new source, test, directly related config, and this plan/manifest are copied with complete contents and relative structure.
- [ ] Record per-file type, purpose, production/test status, additions/deletions/net delta, verification results, known issues, and follow-ups in `AUDIT-MANIFEST.md`.
- [ ] Commit atomically, push the current branch, and verify local HEAD, upstream HEAD, remote HEAD, and ahead/behind.
- [ ] Leave user visual/real-device acceptance explicitly pending unless the user supplies current acceptance evidence.
