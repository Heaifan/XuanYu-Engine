# WORLD-B-R0 编辑器基本操作现状审计与合同冻结

版本：v0.2.20.1-rz
日期：2026-07-26 10:16:13
阶段：WORLD-B：编辑器基本操作与实体变换闭环 / R0
分支：feat/WORLD-B-editor-interaction

## 0. 裁定

上一阶段 ARCH-WORLD 已完成分层边界收口；下一阶段不进入 `XuanYu.WarCore`，也不创建 `MilitaryIdentity`、`FactionId`、士兵、战斗、士气、补给或战争结算。WORLD-B 的正确目标是把玄域编辑器先做成稳定可操作的三维世界工具。

R0 只冻结合同与现状，不实现 Orbit / Pan / Zoom、Rotate、Scale、Local 或 Inspector 数值写回。后续实装必须复用当前 World / Selection / CameraState / Picking / Move Gizmo / History 主链，不得重造第二套权威状态。

## 1. 阶段主线

```text
WORLD-A
世界实体与空间基础
        ↓
ARCH-WORLD
Core / World / Render / Editor 边界收口
        ↓
WORLD-B
相机 + 选择 + Move / Rotate / Scale
        ↓
WORLD-C 或 WARCORE-A
编辑器增强 / 一个士兵闭环
```

WORLD-B 完成定义：看得清、选得中、移得动、转得对、缩得准、可以取消、可以撤销、可以重做。

## 2. 当前真实能力审计

| 审计项 | 当前代码事实 | R0 裁定 |
| --- | --- | --- |
| CameraState 权威位置 | `UiVm.Camera.cs` 持有 `_camera`、`_viewportAspect`、`_cameraRevision`；`CurrentCamera()` 为 Picking / Render 提供显式相机。 | 保留。R1 在 Editor/UI 输入侧扩展相机会话，不新增第二个相机权威。 |
| Frame All / Selected | `EditorCameraFraming.FrameAll/FrameSelected` 由 `XuanYu.Editor.Camera` 提供纯函数；`UiVm.Camera.cs` 调用后发布 Render Projection。 | 保留。R1 必须补充焦点/距离状态，使后续 Orbit 围绕同一观察中心。 |
| Orbit / Pan / Zoom | 仅有顶部按钮文案和图标资源；未发现 MMB / Shift+MMB / Wheel 改写 `CameraState` 的真实输入链。 | 缺口。R1 实装，不能伪装为按钮文本。 |
| Viewport Picking | `VulkanNativeHost.Pointer.cs` 左键空闲点击进入 `ReportPointerPicking`；`UiVm.Picking.cs` 构造 `ViewportPickingRequest` 并调用 `ViewportPickingService.Pick`。 | 保留。相机与 Resize 只能通过同一 `CameraState` / `ViewportState` 进入 Picking。 |
| Selection 权威 | `EditorStateOwner` 持有唯一 `EditorSelectionSnapshot`；视口、Project Tree、Hierarchy Tree 都经 `Select/Clear` 命令进入同一 Owner。 | 保留。R2 不新增 `ViewportSelection` / `HierarchySelection` / `InspectorSelection` 权威。 |
| Hierarchy / Inspector 投影 | `UiVm.SelectionProjection.cs` 同步树投影；`UiVm.WorldProjection.cs` 由 `_sceneState` 查询选中实体并生成 Inspector 字段。 | 保留。R2 只修一致性缺口；R4 才加入可编辑 Transform 数值。 |
| ToolMode 所有者 | `EditorStateOwner.Tool.cs` 持有 `EditorToolSnapshot.ActiveTool`；`Select/BoxSelect/Move/Rotate/Scale` 是持续工具，Snap 是独立 Toggle。 | 保留。R2 冻结唯一 `ActiveTool`，Rotate / Scale 未实现前不得显示伪 Gizmo。 |
| Move Gizmo 显示与捕获 | `EditorTransformCapturePolicy` 只允许 `ActiveTool=Move` 且有选择时显示/捕获 Move Gizmo。 | 保留。R3 在此基础上补平面、精度和真机门禁。 |
| Transform Preview Owner | `XuanYu.Editor.Transform.TransformSession` 持有 SessionId、Axis、StartSnapshot、Preview；Preview 进入 `SceneRenderSnapshot`，未写正式 World。 | 保留。Rotate/Scale 应扩展同类 Session 语义，不把永久 Transform 搬到 Editor.UI。 |
| Commit 写回入口 | `TransformSession.TryCommit()` 调 `SceneStateOwner.CommitPositionWithResult()`；`SceneStateOwner` 再写 `GlobalWorld.UpdateTransform()`。 | 保留。WORLD-B 全部 Transform Commit 必须仍写回 GlobalWorld。 |
| Undo / Redo | `EditorHistoryOwner` 记录 `TransformHistoryEntry(Before/After)`；Undo/Redo 通过 `SceneStateOwner.RestoreTransform()` 恢复快照。 | 保留。R3/R4 扩展 Move/Rotate/Scale 后必须保持 Snapshot 恢复，不重新模拟输入 Delta。 |
| Mouse Capture 生命周期 | Win32 子窗口 `SetCapture/ReleaseCapture`，处理 `WM_CAPTURECHANGED`、`WM_KILLFOCUS`、`WM_CANCELMODE`；Avalonia 侧处理 `PointerCaptureLost`。 | 保留。R1/R3/R4 任何新会话都必须接入同一取消口。 |
| Escape Cancel | `UiWin.axaml.cs` 和 `Left.axaml.cs` 将 Escape 转入 `CancelInteractionFromEscape()`。 | 保留。R1 相机会话也必须响应 Escape。 |

