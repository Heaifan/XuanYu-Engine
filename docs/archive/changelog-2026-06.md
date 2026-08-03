# changelog 归档：2026-06
> 归档时间：2026-08（SHR-2026-08 月度治理归档）
> 条目范围：v0.1.1.5-rz（2026-06-01）～ v0.1.8.10-fix（2026-06-26）
> 内容按归档时原文迁移，未润色；索引见 changelog.md「历史归档索引」

## v0.1.8.10-fix — 9.0X Native Viewport 鼠标捕获生命周期审计与修复 (2026-06-26)
- 原历史编号：9.0D-R2E
- 收口所有 Win32 鼠标捕获到 `NativeViewportMouseCapture`，禁止其他模块直接调用 `SetCapture` / `ReleaseCapture`
- 修复 `WM_MBUTTONUP` 此前只清内部状态、不调用 `ReleaseCapture()` 导致 Native Viewport 继续吞鼠标消息的问题
- `Release(nint ownerHwnd, string reason)` 以 `GetCapture() == ownerHwnd` 作为是否真实释放的最终依据，不再依赖内部 `_captured` 标志
- 新增 `WM_CANCELMODE` 兜底释放路径
- `WM_KILLFOCUS` / `WM_DESTROY` / `Dispose` 均兜底释放或清理捕获状态
- `WM_CAPTURECHANGED` 只同步内部状态，新捕获窗口句柄从 `lParam` 读取，不递归调用 `ReleaseCapture()`
- 增加中文 probe log：`Debug.WriteLine` + `EditorProbe` 双写，记录捕获开始/释放/来源/按钮/hwnd/Win32 当前捕获/释放原因
- 新增审计文档 `docs/audit-NativeViewportMouseCapture-lifecycle-9.0X.md`
- 新增回归验证脚本 `tools/mouse_capture_lifecycle_verify.ps1`：中键旋转 + MoveGizmo 拖动自动验证
- 项目窗口标题改为动态读取程序集版本，build 后自动显示当前版本号
- Build: 0 Warning, 0 Error / Tests: 697/698 passed（1 个预存：中文排序依赖 locale）
- commits: `8d6e7fd` `a48ecfd`

## v0.1.8.9-fix — Gizmo 拖动 Preview 高频路径复审 (2026-06-25 22:56)
- 原历史编号：9.0D-R2D
- 修复 `TransformPreview` 帧完成后仍可能调用 Diagnostics refresh 的路径：Preview 回调改为只记录“跳过 Diagnostics 刷新”
- 补齐中文 probe log：PointerMoved、Gizmo hit/drag、Preview transform、RenderScene preview、Redraw、PickSnapshot、Dispatcher、Inspector、Diagnostics、WorldState、日志面板、WorldHierarchy
- 保留 DebugDock 轻量化结果：Diagnostics/Performance/RenderScene 页不复活，仅提供 no-op 兼容方法，避免重建重型 Avalonia UI
- 新增 `docs/gizmo_drag_audit_2026-06-25.md`：完整调用链、频率分级和日志结论
- 复现日志：`docs/gizmo_drag_audit_probe.log` 共 355 行；Preview 拖动中 UI/WorldState/Diagnostics/Inspector 均为 0 次，PickSnapshot 跳过 20 次

## v0.1.8.8-fix — Gizmo 拖动高频路径探针审计 (2026-06-25 21:41)
- 原历史编号：9.0D-R2C
- 目标：确认 Move Gizmo Preview 高频路径未触发 Inspector / Diagnostics / PickSnapshot / WorldState / Avalonia 重布局
- 在 PointerMoved → Gizmo Hit/Drag → Preview Transform → RenderScene 写入 → Redraw 请求全链路植入中文 probe log
- 探针字段：阶段名、耗时 ms、UI 刷新、WorldState 写入、Diagnostics 刷新、Inspector 刷新
- 审计结论：
  - Preview（TransformPreview）帧：UI=否 / WorldState=否 / Diagnostics=否 / Inspector=否 / PickSnapshot=跳过
  - Commit（EntityTransformChanged）帧：WorldState=是 / Inspector=是 / Diagnostics=是 / PickSnapshot=执行
  - PointerMoved 高频路径未触发 Inspector/Diagnostics/PickSnapshot/WorldState，符合 9.0D-R2B 优化目标
- 新增 `tools/gizmo_drag_postmessage.ps1`：通过 PostMessage 向 Vulkan 视口子窗口发送鼠标事件，复现拖动并采集探针日志
- 新增 `tools/gizmo_drag_audit.ps1`：SendInput 方案（未能穿透 WinExe 输入队列，保留参考）
- 新增 `artifacts/gizmo_drag_audit_probe.log`：本次审计原始探针日志（299 行）
- 新增 `XuanYu.Engine.Editor.Windows/Shell/Diagnostics/GizmoDrag/` 探针实现
- `EditorProbe` 同时输出到终端与 `%APPDATA%/XuanYuEngine/editor_probe.log`，便于 WinExe 审计采信
- Build: 0 Warning, 0 Error / Tests: 693/694 passed（1 个预存 flaky：中文排序依赖 locale）

## v0.1.8.7-fix — 降低 Move Gizmo 拖动帧负载 (2026-06-25 00:18)
- 原历史编号：9.0D-R2B
- TransformPreview 不再每帧刷新 Inspector
- TransformPreview 帧不再刷新 Diagnostics / DebugDock
- TransformPreview 帧不再重建 PickSnapshot
- AxisDragAnchorBuilder 删除未使用的 DragPlane 构建路径
- Inspector 更新保留在 TransformCommit / TransformCancel 路径
- Trace 审计确认未接入 UI 日志
- Build: 0 Warning, 0 Error / Tests: 693/694 passed
- commit `26f2006`

