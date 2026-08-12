# 玄域引擎事故库

> 整理时间：2026-08-10 17:56（UTC+08:00）
> 事故库记录“发生了什么、为什么、最终如何收口”；可复用结论已提升到对应 `K-*` 条目。
> 本文件只放具有明确工程复用价值的代表性事故，不追求把每个 Fix 都登记成事故。

## INC-2026-08-12-001 删除图层确认被 Native HWND 覆盖并存在业务路由漏分支

**发生日期**：2026-08-12（UTC+08:00）
**来源 Milestone**：MAP-DATA-A-R2-F2-F2 / F2-F2-F1
**最终功能收口 Commit**：`3d53de05c49c958cd1821303f8c6e302c2abe2ef`
**影响**：点击图层“删除”后主 Avalonia UI 失去输入，而 Vulkan 视口仍可操作；确认界面不可见。

### 已确认事实

1. 主窗口 `DialogCard` 覆盖 `VulkanNativeHost : NativeControlHost` 的 Native HWND 时，Dialog 逻辑 Active、Esc 可取消，但 Native HWND airspace 使卡片不可见。
2. 普通删除首先改为 Editor Owned Avalonia Window；随后真机仍失败。
3. P1 Runtime Probe 的决定性证据是 `REQUEST_RECEIVED name=解除注册数据集`：Dataset-backed 图层并不进入普通“删除图层”分支，而是走旧 `ShowDanger()` Overlay/DialogCard。

### 最终修复

- 普通删除与 Dataset-backed 解除注册共用独立 Owned Confirmation Window；
- 领域语义保持不同：普通 Layer 删除，Dataset 从当前地图解除注册且磁盘文件保留；
- 两条路径在确认前捕获稳定 `LayerId`，确认后按 ID 重解析目标；
- 一次性 Runtime Probe 在得到路由证据后删除，未进入正式提交。

**经验提升**：K-NATIVE-001、K-VAL-002、K-DATA-003、L-VAL-001。

---

## INC-2026-06-24-001 Editor Composition 初始化顺序导致启动崩溃

**发生时间**：2026-06-24 11:45（changelog）
**版本**：`v0.1.7.1-fix`
**Commit**：`359e3cee71f08b9a683753f089d53f01b4c5e7b2`
**影响**：Editor 无法正常启动。
**现象**：退出码 `-1073741819 / 0xC0000005`，表象类似 native AccessViolation。
**根因**：`ProjectBootstrapRoute` 构造时引用 `ctx.HierarchyRoute`，但后者尚未初始化，实际是 NullReferenceException。
**最终修复**：调整 Composition Root 构造顺序，让 `HierarchyRoute` 在依赖它的 Route 前完成。
**经验提升**：K-ARCH-001。

---

## INC-2026-06-25-001 Gizmo Preview 高频路径被 UI/Diagnostics 重工作拖慢

**发生窗口**：2026-06-25 00:18 ～ 22:56（UTC+08:00）
**版本链**：`v0.1.8.7-fix` → `v0.1.8.8-fix` → `v0.1.8.9-fix`
**关键 Commit**：`26f2006`（首轮修复；完整 SHA 待本地 Git 补证）
**影响**：Move Gizmo 拖动帧负载过高，诊断代码本身存在卡顿风险。
**根因**：TransformPreview 曾刷新 Inspector/Diagnostics/PickSnapshot；首轮优化后，Frame Complete 仍残留 Diagnostics refresh 路径。
**最终修复**：Preview 只保留轻量渲染链，Commit 才写 World/UI；Probe 验证 Preview 中相关重操作为 0。
**经验提升**：K-PERF-001。

---

## INC-2026-06-26-001 Native Viewport Mouse Capture 未真实释放

**发生日期**：2026-06-26（changelog 未记录时分）
**关键 Commit 时间**：2026-06-26 09:42:31（UTC+08:00）
**版本**：`v0.1.8.10-fix`
**Commit**：`8d6e7fd9ef6f430c0888f83e3dd8b1901501d741`
**影响**：Native Viewport 可能继续吞鼠标；UI 点击无反应、Gizmo hover 有反馈但拖不动、窗口关闭卡顿。
**根因**：`WM_MBUTTONUP` 只清内部状态，没有可靠 ReleaseCapture；Release 又过度相信内部 `_captured`，缺少 WM_CANCELMODE / Destroy 等兜底。
**最终修复**：Capture API 集中管理，以 GetCapture() 为真实依据，覆盖 ButtonUp/CancelMode/KillFocus/Destroy/Dispose；CaptureChanged 仅同步。
**经验提升**：K-INP-002。

