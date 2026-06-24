# 项目文件树 — XuanYu Engine

```
XuanYu.Engine/                          ← 玄域引擎 / XuanYu Engine（原名 FluidWarfare）
│
├── changelog.md                        # 变更记录（倒序，最新在最前）
├── file-tree.md                        # 本文件 — 项目结构地图
│
├── XuanYu.Engine.sln                   # 解决方案文件
│
├── docs/                               # 项目文档
│   ├── AI_DEVELOPMENT_RULES.md         # AI 开发规则
│   ├── CODE_CONSTITUTION.md            # 代码宪法（100 行硬线等）
│   ├── ENGINE_ARCHITECTURE.md          # 引擎架构说明
│   ├── LEGACY_FLUIDWARFARE_OLD_AUDIT.md # 旧仓库考古报告
│   ├── MILESTONE1_PUBLIC_VALIDATION.md # 里程碑验收命令
│   ├── NAMING_RULES.md                 # 命名规则
│   ├── PHASE1_SCOPE.md                 # Phase 1 范围
│   ├── PROJECT_CHARTER.md              # 项目宪章
│   ├── naming-XuanYu-Engine.md         # 命名规范
│   └── reports/
│       └── namespace-migration-R3-plan.md  # R3 namespace 迁移计划
│
├── tools/shaders/                      # Shader 编译工具链
│   ├── compile_basic_3d.ps1            # GLSL → SPIR-V 编译
│   ├── validate_basic_3d.ps1           # SPIR-V 合法性验证
│   └── embed_basic_3d_shaders.ps1      # .spv → CompiledShaders.cs 嵌入
│
├── GameProjects/SampleProject/         # 示例游戏项目
│   └── game.project.json               # 项目内容目录声明
│
├── XuanYu.Engine.Core/                 # 基础值对象层（9 .cs 文件）
│   ├── Identity/EntityId.cs            # 实体 ID 值对象
│   ├── Logging/                        # 日志条目与级别
│   ├── Math/                           # Vector3d / YawRotation
│   ├── Results/                        # EngineError / EngineResult
│   └── Time/                           # TimeStep / SimulationTime
│
├── XuanYu.Engine/                      # 引擎运行层（8 .cs 文件）
│   ├── World/                          # WorldState / 实体来源
│   │   └── EntityPosition/             # 实体位置写入
│   └── Components/                     # DisplayName / Position 组件
│
├── XuanYu.Engine.Project/              # 项目系统层（17 .cs 文件）
│   ├── Loading/                        # 项目加载器 / Manifest / FolderParser
│   ├── Content/                        # 文件扫描 / 目录声明
│   ├── Metadata/                       # 项目元数据
│   ├── Paths/                          # 示例项目路径
│   ├── Validation/                     # 校验报告
│   └── World/Transform/                # SceneTransform / 矩阵 / 校验
│
├── XuanYu.Engine.Bridge.ProjectEngine/ # 项目→引擎桥接（2 .cs 文件）
│   └── World/                          # 项目内容→World 实体种子
│
├── XuanYu.Engine.Render/               # 抽象渲染层（47 .cs 文件）
│   ├── Camera/                         # 相机状态 / 运动 / 姿态
│   ├── Coordinates/                    # 坐标转换
│   ├── Scene/                          # 场景位置写入
│   ├── Selection/                      # 选择 / 指针 / 地面 / 屏幕
│   ├── ViewportNavigation/             # 视口导航
│   └── World/                          # World→RenderScene 构建
│
├── XuanYu.Engine.Render.Vulkan/        # Vulkan 渲染实现（154 .cs 文件）
│   ├── Backend/                        # Vulkan 后端信息
│   ├── Camera/                         # 相机矩阵 / 投影
│   ├── Clear/Probe/                    # 清屏探针
│   ├── Context/                        # 渲染上下文
│   ├── Device/                         # 设备选择 / 信息
│   ├── Instance/                       # Instance 创建
│   ├── Scene3D/                        # 3D 管线全程（Session / Render / Depth）
│   ├── Shaders/                        # 着色器 / CompiledShaders.cs
│   ├── Surface/                        # Surface 创建
│   ├── Swapchain/                      # Swapchain Probe / Context
│   └── Validation/                     # Validation Layer
│
├── XuanYu.Engine.Editor/               # 编辑器模型层（60 .cs 文件）
│   ├── EntityTransform/                # 实体变换验证
│   ├── Input/                          # 绑定 / 动作 / 运行时 / 设置
│   ├── ProjectContentTreeModel/        # 项目内容树
│   ├── Selection/                      # 选择状态
│   ├── Transform/                      # 变换编辑 / 平移 / 数据
│   ├── ViewportGround/                 # 地面指针状态
│   └── WorldHierarchy/                 # 层级树构建 / 搜索
│
├── XuanYu.Engine.Editor.Windows/       # 编辑器 UI 层（260 .cs + 16 .axaml 文件）
│   ├── App.axaml                       # 应用入口
│   ├── MainWindow.axaml                # 主窗口
│   ├── app.manifest                    # Windows 程序清单
│   ├── GlobalUsings.cs                 # 全局 using（90 条）
│   ├── About/                          # 关于玄域引擎窗口
│   ├── Preferences/                    # 偏好设置窗口
│   ├── Shell/                          # EditorShell 组合根（28 行入口）
│   │   ├── Composition/                # 组合根（Context / Wiring）
│   │   └── 18 个子目录                 # Commands / Input / Lifecycle / 等
│   ├── Panels/                         # 编辑器面板
│   │   ├── DebugDock/ / HierarchyVisual/
│   │   ├── Inspector/ / LeftDock/ / Logging/
│   │   ├── ProjectContentTree/ / Status/
│   │   ├── Viewport/                   # 3D 视口（Placeholder / NativeHost / Tools）
│   │   └── WorldHierarchy/             # World 层级树
│   └── Viewport/                       # 视口功能模块
│       ├── Camera/ / Navigation/ / Picking/ / Project/
│       ├── Scene3D/ / Selection/
│       └── Transform/                  # Gizmo / Drag / Interaction
│
└── XuanYu.Engine.Tests/                # 单元测试（73 .cs 文件）
    ├── Architecture/                   # 10 个门禁测试
    ├── Core/ / Engine/ / Bridge/ / Project/
    ├── Render/ / Editor/
    └── Render.Vulkan/
```
