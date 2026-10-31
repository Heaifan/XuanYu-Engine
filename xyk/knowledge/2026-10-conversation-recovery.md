# 2026年10月历史对话追查：新增工程知识 R2

> 来源：10/03～10/31历史对话、Codex报告与可复核的GitHub产品提交。状态为长期可复用规则；报告的静态/CPU/GPU/用户验收严格分级。历史建议ID有冲突时不覆盖已经登记的正式ID。

## K-DRAW-001｜唯一 Drawing Session Snapshot 是绘制会话事实源
**ACTIVE；P0；E2；2026-10-04；历史别名 DRAWING-UX-SESSION-001。**
绘制中的Feature、Active Layer、阶段、阻断原因、预览由Drawing Session权威状态拥有；Toolbar、Inspector、Viewport与通知只投影同一Snapshot，不得各自保有Session Authority。Gesture Owner与Session Owner分开，Block不等于Pointer Rejection。产品W1证据：518a0849e082d205a3bee5f871f7a9ddeed84e10（Drawing功能代码）；10/04报告24/24与World 2273/2273为W1局部验收，不覆盖后续完整Drawing产品P4。关联 K-INP-001、K-ARCH-002。

## K-DRAW-002｜绘制动作必须反馈 Accepted / Rejected / Blocked 及原因
**ACTIVE；P0；E2；2026-10-04；历史别名 DRAWING-UX-FEEDBACK-001。**
Accepted说明输入被当前Session接收；Rejected是单事件拒绝；Blocked是会话或环境前置条件阻断，必须给Reason。GroundPick、目标图层、Active Session缺失不得默默忽略。测试排错次序为Activation→Session→Input→Gesture/Capture→Commit；handled=false不是Router错误的充分条件。10/04历史W0/W1报告仅支持对应局部流程，不是所有Region/Road/Marker完成的证明。

## K-SPA-004｜净空查询只读，不得夺取 Anchor/Orbit/ObservationCenter
**ACTIVE；P0；E2（用户设计裁定）；2026-10-10。**
允许狭义只读净空查询，但不得在查询中改变Anchor、Orbit Pivot或ObservationCenter。相机Wheel Dolly保持观察中心不变量，MinDistance=0.25m。渲染地形三角面、Ground Query及GeographicWorldMapping须明确同一事实基准；Camera-to-Pivot与相机距显示网格的净空分别报告。10/16 R6的Pivot修复不能推断513密度、ScaleBar或近地视觉P4均修复。关联旧K-SPA-002、K-GEO-004。

## K-XYC-001｜XYC必须走Editor权威状态和唯一UI写线程
**ACTIVE；P0；E2；2026-10-14；产品证据 6b1979899d7421ee63d75833ebc7ceb0648f1505。**
XYC是后台、不占鼠标的语义控制层；复用Editor的相机/场景/对象权威状态、UI线程和事务，不另设Shadow State。成功返回应以独立会话的生产状态与Revision回读证明执行，不等于屏幕帧完成。能力分READ/DIAGNOSTIC/SANDBOX_WRITE/LIVE_WRITE；规划中的单位、装备、Linux或模拟命令不因为接口示例而成为已交付功能。

## K-XYC-002｜自动化截图须证明目标GPU帧的新鲜度与对应身份
**ACTIVE；P0；E2；2026-10-14～15；产品阶段证据 d2d011ac98314f172e0e3d729583e67832099f1f。**
CameraRevision/Present回读和PrintWindow图像均不足以证明新相机/地形已渲染；重复帧不得作为尺度/画质证据。可靠截图要求声明Surface或Offscreen能力，Capture绑定Terrain RenderPass、GPU copy/readback、Fence、FrameId、CameraRevision、SourceRevision和PNG哈希；Manifest与构建输入/输出身份一致。没有真实GPU复制证明时，只能描述为目标合同。Headless、Silent Vulkan、真实桌面UiWin和用户P4彼此不能替代。

## K-XYC-003｜业务写成功不等于World/Projection/History完整提交
**ACTIVE；P0；E2；2026-10-18～19；产品阶段证据 fa666cb17d47fa96eeb13867ffc06572c803dac9。**
业务命令必须绑定会话实体和Expected SceneRevision，回读World/Scene、RenderSnapshot投影、History/Undo顺序；World已经写入而RenderSnapshot异常时存在半提交风险，不能因Command返回Success就宣布事务闭环。应验证权威World并执行可恢复补偿，记录阶段及恢复结果。P2-A 46/46及4/4事务专项仅限当时提交，非后续全功能保障。

## K-PERF-002｜动态Terrain SSE需与Renderer共享视锥及Revision
**ACTIVE；P0；E2（明确反例）；2026-10-18～19。**
动态Selector输入当前Camera、Viewport、Source Revision，并使用与Renderer等价的Conservative Frustum；镜头后方投影可能产生无效无限SSE并耗尽预算。Selected、Prepared、Submitted、Pick-active各阶段节点身份分别比较。历史样本Dynamic P95高于513基线92～136%、128提交节点、目标测试11/11；这是局部样本，不是性能合格。20-cycle、真实NoData/Seam/MultiTile/EditPick、Fence Retirement均未全验；生产默认513+Auto，动态opt-in，相关状态PARTIAL。

## 既有条目的去重映射（须独立STRENGTHEN）
- K-REN-004：逻辑ReferencePlane、Visible Grid、MapGround、Terrain的权威边界；历史10/03曾建议K-REN-006，但该ID已被R1正式用于Renderer双帧，禁止冲突重用。
- K-GOV-005：Candidate Tree/测试身份及失败时的P4状态。
- K-REN-006：GPU缓存退休须同时满足GPU已完成且旧CommandBuffer已不再引用；仅有Fence/Readback不足。
- K-DATA-001/002：MapDataset多文件保存/Manifest/generation/previous恢复与Editor MapDocument问题。
- K-WORLD-001：R2-A的Candidate验证不证明GlobalWorld.Replace整体原子性；R2-B需预构建Registry/Partition/SpatialIndex和故障注入回滚。
- K-INP-003：绕过Router的Gizmo/Draw直接入口也必须进入统一终止流程。
- K-GEO-004/K-REN-005：Raw/Resampled/Render/Pick/Bounds/LOD一致性、WP2用户P4失败与最终像素Gate。
- K-VAL-002/EXP-TEST-001：XYUI可视桌面、Silent Vulkan、Headless、Protocol Smoke、用户P4分层。
