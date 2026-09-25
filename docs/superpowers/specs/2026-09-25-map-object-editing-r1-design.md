# MAP-OBJECT-EDITING-R1 设计规格

## 目标

完成地图编辑器最后一个功能收口：点、线、面可以通过视口右键进入合法对象命令；Region、Road、Marker 获得稳定默认名称并可在 Inspector 修改、撤销、保存和重载。完成后冻结地图编辑器，转入地形编辑器。

## 明确范围

### A：Geometry Context，必须完成

统一视口右键链路：

```text
Pointer Right
→ Input Router
→ Geometry Picker
→ Hit Result
→ Selection
→ Command Availability
→ XYUI Context Menu
→ Existing Edit Command
→ Undo / Redo
→ Save
```

命中优先级固定为：

```text
Vertex > Edge / Line > Face
```

Marker 作为独立点对象参与命中。右键未选中对象时，先更新 Selection，再弹出菜单，Inspector 必须同步。

菜单能力：

- Region 内部：编辑几何、删除区域。
- Region 边：添加顶点、删除区域。
- Region 顶点：删除顶点。
- Road 线段/内部：添加顶点、删除道路、编辑道路。
- Road 顶点：删除顶点。
- Marker：编辑、删除标记。

删除顶点必须先做合法性判断：Region 删除后至少保留合法多边形，Road 删除后至少保留合法线段。非法命令显示为禁用，不允许点击后才报错。

所有右键命令必须复用现有 MapEditSession / 命令 / Undo 链；Context Menu 不得直接修改模型。

### B0：Object Name，必须完成

- 新建 Region 默认命名为 `区域1`、`区域2`……
- 新建 Road 默认命名为 `道路1`、`道路2`……
- 新建 Marker 默认命名为 `标记1`、`标记2`……
- Inspector 可以修改对象名称。
- 名称修改进入现有 Undo/Redo 历史。
- Save / Restart / Reload 后名称保持。

## 明确延期

本轮禁止实现：

- Region 显示名称开关。
- Region 填充色、边界色。
- Region 内部 Label。
- Label Anchor / 视觉中心算法。
- Hover 色、选中色主题扩展。
- Boundary Trace、Shared Topology、地形编辑器功能。

## 技术约束

- 复用现有 Unified Pointer Model、Viewport Input Router、Map Geometry Picker、Selection、Inspector、MapEditSession 和 Undo/Redo。
- 右键菜单必须使用 XYUI 的 `XYContextMenu` / `XYMenuItem` 或项目现有等价 XYUI 控件，不直接引入普通 Avalonia 菜单替代品。
- 不重构 Viewport Router，不修改 Map Schema，不引入共享拓扑，不新增依赖。
- 生产 `.cs` / `.axaml` / `.js` 单文件遵守 5+100。
- 既有未提交的输入文件和旧审计目录不属于本任务，必须保留且不得夹带提交。

## 验收场景

- 空白处右键不弹对象菜单。
- Region 面、边、顶点右键命中正确，非法操作禁用。
- Road 线、顶点右键命中正确，添加/删除顶点可 Undo/Redo。
- Marker 右键可编辑和删除。
- 右键未选中对象先改变 Selection，Inspector 同步。
- Region / Road / Marker 自动命名，Inspector 改名可 Undo/Redo。
- Save / Restart / Reload 后名称不丢。
- 官方入口 `D:/MyDoc/project-vsCode/XuanyuEngine/run.bat` 完成最终人工验收。

## 完成定义

```text
MAP-OBJECT-EDITING-R1-A: PASS
MAP-OBJECT-NAME-R1: PASS
颜色/标签：延期，不计入本轮
地图编辑器 R1：FREEZE
```
