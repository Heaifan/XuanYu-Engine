# Viewport Migration Regression Manifest R1

任务：`XYE-WAVE2-D-VIEWPORT-REGRESSION-R1`

基线提交：`330d924f1a3bfccbb8829b361c5f66083eec01f9`

统一基线来源：`2189e7f4a45d0bbd2b0388e6f26bed110aa2918a`

本清单只登记迁移前的行为契约。它不把 NativeControlHost、child HWND 或 Native Diagnostic Popup 重新定义为长期架构。

## 状态定义

- `EXISTING`：已有自动测试直接验证核心 invariant，本轮不复制测试。
- `PARTIAL`：已有局部或 headless 覆盖，但仍缺平台、宿主或完整输入链。
- `MISSING`：当前没有可接受的自动覆盖。
- `MANUAL BASELINE`：必须由真实 Avalonia/Win32/Vulkan UI 观察，不能用测试替代。

## 回归矩阵

| 能力 | 状态 | 现有证据 | 缺口/迁移风险 |
|---|---|---|---|
| Camera pointer navigation | EXISTING | `Core.Tests/Camera/CameraNavigationTests.cs`; `CameraNavigationSequenceTests.cs`; `World.Tests/Camera/CameraNavigationUiTests*.cs` | 真实 Native host 路由仍需人工确认 |
| Wheel Dolly | EXISTING | `CameraOrthographicNavigationTests.cs`; `CameraNavigationUiTests.cs`; `AvaloniaPointerEventAdapterTests.cs` | Win32 设备滚轮与宿主链为人工项 |
| Picking | EXISTING | `Core.Tests/Picking/ViewportPickingServiceTests.cs` | Vulkan surface 真实命中为人工项 |
| Selection | EXISTING | `World.Tests/MapEditing/MapEditSessionSelectionTests.cs`; Transform UI tests | 真实 Viewport 点击路径为人工项 |
| Navigation Gizmo | EXISTING | `World.Tests/Camera/UiViewGizmoTests.cs`; `Core.Tests/Camera/CameraNavigationUiSequenceTests.cs` | 六面真实点击与焦点需人工项 |
| Transform Gizmo | EXISTING | `Core.Tests/Gizmo/*`; `World.Tests/Transform/{Move,Rotate,Scale}/*` | Win32 capture 生命周期为 PARTIAL |
| Region Preview | EXISTING | `UiRuntime/RegionDrawing*RuntimeTests.cs`; `MapEditing/RegionDrawingStateTests.cs` | 真实 overlay 显示为人工项 |
| Region Edit | EXISTING | `MapEditSessionGeometryTests.cs`; `RegionDrawingStateTests.cs` | 真实 Viewport 编辑链为人工项 |
| Road Preview | PARTIAL | `UiRuntime/RoadDrawingSelectionF1Tests.cs` | 独立的真实 preview 宿主基线缺失 |
| Road Edit | EXISTING | `MapEditSessionGeometryTests.cs`; `RoadVertexDragD2Tests.cs` | 真实 capture/commit 链为人工项 |
| Marker interaction | EXISTING | `MapMarkerPlacementTests.cs`; `MapMarkerInspector*Tests.cs`; `MapMarkerInspectorWorkflowTests.cs` | 空选择 workflow 当前有已知失败 |
| Map Geometry | EXISTING | `MapGeometryHitTesterTests.cs`; `MapPickingRoundTripTests.cs`; `MapEditSessionGeometryTests.cs` | 真实 DPI surface 为人工项 |
| Snap | EXISTING | `RegionSnapPipeline*Tests.cs`; `RegionEdgeSnapResolverTests.cs`; `Generic*SnapIntegrationTests.cs` | Alt 与真实输入宿主组合为 PARTIAL |
| Native Alt semantics | EXISTING | `NativePointerRoutePolicyTests.cs`; `RegionDrawingInputModifierTests.cs` | 无真实键盘设备自动覆盖 |
| Pointer Capture / Release | PARTIAL | `UnifiedPointerModelTests.cs`; Transform session tests | OS capture 交接与释放为 MANUAL BASELINE |
| Cancel | EXISTING | Camera cancel、Transform cancel、Region/Road session tests | 真实 Esc/鼠标取消为人工项 |
| FocusLost | PARTIAL | 现有 Camera/Transform focus safety tests | Native/Avalonia FocusLost 全链缺失 |
| CaptureLost | PARTIAL | session safety 与 stale pointer end 测试 | 平台 CaptureLost 事件注入缺失 |
| DPI | EXISTING | `ViewportPickingServiceTests.Dpi_viewport_uses_logical_coordinates` | 多显示器真实 DPI 为人工项 |
| Logical Coordinate | EXISTING | Picking round-trip、Pointer Adapter tests | Native 一次且仅一次换算仍需人工复核 |
| Diagnostic Placement | EXISTING | `DiagnosticPlacementPolicy*Tests.cs`; `DiagnosticR1*Tests.cs`; Diagnostic runtime tests | Native PopupRoot 几何必须人工确认 |
| Overlay placement contract | PARTIAL | `DiagnosticOverlayRuntimeTests.cs`; `DiagnosticProbeOverlayRuntimeTests.cs` | Native legacy bridge 仍是迁移债务，终局覆盖缺失 |

## MISSING 与人工基线

当前没有新增 headless 测试：可自动验证的主要 invariant 已由既有 Core/World 测试覆盖，复制它们会制造第二套基线。仍登记以下不可替代的人工基线：真实 Vulkan surface、Win32 capture/release、FocusLost/CaptureLost、真实 DPI、多显示器坐标、Diagnostic PopupRoot 与 Overlay 覆盖关系。

`Road Preview`、完整宿主级 `FocusLost/CaptureLost` 与终局 `Overlay placement` 没有独立自动闭环，状态保持为 `PARTIAL`，不得在迁移后解释为自动 PASS。

## 当前统一基线结果

脚本对 Core.Tests 与 World.Tests 串行执行，并读取 TRX 的单测试结果。当前统一基线已登记的 22 个 World 失败见 `viewport-migration-known-failures-r1.txt`；它们是 `KNOWN BASELINE FAILURE`，未来未出现在清单中的失败才是 `NEW REGRESSION`。

一键入口：`scripts/test-viewport-migration-baseline.ps1`
