# 顶部 SVG 图标化 — 9.1C

目标：在 9.1B 顶部两行结构基础上，为主命令栏和编辑器工具栏加入统一风格的 SVG PathIcon，提升顶部区域正式编辑器质感。

---

## 一、原则

- 使用 Avalonia `PathIcon`，不引入外部 SVG 文件
- 图标数据集中管理，禁止散落在按钮里
- 图标风格统一：线性、简洁、16px、非 emoji
- 当前阶段按钮显示为“SVG 图标 + 中文文字”，不做纯图标模式
- 顶部仍保持两行，不新增第三行

---

## 二、Row 1 主命令栏图标清单

| Code Key | 图标语义 | 状态 |
|---|---|---|
| IconCommandUndo | 左弯箭头 | ✅ 已完成 |
| IconCommandRedo | 右弯箭头 | ✅ 已完成 |
| IconCommandSave | 软盘保存 | ✅ 已完成 |
| IconCommandResetLayout | 窗口布局 + 重置箭头 | ✅ 已完成 |

菜单项（文件、编辑、视图、窗口、设置、帮助）保持纯文字，不加图标。

---

## 三、Row 2 编辑器工具栏图标清单

| Code Key | 图标语义 | 状态 |
|---|---|---|
| IconToolSelect | 鼠标指针 | ✅ 已完成 |
| IconToolMove | 四向十字箭头 | ✅ 已完成 |
| IconToolRotate | 环形箭头 | ✅ 已完成 |
| IconToolScale | 方框 + 对角缩放箭头 | ✅ 已完成 |
| IconToolGlobalLocal | 三轴坐标 | ✅ 已完成 |
| IconToolSnap | 磁铁 | ✅ 已完成 |
| IconToolGrid | 3×3 网格 | ✅ 已完成 |
| IconCommandPlay | 三角播放 | ✅ 已完成 |
| IconCommandStop | 方块停止 | ✅ 已完成 |

---

## 四、PathIcon 使用规则

```xml
<PathIcon Width="16" Height="16" Data="{StaticResource IconToolSelect}" />
```

- 图标通过 `StaticResource` 引用集中管理的 `StreamGeometry`
- 显示尺寸统一 16×16
- 源数据按 16×16 坐标系设计
- 颜色继承父控件 `Foreground`

---

## 五、图标数据位置

| 文件 | 内容 | 行数 |
|---|---|---|
| `UI/Icons/CommandIconData.axaml` | 命令类图标（Undo/Redo/Save/ResetLayout） | 11 |
| `UI/Icons/ToolIconData.axaml` | 工具类图标（Select/Move/Rotate/Scale/Global/Snap/Grid/Play/Stop） | 21 |

---

## 六、按钮样式

| 文件 | 内容 | 行数 |
|---|---|---|
| `UI/Styles/TopAreaButtonStyles.axaml` | TopCommandButton / EditorToolButton / SimulationButton 样式统一定义 | 约 60 |

样式要点：

- 默认透明背景，hover 为 `#3A3F47`
- 当前工具高亮使用 `EditorToolButton.SelectedTool`，背景 `#2D4A6E`
- `SimulationButton` 默认透明，hover 时轻微绿色强调 `#3A6B4A`
- 无内联廉价强蓝/鲜艳绿色

---

## 七、禁用项

- ❌ 不使用 emoji
- ❌ 不引入外部图标文件
- ❌ 不做纯图标模式
- ❌ 不改自定义标题栏
- ❌ 不做完整 Docking/Floating
- ❌ 不恢复 stash
- ❌ 不继续扩展 ShellV2