## v0.1.8.6-rz — 诊断日志与 UI 调度安全规范 (2026-06-24)
- 原历史编号：9.0D-R3
- 新增 `docs/diagnostic-safety.md`：收录 9.0D 诊断回调导致 UI 卡死事故的根因与防护规范
- 覆盖启动期规则 / 高频路径规则 / 诊断 Sink 接口 / UI 日志异步投递 / 代码审查清单
- commit `e57d5c9`

## v0.1.8.5-rz — 选中实体自动显示并可拖 Move Gizmo (2026-06-24)
- 原历史编号：9.0D-R2
- **取消「按 G 才显示 Gizmo」的交互入口**，改为选中实体 + 相机有效即自动显示
- 改动 4 个点：
  - `MoveGizmoFrameSource.Build`：闸门从 `MoveToolActive` 改为 `selectedEntity.IsValid || MoveToolActive`
  - `MoveGizmoVisibility.ShouldShow`：同步去掉 `moveToolActive` 参数
  - `EditorTransformInputRoute.HandlePointerMoved`：Gizmo Hover 检测改为选中实体即启用
  - `EditorSceneToolInputRoute.HandlePressed`：去掉 `IsMoveToolActive`，选中实体即可拖动
- G 键保留为快捷移动入口，不再是 Gizmo 出现的必要条件
- build: 0 Error / test: 693/694 (1 pre-existing)
- commit `e66cbb4`

## v0.1.8.4-rz — Move Gizmo 轴约束求解器 (2026-06-24)
- 原历史编号：9.0D-R1
- **AxisDragAnchorBuilder 重写**：从 ScreenProjection 升级为 DragPlane 射线约束方案
- **Gram-Schmidt 法线构造**：轴约束平面包含目标轴并尽量面向摄像机，三级 fallback
- **双保险**：DragPlane 优先 + ScreenProjection 降级，Builder 必定返回有效锚点
- **TransformDragRoute 重构**：Move 方法提取公共 MoveDrag 辅助，Axis/Plane 共用
- **诊断追踪**：TransformDragRoute 添加 Trace 回调，输出 Begin/Move/Confirm 位置值与模式
- **死代码清理**：删除 AxisTranslationStart.cs（旧 ScreenProjection 实现）
- **新增测试**：AxisPlaneTranslationSolverTests（X/Y/Z 轴正交性 / 45°视角无倍率异常 / 射线平行失败路径 / Gram-Schmidt 数学正确性）
- 后续修复 4 轮（提交 7c88740 → ba85b92 → 4ac549e → c82bb41）
- build: 0 Error / test: 693/694 (1 pre-existing)
- 当前 Gizmo 可见时直接拖轴即可移动，不必按 G

## v0.1.8.3-rz — Inspector 与 Transform 同步 (2026-06-24)
- 原历史编号：9.0C
- WorldState 新增 `SetRotation()` / `SetScale()`
- 新增 TransformEdit 应用层（TransformInspectorSnapshot / TransformEditRequest / TransformEditResult / SelectedEntityTransformReader / SelectedEntityTransformApply）
- Inspector 支持显示并编辑 Position / RotationDegrees / Scale
- Apply 层统一校验 Scale > 0 / NaN / Infinity，非法值拒绝写入
- 旧 Position 编辑链路保持向后兼容
- Transform 修改后标记 Dirty + 请求视口刷新
- 预留 `SetSnapshot()` 入口供后续 Gizmo 同步 Inspector 使用
- 审计文档 `docs/audit-inspector-transform-9.0C-0.md`
- 新增测试 15 项（Reader 3 + Apply 11 + SaveLoad 1）
- build: 0 Error / test: 685/686 (1 pre-existing)
- commits: `69f056b` `d1cc14c` `d4cb881` `8fb4aaa`

## v0.1.8.2-rz — TransformComponent 补全：Position + Rotation + Scale (2026-06-24)
- 原历史编号：9.0B
- 新增 RotationComponent / ScaleComponent（Engine 层实体组件）
- TransformComponentDocument 增加 RotationDegrees / Scale（可空，兼容旧文件）
- WorldState 支持旋转/缩放存储与查询
- WorldDocumentValidator 增加 RotationDegrees 有限校验 / Scale 有限+正数校验
- 旧版只有 Position 的 world 文件兼容加载（缺 Rotation→补 0,0,0；缺 Scale→补 1,1,1）
- WorldStateDocumentConvert 单向/双向转换同步支持完整 Transform
- 新增/更新测试 20 项（Writer/Reader/Validator/RoundTrip）
- build: 0 Error / test: 670/671 (1 pre-existing)
- commits: `80230a2` `222f49a` `2043f4e` `39dc201`

## v0.1.8.1-rz — World 保存 / 加载 (2026-06-24)
- 原历史编号：9.0A
- 新增 WorldDocument / WorldEntityDocument / TransformComponentDocument / WorldVector3Document / WorldMetadataDocument 文档模型
- 新增 WorldDocumentReader / WorldDocumentWriter 支持 .world.json 读写
- 新增 WorldDocumentValidator（SchemaVersion / WorldId / EntityId / Transform Position 校验）
- 编辑器打开项目时自动加载 Content/Worlds/main.world.json
- 运行菜单新增「保存 World」入口
- 新增 WorldState ↔ WorldDocument 转换
- 新增保存 / 读取 / 校验 / RoundTrip 测试 23 项
- build: 0 Error / test: 661/662 (1 pre-existing, SampleProjectSmokeTests 受未跟踪 Content/ 目录影响)
- commits: `8bd920b` `825f3b3` `70099ce` `8f3d9a1`
- 新增 4 项命名回潮门禁测试（CodeFileBudgetTests +4 → 14 项）：
  - `NoNamespaceFluidWarfare` — 禁止生产代码出现 namespace FluidWarfare
  - `NoUsingFluidWarfare` — 禁止生产代码出现 using FluidWarfare
  - `NoXClassFluidWarfare` — 禁止 .axaml 出现 x:Class="FluidWarfare.*
  - `NoClrNamespaceFluidWarfare` — 禁止 .axaml 出现 clr-namespace:FluidWarfare.*
