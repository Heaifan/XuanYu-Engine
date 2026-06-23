# 8.7.8G-1 — EditorPreferencesWindow 拆分审计

审计日期：2026-06-23
目标文件：`FluidWarfare.Editor.Windows/Preferences/EditorPreferencesWindow.axaml.cs`
目标行数：587 行

---

## 1. 当前文件状态

| 维度 | 值 |
|------|-----|
| **行数** | 587 行 |
| **类型** | `sealed partial class EditorPreferencesWindow : Window` — Avalonia UI 窗口 |
| **Preferences/ 目录** | 2 文件（axaml + cs），余量 +3 |
| **白名单** | ✅ 在 `CodeFileBudgetTests` 中（行白名单） |

## 2. 职责拆解

### 10 个职责

| # | 职责 | 方法/区域 | 行数 | 占比 |
|---|------|-----------|------|------|
| 1 | **捕获状态机** | 状态字段 + `BeginCapture`/`CancelCapture`/`CompleteCapture`/`ApplyCapture` | 35 | 6% |
| 2 | **草稿绑定集管理** | `LoadDraftFromService`/`SetDraftOverride`/`RemoveDraftOverrides`/`ClearAllDraftOverrides`/`HasAnyChanges` | 28 | 5% |
| 3 | **绑定列表生成** | `PopulateBindings`（分类+过滤+渲染循环） | 36 | 6% |
| 4 | **单行绑定 UI 创建** | `CreateBindingRow`/`CreateBindingButton` | 68 | 12% |
| 5 | **手势文本格式化** | `FormatGestureText` — 110 行 switch | 51 | 9% |
| 6 | **按键捕获 + 键码映射** | `OnWindowKeyDown` + `AvaloniaKeyToCode` — 33+32 行 switch | 65 | 11% |
| 7 | **鼠标/滚轮捕获** | `OnWindowPointerPressed` + `OnWindowPointerWheelChanged` | 34 | 6% |
| 8 | **冲突检测与绑定查询** | `DetectConflict`/`GetEffectiveGesture`/`GetBlenderDefaultGesture`/`HasEffectiveOverride`/`BuildEffectiveBindingList` | 38 | 6% |
| 9 | **恢复/搜索** | `OnRestoreSingleClicked`/`OnRestoreAllClicked`/`OnSearchTextChanged` | 18 | 3% |
| 10 | **保存/应用/取消** | `OnSaveClicked`/`OnApplyClicked`/`OnCancelClicked`/`FlushDraft`/`UpdateButtonStates`/`ShowError` | 35 | 6% |
| — | **XAML 事件绑定** | 8 个 XAML 声明 + 5 个代码绑定 | 13 | 2% |
| — | **窗口字段 + 构造函数** | 状态字段、`InitializeComponent`、`OnOpened` | 22 | 4% |
| — | 空行/注释 | — | ~86 | 15% |

### XAML 事件绑定（拆分时必须保留）

**8 个 XAML 声明事件**（在 `EditorPreferencesWindow.axaml` 中）：
```xml
Opened="OnOpened"
KeyDown="OnWindowKeyDown"
PointerPressed="OnWindowPointerPressed"
PointerWheelChanged="OnWindowPointerWheelChanged"
TextChanged="OnSearchTextChanged"
Click="OnRestoreAllClicked"
Click="OnCancelClicked"
Click="OnApplyClicked"
Click="OnSaveClicked"
```

**5 个代码绑定事件**（在 `.cs` 中）：
```csharp
restoreBtn.Click += OnRestoreSingleClicked;
btn.Click += OnBindingButtonClicked;
```

### 状态字段归属

| 字段 | 归属 |
|------|------|
| `_captureState` / `_captureActionId` / `_captureSlot` / `_capturedGesture` / `_captureButton` / `_conflictActionId` / `_conflictSlot` | **捕获状态机** |
| `_originalBindingSet` / `_draftBindingSet` | **草稿管理** |

## 3. 最大风险

