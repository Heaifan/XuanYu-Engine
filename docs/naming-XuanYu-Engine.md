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

## 代码命名空间迁移状态

以下为已完成的 R2 阶段迁移（2026-06-24）。

| 旧名（FluidWarfare） | 新名（XuanYu.Engine） | 阶段 | 状态 |
|---|---|---|---|
| `FluidWarfare.Core` | `XuanYu.Engine.Core` | R2 | ✅ 已迁移 |
| `FluidWarfare.Engine` | `XuanYu.Engine` | R2 | ✅ 已迁移 |
| `FluidWarfare.Editor` | `XuanYu.Engine.Editor` | R2 | ✅ 已迁移 |
| `FluidWarfare.Editor.Windows` | `XuanYu.Engine.Editor.Windows` | R2 | ✅ 已迁移 |
| `FluidWarfare.Project` | `XuanYu.Engine.Project` | R2 | ✅ 已迁移 |
| `FluidWarfare.Bridge.ProjectEngine` | `XuanYu.Engine.Bridge.ProjectEngine` | R2 | ✅ 已迁移 |
| `FluidWarfare.Render` | `XuanYu.Engine.Render` | R2 | ✅ 已迁移 |
| `FluidWarfare.Render.Vulkan` | `XuanYu.Engine.Render.Vulkan` | R2 | ✅ 已迁移 |
| `FluidWarfare.Tests` | `XuanYu.Engine.Tests` | R2 | ✅ 已迁移 |
| `FluidWarfare.sln` | `XuanYu.Engine.sln` | R2 | ✅ 已迁移 |

以下为未来 R3 阶段目标（**当前阶段不修改**）。

| 当前（仍保留） | 未来目标 | 阶段 |
|---|---|---|
| `namespace FluidWarfare.*` | `namespace XuanYu.Engine.*` | R3 |
| `x:Class="FluidWarfare.*"` | `x:Class="XuanYu.Engine.*"` | R3 |
| `AboutFluidWarfareWindow` 等类型名 | `AboutXuanYuEngineWindow` 等 | R3 |
| `using FluidWarfare.*` | `using XuanYu.Engine.*` | R3 |

---

## 迁移路线

| 阶段 | 内容 | 时间 |
|------|------|------|
| ✅ **8.8-R0/R1** | 品牌层换名 | 已完成 ✅ |
| ✅ **8.8-R2** | 解决方案与项目名迁移 | 已完成 ✅ |
| ✅ **8.8-R3** | C# 命名空间迁移 | 已完成 ✅ 全仓 namespace FluidWarfare.* 清零 |

---

## 验收检查项

### R0/R1 — 品牌层 ✅
- [x] 窗口标题显示 "XuanYu Engine Editor"
- [x] About 窗口显示 "XuanYu Engine Editor"
- [x] README 使用玄域引擎 / XuanYu Engine
- [x] CHANGELOG 使用新品牌名
- [x] docs 中有命名迁移说明
- [x] FluidWarfare 仅以"历史代号"语境存在
- [x] namespace 未改动
- [x] .sln / .csproj 未改动（R0/R1 阶段）
- [x] build 0 error
- [x] test 通过

### R2 — 工程外壳迁移 ✅
- [x] `.sln` 文件已改名：`FluidWarfare.sln` → `XuanYu.Engine.sln`
- [x] 所有 9 个项目目录已改名
- [x] 所有 `.csproj` 文件已改名
- [x] 所有 `ProjectReference` 路径已更新
- [x] `InternalsVisibleTo` 已更新
- [x] `app.manifest` assemblyIdentity 已更新
- [x] 所有 `.gitkeep` 标注已更新
- [x] 测试路径常量已更新
- [x] PowerShell 脚本路径已更新
- [x] 文档路径引用已更新
- [x] `namespace FluidWarfare.*` 未改动（留 R3）
- [x] `using FluidWarfare.*` 未改动（留 R3）
- [x] `x:Class="FluidWarfare.*"` 未改动（留 R3）
- [x] `EditorSettingsPath.AppFolderName` 未改动（留 R4）
- [x] build 0 error
- [x] test 629/630（1 flaky pre-existing）