- 允许的例外：EditorSettingsPath.LegacyAppFolder / EditorSettingsPathMigration / 历史文档
- 确认最终残留仅限：R4 Legacy 迁移路径 / 历史记录 / LEGACY 文档 / naming 说明
- docs/naming-XuanYu-Engine.md 标记 RZ 完成
- build: 0 Error / test: 638/639 (1 flaky pre-existing)
- 架构门禁：14/14
- commit `fbf509b`

## v0.1.7.1-fix — Editor 启动 AccessViolation 修复 (2026-06-24 11:45)
- 原历史编号：8.8-RZ-Fix1
- **根因**：`EditorShellComposition.Build()` 中初始化顺序错误 — `ProjectBootstrapRoute` 在第 41 行创建时引用了 `ctx.HierarchyRoute`，但 `HierarchyRoute` 直到第 45 行才赋值，导致 `hierarchyRoute` 为 null
- **现象**：Editor 启动崩溃，退出码 -1073741819（0xC0000005），实际为 NullReferenceException
- **修复**：将 `ctx.HierarchyRoute = new(...)` 移到 `ctx.ProjectBootstrapRoute = new(...)` 之前
- **验收**：
  - build: 0 Error / 0 Warning ✅
  - test: 638/639 通过（1 个预存 flaky：中文排序依赖 locale）
  - Editor 启动：成功，不再崩溃
  - 架构门禁：14/14
- 附带修复：run.bat CRLF 行尾（Windows 批处理兼容性）
- commit `359e3ce`

## v0.1.7.4-rz — 应用图标入库 (2026-06-24 12:30)
- 原历史编号：8.8-RZ-Fix1d
- 将仓库根目录 `LOGO.png`（1254×1254）复制到 `Assets/Icons/logo.png` 作为应用图标
- `.csproj` 注册 `logo.png` 为 `AvaloniaResource`（同时补注新的 ViewportNavigation SVGs）
- `MainWindow.axaml` 设置 `Icon="/Assets/Icons/logo.png"`，标题栏显示玄域引擎 LOGO
- `file-tree.md` 同步记录
- build: 0 Error / 0 Warning ✅ / test: 638/639（1 flaky pre-existing）

## v0.1.7.3-rz — 视口导航按钮 SVG 图标资源入库 (2026-06-24 12:16)
- 原历史编号：8.8-RZ-Fix1c
- 新增 4 个 SVG 图标资源到 `Assets/Icons/ViewportNavigation/`：
  - `nav_pan.svg` — 四向箭头，表示平移视图
  - `nav_frame.svg` — 取景框角 + 中心点，表示聚焦/查看全部
  - `nav_projection_persp.svg` — 视锥图形，表示透视投影
  - `nav_projection_ortho.svg` — 网格方框，表示正交投影
- 所有 SVG 使用 `viewBox="0 0 30 30"` + `currentColor`，匹配按钮尺寸且支持主题色
- `file-tree.md` 同步记录新资源
- 路线规划：短期为资源预案，后续接 Avalonia Overlay 或 Vulkan 贴图渲染路径
- build: 0 Error / 0 Warning ✅ / test: 638/639（1 flaky pre-existing）/ 架构门禁 14/14

## v0.1.7.2-fix — Warning 全清理 (2026-06-24 12:05)
- 原历史编号：8.8-RZ-Fix1b
- **7 个 Warning 逐项处理**：
  - `VulkanScene3dFrameHandles.cs` — 去重 `using Silk.NET.Vulkan`
  - `VulkanScene3dSwapchainCreateResult.cs` — `Message` 改为 `string?`
  - `VulkanScene3dRendererProbeFrame.cs` — `r.Vk` 增加 null 安全检查
  - `EditorViewportInputRequest.cs` — `ToolPalette` 改为 `ViewportToolPalette?`
  - `EditorTransformInputRequest.cs` — `ToolPalette` 改为 `ViewportToolPalette?`
  - `EditorShellGroundPointerRoute.cs` — suppress CS9113（API 设计预留）
  - `EditorPickInputRoute.cs` — `applySelection` 改为 `Action<string?,...>`
  - `StatusBarPanel.SetCurrentSelection` — 改为 `string?`，null→"无"
  - `VulkanScene3dFrameResult.Failed` — 参数改为 `string?`
  - `VulkanScene3dSession.FailFrame` — 参数改为 `string?`
- **验收**：build 0 Error / 0 Warning ✅ / 架构门禁 14/14 ✅
- commit `e3f644f`

## v0.1.6.11-rz — 用户数据目录迁移 (2026-06-24 10:08)
- 原历史编号：8.8-R4
- 编辑器设置目录从 `%APPDATA%/FluidWarfare/` 迁移到 `%APPDATA%/XuanYuEngine/`
- 新增 `EditorSettingsPathMigration.cs`：旧→新目录迁移逻辑（幂等、不覆盖、不崩溃）
- `EditorSettingsPath.cs`：新增 `CurrentAppFolder = "XuanYuEngine"` / `LegacyAppFolder = "FluidWarfare"`
- `EditorSettingsPath.cs`：GetSettingsFilePath 首次调用时触发迁移
- 迁移策略：新目录存在→跳过 / 仅旧目录存在→复制 / 新旧都不存在→默认
- 不删除旧目录 / 不覆盖新目录已有文件 / 迁移失败不阻止编辑器启动
- 对应测试 5 项（EditorSettingsPathMigrationTests.cs）
- 生产 `Input/Settings/` 目录 5 文件（合规）
- build: 0 Error / test: 634/635 (1 flaky pre-existing)
- 架构门禁：10/10
- commit `644aff7`

