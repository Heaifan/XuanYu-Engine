# XYK｜2026-10-10～31 新增工程知识

> 正式知识版本：R1（2026-10-31，北京时间）；Owner：ChatGPT；适用：XYE / XYUI；范围：工程规则，不代表产品集成已通过。
> 证据分类：R=可查 GitHub 产品提交；U=用户批准/本轮施工报告，可能未推送产品源码。E2 表示有限工程证据与明确裁定，不意味着整机动态 PASS。

## K-GOV-007｜仓库内可写根、缓存和清理边界
**ACTIVE｜P0｜E2｜U：10/22 工作区复盘、10/26 T0+ 事故、10/30 Build R2。**
全部正式写入须限制于 E:\MyDoc\project-VSCode\XuanYuEngine 内已经批准的固定可复用目录，包括 Build/obj/bin、NuGet Packages、Scratch、诊断和 Manifest；旧 E:\XYE-Workspace 不得再用。禁止因新工作区、重试或清理动作触发仓库外写入。发现重解析点、路径越界、缓存根不明时 STOP，保留证据并只读复核；怀疑 NuGet 访问不等于已证实写入。清理与构建都受白名单/所有权约束。

## K-GOV-008｜有界构建和 Restore，失败禁止自动重试
**ACTIVE｜P0｜E2｜U：10/30 用户正式硬约束。**
静态架构迁移优先零构建，只读盘点、改目录、再统一提交审批。不得每迁一个目录、每个 Agent 并发、每次失败后自动反复 Build/Restore。获授权后冻结 Source Tree、进程入口、缓存根、输出身份，仅一次受控验证；失败保留诊断，下一轮需重新批准。所有缓存集中复用，严禁复制多个嵌套 .xye-workspace。静态 PASS 不能冒称 0W0E。

## K-GOV-009｜XYK 候选必须逐轮去重、裁定、写回

### 2026-10-31 R2｜专用XYK Candidate Changelog
用户裁定从下一轮Codex工程任务开始，施工Agent先核查仓库记录约定，在专门的XYK Candidate Changelog追加每轮全部候选草案，含任务、日期、Source Tree/Commit、证据、风险、去重建议与候选状态；不得混入产品发布CHANGELOG，也不得把本地候选日志当正式K Event。ChatGPT审核后才可正式提交xyk/main及回执。

**ACTIVE｜P0｜E2｜U：10/30 用户 XYK 流程裁定。**
凡 Codex/施工报告含 XYK、XYK CANDIDATE 或长期可复用规则，知识 Owner 必须按既有 ID 搜索，逐项 CREATE / UPDATE / STRENGTHEN / SUPERSEDE / REJECT / NONE，并区分「经验候选、知识已写入、Git 远端已推送」。正式改动都须有 K Event、更新索引、K Commit；未提交产品源码填 -，不得用候选报告当已落实的产品验收。避免因换任务名重复造知识卡。