---

## INC-2026-08-01-001 背景全屏三角写 Depth 遮挡静态模型

**发生时间**：2026-08-01 16:56:53（UTC+08:00）
**版本**：`v0.2.21.21-fix`
**Commit**：`e0a994ae11b7d7a2c383d3e4a6e4100385c46ecf`
**影响**：模型不能完整显示，继续缩放后才出现。
**根因**：主管线启用 DepthTest/DepthWrite 后，背景三角仍写 `z=0.98`，先占深度。
**修复演进**：先把背景 depth 改为 1.0；`v0.2.22.0-rz` 再建立天空专用 `DepthTest=Off / DepthWrite=Off` Pipeline。
**经验提升**：K-REN-003。

---

## INC-2026-08-02-001 Real GLB BaseVertex 被重复应用并触发 GPU 失败/日志风暴

**发生时间**：2026-08-02 12:45:00（UTC+08:00）
**版本**：`v0.2.21.23-fix`
**Commit**：`a9c1ec6c302dce5efec2215931eafb58eb9b4f75`
**真实资产**：`german_ss_soldier_mp40.glb`；211,517 Vertices / 926,148 Indices。
**现象**：导入前半段成功，GPU 创建失败：`non-zero BaseVertex not supported`；同一错误后续反复刷屏。
**根因 A**：索引已做 `localIndex + baseVertex` 全局化，但 Primitive 元数据仍留非零 BaseVertex。
**根因 B**：失败没有 Key+Revision 记录，每次投影更新都重试。
**最终修复**：Normalize 后 `BaseVertex=0`；新增 `VulkanStaticModelFailureTracker` 负缓存。
**经验提升**：K-ASSET-001、K-ASSET-002。

---

## INC-2026-08-02-002 托管资产覆盖保存需要可回滚事务

**确认时间**：2026-08-02 14:10:00（UTC+08:00）
**版本**：`v0.2.21.24-rz`
**Commit**：`e0893253a4d7bf27dbcdb5a8f3d308aef9be583d`
**风险**：若覆盖正式 `.xyassets` 时中途失败，可能破坏旧资产根。
**解决**：建立 Prepare/Activate/Complete/Rollback 状态机，staging 与 backup 保证旧数据优先恢复；路径策略防逃逸。
**经验提升**：K-DATA-001。

---

## INC-2026-08-02-003 Scene Load 若直接修改当前状态会导致失败污染

**确认时间**：2026-08-02 15:30:00（UTC+08:00）
**版本**：`v0.2.21.25-rz`
**Commit**：`cafe400fff6a1dde179d011ec14ddc9dfb3a5724`
**风险**：结构错误在加载后半段才发现时，当前场景可能已经被部分替换；单资源缺失又不应让整个场景报废。
**解决**：Candidate World/Catalog/Resources 构建完成后一次 Commit；结构失败整场拒绝且旧状态不变；资源 Missing/Failed 使用占位并保留实体语义。
**经验提升**：K-DATA-002。

---

## INC-2026-08-09-001 LayerPanel 冷启动错位与拖拽热区过小