## v0.1.6.10-rz — namespace 迁移全仓收口 (2026-06-24 09:54)
- 原历史编号：8.8-R3-Z
- 全仓 namespace FluidWarfare.* 清零确认 ✅
- AboutFluidWarfareWindow → AboutXuanYuEngineWindow（类名 + 文件名 + x:Class + 全部引用）
- 清理 14 处非 namespace 的 FluidWarfare 字符串（Vulkan 窗口标题 / Win32 类名 / 日志 / 测试路径等）
- 删除 docs/reports/namespace-migration-R3-plan.md（生命周期完成）
- 更新 docs/naming-XuanYu-Engine.md R3 状态、file-tree.md
- 残留说明：EditorSettingsPath.AppFolderName = "FluidWarfare" 保留到 R4
- build: 0 Error / test: 629/630 (1 flaky)
- commit `710dd88`

## v0.1.6.9-rz — Tests namespace 迁移 (2026-06-24 09:48)
- 原历史编号：8.8-R3-4
- 迁移 namespace `FluidWarfare.Tests.*` → `XuanYu.Engine.Tests.*`（73 文件）
- 全仓 namespace `FluidWarfare.*` 清零 ✅
- 剩余：EditorSettingsPath（R4）/ AboutFluidWarfareWindow（R3-Z）/ 历史记录
- build: 0 Error / test: 629/630
- commit `5c8966b`

## v0.1.6.8-rz — Editor.Windows 全仓 namespace + x:Class 成对迁移 (2026-06-24 09:42)
- 原历史编号：8.8-R3-3BC
- 合并 R3-3B + R3-3C 为原子提交（partial class 必须同 namespace）
- 244 纯 C# + 16 .axaml.cs + 16 .axaml x:Class + 7 clr-namespace
- GlobalUsings.cs: 43 条 Editor.Windows 全局 using（100 行门禁）
- 清零：namespace/x:Class/clr-namespace FluidWarfare.Editor.Windows 全部 ✅
- build: 0 Error / test: 629/630
- commit `775ba48`

## v0.1.6.7-rz — Render 层 namespace 迁移 (2026-06-24 09:10)
- 原历史编号：8.8-R3-2
- 迁移 Render/Render.Vulkan namespace：`FluidWarfare.Render.*` → `XuanYu.Engine.Render.*`
- Render：47 文件 namespace + 147 文件跨项目 using；Render.Vulkan：154 文件 namespace
- 修复 1 处完全限定类型引用；相机白名单文件 namespace 正确迁移
- Editor/Tests namespace 保持不动（R3-3/R3-4）
- x:Class/EditorSettingsPath 未改动
- build: 0 Error / test: 629/630 (1 flaky)
- commit `aa94a43`

## v0.1.6.6-rz — 底层模块 namespace 迁移 (2026-06-24)
- 原历史编号：8.8-R3-1
- 迁移 Core/Engine/Project/Bridge namespace：`FluidWarfare.*` → `XuanYu.Engine.*`
- 模块内 namespace 声明：36 文件；全仓 using 引用：209 文件；总计 185 文件改动
- 命名映射：`FluidWarfare.Core→XuanYu.Engine.Core`, `FluidWarfare.Engine→XuanYu.Engine`（注意无 `.Engine` 后缀）, `FluidWarfare.Project→XuanYu.Engine.Project`, `FluidWarfare.Bridge.ProjectEngine→XuanYu.Engine.Bridge.ProjectEngine`
- Render/Editor/Tests namespace 保持不动（R3-2/R3-3/R3-4）
- x:Class/EditorSettingsPath 未改动
- build: 0 Error / test: 629/630 (1 flaky)
- commit `6a90c9e`

## v0.1.6.5-rz — docs audit 文件清理 (2026-06-24)
- 原历史编号：8.8-R2C
- 删除 14 个临时 audit-* / whitelist-* / renderer-* 文件
- 旧 `docs/CHANGELOG.md`（179KB，表格密集）→ `changelog.md`（简洁格式，倒序）
- `file-tree.md` 中 31KB 的"未发布变更日志"区 → 指向 `changelog.md` 的引用
- build: 0 Error / test: 629/630 (1 flaky)
- commit `68ffde8`

## v0.1.6.4-rz — 旧占位目录清理 (2026-06-24)
- 原历史编号：8.8-R2B
- 删除 9 个仅含 `.gitkeep` 的空占位目录：`FluidWarfare.AI` / `Combat` / `Data` / `Ecs` / `Exporter` / `Runtime.Android` / `Runtime.Windows` / `Simulation` / `World`
- 删除审计确认：9 个文件全部为 `.gitkeep`，无误伤
- 未来需要时按命名规范重新声明（`XuanYu.Engine.*` / `XuanYu.SunWu.*` / `XuanYu.Tools.*`）
- build: 0 Error / test: 629/630 (1 flaky)
- commit `5bdda34`

## v0.1.6.3-rz — 工程外壳迁移 (2026-06-24)
- 原历史编号：8.8-R2
- `.sln` / 9 项目目录 / `.csproj` / `ProjectReference` 全部迁至 `XuanYu.Engine.*`
- 映射：`FluidWarfare.Core→XuanYu.Engine.Core`, `FluidWarfare.Engine→XuanYu.Engine`（无后缀）, `FluidWarfare.Editor.Windows→XuanYu.Engine.Editor.Windows`, 等
- 同步更新：`InternalsVisibleTo` / `app.manifest` / 测试路径常量 / PowerShell 脚本 / `.gitkeep`
- 故意保留：`namespace FluidWarfare.*`（R3）, `using FluidWarfare.*`（R3）, `x:Class`（R3）, `EditorSettingsPath.AppFolderName`（R4）
- build: 0 Error / test: 629/630 (1 flaky)
- commit `6ad57bd`