## 3. 输入映射冻结

| 操作 | WORLD-B 默认输入 | 当前状态 | 裁定 |
| --- | --- | --- | --- |
| Orbit | 鼠标中键 | 未实现真实相机会话 | R1 实装。 |
| Pan | Shift + 鼠标中键 | 未实现真实相机会话 | R1 实装。 |
| Zoom / Dolly | 滚轮 | 未实现真实相机会话 | R1 实装。 |
| Frame All | `Home` 或现有“查看全部”命令 | 顶部按钮已有，命令链已接入 | R1 补快捷键与焦点合同。 |
| Frame Selected | Blender 对应操作或现有“聚焦”命令 | 顶部按钮已有，命令链已接入 | R1 补焦点保持。 |
| Select | 左键点击实体 / Hierarchy 点击 | 已有主链 | R2 收口一致性。 |
| Clear Selection | 点击空白 | 已有主链 | R2 验证删除/失效实体清理。 |
| Move | 工具栏按钮 + `G` 兼容 | 工具栏已有；快捷键未冻结 | R2/R3 收口。 |
| Rotate | 工具栏按钮 + `R` 兼容 | ActiveTool 有枚举，无真实 Transform | R4 实装前不得伪装能力。 |
| Scale | 工具栏按钮 + `S` 兼容 | ActiveTool 有枚举，无真实 Transform | R4 实装前不得伪装能力。 |
| Axis Constraint | `X/Y/Z` | Move Gizmo 当前仅点击轴段 | R3/R4 再冻结键盘轴约束。 |
| Cancel | Escape / LostCapture / WM_CANCELMODE / Window lost focus | 已有取消入口 | 后续所有会话复用。 |
| Commit | 鼠标释放或确认操作 | Move Commit 已有 | R3/R4 统一一次性 Commit。 |

输入优先级冻结：

```text
正在执行的 Gizmo Capture
        ↓
正在执行的 Camera Capture
        ↓
Inspector 文本编辑
        ↓
Viewport Picking
        ↓
普通工具快捷键
```

一次鼠标输入只能有一个所有者。相机 Capture 与 Gizmo Capture 不得同时生效。

## 4. 权威图

```text
CameraState
  Owner: UiVm.Camera.cs
  Consumer: Picking / Render Projection / Gizmo Projection
  R1: 增加 Orbit/Pan/Zoom 会话，不新增第二相机

Selection
  Owner: EditorStateOwner.EditorSelectionSnapshot
  Writers: Viewport Picking / Project Tree / Hierarchy Tree / Clear
  Consumers: Hierarchy Projection / Inspector / Gizmo Visibility / Render Selection

ToolMode
  Owner: EditorStateOwner.EditorToolSnapshot.ActiveTool
  Writers: Toolbar / future shortcuts
  Consumers: Capture Policy / UI Highlight / Footer / Gizmo Visibility

Transform Truth
  Owner: GlobalWorld -> EntityRegistry
  Writer: SceneStateOwner -> GlobalWorld
  Preview: XuanYu.Editor.Transform.TransformSession
  History: EditorHistoryOwner TransformHistoryEntry(Before, After)
```

## 5. 明确保留项

