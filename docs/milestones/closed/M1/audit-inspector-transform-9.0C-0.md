# 9.0C-0 审计：Inspector / Selection / WorldState / Viewport 链路

审计日期：2026/06/24
审计目标：确认 9.0C Inspector ↔ Transform 同步所需的修改点

---

## 一、当前架构总览

```
选择变化（层级树 / 视口拾取 / 焦点）
  → EditorShellSelectionSyncRoute
  → WorldEntitySelectionPresenter（读取 WorldState.FindPosition）
  → EditorPanelApplyRoute.ApplyEntitySelection
  → InspectorPanel.ShowWorldEntitySelection（仅 Position）
  → StatusBar + ViewportPlaceholder 同步

Inspector 编辑
  → EditorShellTransformRoute / EditorShellScrubRoute
  → EditorTransformApplyRoute.Apply
  → EntityTransformCommit（Position 写入）
  → WorldTransformWriter.TrySetPosition → WorldState.SetPosition
  → EditorWorldDirtyState.MarkDirty
  → ScheduleFrame(EntityTransformChanged)
  → Viewport Redraw
```

---

## 二、各模块现状

### 1. Inspector 界面

| 文件 | 现状 |
|---|---|
| `Panels/Inspector/InspectorPanel.axaml` | 只有 Position 的 X/Y/Z 三行 TextBox |
| `Panels/Inspector/InspectorPanel.axaml.cs` | 事件：`TransformDraftChanged` / `TransformApplyRequested` / `TransformResetRequested` / `GroundPlacementRequested` / Scrub 事件 |
| `Panels/Inspector/InspectorTransformView.cs` | `SetTexts(x,y,z)` / `GetTexts()` / 错误显示。无 Rotation/Scale |
| `Panels/Inspector/InspectorTransformBinder.cs` | 绑定 Enter/Esc + Apply/Reset 按钮。无 Rotation/Scale |
| `Panels/Inspector/InspectorScrubInput.cs` | 标签拖动微调，用 `TransformPositionAxis` 枚举。无 Rotation/Scale |
| `Panels/Inspector/InspectorSelectionView.cs` | 选择信息展示。不受Transform影响 |
| `Viewport/Transform/Application/Capabilities/InspectorTransformDisplay.cs` | `SetPosition(x,y,z)` 包装。无 Rotation/Scale |

**结论：Inspector 当前仅支持 Position。需要新增 Rotation/Scale 的 UI 行和事件。**

### 2. 选中状态

| 组件 | 位置 | 详情 |
|---|---|---|
| `EditorEntitySelectionState` | `XuanYu.Engine.Editor/Selection/` | `SelectedEntityId` (string?) + `Revision` |
| `EditorSelectionState` | `Editor.Windows/Viewport/Selection/Route/` | `SelectedWorldEntity` (WorldEntityInfo?) |
| `EditorSelectionRoute` | 同上 | `SelectEntity(req)` / `ClearSelection(reason)` |
| `EditorSelectionRequest` | 同上 | `(string? EntityIdStr, EditorSelectionReason, WorldState?)` |

**选择传播链**已完整，选中实体后通过 `WorldEntitySelectionPresenter` 读取 Transform 并下发到 Inspector。

**结论：选择链路完整，9.0C 只需要扩展 Presenter 读取 Rotation/Scale。**

### 3. WorldState Transform 读写

| 操作 | 现状 |
|---|---|
| `FindPosition(id)` | ✅ 存在 |
| `FindRotation(id)` | ✅ 存在（9.0B 新增） |
| `FindScale(id)` | ✅ 存在（9.0B 新增） |
| `SetPosition(id, vec)` | ✅ 存在 |
| `SetRotation(id, vec)` | ❌ 不存在 |
| `SetScale(id, vec)` | ❌ 不存在 |
| `CreateEntity(name, pos, rot, scale)` | ✅ 存在 |

**结论：9.0C 必须新增 `SetRotation` 和 `SetScale` 方法到 WorldState。** 模式参照 `SetPosition`。

### 4. Transform 应用层

| 组件 | 位置 | 现状 |
|---|---|---|
| `WorldTransformWriter` | `Editor.Windows/Viewport/Transform/Application/Capabilities/` | 仅有 `TrySetPosition`。需 `TrySetRotation` / `TrySetScale` |
| `EntityTransformCommit` | 同上 | 仅 Position 提交。SceneTransform 已有 Rotation/Scale 字段但未写入 |
| `EntityTransformPreview` | 同上 | 仅 Position 预览 |
| `EntityTransformCancel` | 同上 | 仅 Position 回滚 |
| `EditorTransformApplyRoute` | `Editor.Windows/Shell/Transform/` | `CurrentEntityTransform` 只读 Position。`HandleInspectorApply` 只处理 Position |
| `EditorShellTransformRoute` | 同上 | `HandleTransformDraftChanged` / `HandleTransformApply` 只处理 Position |
| `EditorShellScrubRoute` | 同上 | 只处理 Position Scrub |

**结论：Transform 应用层需要全面扩展以支持 Rotation/Scale。** 建议新增独立于 Position 的 Rotation/Scale 编辑入口，不与现有 Position 逻辑耦合。