## v0.1.6.2-rz — 品牌换名：玄域引擎 (2026-06-24)
- 原历史编号：8.8-R0/R1
- 正式技术品牌从 FluidWarfare 迁移为"玄域引擎 / XuanYu Engine"
- 窗口标题：`FluidWarfare Editor` → `XuanYu Engine Editor`
- About 窗口：品牌名 / 版权 → XuanYu Engine / 玄域引擎贡献者
- 菜单：`关于 FluidWarfare` → `关于 玄域引擎`
- 示例项目描述：`FluidWarfare 示例项目` → `玄域引擎 示例项目`
- Vulkan app/engine name：8 文件 → `"XuanYu Engine"`
- 文档标题：CHANGELOG / AI 规则 / 代码宪法 / 命名规则 / file-tree / shaders
- namespace / .sln / .csproj / 程序集名 / 目录名未改动
- build: 0 Error / test: 629/630 (1 flaky)
- commit `71d6187`

## v0.1.6.1-rz — 架构防回潮门禁 (2026-06-24)
- 原历史编号：8.8-0
- `CodeFileBudgetTests.cs` 新增 5 个门禁测试：
  - `ProductionWhitelist_OnlyApproved` — 生产白名单精确锁死为 2 个相机文件
  - `GlobalUsings_Max100Lines` — `GlobalUsings.cs` ≤ 100 行
  - `EditorShellContext_Max95Lines` — `EditorShellContext.cs` ≤ 95 行
  - `EditorShell_NotInWhitelist` — EditorShell 不得回归白名单
  - `DirectoryWhitelist_RemainsZero` — 目录白名单保持 0
- build: 0 Error / test: 629/630 (1 flaky)
- commit `4c4d82c`

## v0.1.5.21-rz — EditorShell 组合根彻底薄化 (2026-06-23)
- 原历史编号：8.7.8-Z2
- `EditorShell.axaml.cs`：3,041→28 行，**从白名单移除**
- 95 个 using 移入 `GlobalUsings.cs`
- 新建 Composition 架构：
  - `EditorShellContext.cs` (88 行) — 上下文持有
  - `EditorShellComposition.cs` (59 行) — Build
  - `EditorShellCompositionRuntime.cs` (65 行) — 运行时
  - `EditorShellEventWiring.cs` (67 行) — 事件接线
  - `EditorShellLifecycle.cs` (29 行) — 生命周期
- 生产白名单：3→2（只剩两个相机算法文件）
- build: 0 Error / test: 624/625 (1 flaky)
- commit `913b66b`

## v0.1.5.20-rz — EditorShell 收口审计 (2026-06-23)
- 原历史编号：8.7.8H-5
- EditorShell 从 3,041 行压到 491 行（含 using，body ~396 行），累计削减 2,550 行
- 决策：Transform 管线暂缓（收益 ~30 行，风险影响全链路）
- 决策：EditorShell 白名单保留（组合根例外）
- 后续策略：只出不进，新增职责必须进 Route / 子模块
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.19-rz — EditorShell P2 中等风险清理 (2026-06-23)
- 原历史编号：8.7.8H-4B
- 提取日志委托 → `Shell/Diagnostics/Log/EditorShellLogRoute.cs` (18 行)
- 提取视口焦点 → `Shell/Viewport/EditorShellViewportFocusRoute.cs` (41 行)
- 提取 Scene3D 命令 → `Shell/Scene3D/EditorShellScene3dCommandRoute.cs` (19 行)
- EditorShell: 496→491 行
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.18-rz — EditorShell P1 低风险清理 (2026-06-23)
- 原历史编号：8.7.8H-4A
- 提取 Raw 输入处理 → `Shell/Input/Raw/EditorShellRawInputRoute.cs` (26 行)
- 提取视口帧命令 → `Shell/Viewport/EditorShellViewportFrameRoute.cs` (43 行)
- 提取视口尺寸工具 → `Shell/Viewport/EditorShellViewportSizeGuard.cs` (24 行)
- 删除空 `ExecuteTransformApply`（无调用者）
- EditorShell: 656→496 行（含 using，body ~403 行）
- build: 0 Error / test: 624/625 (1 flaky，白名单不删)

