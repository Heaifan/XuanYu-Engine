# 地图右键菜单与 Region 显示问题审计需求单

## 1. 审计目标

请审计地图 Region 右键菜单、XYUI Popup 关闭行为、Region 名称显示、Region 颜色显示，以及 Inspector 动态刷新之间的真实运行链路。

本需求单用于交给其他 AI 做独立代码审计。不得只根据专项测试全绿就判定通过，必须核对代码入口和运行时坐标/输入链。

## 2. 当前可观察现象

1. 在 Vulkan 地图视口右键 Region，会出现地图对象菜单。
2. 菜单需要使用 XYUI 组件，当前预期组件为 `XYContextMenu`、`XYMenu`、`XYMenuItem`。
3. 菜单在 Vulkan 原生子窗口上方显示；点击菜单外的 Vulkan 区域后，菜单必须关闭。
4. Region 名称必须在右键菜单标题和右侧 Inspector 中显示；名称应来自 `MapRegion.DisplayName`，不能写死。
5. Region 填充和边框必须有可辨识的颜色；选中态继续使用选中边框语义。
6. Inspector 从未选中对象切换到 Region 时，不能因为 Diagnostic Auto ID 刷新而崩溃。

## 3. 已知代码事实

- 地图菜单入口：`VulkanNativeHost.ContextMenu.cs`。
- 原生指针入口：`VulkanNativeHost.Pointer.cs`、`Win32ViewportHost.Input.cs`。
- XYUI 菜单实现：`xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-03-ContextMenu/`。
- Region 投影颜色：`XuanYu.Editor.UI/Vm/Map/MapVectorOverlayBuilder.cs`。
- Region 菜单名称来自 `DisplayName(...)`，Inspector 标题来自 `InspectorSelectionTitle`。
- Diagnostic Auto ID 修复已提交在 `29688ed9`；本轮 XYUI 原生点击关闭和颜色调整已提交在 `6c1d5936`。
- 当前环境之前已确认没有可用 .NET SDK，因此未能运行 `dotnet test`；不得把静态检查当作运行测试通过。

## 4. 必须审计的问题

### A. XYUI 菜单身份

- 是否始终由 `XYContextMenu + XYMenu + XYMenuItem` 构成？
- 是否存在运行时又创建 Avalonia 原生 `ContextMenu`、`MenuFlyout` 或临时 `Border` 替代 XYUI 的路径？
- Popup 的 `PlacementTarget`、坐标空间和 `IsLightDismissEnabled` 是否与 Vulkan 原生 HWND 的输入边界匹配？

### B. 左键关闭

- 左键点击 XYUI 菜单内部：菜单项应正常执行，不应被外部关闭逻辑抢先吞掉。
- 左键点击 Vulkan 视口菜单外区域：菜单必须关闭，然后地图继续处理该左键输入。
- 左键点击普通 Avalonia 区域：Popup 也必须关闭。
- 右键再次点击其他 Region：旧菜单必须正确关闭/重建，标题和命令集合必须更新。

### C. Region 名称

- 菜单标题、Inspector 标题、Inspector 可编辑属性是否都读取同一个 `MapRegion.DisplayName` 事实源？
- 新建第一个、第二个 Region 的名称是否分别为“区域1”“区域2”？
- 重命名后，当前选中对象是否保持，菜单标题和 Inspector 是否同步？
- 如果需求是“文字直接画在地图 Region 内部”，请明确指出当前渲染协议是否支持文字图元；不得用一个不可见的 Avalonia XYUI 覆盖层冒充已实现。

### D. Region 颜色

- 未选中 Region 的填充、边框是否有足够对比度？
- 选中 Region 是否保持选中边框/顶点视觉，不应被普通颜色覆盖？
- 颜色是否来自当前 Region 投影，而不是只改测试常量或 Inspector 样式？
- 请核对 Vulkan Vector Overlay 的 Alpha、混合和最终屏幕颜色，不能只看 CPU 侧 `RenderStaticModelColor`。

### E. Diagnostic 刷新

- InspectorIdentity 切换导致 VisualTree 动态变化时，已有 DebugId 是否被纳入 used 集合？
- `XYE.AUTO.*` 冲突是否自动修复，人工固定 ID 冲突是否仍然报错？
- Region 右键真实链是否可以完成：命中 → 选择 → Inspector 切换 → Diagnostic Refresh → 菜单显示，且不退出编辑器？

## 5. 强制验收用例

1. 右键 Region 面：菜单使用 XYUI，显示“地图对象”和真实 Region 名称。
2. 点击菜单项外的 Vulkan 区域：Popup 关闭，地图左键输入仍继续。
3. 点击菜单内部菜单项：命令执行，不能被外部 dismiss 逻辑拦截。
4. 右键第二个 Region：菜单标题、菜单命令和选中对象全部更新。
5. 新建两个 Region：名称为“区域1”“区域2”。
6. Inspector 选择 Region：默认进入“基础”，属性列表非空，包含“区域名称”。
7. Inspector 改名：Region、菜单标题、Inspector 标题同步，选择不丢失，Undo/Redo 正常。
8. Region 颜色：未选中和选中态均可在实际窗口中观察到，不只通过单元测试判断。
9. Inspector 动态切换：重复刷新不抛 `诊断 ID 重复：XYE.AUTO.*`。
10. 右下窗口边缘右键：菜单仍靠近点击点，且不越过 TopLevel 边界。

## 6. 禁止事项

- 不得把 Vulkan Fence/Swapchain Timeout 当作第一根因，除非先证明 UI 线程没有异常。
- 不得删除或弱化现有测试断言。
- 不得用空 catch、延迟吞异常或只关闭日志来“修复”。
- 不得把 XYUI 菜单替换成临时 Avalonia 控件。
- 不得把整个仓库、用户私有文件或无关输入改动纳入审计包。

## 7. 审计输出要求

请输出：

- 每个现象对应的代码入口、真实根因和证据文件。
- 哪些行为已实现、哪些只在测试中实现、哪些仍未实现。
- 运行时验收结果；如果无法运行，说明缺失的 SDK/环境，不得声称通过。
- 最小修复建议和新增回归测试建议。