## K-GOV-010｜静态、Lane、Candidate、Integration 与发布是不同门禁
**ACTIVE｜P0｜E2｜U/R：M2 10/23、Vulkan 10/28、XYUI 10/30。**
报告逐一给出 PASS / PARTIAL / BLOCKED / NOT RUN 的作用域。CPU 合同 PASS、GPU Smoke、Editor 视觉、Release 0W0E、全仓测试、Git 本地提交/远端 Push/PR 合并互不替代。测试 TRX、SourceFingerprint、Build DLL SHA、资产身份、Git Tree、Fixture 和被排除测试必须匹配同一 Candidate。不同算法指纹不可直接比较。M2 远端 [6dd62073](https://github.com/Heaifan/XuanYu-Engine/commit/6dd62073e0c91a925000c6297d873020dd0df36c) 只能证明该候选存在，不能覆盖之后所有 Dirty。

## K-ARCH-003｜5+100 与严格三级目录的定位合同
**ACTIVE｜P0｜E2｜U：10/24～30；R：[84d49d43](https://github.com/Heaifan/XuanYu-Engine/commit/84d49d437d6a59060c18d37fc72b2d4594fb07ac)。**
单 C# 文件≤100 行、末级目录≤5 文件、SRP、严格三级目录；不允许无限增加四级目录躲避 5+100。一级有超过10个二级是考虑拆编的警戒线而非绝对违例。短文件名、测试源码 test_ 前缀且放独立 *.Tests。Core / World / Render.Abstractions 不可反向依赖 Editor 或 Render.Vulkan；迁移路径静态通过不代表 Build/Test/运行通过。Scene 短名迁移远端证据：[bfaa3342](https://github.com/Heaifan/XuanYu-Engine/commit/bfaa33425abd51325544c743168c764805376a83)。

## K-WORLD-001｜WorldState Candidate 发布前先验身份/父子不变量

### 2026-10-31 R2｜GlobalWorld.Replace原子性仍待B批验证
10/29 R2-A 12/12、63/63只证明Candidate隔离/层级合同；原EntityRegistry.Replace从Clear开始逐项写入，跨Registry、Partition、SpatialIndex可能异常后部分污染。R2-B目标是在隔离结构中预建，短锁+Generation比对后整体交换，故障注入后旧World保持；B批当时只有DESIGN READY，不得把规则写作已完工。

**ACTIVE｜P0｜E2｜R：[b8ae01f9](https://github.com/Heaifan/XuanYu-Engine/commit/b8ae01f92f942074c81a1da70c2fc8220c3870b5)。**
World 更改须在隔离 Candidate 中验证 Identity、Parent Graph、层级环和发布前不变量；失败不可部分污染当前 World。相关远端提交具有 Candidate、Validator 与隔离性测试，只能证明 R2-A 局部整改；Registry.Replace 回滚、其他 World R2 能力及全仓门禁未因该提交自动 CLOSED。

## K-GEO-004｜raw DEM、显示 LOD 与相机观察中心分离
**ACTIVE｜P0｜E2｜U：10/21～23 WP2、NASA DEM、R2-A 评估。**
30m HGT raw 采样与低密度显示网格不是同一测量精度；近地 ray hit、GroundPick/绑定和渲染三角面须分别验证，不得假定粗 LOD 表面即地理事实。Wheel Dolly 保持 ObservationCenter 不变量，MinDistance=0.25m，不改成 Cursor-anchored Zoom；Camera 与渲染地形查询一致性需固定样本。GroundPick=Terrain/Plane/Invalid 时 Invalid 不可静默回退 Z=0。2.5m 近地画面/513、1025、2049 对照属于实验样本，未证明完整实时性能。

## K-REN-006｜在途帧槽、交换链图像与 GPU 资源生命周期分离

### 2026-10-31 R2｜Terrain Cache双条件退休
10/22～23 M2-B1报告发现GPU Fence/Readback完成仍不足，旧CommandBuffer可能持有GPU缓存Buffer引用。TerrainGpuCache追踪退休资源，FrameOwner追踪命令缓冲Committed引用集合；只有成功重录后替换旧集合，取消录制保留。销毁须同时满足GPU已完成、旧CommandBuffer不再引用。RTX3060 Offscreen样本Evict后3 retired/0 destroyed，重录后4 destroyed；不覆盖DeviceLost、录制失败、Swapchain/Present或线程安全。

**ACTIVE｜P0｜E2｜R：[6dd62073](https://github.com/Heaifan/XuanYu-Engine/commit/6dd62073e0c91a925000c6297d873020dd0df36c)，U：M2-A2 51/51 CPU 合同。**
Frame Slot、Swapchain Image、Fence、Semaphore、每图像 Depth Attachment 必须分别追踪；1↔2帧切换明确排空、资源租约和延迟退休，防止 GPU 在途引用被提前 Dispose。Acquire/Submit/Present/Resize/OutOfDate 分门禁记录。CPU 合同通过不代表双帧 RTX3060 GPU 性能或运行时切换全验收；默认单帧的事实不得隐去。

## K-REN-007｜性能 Schema 通过不等于真实采样
**ACTIVE｜P0｜E2｜R：[6dd62073](https://github.com/Heaifan/XuanYu-Engine/commit/6dd62073e0c91a925000c6297d873020dd0df36c)。**
FrameSample、FenceWait Adapter、P50/P95、Terrain Counters、DataRevision 与导出测试证明的是处理能力。GPU Timestamp、真实 Editor 帧 Sink、Input-to-Visible 延迟在没接线/没采时必须显式 NOT AVAILABLE；应记录硬件、Scene、SourceFingerprint、统计区间、同步策略和 GPU 样本，不能用计数冒充 GPU 时序。

## K-UI-003｜XYUI 组件编号及 Typography 语义必须稳定
**ACTIVE｜P0｜E2｜U：10/29～30 用户裁定；R：[a88a06aa](https://github.com/Heaifan/XuanYu-Engine/commit/a88a06aa05608bfc279d9028650749c83bae62e7)。**
保留 XYUI1～XYUI4 和 XYUIx-编号-组件名的组件索引习惯，允许编号内部按职责重组但禁止擅自改纯功能分类。Typography 改用 DynamicResource 时必须保留字号/字重值、核对资源键/引用数并进行 Gallery 运行时视觉验收。10/30 报告 58 个 Gallery AXAML、452 字号、293 字重，XYUI 708/708 和视觉认可，但最终一批未提交/未推送不得写为远端已发布。

## 现有知识须强化，禁止重复建卡
- **K-GOV-005**：Candidate Tree Purity 增加 WorkspaceRoot/SourceRoot、测试 DLL SHA、Manifest/Tree 与 DEM Fixture 同身份；Lane PASS 不可跨 Tree 复用。
- **K-VAL-001**：产物、测试和用户运行必须匹配同一 SourceFingerprint、Commit、DLL 和资产 Manifest。
- **K-VAL-002**：独立 GPU Present、Resize、Shader、真实 Editor、性能样本各自有 Gate；未运行时 NOT RUN。
- **K-XYT-EXEC-001**：清洁基线失败、间歇失败、环境失败、测试基础设施与未知根因必须分别标注；不可修绿。
- **K-GEO-003**：Terrain GroundPick 失败不回退 Z0、Region XY 与 SurfaceBinding 权威分离。
- **K-REN-005**：Shader Source—SPIR-V 字节码一致性须按具体 Source Tree 逐一证明；静态成功不能取代 GPU 最终像素。
- **K-INP-001 / K-UI-002**：Router/Owner/Capture 生命周期覆盖 Up、CaptureLost、FocusLost、工具切换；合成 Headless ≠ 真实 HWND/OS 消息验证。

**状态边界**：本文件收录长期规则并引用报告，不能当成这些产品子功能全部已 CLOSED 的证据。具体未收口项参见同批审计覆盖清单。