## v0.1.5.17-rz — EditorShell 第七刀：项目加载 + World Bootstrap (2026-06-23)
- 原历史编号：8.7.8H-2G
- 提取项目加载残留 → `Shell/Project/EditorShellProjectBootstrapRoute.cs` (46 行)
- EditorShell: 576→567 行
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.16-rz — EditorShell 第六刀：Startup Vulkan Probe (2026-06-23)
- 原历史编号：8.7.8H-2F
- 提取 Startup Vulkan Probe → `Shell/Startup/EditorShellStartupVulkanProbeRoute.cs` (46 行)
- EditorShell: 589→576 行
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.15-rz — EditorShell 第五刀：层级树 + 选择同步 (2026-06-23)
- 原历史编号：8.7.8H-2E
- 提取层级树 → `Shell/Hierarchy/EditorShellHierarchyRoute.cs` (37 行)
- 提取选择同步 → `Shell/Selection/EditorShellSelectionSyncRoute.cs` (51 行)
- EditorShell: 622→589 行
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.14-rz — EditorShell 第四刀：窗口菜单命令 (2026-06-23)
- 原历史编号：8.7.8H-2D
- 提取窗口命令 → `Shell/Commands/EditorShellWindowCommandsRoute.cs` (24 行)
- EditorShell: 629→622 行
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.13-rz — EditorShell 第三刀：Viewport 生命周期 + Vulkan Redraw (2026-06-23)
- 原历史编号：8.7.8H-2C
- 提取 Viewport 重绘 → `Shell/Viewport/EditorShellViewportRedrawRoute.cs` (83 行)
- EditorShell: 665→629 行
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.12-rz — EditorShell 第二刀：Transform 编辑 + Scrub (2026-06-23)
- 原历史编号：8.7.8H-2B
- 提取 Transform 路由 → `Shell/Transform/EditorShellTransformRoute.cs` (62 行)
- 提取 Scrub → `Shell/Transform/EditorShellScrubRoute.cs` (24 行)
- EditorShell: 725→665 行
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.11-rz — EditorShell 第一刀：Overlay 导航 + 地面指针 + Picking (2026-06-23)
- 原历史编号：8.7.8H-2A
- 提取 Overlay 导航 → `Shell/Navigation/EditorShellOverlayNavigationRoute.cs` (78 行)
- 提取地面指针 → `Shell/Picking/EditorShellGroundPointerRoute.cs` (63 行)
- 提取 Picking → `Shell/Input/Picking/EditorPickInputRoute.cs` (79 行)
- EditorShell: 969→725 行
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.10-rz — EditorPreferencesWindow SRP 拆分 (2026-06-23)
- 原历史编号：8.7.8G-2
- `EditorPreferencesWindow.axaml.cs`：587→78 行
- 提取 Capture 逻辑 → `EditorPreferencesCapture.cs` (77 行)
- 提取 BindingList 管理 → `EditorPreferencesBindingList.cs` (81 行)
- 提取 DraftHandler → `EditorPreferencesDraftHandler.cs` (79 行)
- 提取 Helpers → `EditorPreferencesHelpers.cs` (30 行)
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.9-rz — VulkanRenderContext SRP 拆分 (2026-06-23)
- 原历史编号：8.7.8F-2
- `VulkanRenderContext.cs`：476→92 行
- 提取 Context Setup → `Context/VulkanRenderContextSetup.cs` (78 行)
- 提取 Device Selector → `Context/VulkanRenderContextSelector.cs` (32 行)
- 死代码锁定 Legacy
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.8-rz — VulkanClearProbe SRP 拆分 (2026-06-23)
- 原历史编号：8.7.8E-2B
- `VulkanClearProbe.cs`：416→99 行
- 提取 ContextScope → `Clear/Probe/VulkanClearProbeContextScope.cs` (96 行)
- 提取 DeviceSelector → `Clear/Probe/VulkanClearProbeDeviceSelector.cs` (42 行)
- 提取 SurfaceQuery → `Clear/Probe/VulkanClearProbeSurfaceQuery.cs` (60 行)
- 提取 RenderTargetScope → `Clear/Probe/Render/VulkanClearProbeRenderTargetScope.cs` (98 行)
- 提取 RenderSubmitScope → `Clear/Probe/Render/VulkanClearProbeRenderSubmitScope.cs` (54 行)
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.7-rz — Clear 目录容量整理 (2026-06-23)
- 原历史编号：8.7.8E-2A
- `Clear/Probe/` 目录 9→6 文件（容量达标）
- build: 0 Error / test: 624/625

## v0.1.5.6-rz — VulkanSwapchainProbe SRP 拆分 (2026-06-23)
- 原历史编号：8.7.8D-2B
- `VulkanSwapchainProbe.cs`：301→78 行
- 提取 ContextScope → `Swapchain/Probe/VulkanSwapchainProbeContextScope.cs` (100 行)
- 提取 DeviceSelector → `Swapchain/Probe/VulkanSwapchainProbeDeviceSelector.cs` (46 行)
- 提取 SurfaceQuery → `Swapchain/Probe/VulkanSwapchainProbeSurfaceQuery.cs` (64 行)
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.5-rz — Swapchain 目录容量整理 (2026-06-23)
- 原历史编号：8.7.8D-2A
- `Swapchain/` 子目录重建：Probe/ / Context/ / Image/ / Sync/
- 文件迁移确保 ≤5/目录
- build: 0 Error / test: 624/625

## v0.1.5.4-rz — GameProjectLoader SRP 拆分 (2026-06-23)
- 原历史编号：8.7.8C-2
- `GameProjectLoader.cs`：392→82 行
- 提取 ManifestReader → `Loading/GameProjectManifestReader.cs` (89 行)
- 提取 FolderParser → `Loading/GameProjectFolderParser.cs` (100 行)
- 提取 ExtensionParser → `Loading/GameProjectExtensionParser.cs` (52 行)
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.3-rz — VulkanDeviceProbe SRP 拆分 (2026-06-23)
- 原历史编号：8.7.8B-4
- `VulkanDeviceProbe.cs`：288→77 行
- 提取 InstanceScope → `Device/VulkanDeviceInstanceScope.cs` (61 行)
- 提取 Selector → `Device/VulkanDeviceSelector.cs` (80 行)
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.2-rz — VulkanSurfaceProbe SRP 拆分 (2026-06-23)
- 原历史编号：8.7.8B-2
- `VulkanSurfaceProbe.cs`：203→66 行
- 提取 InstanceScope → `Surface/VulkanSurfaceInstanceScope.cs` (98 行)
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.5.1-rz — WindowsViewportInputTranslator SRP 拆分 (2026-06-23)
- 原历史编号：8.7.8A-2
- `WindowsViewportInputTranslator.cs`：284→54 行
- 拆为：`WindowsViewportModifierState.cs` (37) / `WindowsViewportRawInputTranslate.cs` (76) / `WindowsViewportGestureMatch.cs` (28)
- 白名单：1 项删除
- build: 0 Error / test: 624/625 (1 flaky)

