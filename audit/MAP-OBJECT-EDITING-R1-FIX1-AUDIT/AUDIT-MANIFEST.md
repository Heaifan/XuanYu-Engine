# MAP-OBJECT-EDITING-R1-FIX1 Audit

本包对应 Context Menu Contract 修复，包含本轮修改的完整源码与测试文件。

## 修复范围

- XYUI ContextMenu 使用根组件渲染，支持明确 DIP 坐标、边缘滑移与翻转。
- Native / Avalonia 右键统一经过 Viewport Input Router。
- 区域 / 道路边命中保存线段投影点，添加顶点复用精确世界坐标。
- 面命中按区域图层 Order 选择视觉最上层对象。
- 菜单标题使用对象实际名称；道路边菜单移除冻结范围外的编辑项。

## 验证记录

- Solution Build: 0 warnings / 0 errors
- Map geometry and context menu tests: 7/7 passed
- XYUI context menu runtime tests: 8/8 passed
- Viewport input regression tests: 7/7 passed
- `git diff --check`: passed
- ARCH-A: baseline failure at `XuanYu.Editor.UI/Diagnostic/DiagnosticFloatingToolWindow.cs` (`SetWindowPos`)

## 文件清单

本目录中的源码文件均为完整文件副本，不是 diff 片段。
