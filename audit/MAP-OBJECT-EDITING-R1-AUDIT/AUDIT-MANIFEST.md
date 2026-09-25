# MAP-OBJECT-EDITING-R1 Audit Manifest

本包对应 `MAP-OBJECT-EDITING-R1-A/B0`。包含本轮新增或修改的完整生产源码与测试源码；未包含此前工作区已有的 R2 输入文件和旧审计目录。

## Production

- XuanYu.Editor/MapEditing/MapEditReason.cs
- XuanYu.Editor/MapEditing/MapEditSession.ObjectCommands.cs
- XuanYu.Editor/MapEditing/MapEditSession.RegionVertices.cs
- XuanYu.Editor/MapEditing/MapGeometryContextHit.cs
- XuanYu.Editor/MapEditing/MapGeometryContextHitTester.cs
- XuanYu.Editor/MapEditing/MapGeometryContextHitTester.Geometry.cs
- XuanYu.Editor/MapEditing/MapGeometryContextMenuSpec.cs
- XuanYu.Editor/MapEditing/MapObjectNameAllocator.cs
- XuanYu.Editor.UI/Input/UiVmMapBackend.cs
- XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.AvaloniaPointer.cs
- XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Pointer.cs
- XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.ContextMenu.cs
- XuanYu.Editor.UI/Vm/Map/UiVm.MapContextMenu.cs
- XuanYu.Editor.UI/Vm/Map/UiVm.MapMarkerPlacement.cs
- XuanYu.Editor.UI/Vm/Map/UiVm.RegionDrawing.Commit.cs
- XuanYu.Editor.UI/Vm/Map/UiVm.RoadDrawing.Commit.cs

## Tests

- XuanYu.World.Tests/MapEditing/MapGeometryContextHitTesterTests.cs
- XuanYu.World.Tests/MapEditing/MapGeometryContextMenuSpecTests.cs
- XuanYu.World.Tests/MapEditing/MapObjectNameAllocatorTests.cs
- XuanYu.World.Tests/MapEditing/MapEditSessionObjectCommandTests.cs
- XuanYu.World.Tests/MapEditing/MapEditSessionGeometryTests.cs

## Verification

- Solution build: 0 warnings / 0 errors.
- Focused geometry/context/name tests: passed.
- Full World.Tests: existing unrelated baseline failures remain; none were cleaned or suppressed.
