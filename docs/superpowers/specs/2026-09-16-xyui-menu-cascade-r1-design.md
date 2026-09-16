# XYUI-MENU-CASCADE-R1 — Fixed Root / Multi-Sibling Cascading Menu

## 状态

设计冻结，可按配套实施计划执行。适用范围仅为仓库内 XYUI 基础设施；本阶段禁止修改 `XuanYu.Editor.UI` 或任何 Engine 业务代码。

## 目标

为 XYUI 提供可复用的桌面级联菜单能力：一个根 `XYMenu` 可以拥有多个兄弟级联项；级联项支持 Hover 展开和 Click/Invoke 展开；同一父菜单下最多打开一个直接子菜单；根菜单和子菜单的视觉宿主关系稳定，不通过 Editor 手工替换 Popup 内容。

## 固定视觉宿主模型

```text
Root Menu Host
└─ Root XYMenu                         只渲染一次
   ├─ 点 ── SubMenu ── 地图标记
   ├─ 线 ── SubMenu ── 道路
   └─ 面 ── SubMenu ── 区域面
```

根宿主可以是一个外部 Popup，但 Popup 的 `Child` 在打开生命周期内固定为根 `XYMenu`。`XYSubMenu` 不得再次把 `ParentMenu` 加入自身视觉树；它只管理触发项、子菜单视觉和级联生命周期。

## API 责任

### `XYMenuItem`

- `SubMenu` 是正式父子关系；设置非空 `SubMenu` 必须同步 `HasSubMenu = true`。
- 有 `SubMenu` 的条目只负责打开/关闭子菜单，不执行普通叶子 Command。
- 无 `SubMenu` 的叶子条目按现有 `ICommand` 优先规则执行 Command，并触发正常关闭链。
- Hover `PointerEntered` 和 Click/Invoke 都必须进入同一套 SubMenu 开关逻辑。

### `XYSubMenu`

保留并实现以下关系和状态：

```text
ParentMenu
ChildMenu
Trigger
ParentSubMenu
OpenLeft
IsOpen
Open()
Close()
```

`ParentMenu` 只作为关系、事件和兄弟菜单组的来源，不得成为 `XYSubMenu` 的视觉子节点。

### `XYMenu`

- 一个 `XYMenu` 的直接子菜单同时最多打开一个。
- 打开某个直接子菜单时，关闭其他直接兄弟及其全部后代。
- `Close()` 必须递归关闭全部后代。
- `FromModels()` 必须构造可用的多兄弟树，不能只建立 `HasSubMenu` 标志。

### `XYSplitButton`

本 R1 不修改 `XYSplitButton` API。其 `MainCommand` 和 `MenuCommand` 保持独立；外部宿主负责把一个根 Popup 固定连接到根 `XYMenu`。

## 交互规则

- 默认子菜单向 Trigger 右侧展开。
- `OpenLeft = true` 时向左展开；R1 不新增复杂碰撞检测。
- Hover 进入带 SubMenu 的条目：打开其子菜单。
- Click/Invoke 带 SubMenu 的条目：打开其子菜单，不执行叶子业务命令。
- 从父项移动到子菜单的过渡不能因离开父项立即关闭。
- 切换到同级另一个 Trigger：旧分支全部关闭，新分支打开。
- 关闭一级：所有后代递归关闭。
- 关闭根：所有后代递归关闭。
- 叶子 Invoked：业务 Command 执行一次，并关闭当前菜单链。

## `FromModels` 约束

模型树必须支持如下结构并保持关系可观察：

```text
Root
├─ 点
│  └─ 地图标记
├─ 线
│  └─ 道路
└─ 面
   └─ 区域面
```

每个一级项都必须满足：`HasSubMenu == true`、`SubMenu != null`、`SubMenu.ParentMenu` 指向同一个 Root、`SubMenu.Trigger` 指向自身、叶子位于该 SubMenu 的 `ChildMenu` 中。

## 测试要求

XYUI 测试必须覆盖：

1. 一个 Root 同时拥有三个不同 SubMenu，Root 只有一个视觉父级。
2. Hover 线打开道路。
3. Click/Invoke 线打开道路。
4. Hover 面关闭道路并打开区域面。
5. Root Close 递归关闭全部后代。
6. 二级 Close 递归关闭三级及更深后代。
7. 叶子 Invoked 执行一次并关闭菜单链。
8. 任意 SubMenu 不得把 ParentMenu 重新加入自身视觉树。
9. `XYMenu.FromModels()` 生成的多兄弟树可交互。
10. OpenLeft 仅验证明确左右方向，不扩展碰撞引擎。

## 范围红线

本 R1 只允许修改：

```text
xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-02-Menu
xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-04-SubMenu
xyui/avalonia/tests/XYUI.Avalonia.Tests
xyui/avalonia/gallery/XYUI.Avalonia.Gallery
XYUI 对应 canonical/docs
```

禁止修改：

```text
XuanYu.Editor.UI
XuanYu.Editor
XuanYu.World.Tests 的 Engine 菜单接入测试
XYSplitButton API
全局 Token、键盘系统架构、无关菜单样式
```

## 门禁与停止条件

- 所有新增或修改的手写 `.cs` / `.axaml` / `.js` 文件 ≤100 行。
- `dotnet build` 与 `dotnet test` 必须串行执行。
- XYUI 相关专项测试、Gallery 构造测试、`git diff --check`、5+100 必须通过。
- 若固定 Root + 多兄弟 SubMenu 仍无法在现有 XYUI 视觉模型中成立，立即停止，不向 Engine 添加替代实现。
- XYUI 自动测试通过不等于 Engine 真机验收通过；Engine 恢复接入后仍需单独进行真实 UI 验收。
