# Vulkan Render / Device / Pipeline 125 文件问题登记册

基线：`fa3baecc07a5c6b3a1b49a1abc19348b69fe4072`。问题描述取自 Render、Device、Pipeline 首轮审计和整改授权，共 40 条（Render 22、Device 8、Pipeline 10）。本登记册与该批候选代码一起交付；关闭状态以本次 Release 构建及定向测试证据为限。真实 Vulkan/GPU 验收尚未执行，不能以 CPU 合同测试替代。

| ID | 根因与源码证据 | 风险 | 独立验收要求 | 当前状态 |
|---|---|---|---|---|
| R1-01 | `Render/ClearFrame/VulkanClearFrameOwner.cs` 与 41 个 partial 共用状态，MapSurface 快照、缓存键、动态 patch 与 GPU 缓冲生命周期属于独立子系统。 | 高 | 抽离真实 MapSurface Owner；保持帧槽、Fence 租约、Retirement、绘制顺序合同。 | CANDIDATE FIX；MapSurface Owner 已拆；合同测试通过；未做 GPU 实机验收；未提交 |
| R1-02 | 基线 framebuffer 原位重建存在失败后部分资源残留及失效代次被使用风险。 | 高 | 失败关闭、代次更新、无残留/重复释放。 | FIXED in `768fdab8`；`VulkanFramebufferRebuildFailureTests` 通过；GPU 验证未执行 |
| R1-03 | ClearFrame formatter 承担 Session 释放事件格式化。 | 低 | 由 Session 诊断层持有格式化职责并保持日志一致。 | FIXED；迁至 `VulkanRenderSessionLogFormatter`；合同测试通过 |
| R2-01 | `FrameDirty` 只有写入与清除，无生产读取者。 | 中 | 全仓核实后删除死状态并证明行为合同。 | FIXED；移除 FrameDirty/GpuResourceState；Core 合同测试通过 |
| R2-02 | `RenderChangeSet` 标志未门控资源上传，和 revision/key 事实重复。 | 中 | 删除未消费状态，不把它接入上传门控。 | FIXED；移除 RenderChangeState；Core 合同测试通过 |
| R2-03 | Projection 生产线程入队前读取 Present 所有的 frame state。 | 中 | 生产端只发布；消费端做重复投影过滤；避免跨线程读。 | FIXED；World projection 线程合同通过 |
| R2-04 | Present 结果处理放在 TerrainProbe 文件；队列/代次策略存在只测不生产用的方法。 | 低 | 文件归位；生产复用 CanSelectImage/RequiresFenceWait；删除未用策略入口。 | FIXED；Present 路径归位、死 API 删除/复用；定向 Present 测试通过 |
| R3-01 | MapLabel 的保留集合需按完整 DrawPlan 汇总，空集合也必须可驱逐。 | 中 | 完整帧集合保留；空集合可回收；GPU 在途资源经 Fence 退休。 | FIXED；`VulkanMapLabelCacheLifecycleTests` 通过 |
| R3-02 | Descriptor set 驱逐必须回收句柄且等待帧租约。 | 中 | 驱逐可回收 descriptor；在途引用不提前释放。 | FIXED；MapLabel 生命周期合同通过；未做 GPU 长时压力验收 |
| R3-03 | MapLabel image/memory/view/upload/descriptor 创建的失败回滚。 | 中 | 分阶段失败全部逆序清理且保留同步安全。 | FIXED；`VulkanMapLabelTextureRollbackTests` 通过；未做 Vulkan Validation Layer 验收 |
| R3-04 | 上传后 `QueueWaitIdle` 与导航/比例尺每次绘制托管数组分配。 | 中 / 低 | 不移除缺少退休替代方案的 QueueWaitIdle；移除绘制分配且保留绘制顺序。 | 部分 FIXED：Push Constant 改用清零栈上 Span，合同测试通过；保留 QueueWaitIdle（GPU 同步/性能未实测） |
| R3-05 | `Render/Overlay/VectorDrawing` validator 未校验 `BaseVertex` 叠加后的最终索引及数值边界。 | 中 | 覆盖最终索引范围、溢出、非有限顶点；坏输入在 GPU draw 前拒绝。 | CLOSED：候选修复检查最终索引与 primitive 范围溢出；4 个边界用例随 36 项定向测试通过。真实 GPU 验收仍未执行。 |
| R3-06 | VectorOverlay `Update` 仅改 CPU 数据；CanReuse/Stride 无生产消费者。 | 低 | 删除无效 API 和重复布局事实，保留被消费的布局合同。 | FIXED；删除死 API/策略与旧用例；`MapVectorOverlayV1Tests`、Push Constant 合同通过 |
| R3-07 | Present、日志、Overlay 文件/类型名与职责不一致。 | 低 | 按真实职责改名并更新调用方/源码合同。 | FIXED；PresentLoop、Pipeline/Session/Overlay 命名已校正；合同测试通过 |
| R4-01 | `Render/Scene/Models/VulkanStaticModelBuffer.cs:29-30` 的 MapMemory 失败分支曾先释放已绑定 memory、再销毁 buffer。 | 高 | 注入 MapMemory 失败，证明 DestroyBuffer → FreeMemory 以及仅回收成功取得的句柄；验证层无生命周期错误。 | CANDIDATE FIX / PENDING VALIDATION |
| R4-02 | `VulkanStaticModelValidator` 检查 primitive 区间，但未逐项确认 index 小于顶点数量。 | 中 | 越界 index 在 `CmdDrawIndexed` 前被拒绝，并覆盖索引边界用例。 | CLOSED：候选验证器拒绝越界 index 和溢出的 primitive 区间；3 个边界用例通过。 |
| R4-03 | Draw kind 到 owner/readiness/handler 曾重复维护映射。 | 低 | 所有 draw kind 使用同一 Owner 解析结果。 | FIXED；分发复用 ResolveDrawOwner；World 映射合同通过 |
| R4-04 | FailureTracker 与 StaticModelBuffer/Resource 存在未用成员疑点。 | 低 | 以全仓调用证明生产消费者；删除真实死 API。 | FIXED；移除 Clear/Count 与资源 IndexCount；保留生产使用的 CapacityBytes/TryUpdate；Build 与 tracker 测试通过 |
| R5-01 | Map key 含不影响当前固定 Flat 地面/边界几何的 surface metadata。 | 低 | 比对 GeometryBuilder 输入；避免 metadata-only 重建并回归几何相等。 | FIXED；只按 MapId/尺寸/高度/显隐判缓冲身份；Core 9 项地图策略测试通过 |
| R5-02 | Terrain chunk cache key 忽略同一 revision 下可变的 `ChunkCellsOverride`。 | 中，条件性 | 将 CellsPerChunk 纳入 key，避免错误复用。 | FIXED；增加 TerrainChunkListKey；World 定向测试通过 |
| R5-03 | Terrain 上传前没有独立 index 上界扫描，索引由内部 mesh builder 生成。 | 中，条件性 | 核实 builder 的输出约束和输入边界；无效 chunk 不产生越界 GPU 读取。 | CLOSED：候选在转换/上传前验证全部 builder indices；有效/越界用例通过。未复现 builder 产生坏 mesh，属于边界防护补齐。 |
| R5-04 | GridScale 有未使用 viewport 局部；Metric/VP 网格计算用途不同。 | 低 | 删除无用局部；验证网格比例与绘制语义。 | FIXED；移除未使用局部；Core grid/viewport 定向合同通过 |
| DEV-01 | Instance/Surface 创建后异常路径需销毁已创建 native handle。 | 高 | 验证失败回滚和成功句柄唯一归属。 | FIXED in `768fdab8`；`VulkanDeviceCreationSafetyTests` 通过；真实 Vulkan 故障注入未执行 |
| DEV-02 | `VulkanDeviceOwner.Physical.cs` 只有一个访问器的机械 partial。 | 低 | 移除机械拆分；不引入第二个物理设备 Owner。 | FIXED；PhysicalDevice 属性并入主 Owner；等待最终 Build |
| DEV-03 | Graphics/Offscreen 队列族能力检测重复。 | 中 | 共用 QueueFamily selector 并保留两种能力约束差异。 | FIXED in `768fdab8`；Device safety tests 通过 |
| DEV-04 | 物理设备枚举及 Surface support 查询的 VkResult 需要逐次分类。 | 高 | 每次查询失败都传播错误，不折算成不支持。 | FIXED in `768fdab8`；Device safety tests 通过 |
| DEV-05 | Instance result 的 Success 与 Owner 状态可能不一致。 | 中 | 构造时强制状态不变量。 | FIXED；构造器和一致性合同已加入；World 定向测试通过 |
| DEV-06 | Instance extension 名称的 UTF-8/NUL 终止约定分散且可能含内嵌 NUL。 | 中 | 集中编码并保证恰好单 NUL 终止。 | FIXED；`VulkanExtensionNameEncoder`；Device 合同测试通过 |
| DEV-07 | Device 文件/类型/方法命名优化建议。 | 低 | 回核定位价值与外部/API 兼容影响。 | NO CHANGE / VERIFIED；没有足够证据支持进一步缩名 |
| DEV-08 | PhysicalDeviceSelector 选择策略与 Offscreen 条件分离建议。 | 低 | 核实职责和生产调用，不制造转发/空壳类型。 | NO CHANGE / VERIFIED；共享选择职责已明确，Offscreen 条件保持独立 |
| PIPE-01 | Scene/MapLabel/Vector/Terrain Push Constants CPU/GPU 布局一致性。 | 高 | 核对字节数、offset、shader prefix 和 writer。 | NO CHANGE / VERIFIED；源码布局证据一致；Vector/Navigation 合同测试通过；未做 GPU shader runtime 验收 |
| PIPE-02 | Pipeline formatter 的 Created/Skipped 无生产消费者。 | 低 | 删除无用格式化成员。 | FIXED；死 formatter 移除；Release Build 通过 |
| PIPE-03 | GridLine factory 无生产消费者。 | 低 | 删除死入口且保留仍被引用的 shader assets。 | FIXED；factory 删除、shader 资产保留；Release Build 通过 |
| PIPE-04 | GraphicsPipelineOwner 混合资源生命周期与创建工厂职责。 | 中 | Owner 只持有/释放已创建句柄；factory 承担创建编排。 | FIXED；提取 ScenePipelineFactory；owner boundary 测试通过 |
| PIPE-05 | GridPipelineSet 部分创建失败时需释放已创建 pipeline。 | 高 | 创建中途失败逆序 Dispose。 | FIXED in `768fdab8`；`VulkanPipelineCreationSafetyTests` 通过 |
| PIPE-06 | 日志回调异常可能打断资源失败清理。 | 高 | 资源清理不受日志异常中断。 | FIXED in `768fdab8`；Pipeline creation safety tests 通过 |
| PIPE-07 | StaticModel vertex input 文件归属不正确。 | 低 | 移到 Contracts 并更新生产调用。 | FIXED；迁移到 `Pipeline/Contracts/VulkanStaticModelVertexInput.cs`；Build 通过 |
| PIPE-08 | GridPipelineSet 命名空间与路径不对应。 | 低 | 对齐 Namespace、文件夹、类型和消费方。 | FIXED；迁移并改名为 `VulkanGridAndOverlayPipelineSet`；Build/合同测试通过 |
| PIPE-09 | Owner/Creation/Depth/State/ShaderModule 等名称过于泛化。 | 低 | 按具体创建、状态和资源职责命名。 | FIXED；Pipeline 工厂/状态/模块文件与消费方已重命名；定向测试通过 |
| PIPE-10 | 多个 Graphics Pipeline 重复配置相同 raster/multisample/viewport/blend 状态。 | 低 | 提取共享常量构造，同时保留深度/混合差异。 | FIXED；新增 `VulkanGraphicsPipelineStates`；更新共享状态合同并通过 |