### 5. 视口刷新

| 机制 | 现状 |
|---|---|
| `ScheduleFrame(reason)` | ✅ 可以触发 `EntityTransformChanged` 刷新 |
| `EditorWorldDirtyState` | `MarkDirty(entityId)` / `Revision` / `IsDirty` 可用 |
| `Scene3dEntityPositionWriter` | 写入 Vulkan 会话。仅 Position |

**结论：刷新机制可用，9.0C 直接复用。** Rotation/Scale 的视口同步取决于渲染链路是否支持——如果暂不支持，则确保数据正确写入即可。

### 6. 保存链路

| 环节 | 现状 |
|---|---|
| `WorldStateDocumentConvert.ToDocument()` | ✅ 已读取 Position/Rotation/Scale |
| `WorldDocumentWriter.Write()` | ✅ 已写入三个字段 |
| `EditorShellComposition.SaveWorldRequested` handler | ✅ 已接入保存菜单 |

**结论：保存链路已完整支持 Rotation/Scale，9.0C 无需修改。**

---

## 三、9.0C 修改点清单

### 必须改

| # | 文件 | 修改内容 |
|---|---|---|
| 1 | `XuanYu.Engine/World/WorldState.cs` | 新增 `SetRotation()` / `SetScale()` |
| 2 | `Editor.Windows/Viewport/Transform/Application/Capabilities/WorldTransformWriter.cs` | 新增 `TrySetRotation()` / `TrySetScale()` |
| 3 | `Editor.Windows/Panels/Inspector/InspectorTransformView.cs` | 扩展为三组 (Position/Rotation/Scale) 读写 |
| 4 | `Editor.Windows/Panels/Inspector/InspectorPanel.axaml` | 新增 Rotation/Scale 的 X/Y/Z TextBox |
| 5 | `Editor.Windows/Panels/Inspector/InspectorPanel.axaml.cs` | 新增 Rotation/Scale 事件和方法 |
| 6 | `Editor.Windows/Panels/Inspector/InspectorTransformBinder.cs` | 新增 Rotation/Scale 按钮绑定 |
| 7 | `Editor.Windows/Panels/Inspector/InspectorScrubInput.cs` | 新增 Rotation/Scale Scrub |
| 8 | 新增 `Editor.Windows/Inspector/Transform/TransformEditSnapshot.cs` | 读取快照模型 |
| 9 | 新增 `Editor.Windows/Inspector/Transform/TransformEditRequest.cs` | 编辑请求模型 |
| 10 | 新增 `Editor.Windows/Inspector/Transform/TransformEditResult.cs` | 编辑结果模型 |
| 11 | 新增 `Editor.Windows/Inspector/Transform/SelectedEntityTransformReader.cs` | 从 WorldState 读取完整 Transform |
| 12 | 新增 `Editor.Windows/Inspector/Transform/SelectedEntityTransformApply.cs` | 写回 WorldState + Scale 校验 |
| 13 | `Editor.Windows/Viewport/Selection/Presentation/WorldEntitySelectionPresenter.cs` | 读取 Rotation/Scale |
| 14 | `Editor.Windows/Panels/Inspector/InspectorTransformDisplay.cs` | 添加 Rotation/Scale 显示 |
| 15 | 测试: Reader / Apply / RoundTrip | 新增测试文件 |

### 不改

| 范围 | 原因 |
|---|---|
| Move Gizmo 数学逻辑 | 9.0D |
| Rotate / Scale Gizmo | 本阶段不做 |
| `SceneTransform` 相关文件 | 渲染链路暂不动 |
| `Scene3dEntityPositionWriter` | 渲染链路暂不动 |
| 保存链路 (`WorldDocumentWriter`/`WorldDocumentReader`) | 9.0B 已完成 |
| `WorldStateDocumentConvert` | 9.0B 已完成，当前 67 行 |
| `EditorShellComposition.cs` SaveWorldRequested handler | 9.0A 已完成 |

---

## 四、9.0C 数据流设计

```
选中实体
  → SelectedEntityTransformReader.Read(entityId, worldState)
  → TransformEditSnapshot { Position, RotationDegrees, Scale }
  → InspectorPanel 显示三组 X/Y/Z

用户修改 RotationDegrees.Y
  → InspectorTransformView → 事件
  → TransformEditRequest { EntityId, Position, RotationDegrees, Scale }
  → SelectedEntityTransformApply.Apply(request, worldState)
     → 校验 Scale > 0 / 有限数字
     → WorldState.SetRotation / SetScale
     → WorldTransformWriter.TrySetRotation / TrySetScale
     → MarkDirty
  → ScheduleFrame(EntityTransformChanged)
  → Inspector 刷新最新值
```

---

## 五、UI 防死循环策略

```csharp
// InspectorTransformView
private bool _isRefreshing;

public void SetTexts(TransformEditSnapshot snapshot)
{
    _isRefreshing = true;
    // 更新 TextBox.Text
    _isRefreshing = false;
}

private void OnTextChanged()
{
    if (_isRefreshing) return;
    // 触发编辑事件
}
```