## v0.1.4.11-rz — 全仓白名单债务审计与 8.7.8 路线图 (2026-06-23)
- 原历史编号：8.7.7F
- F-1：全仓盘点，49 项行白名单 + 8 项目录白名单
- F-2：3 项立即可清（InspectorPanel 84 行 / NativeHost 87 行 / SceneCameraPose 99 行）
- F-3：9 文件压缩 + 4 目录子目录化，13 项白名单删除
  - 文件压缩：WorldHierarchyTreeIndex (112→46) / ProjectContentNodeView (114→52) / WorldHierarchyTreeBuilder (126→49) / EditorInputActionCatalog (148→60) / VulkanInstanceProbe (122→43) / VulkanDebugMessengerScope (133→55) / VulkanValidationAvailabilityProbe (118→40) / GameContentFileScanner (130→38) / WorldState (121→48)
  - 目录子目录化：Validation 7→5, Camera 7→5, ProjectContentTree 6→5, Transform/Drag 6→5
- F-4A：DebugDockPanel SRP 提取 (145→53) / ViewportPlaceholderPanel SRP 提取 (189→46) / WorldHierarchy 目录 8→5
- F-4B：Panels/Viewport 11→5 文件 / Transform/Gizmo 8→5 文件
- F-4C：ViewportNavigation 9→5 文件（最后 1 个目录白名单删除）/ VulkanSceneRayBuilder SRP 167→40 / VulkanCameraMatrices SRP 189→21
- F-5：大债务登记到 8.7.8（13 项生产白名单分 A/B/C/D 四类）
- F-6：最终收口 — VulkanViewportHostPanel 158→43 / EditorInputBindingSnapshot 175→38；SceneNavigationCameraMotion(173) 与 SceneOrbitCameraMotion(202) 因相机算法放弃
- build: 0 Error / test: 625/625

## v0.1.4.10-rz — 全仓白名单深度清理 (2026-06-22)
- 原历史编号：8.7.7E
- E-1：VulkanScene3dRenderer SRP — 主文件 261→41 行，5 子模块全部 ≤100
- E-2A：Scene3D Session SRP — Session 主文件 371→46，CreateInstance/FrameFlow/Handles/FrameAcquire/Lifecycle
- E-2B：Swapchain SRP — 去重合并后 6 文件，全 ≤100
- E-2C：VulkanScene3dRenderer 去重式 SRP — 消除 3 个重复文件
- E-2D：Scene3D 白名单删除 + Overlay 目录 8→4
- 最终 9 文件白名单删除，ViewportNavigation 目录白名单清理
- build: 0 Error / test: 625/625

## v0.1.4.9-rz — 目录子目录化 + 文件重组 (2026-06-22)
- 原历史编号：8.7.7D
- D-1：Shell/Scene3D/ 11→5 文件（Scene3dFrameState/Scene3dDrawListBuilder/Scene3dPresentedState 迁入子目录）
- D-2：Viewport/Picking/ 重构 + Viewport/Transform/ 子目录重组
- build: 0 Error / test: 625/625

## v0.1.4.8-rz — NativeHost / ViewportPlaceholder / DebugDock SRP (2026-06-22)
- 原历史编号：8.7.7C
- C-1：NativeHost.axaml.cs 158→43 行（HWND 生命周期提取 / HostInfo 提取 / Input 提取）
- C-2：ViewportPlaceholderPanel.axaml.cs 189→46 行
- C-3：DebugDockPanel.axaml.cs 145→53 行
- 白名单 -3
- build: 0 Error / test: 625/625

## v0.1.4.7-rz — Project / World Tree Panels SRP (2026-06-22)
- 原历史编号：8.7.7B
- `ProjectContentTreePanel.axaml.cs`：128→63 行
- `WorldHierarchyTreePanel.axaml.cs`：229→95 行
- 新建 WorldHierarchyTreeItems.cs(14) / TreeExpansion.cs(43) / TreeSelection.cs(87)
- 白名单 -2
- build: 0 Error / test: 625/625

## v0.1.4.6-rz — InspectorPanel SRP 拆分 (2026-06-22)
- 原历史编号：8.7.7A
- `InspectorPanel.axaml.cs`：145→53 行
- 提取 TransformHeader.cs(31) / EntityIdRow.cs(26) / GroupSeparator.cs(16)
- 白名单 -1
- build: 0 Error / test: 625/625

## v0.1.4.5-rz — EditorShell Route 化重构 Phase 3：Composition (2026-06-21 ~ 22)
- 原历史编号：8.7.6
- 8.7.6.8C — Startup Bootstrap / Lifecycle / Vulkan Probe Route 化
- 8.7.6.8D-1 — Input Pipeline / Raw Viewport Events 提取
- 8.7.6.8D-2 — Transform / SceneTool Input Bridge
- 8.7.6.8D-3 — Ground Hover / Pick Bridge
- 8.7.6.8D-4 — Scene3D Manual Run / Session Commands
- 8.7.6.8D-5 — Panel Operation Apply
- 8.7.6.8E-1 — Transform / Ground Placement Apply 收口
- 8.7.6.8E-2 — Diagnostics / Refresh / Probe Residual 收口
- 8.7.6.8E-3 — Constructor / FindControls / Route Wiring
- 8.7.6.8E-3R/4 — Composition Cleanup + Final Stabilization（EditorShell 3,041→2,157 行）
- build: 0 Error / test: 625/625

