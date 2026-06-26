# 顶部区域设计目标 — 9.1B

目标：固定顶部区域设计，只允许两行 Box，不做第三行。

---

## 顶部结构

```text
Top Area
├─ Main Command Box   (主命令栏)
└─ Editor Tool Box    (编辑工具栏)
```

禁止第三行出现。

## 第一行：主命令栏 Main Command Box

```text
[文件] [编辑] [视图] [窗口] [帮助] | [撤销] [重做] [保存]        [DemoWorld ▼] [布局 ▼] [重置布局]
```

| 中文 | English | 显示 | 说明 |
|---|---|---|---|
| 文件 | File | 显示 | 新建、打开、保存、退出 |
| 编辑 | Edit | 显示 | 撤销、重做、复制、粘贴、删除 |
| 视图 | View | 显示 | 显示/隐藏面板、网格、辅助线 |
| 窗口 | Window | 显示 | 面板、布局、重置窗口 |
| 帮助 | Help | 显示 | 文档、快捷键、关于 |
| 撤销 | Undo | 显示 | 放左侧，高频 |
| 重做 | Redo | 显示 | 和撤销成组 |
| 保存 | Save | 显示 | 保存当前工程/地图 |
| 项目/世界名 | Project Name | 显示 | 放右侧 |
| 布局菜单 | Layout Menu | 显示 | 保存布局、恢复布局 |
| 重置布局 | Reset Layout | 显示 | 一键恢复默认布局，必须显眼 |

暂不单独显示（放对应菜单内）：Save All、Settings、Lock Layout、Save Layout、Restore Layout。

## 第二行：编辑工具栏 Editor Tool Box

```text
[选择] [移动] [旋转] [缩放] | [世界/本地] | [吸附: 10 ▼] [网格]        [运行] [停止]
```

| 中文 | English | 显示 | 说明 |
|---|---|---|---|
| 选择 | Select | 显示 | 选择对象 |
| 移动 | Move | 显示 | Move Gizmo |
| 旋转 | Rotate | 显示 | 先显示，后续可接功能 |
| 缩放 | Scale | 显示 | 先显示，后续可接功能 |
| 世界/本地 | Global/Local | 显示 | Gizmo 坐标系切换 |
| 吸附 | Snap | 显示 | 数值可下拉 |
| 网格 | Grid | 显示 | 显示/隐藏地面网格 |
| 运行 | Play | 显示 | 运行模拟 |
| 停止 | Stop | 显示 | 停止模拟 |

暂不放顶部（后续进入 Viewport Overlay 或 View 菜单）：Perspective/Orthographic、Lit/Wireframe、Camera Speed、Axis Constraint、Build/Export、Diagnostics/Console Toggle。

## 布局原则

- 尺寸不写死：只允许默认尺寸和最小尺寸，不允许硬编码固定宽度。
- 可调节：后续支持面板折叠/展开。
- 可重置：Reset Layout 必须可见，能一键恢复默认顶栏与面板状态。
- Viewport 最大化优先：顶部占用最小必要高度，不挤占 Viewport 空间。

## 禁止项

- 顶部禁止出现第三行
- 顶部禁止出现 Diagnostics / Console
- 顶部禁止出现 Camera Speed / Perspective 切换
