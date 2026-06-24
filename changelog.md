# changelog

## [0.0.1-dev] - 2026-06-10 ~

### 初始阶段 — 骨架搭建与核心值对象
- 解决方案骨架、项目宪章、架构说明、代码宪法、命名规则
- `EntityId` / `TimeStep` / `SimulationTime` / `Vector3d` / `YawRotation` / `EngineError` / `EngineResult`
- 项目内容文件入口声明与扩展名校验、项目校验报告

### Milestone 5.x — World 实体与选择
- 最小 World 实体、项目内容→占位实体、实体列表面板、选择与视口联动

### Milestone 6.x — RenderScene 抽象
- RenderScene 最小抽象、视口调试显示

### Milestone 7.x — Vulkan 集成
- Vulkan 最小清屏、Instance/Device/Surface 创建、Win32 原生视口子窗口
- Swapchain 创建、最小可见渲染闭环、调试终端
- Vulkan 3D 基础管线、Scene3D 隔离与 SPIR-V 废弃

### Milestone 8.x — 3D 渲染管线
- RenderScene GPU 点位绘制、Vulkan 3D 管线、标准 Shader 编译链
- 多对象 3D 绘制、Depth Buffer、持久渲染会话、RTS 相机控制
- 地面拾取、单位选择、World Hierarchy 节点树
- Transform 编辑与地面放置、3D Picking

### 8.7.6.8 系列 — EditorShell Route 化重构
- Shell Route 化 Phase 3：Startup Bootstrap / Lifecycle / Vulkan Probe
- Input Pipeline / Raw Viewport Events / Transform Bridge
- Ground Hover / Pick / Scene3D Commands / Panel Operation Apply
- Composition Cleanup & Final Stabilization

## [8.7.7] — 白名单债务审计与 SRP 拆分

### 8.7.7A — InspectorPanel SRP 拆分
- `InspectorPanel.axaml.cs`: 145→53行，拆分 TransformHeader/EntityIdRow/GroupSeparator
- commit `c1fec95`

### 8.7.7B — Project / World Tree Panels SRP
- `ProjectContentTreePanel.axaml.cs`: 128→63行 + `WorldHierarchyTreePanel.axaml.cs`: 229→95行
- 白名单-2，commit `a3437a5`

### 8.7.7C — NativeHost / ViewportPlaceholder / DebugDock SRP
- `NativeHost.axaml.cs`: 158→43行、`ViewportPlaceholderPanel.axaml.cs`: 189→46行
- 白名单-3，commit `18a4663`

### 8.7.7D — 目录子目录化 + 11→5文件
- Panels/Viewport: 11→5文件、Transform/Gizmo: 8→5文件
- 白名单-2，commit `8a19867`

### 8.7.7E — ViewportNavigation 子目录化 + 最终目录白名单删除
- Panels/Viewport 目录白名单删除 ✅
- `VulkanSceneRayBuilder` SRP 167→40、`VulkanCameraMatrices` SRP 189→21
- build: 0 Error / test: 625/625

### 8.7.7F — 全仓白名单债务审计与 8.7.8 路线图
- 全仓盘点：49项白名单，13项生产+20项测试+16项目录
- 删除 vulkanViewportHostPanel(158→43) / EditorInputBindingSnapshot(175→38)
- SceneNavigationCameraMotion(173) / SceneOrbitCameraMotion(202) 因相机算法放弃
- 产出 `whitelist-debt-roadmap-8.7.8.md`

## [8.7.8] — 全仓 Boss 文件债务清算

### 8.7.8A — WindowsViewportInputTranslator 拆分 (284→54行)
- 门面+3子组件(Axis/Plane/Free)，全仓≤100硬线首战
- build: 0 Error / test: 625/625

### 8.7.8B — VulkanSurfaceProbe / VulkanDeviceProbe 拆分
- SurfaceProbe: 203→66行、DeviceProbe: 288→77行
- build: 0 Error / test: 625/625

### 8.7.8C — GameProjectLoader 拆分 (392→82行)
- ManifestReader(89) / FolderParser(100) / ExtensionParser(52)
- build: 0 Error / test: 625/625