## 本轮验收边界

- 当前候选源码基于 `codex/vulkan-remediation-r1` 的既有 `768fdab8681dfe007e5ffae6cc6c4c897ca134c5`；本轮 Candidate commit 与最终 tree 在交付报告中列出。
- Release 解决方案 Build：0 warnings、0 errors。
- 定向测试：Core 97/97、World Vulkan 123/123；不等同于全量测试。
- 当前候选的 Render/Device/Pipeline 5+100 源码扫描：Render 97 个、Device 20 个、Pipeline 18 个 C# 文件；无文件超过 100 行；所有目录直接 C# 文件数和直接子目录数均不超过 5。
- CPU 合同测试不等同于真实 Vulkan 故障注入、Validation Layer 或 GPU 画面验收。真实 Editor Vulkan 绘制、Resize/Swapchain recreate/Present/Close、Scene/World/Overlay、DEM 近地回放均未执行，必须保持待验收。
- 工作目录没有 `.xye-workspace`；本轮 Build/Restore 仅使用规范仓库树内标准 `bin/obj`。该状态不构成对 E 盘整体空间变化的归因。
- 保留 `VulkanMapLabelTextureCache.Upload.cs` 中的 `QueueWaitIdle`，因为当前 staging 与 command buffer 释放需要完成证明；未建立 fence/异步退休替代机制。性能影响仍 UNKNOWN。