## v0.1.4.4-rz — EditorShell Route 化 Phase 2：Selection & Gizmo (2026-06-20 ~ 21)
- 原历史编号：8.7.5
- 8.7.5.1-3 — Scene3D Frame Route 提取（FrameRoute/FrameState/DrawListBuilder/PresentedState）
- 8.7.5.4 — Viewport Pointer Pick Route
- 8.7.5.5A-C — Transform Interaction（State/Result/PointerRoute/KeyboardRoute/StartRequest）
- 8.7.5.5D — Transform Application（Preview/Commit/Cancel/Result）
- 8.7.5.5E — Gizmo（MoveGizmoDrawList/Element/HitTest/Interaction/Layout/VisualState/Snapshot）
- 8.7.5.6A-C — Camera Route / Focus / Navigation
- 8.7.5.6D-E — Frame Submit / Session Lifecycle
- 8.7.5.6F-G — Diagnostics / Vulkan Probe
- 8.7.5.7A-C — Selection Presenter / Route / State
- 8.7.5.8A — Project Bootstrap Route
- 8.7.5.8B — World Bootstrap Route (EntitySeed/RenderSeed/Input/Result)
- EditorShell 3,041→1,200+ 行（Route 化 Phase 1 后继续拆解）

## v0.1.3.9-vk — Scene3D 渲染与选择系统独立 (2026-06-18 ~ 20)
- 原历史编号：8.7.4
- Scene3D 渲染模块独立化（Scene3dFrameRun/Scene3dSessionLifecycle 等）
- 选择系统 Route 化（EditorSelectionRoute/State/Request/Result/Reason）
- 选择呈现（ViewportSelectionPresenter/WorldEntitySelectionPresenter）
- Picking 管线独立（ViewportPointerPickRoute）
- 多对象绘制与 Depth Buffer

## v0.1.3.8-vk — Vulkan 管线稳定化与 Swapchain 重构 (2026-06-17 ~ 18)
- 原历史编号：8.7.3
- Swapchain API 结果加固与生命周期规则收口
- Vulkan Clear 与 Swapchain Probe 重构
- Surface/Device/Instance 创建链路稳定化

## v0.1.4.3-rz — Transform 编辑基础 (2026-06-16 ~ 17)
- 原历史编号：8.7.2
- 单实体 Transform 编辑与地面放置
- 3D 地面拾取、世界坐标反馈与落点标记
- Gizmo 基础呈现（MoveGizmo）

## v0.1.4.2-rz — 视口与输入系统 (2026-06-15 ~ 16)
- 原历史编号：8.7.1
- 默认 3D 主视口、俯视矩阵修复
- Windows 原生视口子窗口宿主完善
- 输入管线路由化（RawInput→Transform→SceneTool）

## v0.1.4.1-rz — Shell Route 化 Phase 1 (2026-06-14 ~ 15)
- 原历史编号：8.7.0
- EditorShell 从 ~3,041 行开始 Route 化重构
- 第一批 Route 提取：Startup、Lifecycle、Log、PanelSwitch
- Route 装配与组合根（EditorShellComposition）

## v0.1.3.7-vk — 3D 地面拾取与 World Hierarchy (2026-06-12 ~ 14)
- 原历史编号：8.6
- 3D 地面拾取、世界坐标反馈与落点标记
- World Hierarchy 节点树与编辑器选择收口
- SVG 经典资源管理器式双树菜单
- 左侧双树页签、项目文件树与中文界面收口

## v0.1.3.6-vk — World Hierarchy 与选择系统 (2026-06-11 ~ 12)
- 原历史编号：8.5
- World Hierarchy 节点树（WorldHierarchyNode/TreeBuilder/Search）
- 编辑器选择 Route 化
- 项目内容树面板拆分

## v0.1.3.5-vk — 3D Picking 与单位选择 (2026-06-10 ~ 11)
- 原历史编号：8.4
- 3D Picking 管线（ScenePointerPicker/SceneRayGroundIntersection）
- 单位选择与高亮
- Picking 与选择 Route 化

## v0.1.3.4-vk — 持久 Scene3D 渲染会话与 RTS 相机 (2026-06-09 ~ 10)
- 原历史编号：8.3
- 持久 Scene3D 渲染会话（Session/Surface/Swapchain/Lifecycle）
- RTS 相机基础控制（ViewportNavigation）
- Overlay 渲染

## v0.1.3.3-vk — 多对象 3D 绘制与 Depth Buffer (2026-06-08 ~ 09)
- 原历史编号：8.2
- 多对象 3D 绘制（顶点缓冲/索引缓冲）
- 基础 Depth Buffer
- Ground Cursor 绘制

## v0.1.3.2-vk — Vulkan 3D 基础管线 (2026-06-06 ~ 08)
- 原历史编号：8.1
- Vulkan 3D 基础管线（ShaderModules/PipelineLayout/Pipelines/CommandRecorder）
- Scene3D 隔离（手动触发，不与 Editor 自动绑定）
- SPIR-V 手写编码废弃 → 标准 glslangValidator 工具链
- Validation Layer 开关接入
- Scene3D Renderer SRP 拆分

## v0.1.3.1-vk — RenderScene GPU 点位绘制 (2026-06-05 ~ 06)
- 原历史编号：8.0
- RenderScene 单对象 GPU 点位绘制
- Vulkan 战场视口填充与重绘修复
- 多对象点位绘制

## v0.1.2.2-vk — Vulkan 最小可见渲染闭环 (2026-06-04 ~ 05)
- 原历史编号：7.8
- 最小可见渲染闭环（CreateInstance→CreateSurface→CreateDevice→CreateSwapchain→Render→Present→Cleanup）
- Swapchain 扩展加载修复
- 底部调试终端与主视口收束

## v0.1.2.1-vk — Vulkan 基础集成 (2026-06-02 ~ 04)
- 原历史编号：7.0~7.7
- Vulkan 最小清屏（Clear Probe）
- Vulkan Instance 最小创建与释放
- Vulkan Device 最小选择与释放
- Vulkan Surface 宿主边界
- Windows 原生视口子窗口宿主
- Vulkan Surface 创建成功回归

## v0.1.1.5-rz — RenderScene 抽象 (2026-06-01 ~ 02)
- 原历史编号：6.0~6.1
- RenderScene 最小抽象
- 视口 RenderScene 调试显示