| 风险 | 级别 | 说明 |
|------|------|------|
| **XAML 事件绑定** | **高** | 8 个 XAML 声明事件直接引用 `.cs` 方法名。拆分后如果方法名改了，XAML 编译报错 |
| **`PopulateBindings` 耦合** | 中 | 同时负责分类、过滤、UI 创建、按钮状态更新。拆的时候要小心保持重建流程完整 |
| **`FormatGestureText`** | 低 | 110 行 switch 表达式，纯展示逻辑，提取无风险 |
| **`AvaloniaKeyToCode`** | 低 | 32 行 switch，纯映射，提取无风险 |
| **`FlushDraft` 保存链** | 中 | 涉及 `EditorSettingsWriter` + `EditorInputService`，拆分时必须保持保存行为不变 |

## 4. 推荐拆分方案

### 4 文件方案（推荐）

| # | 新文件 | 预计行数 | 职责 | 备注 |
|---|--------|----------|------|------|
| 1 | `EditorPreferencesWindow.axaml.cs` | ≤100 | **门面**：XAML 事件入口 + 窗口生命周期 + 管线编排 | 8 个 XAML 事件方法 + 构造函数 + 状态字段 |
| 2 | `EditorPreferencesCapture.cs` | ≤100 | **捕获状态机**：状态字段 + `BeginCapture`/`CancelCapture`/`CompleteCapture`/`ApplyCapture` + 冲突检测 + 按键/鼠标/滚轮捕获 + 键码映射 | Avalonia 事件参数 → 内部动作 |
| 3 | `EditorPreferencesBindingList.cs` | ≤100 | **绑定列表**：`PopulateBindings` + `CreateBindingRow` + `CreateBindingButton` + `FormatGestureText` + `UpdateButtonStates` | 纯 UI 创建，无保存逻辑 |
| 4 | `EditorPreferencesDraftManager.cs` | ≤100 | **草稿管理**：草稿字段 + `LoadDraftFromService`/`SetDraftOverride`/`RemoveDraftOverrides`/`ClearAllDraftOverrides` + `FlushDraft` + `HasAnyChanges` + `GetEffectiveGesture`/`BlenderDefault`/`BuildEffectiveBindingList` | 数据操作，无 UI |

### 4 文件的管线示意

```
OnSaveClicked (XAML)
  → _draftManager.FlushDraft(...)     // 保存草稿
  → bindingList.PopulateBindings(...)   // 刷新 UI
```

```
OnBindingButtonClicked
  → capture.BeginCapture(...)           // 进入等待
  → OnWindowKeyDown (XAML)
  → capture.HandleKeyDown(...)          // 解析按键
  → capture.CompleteCapture(gesture)    // 检查冲突
  → _draftManager.SetDraftOverride(...) // 写入草稿
  → bindingList.PopulateBindings(...)   // 刷新 UI
```

```
OnRestoreSingleClicked
  → _draftManager.RemoveDraftOverrides(actionId)
  → bindingList.PopulateBindings(...)
```

### 为什么不分 partial class？

Avalonia 已经用 `partial class` 拆分 XAML 生成代码和用户代码。如果再拆 partial class，事件方法必须在同一个类中，导致文件仍会超过 100 行。所以使用**组合模式**（门面持有子组件的引用）。

### 目录变化

```
Preferences/
├── EditorPreferencesWindow.axaml         (不变)
├── EditorPreferencesWindow.axaml.cs      (回收 → ≤100)
├── EditorPreferencesCapture.cs           (新增)
├── EditorPreferencesBindingList.cs       (新增)
└── EditorPreferencesDraftManager.cs      (新增)
```

5 文件 = 目录上限 ✅

## 5. 结论

| 维度 | 结论 |
|------|------|
| **是否可以拆** | ✅ **可以拆** |
| **风险** | **低** — 纯 UI 窗口，无 Vulkan 崩溃风险 |
| **最大阻碍** | 8 个 XAML 事件绑定方法名不能变，门面必须保留这些方法 |
| **推荐方案** | **4 文件方案**（门面 + Capture + BindingList + DraftManager）|
| **目录** | Preferences/ 2→5 文件 = 上限 |
| **下一轮建议** | G-2：执行拆分 |

### 风险总结

```
XAML 事件绑定：  ████████░░  (8/10)  — 方法名不能变
保存逻辑：       ████░░░░░░  (4/10)  — FlushDraft 涉及外部服务
UI 创建逻辑：    ██░░░░░░░░  (2/10)  — 纯代码，提取简单
捕获状态机：     ███░░░░░░░  (3/10)  — 状态集中在门面即可
```