**根因确认**：2026-08-09 16:18:16（UTC+08:00）
**版本链**：`v0.2.24.49-fix` → `v0.2.24.50-fix`
**最终 Commit**：`60fd339`
**现象**：冷启动布局不稳定，某些操作后恢复；拖拽难以命中。
**根因**：ScrollViewer 横向无限测量使 `*` 列失去合理宽度；拖拽事件直接绑定 14 DIP Path。
**最终修复**：限制横向滚动/保持 Stretch、Auto/Auto/* Grid；24×28 透明 Border 作为热区；新增 Avalonia.Headless Runtime UI Gate。
**经验提升**：K-UI-001、K-VAL-002。

---

## INC-2026-08-10-001 Region Tool 与 Navigation Gizmo 争夺同一 Pointer 手势

**确认时间**：2026-08-10 11:48:28（UTC+08:00）
**版本**：`v0.2.25.9-fix`
**Commit**：`d621755`
**现象**：Region 激活后点击/拖动 Gizmo 可能误加 Region 点或 Move 路径被 Region Preview 抢走。
**根因**：多个输入链各自消费 Down/Move，没有完整手势唯一 Owner。
**最终修复**：恢复并统一输入仲裁与会话清理；`v0.2.25.15-stab` 再统一 Gizmo 命中/所有权。
**经验提升**：K-INP-001。

---

## INC-2026-08-10-002 Region Overlay 世界 Z 偏移与多套 Depth Workaround 叠加

**事故收口窗口**：2026-08-10 13:37:23 ～ 2026-08-10（`v0.2.25.17-stab` 原文未记时分）
**版本链**：`v0.2.25.13-rz` → `.14-fix` → `.15-stab` → `.17-stab`
**关键 Commits**：`ef12f4b`、`8c8dfdd`、`751da52`、`c307c66`
**问题**：Stroke 曾用 `BaseHeight + 0.03` 世界偏移制造层级；随后又尝试 Clip-Z Bias。
**最终修复**：世界锚点统一；建立独立无 Depth Overlay Pass + Fill→Stroke→Marker；删除过期 Clip-Z Bias。
**经验提升**：K-REN-001、K-REN-002。

---

## INC-2026-08-10-003 大尺度 Picking 单精度产生 W=0 / >1 DIP 往返误差

**确认时间**：2026-08-10 12:20:03（UTC+08:00）
**版本**：`v0.2.25.12-rz`
**Commit**：`0594c4c`
**影响范围**：10,000～10,000,000m 场景、多 DPI、斜视相机。
**根因**：Screen→Pick→World→Screen CPU 路径存在单精度计算。
**最终修复**：CameraState/ViewportState 双精度投影与射线，建立 108 项跨尺度/角度往返门禁。
**经验提升**：K-SPA-001。

---

## INC-2026-08-10-004 比例尺 Native Overlay 假验证：窗口层级问题叠加 App 输出副本未同步

**收口时间**：2026-08-10 16:51:42（UTC+08:00）
**版本链**：`v0.2.25.17-stab` → `v0.2.25.18-stab`
**最终 Commit**：`06b26e9`
**现象**：自动验证与源码显示已修，但用户真机仍看不到/看到旧行为。
**根因组合**：Native Overlay 真实 HWND 层级需要从 sibling 模型继续调整；同时 App 输出副本未同步，造成验证对象与用户运行对象不同。
**最终修复**：比例尺变为主窗口 owned `WS_POPUP`；新增 HWND/Visible/Rect/Text/WM_PAINT 探针；修复 App 输出同步；重启 Editor 后真机看到 `100 m`。
**经验提升**：K-VAL-001、K-NATIVE-001、K-VAL-002。

---

## INC-2026-08-10-005 历史版本号重复导致追溯歧义

**审计标识**：`SHR-2026-08-R2`
**审计时间**：2026-08（原始注记未登记具体日与时分）
**涉及版本**：`v0.2.16.2-rz`、`v0.2.17.8-rz`、`v0.2.20.19-fix` 均出现重复分配；另有 18 处版本号/日期非单调。
**处理原则**：历史原文保留，不重排；追溯以 Commit Hash 为准。
**经验提升**：K-GOV-001。

---

## INC-2026-08-10-006 World Reference Grid 与 MapGround 错误耦合导致持续闪烁排障

**发生窗口**：2026-08-10 22:34:00 ～ 23:50:35（UTC+08:00）
**版本链**：`v0.2.25.26-fix` → `v0.2.25.29-fix`
**关键 Commit**：`c1451df`、`2c57893`、`6154078`
**影响**：World Grid 在缩放、低角度观察时出现闪、抖、断与密度不稳定；Region 周边视觉也可能被误判为自身不稳定。

### 已确认事实

- 旧 Grid 为世界空间 LineList，使用 `DepthTest=On`、`DepthWrite=Off`、`LessOrEqual` 与负 Depth Bias；
- Grid 的旧平面语义依赖 Map BaseHeight，与 Ground 存在深度承载耦合；
- Ground 隔离实验改变了旧 Grid 表现，World Axis 在相同条件下连续缩放稳定；
- RW-2A 采用独立 Fullscreen Triangle、世界射线与 World XY（Z=0）求交、DepthTest/DepthWrite 关闭后，Ground ON/OFF 均可独立显示；
- RW-2B 采用 CPU 全帧唯一 Step、1/2/5 序列与 24~80 DIP 回滞，真机通过；Region 同时观察到不再闪烁。

### 高置信机制解释（尚未直接 GPU 捕获证明）

共面 Depth 竞争与世界空间 1px LineList 亚像素覆盖变化共同造成主要闪烁。该解释没有 GPU Capture 的直接证明，故不作为已确认根因事实。

### 最终收口

World Reference Grid 被重新定义为独立 Editor Environment Layer：不属于 Map Surface，不读取 Map BaseHeight，不依赖 Ground Depth；LOD/Step 只由 CPU 全帧统一决定，Fragment 的 `fwidth` 只用于 AA。

**经验提升**：L-REN-001、K-REN-004（并关联 K-REN-001、K-REN-002）。
