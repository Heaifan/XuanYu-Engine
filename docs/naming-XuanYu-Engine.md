# 玄域引擎命名规范

创建时间：2026-06-24
最后更新：2026-06-24（8.8-R0/R1）

---

## 命名体系

| 层级 | 名称 | 用途 |
|------|------|------|
| 总品牌 | **玄域引擎 / XuanYu Engine** | 整个 3D 世界编辑、战略战术、可扩展模拟引擎体系 |
| 战争方向子标识 | **孙武引擎 / SunWu Engine** | 面向战争、战略、战役、RTS 的模块/发行标识 |
| 当前游戏 | **兵无常势** | 未来正式游戏名 |
| 旧项目名 | FluidWarfare | 历史开发代号，逐步废弃 |

---

## 使用规则

### 总品牌

- 正式中文名称：**玄域引擎**
- 正式英文名称：**XuanYu Engine**
- 在代码和标识符中使用 XuanYu（两个单词，无空格，驼峰大写首字母 X 和 Y）
- 用于：窗口标题、About 窗口、README、文档标题、对外发布、Vulkan 应用标识
- 代码命名空间目标：`XuanYu.Engine.*`

### 子标识

- 孙武引擎 / SunWu Engine 仅在战争战略方向独立发布或模块化时启用
- 当前编辑器核心仍归属 XuanYu.Engine 体系
- 不要在核心命名空间中提前引入 SunWu

### 游戏名

- 兵无常势是未来第一款正式游戏的名称
- 当前阶段不修改游戏相关的代码或资源

### 历史代号

- `FluidWarfare` 仅作为历史开发代号保留
- 在文档中以以下语境出现：
  - "formerly FluidWarfare"
  - "历史代号 FluidWarfare"
  - "FluidWarfare（已迁移为 XuanYu Engine）"
  - 纯代码引用（namespace、项目名、类名）等暂不能修改的位置
- 不得作为**当前正式品牌名**出现在用户可见标题或主说明中

---

## 代码命名空间未来目标

以下为未来 R2/R3 阶段目标，**当前阶段不修改代码**。

| 当前（FluidWarfare） | 未来（XuanYu.Engine） |
|---|---|
| `FluidWarfare.Core` | `XuanYu.Engine.Core` |
| `FluidWarfare.Editor` | `XuanYu.Engine.Editor` |
| `FluidWarfare.Editor.Windows` | `XuanYu.Engine.Editor.Windows` |
| `FluidWarfare.Project` | `XuanYu.Engine.Project` |
| `FluidWarfare.Render` | `XuanYu.Engine.Render` |
| `FluidWarfare.Render.Vulkan` | `XuanYu.Engine.Render.Vulkan` |
| `FluidWarfare.Tests` | `XuanYu.Engine.Tests` |
| `FluidWarfare.sln` | `XuanYu.Engine.sln` |

---

## 迁移路线

| 阶段 | 内容 | 时间 |
|------|------|------|
| **8.8-R0/R1** | 品牌层换名（当前阶段） | 改用户可见名称、文档、窗口标题；不改 namespace / .sln / .csproj |
| **8.8-R2** | 解决方案与项目名迁移 | 改 .sln / .csproj / AssemblyInfo / 输出名；保留 namespace 兼容 |
| **8.8-R3** | C# 命名空间迁移 | 分模块改 namespace；R3-1 Project → R3-2 Render → R3-3 Editor → R3-4 Tests → R3-5 文档收口 |

---

## 验收检查项

- [ ] 窗口标题显示 "XuanYu Engine Editor"
- [ ] About 窗口显示 "XuanYu Engine Editor"
- [ ] README 使用玄域引擎 / XuanYu Engine
- [ ] CHANGELOG 使用新品牌名
- [ ] docs 中有命名迁移说明
- [ ] FluidWarfare 仅以"历史代号"语境存在
- [ ] namespace 未改动
- [ ] .sln / .csproj 未改动
- [ ] build 0 error
- [ ] test 通过
