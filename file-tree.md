# XuanYuEngine 文件树
> 当前 Git tracked tree 的结构视图。只描述“现在有什么、放在哪里、负责什么”；不记录历史状态、版本、迁移过程或生成物。
> 目录优先表达职责边界；仅展开关键文件与关键入口，不把重复实现分片和测试逐项灌入本文件。
```text
XuanYuEngine/
├── XuanYu.Core/                                  # 引擎最底层通用能力；不依赖上层项目
│   ├── Space/                                    # 世界空间、相机、投影、Ray、Terrain 可见性/LOD 基础
│   │   ├── CameraState.cs                        # 相机权威状态
│   │   ├── ViewProjectionState.cs                # View/Projection 与 RenderOrigin 相关状态
│   │   └── WorldRayFactory.cs                    # 从视口/相机状态构造世界射线
│   ├── Spatial/                                  # 通用空间 Bounds / Ray-AABB 等基础算法
│   ├── Gizmo/                                    # Move/Rotate/Scale Gizmo 数学与布局基础
├── XuanYu.World/                                 # 世界事实层；依赖 Core，保存可持久化/可查询的世界状态
│   ├── Terrain/                                  # DEM/高度层、TerrainWorld、采样与地形元数据
│   │   ├── TerrainWorld.cs                       # 地形世界聚合与查询入口
│   ├── Map/                                      # Map/Region/Road/Marker/Layer 等权威地图数据
│   │   ├── WorldMapStateOwner.cs                 # 地图状态所有者
│   │   ├── MapRegion.cs                          # Region 权威数据
│   │   └── SurfaceBinding.cs                     # 地理对象表面绑定
│   ├── Scene/                                    # SceneStateOwner 与 World→Render 投影
│   ├── GlobalWorld.cs                            # 全局世界聚合入口
│   └── WorldQuery.cs                             # 世界查询入口
├── XuanYu.Render.Abstractions/                   # 渲染层公共合同；RenderProjection、DrawPlan、Terrain/Map GPU 输入描述
│   ├── RenderProjection.cs                       # 帧级渲染投影数据
│   ├── RenderDrawPlan.cs                         # 渲染计划公共模型
│   ├── TerrainRenderResource.cs                  # Terrain 渲染资源描述
├── XuanYu.Render.Vulkan/                         # Vulkan 后端；依赖 Core + Render.Abstractions
│   ├── Render/                                   # Vulkan 绘制实现与深度附件
│   ├── Pipeline/                                 # Vulkan Pipeline 创建与管理
│   ├── Bridge/                                   # Native Host / Vulkan 桥接
│   └── Shaders/                                  # GLSL Shader 源
├── XuanYu.Editor/                                # 编辑器领域层；依赖 Core + World，不包含具体 Avalonia 视图
│   ├── MapEditing/                               # 地图编辑 Session、Snap、HitTest、Ground Pick、Region/Road 绘制
│   │   ├── GroundPickResolver.cs                 # Ground/Surface 拾取解析
│   ├── Transform/                                # 编辑器 Transform Session
│   ├── Mode/                                     # EditorMode 生命周期与模式切换
├── XuanYu.Editor.UI/                             # Avalonia 编辑器 UI；组合 Editor/World/Render 抽象与 XYUI
│   ├── Vm/                                       # UI ViewModel 与交互编排
│   ├── Viewport/                                 # 视口 UI、Navigation Gizmo、Native Host 路由
│   ├── Input/                                    # Avalonia 输入到编辑器语义的适配
│   ├── Diagnostic/                               # 诊断浮窗/Overlay
├── XuanYu.Editor.App/                            # Avalonia 可执行入口与 Composition Root
│   ├── Program.cs                                # 应用启动入口
│   └── EditorCompositionRoot.cs                  # Editor/UI/Render/Vulkan 依赖装配
├── XuanYu.WarCore/                               # 战争模拟领域核心
├── XuanYu.Core.Tests/                            # Core 与部分 Render/Core 合同测试
├── XuanYu.World.Tests/                           # 历史跨层综合测试；后续按职责逐步迁移
├── XuanYu.Editor.Tests/                          # Editor 领域测试的新正式归属
├── XYT/                                          # 测试真实性与证据治理系统
│   ├── Runtime/                                  # Runtime 能力与执行环境
│   ├── Execution/                                # 测试执行器
│   ├── Witness/                                  # RED/GREEN Witness
│   ├── Acceptance/                               # IPO / 验收映射
│   ├── Integration/                              # 模块集成合同
│   ├── Report/                                   # 报告生成
├── xyui/                                         # XYUI 独立 UI 组件/规范资产
│   ├── avalonia/                                 # Avalonia 实现
│   ├── source/                                   # 组件源码
│   ├── specs/                                    # 组件合同与规范
│   ├── tokens/                                   # Design Tokens
│   ├── registry/                                 # 组件登记
│   ├── governance/                               # XYUI 治理规则
│   ├── audit/                                    # XYUI 审计资料
│   └── packs/                                    # 可分发组件包
├── tools/
│   ├── handoff/                                  # Handoff 控制面、Candidate/Ownership/Work Release
│   ├── governance/                               # Knowledge、Version、Test Truth 治理工具
│   └── terrain/                                  # Terrain 专用工具
├── scripts/
│   ├── governance/                               # 治理 Gate 与 XYT 脚本
│   ├── architecture/                             # 架构检查
│   └── tests/                                    # 测试入口/辅助脚本
├── docs/                                         # 架构、治理、UI、知识、研究与里程碑文档
├── audit/                                        # 当前 Git 已跟踪审计证据/产物
│   ├── requirements/                             # 审计需求与验收要求
│   └── packages/                                 # 原始审计压缩包
├── XuanYu.Editor.Win/                            # Windows 辅助宿主
├── XuanYu.WarCore.Tests/                         # WarCore 测试
├── samples/                                      # 示例内容
├── .superpowers/                                 # SDD/任务过程资料
├── .xyt/                                         # XYT schema/report 配置
├── .github/                                      # GitHub Actions 等远端仓库自动化
├── .gitattributes                                # Git 属性配置
├── .gitignore                                    # Git 忽略规则
├── NuGet.Config                                  # NuGet 源与包管理配置
├── xyui.bat                                      # XYUI 统一入口
├── XuanYu.Engine.slnx                            # 主解决方案入口
├── Directory.Build.props                         # 全仓 .NET 构建/版本公共属性
├── run.bat                                       # 编辑器标准启动入口
├── xyt.ps1 / xyt.bat                             # XYT 统一入口
├── AGENTS.md                                     # Agent 仓库级约束
├── changelog.md                                  # 变更记录
└── file-tree.md                                  # 当前文件结构单一事实视图
```
## 维护规则
- 只从 Git tracked tree 描述当前结构；`bin/`、`obj/`、缓存、临时生成物不进入。
- 目录写“职责边界”，关键文件写“具体职责”；禁止用“C# 源码文件”“项目资源文件”这类无信息量模板。
- 不在本文件记录版本、迁移历史、验收状态、债务状态或时间线；这些进入 changelog / governance / audit。
- 文件树变化时更新本视图；结构争议先在此识别，再决定是否迁移真实文件。