- `CameraState`、`ViewportState`、`ViewProjectionState` 和 `EditorCameraFraming`。
- `ViewportPickingService`、`UiVm.Picking`、`UiVm.ViewportSelection`。
- `EditorStateOwner` 作为 Selection / Tool 的 UI 写入 Owner。
- `EditorTransformCapturePolicy` 对 Move Gizmo 的真实能力限制。
- `TransformSession` 的 Begin / Preview / Commit / Cancel 事务形状。
- `SceneStateOwner -> GlobalWorld` 的 Transform 写回路径。
- `EditorHistoryOwner` 的 Before / After Snapshot Undo / Redo。
- Win32 / Avalonia Capture Lost、WM_CANCELMODE、KillFocus、Escape 的取消入口。

## 6. 已确认缺口

### R1 相机

- 无 MMB Orbit / Shift+MMB Pan / Wheel Dolly 的真实输入路由。
- `EditorCameraFraming` 当前返回相机位置和方向，但没有单独冻结观察中心；Frame Selected 后“后续 Orbit 围绕选中实体”尚未成立。
- Resize 后相机不跳变需要保持现有 `_viewportCameraFramed` 语义，并新增相机会话回归。

### R2 选择与工具

- Selection 主链存在，但删除选中实体后的 Selection / Gizmo / Inspector 失效清理需要专门门禁。
- `BoxSelect`、`Rotate`、`Scale` 已在 Tool 枚举与 UI 出现，但未有真实能力；R2 必须冻结“未实现能力不得显示伪 Gizmo/伪 Session”。
- Inspector 文本编辑抢占快捷键尚未形成合同。

### R3 Move

- 当前 Move 仅 `X/Y/Z` 单轴；`MoveGizmoAxis` 没有 `XY/XZ/YZ` 平面。
- `MoveGizmoDragConstraint` 以屏幕轴投影求解世界轴距离；不同相机角度、大世界坐标、跨 Region、Resize 前后还需要真机门禁。
- Commit / Cancel / Late MouseUp 自动测试已有基础，但 R3 要提升为完整验收。

### R4 Rotate / Scale / Local / Inspector

- `CommittedTransform` 当前只有 `Position`，没有 Rotation / Scale 权威字段。
- Rotate / Scale 只有 UI Tool 枚举和按钮，没有 Transform 数据合同、Gizmo、Preview、Commit、History。
- Local 不能只转 Gizmo 外观；必须在 Rotation 权威字段完成后再启用。
- Inspector 当前只显示 Position 文本，不提供精确数值编辑 Session。

## 7. WORLD-B 分层冻结

- `XuanYu.World`：继续持有 `GlobalWorld`、`EntityRegistry`、`EntityId`、Position / 后续 Rotation / Scale 权威状态、Region、WorldQuery、Snapshot。
- `XuanYu.Core`：仅放稳定数学、Picking/Gizmo 通用计算、History 数据结构；不得重新承载 World 或 Editor 会话事实。
- `XuanYu.Editor`：放编辑操作会话规则、相机操作算法、Transform Session；不得依赖 Avalonia / Vulkan，也不得拥有实体永久 Transform。
- `XuanYu.Editor.UI`：负责输入路由、工具状态、鼠标捕获、快捷键、Inspector 显示和操作意图；不得复制 GlobalWorld Transform 权威。
- Render：只消费 Render Projection 并绘制实体、Gizmo、选择高亮和方向轴；不得修改 Transform。

## 8. R0 出口门禁状态

| 门禁 | 状态 | 证据 |
| --- | --- | --- |
| 不重复创建 Camera 系统 | PASS | 本轮未改生产代码；R1 指定复用 `_camera` / `CameraState`。 |
| 不重复创建 Transform 系统 | PASS | 本轮未改生产代码；R3/R4 指定复用 `TransformSession` 形状与 GlobalWorld 写回。 |
| 不复制 GlobalWorld 状态 | PASS | 本轮只读审计，未新增状态。 |
| 输入所有权明确 | PASS | 本文冻结优先级和 Capture Owner。 |
| WarCore 后移 | PASS | 本文明确禁止 WORLD-B 创建 WarCore 内容。 |
| 全量构建测试通过 | PASS | 非沙箱 `dotnet build .\XuanYu.Engine.slnx --no-incremental` 10 项目 0W0E；`dotnet test .\XuanYu.Engine.slnx --no-build` 171 passed；`scripts/arch-a-guard.ps1` EXIT=0；5+100、SVG XML、`git diff --check` PASS。 |
| commit + push | 待提交推送 | 目标分支 `feat/WORLD-B-editor-interaction`。 |

## 9. 下一轮入口

R1 只做编辑器相机操作：Orbit / Pan / Zoom / Frame All / Frame Selected / Resize / LostCapture / Escape。R1 禁止顺手实现 Rotate、Scale、WarCore、多选、吸附、场景序列化或完整地形编辑。
