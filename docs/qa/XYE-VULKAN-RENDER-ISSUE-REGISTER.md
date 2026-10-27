# Vulkan Render 91 文件审计问题登记册

基线：`fa3baecc07a5c6b3a1b49a1abc19348b69fe4072`。问题描述、证据位置、风险和原始状态取自 R1～R5 首轮审计收口报告；本表未重新审计其余 17 项。原始候选 `4fadfb0f` 中，A～D 关联的 5 项均为 `CANDIDATE FIX / PENDING VALIDATION`；本登记册随版本化 R2 候选保留这些状态，其余 17 项维持原 `OPEN` 或 `UNKNOWN`。候选验证未完成前不得关闭。

| ID | 根因与源码证据 | 风险 | 独立验收要求 | 当前状态 |
|---|---|---|---|---|
| R1-01 | `Render/ClearFrame/VulkanClearFrameOwner.cs` 与其 41 个 partial 共用状态；同一 Owner 持有命令、帧目标、缓存、离屏、读回、帧状态和诊断职责。 | 高 | 明确真正资源 Owner、借用关系、生命周期及 Present/Fence 合同；未迁移前保持静态问题开放。 | OPEN |
| R1-02 | 基线 `VulkanClearFrameOwner.cs:73` 先销毁活动 framebuffer，再原位创建；失败时部分新资源可能存在且旧集合不可用。 | 高 | 各创建阶段失败均无残留；失败后 Present 不再使用失效代次；关闭/恢复无重复释放。 | CANDIDATE FIX / PENDING VALIDATION |
| R1-03 | `Render/ClearFrame/Diagnostics/VulkanClearFrameLogFormatter.cs` 的 `SessionDisposed` 格式化 Session 生命周期事件，超出 ClearFrame 职责。 | 低 | 核对 formatter 所有者与调用范围；日志一致且职责/命名对齐。 | OPEN |
| R2-01 | `Render/Frame/VulkanFrameState.cs` 与 `VulkanGpuResourceState.cs` 的 `FrameDirty` 有写入/清除，未发现生产读取点。 | 中 | 全仓核实消费者；决定保留或删除，确认帧行为未变。 | OPEN |
| R2-02 | `Render/Frame/VulkanRenderChangeState.cs` 的 `RenderChangeSet` 计算 cache invalidation/upload 标志，未发现这些标志实际门控资源上传。 | 中 | 明确标志用途；逐类验证 cache key、revision、上传与退休，不直接将其接成上传门控。 | OPEN |
| R2-03 | `VulkanRenderSession.QueueRenderProjection` 入队前读取共享 frame state；Present 线程消费并更新。 | 中 | 核实发布线程合同；测试并发发布、重复投影、Present 消费和 Resize 顺序。 | UNKNOWN |
| R2-04 | `Render/Frame/PresentDiagnostics/VulkanPresentLoop.TerrainProbe.cs` 含 `HandlePresent` 主路径，其他策略入口可能未用或重叠。 | 低 | 确认 `Complete`、`CanRelease`、`CanSelectImage` 的生产消费者；维持单一事实源。 | OPEN |
| R3-01 | `Render/Overlay/LabelOwner/VulkanClearFrameOwner.DrawMapLabels.cs` 曾按单个 Overlay 调共享 cache 的 `RetainOnly`；空标签提前返回，不能清空保留集。 | 中 | 多 Overlay 同时保留；空集合/隐藏项可回收；帧内资源经 fence 后退休。 | CANDIDATE FIX / PENDING VALIDATION |
| R3-02 | `Render/Overlay/LabelCache/VulkanMapLabelTextureCache.Descriptors.cs` 的 descriptor pool 固定 256 个 set；原驱逐路径不释放 descriptor。 | 中 | 反复创建/驱逐后 descriptor 可回收；仅在所有在途帧引用退休后释放。 | CANDIDATE FIX / PENDING VALIDATION |
| R3-03 | `Render/Overlay/LabelCache/VulkanMapLabelTextureCache.Create.cs` 的 image/memory/view 创建阶段曾有失败路径缺少完整回滚。 | 中 | 对创建、分配、绑定、view、上传、descriptor 失败分别确认句柄所有权与回滚；GPU 在途资源不得提前释放。 | CANDIDATE FIX / PENDING VALIDATION |
| R3-04 | `Render/Overlay/LabelCache/VulkanMapLabelTextureCache.Upload.cs` 上传后 `QueueWaitIdle`；导航/比例尺绘制每次绘制有托管数组分配。 | 中 / 低 | 先验证同步与资源安全合同；再分别测量分配，不改变绘制顺序。 | OPEN |
| R3-05 | `Render/Overlay/VectorDrawing` validator 未校验 `BaseVertex` 叠加后的最终索引及数值边界。 | 中 | 覆盖最终索引范围、溢出、非有限顶点；坏输入在 GPU draw 前拒绝。 | UNKNOWN |
| R3-06 | `VulkanVectorOverlayResource.Update` 不更新 GPU geometry；`CanReuse`、`Stride` 等未发现生产使用，布局契约可能重复。 | 低 | 核对所有调用者；保留成员须有生产用途，stride 只保留一个权威定义。 | OPEN |
| R3-07 | R3 发现若干 Present、日志、Overlay 名称与实际职责/范围不一致。 | 低 | 逐项回到原审计命名清单核实调用影响；改名不得冒充职责拆分。 | OPEN |
| R4-01 | `Render/Scene/Models/VulkanStaticModelBuffer.cs:29-30` 的 MapMemory 失败分支曾先释放已绑定 memory、再销毁 buffer。 | 高 | 注入 MapMemory 失败，证明 DestroyBuffer → FreeMemory 以及仅回收成功取得的句柄；验证层无生命周期错误。 | CANDIDATE FIX / PENDING VALIDATION |
| R4-02 | `VulkanStaticModelValidator` 检查 primitive 区间，但未逐项确认 index 小于顶点数量。 | 中 | 越界 index 在 `CmdDrawIndexed` 前被拒绝，并覆盖索引边界用例。 | OPEN |
| R4-03 | `ResolveDrawOwner`/pipeline readiness 与 `DispatchDraw` 分别维护 draw-kind 映射。 | 低 | 核对每种 draw kind 的 Owner、ready 判定和 handler 映射一致。 | OPEN |
| R4-04 | `FailureTracker.Clear/Count`、`CapacityBytes`、`IndexCount`、静态模型 `TryUpdate` 未发现生产调用者。 | 低 | 检索非源码消费者；保留项需证明生产用途，删除需证明不破坏外部契约。 | OPEN |
| R5-01 | Map resource key 可能包含不会改变当前固定地面几何的字段，造成多余重建。 | 低 | 对照 key 与 geometry builder；证明字段会影响几何或从 key 移除并回归缓存正确性。 | UNKNOWN |
| R5-02 | Terrain chunk-list cache key 未包含 replay `ChunkCellsOverride`。 | 中，条件性 | 先证明同一 revision 内 override 可变；若可变，改键并验证参数变化不命中旧 partition。 | UNKNOWN |
| R5-03 | Terrain 上传前没有独立 index 上界扫描，索引由内部 mesh builder 生成。 | 中，条件性 | 核实 builder 的输出约束和输入边界；无效 chunk 不产生越界 GPU 读取。 | UNKNOWN |
| R5-04 | GridScale 有未使用 `viewport` 局部；另一 VP/metric 网格计算用途不同，不能仅因相似而合并。 | 低 | 清除无用局部；以单位/用途证据确认两条计算路径，回归网格比例和绘制语义。 | OPEN |

## A～D 状态

R4-01、R3-03、R3-01、R3-02、R1-02 是本批 5 条候选修复；没有一条已关闭。剩余 R1（2 条）、R2（4 条）、R3（4 条）、R4（3 条）、R5（4 条）合计 17 条，状态保持 OPEN / UNKNOWN。实机验收结论和最终 HEAD 应在本登记册之外的候选验收记录中绑定；不得把本表状态自动升级为 CLOSED。