### 8.7.8D — VulkanSwapchainProbe 拆分 (301→78行)
- ContextScope(100) / DeviceSelector(46) / SurfaceQuery(64)
- build: 0 Error / test: 625/625

### 8.7.8E — VulkanClearProbe 拆分 (416→99行)
- ContextScope(96) / DeviceSelector(42) / SurfaceQuery(60) / RenderTargetScope(98) / RenderSubmitScope(54)
- build: 0 Error / test: 625/625

### 8.7.8F — VulkanRenderContext 拆分 (476→92行)
- Setup(78) / Selector(32)；死代码锁定 Legacy
- build: 0 Error / test: 625/625

### 8.7.8G — EditorPreferencesWindow 拆分 (587→78行)
- Capture(77) / BindingList(81) / DraftHandler(79) + Helpers
- build: 0 Error / test: 625/625

### 8.7.8H — EditorShell 最终拆分 (969→491行)
- 7 刀提取：Overlay导航/Transform编辑/Viewport重绘/窗口命令/层级树选择/StartupVulkan/项目加载
- P2 清理：日志委托/视口焦点/Scene3D命令
- 281→128 新文件验证：全≤100行
- build: 0 Error / test: 624/625 (1 flaky: 中文排序)

### 8.7.8H-5 — EditorShell 收口审计
- EditorShell: 3,041→491行，累计削减2,550行
- 组合根例外保留

### 8.7.8-Z1 — 全仓最终验收
- 白名单债务总审计，EditorShell仍靠白名单续命

### 8.7.8-Z2 — EditorShell 组合根彻底薄化 (3,041→28行)
- **EditorShell 从白名单移除 ✅**
- 95个using移入GlobalUsings.cs；Composition架构: Context(88)/Composition(59)/Runtime(65)/Wiring(67)/Lifecycle(29)
- build: 0 Error / test: 624/625
- commit `913b66b`

## [8.8] — 架构清账与品牌迁移

### 8.8-0 — 架构防回潮门禁 (2026-06-24)
- CodeFileBudgetTests: +5测试 — ProductionWhitelist/GlobalUsings/EditorShellContext/EditorShellNotInWhitelist/DirWhitelistZero
- 锁死生产白名单为2个相机算法；GlobalUsings≤100行；EditorShellContext≤95行
- build: 0 Error / test: 629/630
- commit `4c4d82c`

### 8.8-R0/R1 — 品牌换名：玄域引擎 / XuanYu Engine (2026-06-24)
- 正式技术品牌从 FluidWarfare 迁移
- 窗口标题/About/菜单/示例项目/Vulkan标识/文档标题全部更新
- namespace/.sln/.csproj 未改动（按路线图留后续阶段）
- build: 0 Error / test: 629/630
- commit `71d6187`

### 8.8-R2 — 工程外壳迁移 (2026-06-24)
- .sln / 9项目目录 / .csproj / ProjectReference 全部迁至 XuanYu.Engine.\*
- namespace/using/x:Class 未改动（留R3）
- EditorSettingsPath.AppFolderName 未改动（留R4）
- build: 0 Error / test: 629/630
- commit `6ad57bd`

### 8.8-R2B — 旧占位目录清理 (2026-06-24)
- 删除 9 个仅含 .gitkeep 的空占位目录（AI/Combat/Data/Ecs/Exporter/Runtime.\*/Simulation/World）
- 删除审计确认无误伤
- build: 0 Error / test: 629/630
- commit `5bdda34`

### 8.8-R2C — docs audit 文件清理 (2026-06-24)
- 删除 14 个临时 audit-\* / whitelist-\* / renderer-\* 文件
- 压缩 CHANGELOG.md 为简洁格式
- 改名 CHANGELOG.md → changelog.md

### 8.8-R3-0 — namespace 迁移审计 (2026-06-24)
- 纯审计，不修改代码
- namespace FluidWarfare.\*: 629处 / using: ~900+ / x:Class: 16处 / 类型名: 1处
- 建议执行顺序: R3-1→R3-2→R3-3→R3-4→R3-Z
- commit `94c9562`

---

*完整 git log 包含每个提交的详细变更。*
