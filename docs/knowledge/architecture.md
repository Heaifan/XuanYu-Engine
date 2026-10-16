# Architecture 架构知识

## K-ARCH-002 产品模式持续膨胀时先建立 Workspace 边界

**状态**：Active
**优先级**：P1
**证据等级**：E1
**标签**：Editor、Workspace、Scope、Migration、Ownership
**适用范围**：一个编辑器工具同时承担多个独立产品模式、面板上下文或输入/渲染流程时。

**首次确认**：2026-08-11（UTC+08:00）
**版本**：`v0.2.25.33-fix`（MAP-A 战略收口基线）
**Commit**：`6724079`
**最近验证**：`v0.2.26.0-rz` / EDITOR-A-R1 Workspace Contract / `4cabf42`
**来源**：`MAP-A-R3-D2-F1-CLOSEOUT`、`R3-backlog.md`、MAP-A → EDITOR-A 单轮过渡计划。

### 问题

Map Editor 同时承载地图上下文、图层、Region Tool、Pointer、Picking、Draft、Render、Commit 与 History 时，产品模式的边界会被工具栏入口掩盖；失败发生后，局部修补很难判断应修改输入、领域、渲染还是产品归属。

### 根因

功能按“继续向一个工具追加入口”组织，而不是先明确 Workspace 身份、上下文插槽和切换时谁拥有临时工具状态；Region Drawing 的未验收路径因此与 Map Editor 的既有职责长期耦合。

### 工程规则

当一个工具持续吸收独立产品模式时，应先建立最小 Workspace Contract：Workspace Identity、布局上下文身份、唯一 Current Workspace Owner、Enter/Leave/Switch 与上下文保留不变量。稳定的 Domain、Camera、Picking 和 Renderer 合同优先迁移复用；Workspace 不得复制它们的权威状态。

### 禁止做法

- 仅靠继续增加 Toolbar/Panel 开关来表示独立产品模式；
- 为切换 Workspace 新建第二份 World、Camera 或 Selection 权威状态；
- 因产品归属调整而重写已验证的 Renderer、Picking 或 Domain 合同；
- 把旧路径的真机 FAIL 改写为新路径已通过。

### 正确做法

1. 先冻结旧路径的真实验收状态和迁移目标；
2. 建立纯 Editor 层的 Workspace 身份、定义和唯一 Manager；
3. 将临时 Tool 状态在切换边界结束，World/Camera/兼容 Selection 继续由既有 Owner 持有；
4. 先用合同测试证明切换不变量，再单独实施可见 Workspace UI 或 Region 能力。

### 真实历史示例

2026-08-11，`MAP-A-R3-D2-F1` 保留 `FINAL ACCEPTANCE FAILED · 5 ITEMS REMAIN`。用户批准旧 Region Drawing 产品路径以 `SUPERSEDED · NOT ACCEPTED` 终止并迁移到 `REGION-A`，同时要求同一 Transition Round 建立 `EDITOR-A-R1 Workspace Contract`，而非继续在 Map Editor 内修补 Region UI。

### 未来应用示例

当 Road Editor 或 Terrain Editor 需要独立 Toolbar、左右面板和主内容时，先注册独立 Workspace，并复用 World、Camera、Selection 和 Render Snapshot；不要向 Map Editor 添加更多条件分支或复制状态。

### 验证方法

- Workspace Manager 的默认、双向切换和重复切换回归；
- Current Workspace 只有一个 Owner；
- Workspace 层不依赖 Vulkan，且不保存 World/Camera 的第二份可写状态；
- 可见 UI 阶段另行进行真实窗口/输入验收。

### 适用边界

只有单一产品模式内的微小命令或面板显示变化，不应为了形式引入 Workspace。Workspace Identity 不是持久化 Schema，也不替代 Domain 的事实所有权。

**关联 Lesson**：L-ARCH-001
**关联 Knowledge**：K-VAL-002、K-INP-001、K-REN-001、K-REN-002

---

## K-SPA-001 大地图 Screen↔World CPU 链必须使用双精度并做往返验证

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：Large World、Double Precision、Picking、Projection、DPI
**适用范围**：地图编辑器 CPU 投影、射线、Picking、屏幕尺度、测距、相机辅助算法。

**首次确认**：2026-08-10 12:20:03（UTC+08:00）
**版本**：`v0.2.25.12-rz`
**Commit**：`0594c4c`
**来源**：`changelog.md` / MAP-A-R3-D2-F1 Metric/Picking 精度门禁

### 问题

10,000～10,000,000m 大尺度地图、斜视相机与高 DPI 组合下，单精度 CPU 投影曾出现 `W=0` 和 Screen→World→Screen 超过 1 DIP 的误差。只验证“Pick 返回非空”无法发现坐标已经漂移。

### 工程规则

玄域地图编辑相关 CPU 空间计算默认使用 `double`。GPU 顶点阶段可以在明确边界转换为 float，但 CPU 的相机状态、投影、逆矩阵、射线构造和关键空间判断不得因为方便复用 GPU float 结构而降精度。

### 验证规则

空间功能必须优先建立往返不变量：

```text
Screen A
→ Ray / Pick
→ World P
→ Project
→ Screen B

|A - B| <= tolerance
```

并跨尺度、DPI、投影视角覆盖。

### 真实历史示例

`v0.2.25.12-rz` 将地图 Screen→Pick→World→Screen CPU 路径改为基于 `CameraState` / `ViewportState` 的双精度投影与射线构造，并加入 100m、10km、10,000km、多 DPI、正交、45°、80°自动回归，共 108 项 Metric/Picking 验证。

### 未来应用示例

新增“地图标尺点击生成测距点”时，测试不能只断言点被创建；必须检查原屏幕点投到地面后再投回屏幕仍在允许 DIP 误差内，并覆盖 100m 到 10,000km 尺度。

### 禁止做法

- CPU 侧为了直接复用 Shader 矩阵全部改用 float。
- 只用近距离俯视测试证明 Picking 正确。
- 把大尺度误差通过放大 Pick 半径掩盖。

### 边界

纯 GPU 视觉效果不自动要求 double；本条约束的是决定用户编辑语义的 CPU 空间链。

**关联 Incident**：INC-2026-08-10-003
**关联 Knowledge**：K-SPA-002、K-REN-001

---

## K-SPA-002 斜视 Metric 具有方向性，计算失败时保持上一合法状态

**状态**：Active
**优先级**：P1
**证据等级**：E1
**标签**：Metric、Camera、Oblique View、Fail-closed、Scale Indicator
**适用范围**：比例尺、Zoom Floor、屏幕世界尺度、斜视地图。

**确认日期**：2026-08-10（原始 changelog 未记录时分）
**版本**：`v0.2.25.17-stab`
**Commit**：`c307c66`
**来源**：`changelog.md` / MAP-A-R3-D2-F1 STAB-4A/4B/4C

### 问题

在斜视相机下，“1 DIP 对应多少米”不一定是单一、与方向无关的标量。把 X/Y 都强行映射成一个 `MetersPerDip`，会让比例尺或 Zoom Floor 在低角度出现不安全的尺度判断。

### 工程规则

当投影关系具有方向性时，模型必须显式保存 X/Y 或其它必要方向值，消费方根据语义选择。数值计算失败时，不应把非法值继续写入相机或把尺度退化成 0；应保持上一合法状态并暴露失败证据。

### 真实历史示例

`v0.2.25.17-stab` 将 Viewport Metric 拆为 X/Y 方向值：比例尺消费 X，Zoom Floor 取更安全的较小方向；Metric 失败时保持上一合法相机，而不是继续推进非法缩放。

### 未来应用示例

未来实现“屏幕上 100 px 的战线长度换算世界距离”，若摄像机 80° 斜视，应明确测量方向。横向 100 px 与纵向 100 px 可能对应不同世界距离，不能无条件共用一个值。

### 验证方法

至少覆盖俯视、45°、80°等角度；对 X/Y 分别断言；注入无法构造合法射线的情况，确认相机/缩放状态不被 NaN、0 或 Infinity 污染。

### 2026-10-16 Terrain ScaleBar 必须投影到当前渲染地表（STRENGTHEN；E1 不升级）

**来源任务**：XYC-WP2-P4-R7
**版本 / Commit**：`v0.3.0.23-fix` / `cb5d2e92de1c5a59431c77e08debb24f5e90b382`

#### 问题与根因

地形场景曾把固定 `BaseHeightMeters` 作为 ScaleBar 的水平参考面。该值不代表当前视口下方的 DEM Render Heightfield 表面，因此斜视和近地状态下，标尺端点计算的距离可能与可见地形局部尺度不符。

#### 工程规则

- 地形上下文应分别将 ScaleBar 两端屏幕射线投到当前渲染 Heightfield 三角面，再按两交点的世界距离计算标尺；查询需复用 Render Surface 的坐标和高度定义，并应用当前 Vertical Exaggeration。
- NoData、覆盖范围外或交点无法收敛时，隐藏未能确认的标尺；不得静默回退到 `BaseHeightMeters` 平面。非地形场景可继续使用明示的参考平面路径。
- ScaleBar 地表投影距离、Camera-to-Pivot 距离、Camera-to-Terrain Clearance 是不同量，记录和验收时不得相互替代。
- 亚米标签可以保留小数，但显示精度只表示计算出的屏幕地面投影距离，不表示 DEM 源分辨率、网格几何精度或相机离地高度。

#### 验证与边界

ScaleBarTruthfulness 及相关 Metric / Viewport 契约测试 25/25 PASS；Editor App 与 XYC CLI Release 构建均为 0 Warning / 0 Error。独立 Windows Vulkan Editor 加载 NASADEM `n24e121`（SHA-256 `BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1`），RTX 3060、776×650、DPI 96；真实 Swapchain Present Readback 关联到目标 CameraRevision。回放取得 500 m、200 m 与近地 Orbit 图像，后者比例尺显示 `0.02 m`。产物目录：`artifacts/diagnostics/wp2-p4-r7-ground-quality/20261016-023-final-6941ee0cc18c4d81a0812c09998c7259/`。BuildIdentity 为 `HEAD=1d9d367fd6cbb0a8956505cbdf37c2ce8e78fe46;DIRTY=YES`，运行代码及版本源随后由上述 Commit 固化。

`0.02 m` 仅是当前局部地面投影的 ScaleBar 距离，不证明厘米级地形精度或相机净空。近地平面化、513/1025/2049 同姿态画质差异、历史异常截断与用户 P4 仍未解决或确认；生产采样密度保持 513，LOD 保持 Auto。近切线视角的交点数值稳定性仍需单独验证。

**关联 Knowledge**：K-SPA-001、K-XYC-002、K-REN-005

---

## K-SPA-003 Cursor-Anchored Zoom Capability 必须保持 Screen→Surface 权威，Pure Dolly 不得修改 Pivot

**原始标题（2026-10-03）**：Cursor-Anchored Zoom 必须保持 Screen→Surface 权威，Pure Dolly 不得修改 Pivot
**状态**：Active
**Editor 全局适用解释**：SUPERSEDED
**Superseded By**：K-SPA-004
**优先级**：P0
**证据等级**：E1
**标签**：Camera、Zoom、Dolly、Terrain、Picking、Screen Anchor、Surface Authority
**适用范围**：Gameplay 或明确要求 Screen-Anchored Navigation 的交互；DEM Terrain、ReferencePlane fallback、Surface Source、Terrain Tile Edge。

**上下文边界**：本条约束 Screen→Surface Anchored Dolly capability，不再定义所有 Editor Wheel Zoom 的产品策略。原先将本条解释为“Editor Wheel 必须使用 Cursor Anchor”的全局规则，已由 K-SPA-004 supersede；Cursor Anchor capability 保留并继续有效。

**首次确认**：2026-10-03
**来源任务**：DEM-ZOOM-ANCHOR-DRIFT-R1
**Branch**：`feat/v0.3-world-authoring-r1`
**代码 Commit**：待补证（修复尚未提交；验证基线 HEAD=`a3d5787ba7c4a52821cb32f0bc895b1334f1daf9`，共享 Workspace Dirty=YES）

### 问题

真实 DEM 导入后，Wheel Zoom 会让鼠标所指地形点发生屏幕漂移。修复前，Wheel 输入本来携带 Screen XY，但 `UiVmD1Handler.HandleWheel` 丢弃坐标并调用无光标参数的 `DollyCamera`；同时 Anchored Dolly 又同步平移 `ObservationCenter`，把纯缩放错误地变成了 Pivot / Focus mutation。Terrain 越界查询还会抛异常，令 SurfaceSource fallback 缺少显式状态。

### 根因

Zoom Anchor 的语义权威没有贯穿完整链路：输入层丢失了 Cursor XY，而 Camera mutation 又混入了 Orbit Pivot 职责。结果是“有 Wheel 路由”和“有 Dolly”都成立，但固定 Screen Anchor、Surface Source 与 Camera 不变量没有形成单一合同。

### Capability 规则

Cursor-Anchored Zoom 的唯一权威链固定为：

```text
Wheel Screen XY
→ GroundPickResult
→ SurfaceSource + World XYZ
→ Pure Dolly
```

- 有效 Terrain Hit 时，必须使用完整 Terrain World XYZ；不得把 Z 归零，也不得再由 ReferencePlane 覆盖。
- Terrain 无效或越界时，必须返回可判定状态，再显式 fallback 到逻辑 ReferencePlane；不得用异常或静默默认值表达 Surface Source。
- Pure Dolly 只允许修改 Camera Position。Forward、Up、FOV、Orbit Pivot、Focus、Target 与 RenderOrigin 必须保持不变。
- Zoom 不是 Orbit，不得通过 `LookAt`、重算 Target、平移 ObservationCenter 或回到 DEM 中心来补偿漂移。
- 输入路由不仅要“接通”，还必须把决定交互语义的 Screen XY / Wheel Delta 等 payload 原样保留到最终 Consumer。

### 禁止做法

- Handler 接收到 Cursor XY 后改调无光标 Dolly；
- Terrain Hit 有效时 fallback 到 ReferencePlane 或强制 Z=0；
- Anchored Dolly 同步平移 ObservationCenter / Pivot / Focus；
- 用 smoothing、magic offset、经验比例或每帧 recenter 掩盖 Anchor 漂移；
- 把 Terrain OutOfBounds 作为异常路径，让调用方猜测是否 fallback。

### 验证方法

固定同一 Screen XY，执行 Zoom In/Out round trip，并同时验证：

```text
Screen XY → Surface P0
Zoom
Screen XY → Surface P1

World XY error
World Z error
Screen reprojection error
Forward angular error
Pivot / Focus invariance
SurfaceSource invariance
```

至少覆盖平地、山坡、高差区域、Tile Edge 与 DEM 外 ReferencePlane 区域；多 Tile、LOD Surface Resolver 或 RenderOrigin 变更后必须重新验证。

### 本轮证据

修复前固定屏幕点误差为 `18.1115 px`，Pivot 从 `(3,2,4)` 变为 `(2.7,2.9,3.55)`。修复后 ReferencePlane 对照序列最大屏幕误差约 `0.000852 px`，In/Out round-trip Camera Position 误差 `8.35e-6`，Forward angular error `≤1e-9`；真实 HGT 集成测试中 Terrain World XY / Z error 均 `≤1e-5`，有效 Terrain Hit 未被 ReferencePlane 覆盖。Core Camera/Render/Map/Vulkan 373/373、DEM/Zoom/Viewport 17/17、真实 HGT Cursor Anchor 1/1 PASS。P4-C 真实用户操作验收仍为 PENDING，因此本条保持 E1，不宣称完整 Product Closure。

**关联 Knowledge**：K-SPA-001、K-INP-003、K-VAL-002
**关联 Lesson**：L-REN-002

**Governance Supersede**：事件 `EDITOR-ZOOM-AUTHORITY-SCOPE-SUPERSEDE-R1`；原 Editor 全局适用解释已由 K-SPA-004 取代。原事故、根因、验证数值与 capability 规则保留。

---

## K-SPA-004 Editor / Gameplay Zoom Authority 必须按产品上下文分离

**状态**：Active
**优先级**：P0
**证据等级**：E1
**标签**：Camera、Navigation Center、ObservationCenter、Editor Zoom、Gameplay Zoom、Orbit、Product Policy
**适用范围**：Editor Navigation；Gameplay 或明确要求 Screen-Anchored Navigation 的交互。

**确认日期**：2026-10-05
**来源任务**：`GOVERNANCE-SUPERSEDE-K-SPA-003-EDITOR-NAVIGATION-R1`
**Effective Baseline**：`4277b242391e273080c4c19b82c9acf53b348843`（用户报告为 LAST USER-VERIFIED GOOD，P4 PASS）
**Supersedes**：K-SPA-003 将 Cursor-Anchored Zoom 提升为全局 Editor Wheel Zoom policy 的解释；不取代 K-SPA-003 的 Screen→Surface capability。

### 问题

K-SPA-003 的 Screen→Surface Zoom 技术 capability 曾被提升为通用 Editor Zoom Authority。技术能力存在，不代表所有产品上下文都必须使用该能力。Editor Navigation 与 Gameplay / 显式 Screen-Anchored Navigation 必须分开定义。

### Editor 产品合同

- Editor Navigation Center 的权威是当前 `ObservationCenter`。
- 默认 Editor / Empty World 的 `ObservationCenter` 为 World Origin `(0,0,0)`。
- Editor Wheel Zoom 与 Orbit 共用当前 `ObservationCenter`；Zoom 不依赖鼠标位置，不执行 GroundPick、Surface Resolve 或 viewport ray repick。Dolly 地形净空可按下方 Scope Clarification，对候选相机路径位置做只读 Surface 查询；不得用它选择导航中心。
- Orbit Begin 冻结当时的 `ObservationCenter` 为该 Session Pivot；Orbit Move 不重新解析 Surface 或刷新 Pivot。
- `ObservationCenter` 只能由已定义的 Center Authority Event 改变：Empty World 初始化设为 World Origin；初次建立内容并执行 Initial Content Establishment Auto Frame 时，设为该 framing 的中心；Pan、Explicit Focus、Explicit Frame / View command 及其它正式授权的 View Reframe 操作可设为各自结果中心。
- 普通 Zoom、Orbit、Selection、Terrain LOD、Terrain refresh 与普通 Render update 不改变 Navigation Center。新内容的初次建立若触发正式 Auto Frame，是明确的 framing authority event，不属于任意数据状态变化；后续普通数据/渲染变化不得隐式夺取中心。
- Editor Wheel 与 Orbit 仍只读取当前 `ObservationCenter`；内容建立后，它们继承 Auto Frame 产生的新中心，不重新使用鼠标、GroundPick 或 Render Surface 选择中心。

### 2026-10-05 治理澄清

原文“DEM Import 本身不得改变 Navigation Center”范围过宽，遗漏了 4277 中已验证产品流程包含的 Initial Content Establishment Auto Frame。该规则现澄清为：普通数据状态变化不得任意修改 Navigation Center；正式的初次内容建立自动取景与显式 View Reframe 是合法 Center Authority Event。此澄清不恢复 Cursor-Anchored Editor Zoom，也不改变 K-SPA-003 capability 的范围。

### Gameplay / 显式 Screen Navigation capability

Gameplay 或明确要求 Screen-Anchored Navigation 的交互可以使用 K-SPA-003 capability：

```text
Screen XY → GroundPickResult → SurfaceSource + World XYZ → Anchored Dolly
```

Screen XY 可以来自 Mouse Cursor。Router 必须保留决定交互语义的输入 payload；有效 Surface 命中、SurfaceSource 与 fallback 必须明确。该 capability 不构成 Editor Wheel policy，也不得作为 Editor 使用 Cursor Anchor 的强制条件。

### 2026-10-10 Scope Clarification：候选相机只读净空查询

用户产品权威裁定：允许 Editor Wheel 在应用 Dolly 地形约束时，对**候选相机位置**执行狭义只读 Surface 查询，以检查相机净空。该查询只返回净空约束所需数据，不得执行 GroundPick、重选导航中心或改变 `Anchor`、Orbit Pivot、`ObservationCenter`；Editor Wheel 的缩放权威仍是当前 `ObservationCenter`。

该许可不等同于批准任意 Terrain Query。实现必须固定被约束的 Surface（源高度场、TerrainWorld 查询面或渲染 LOD 面不得混称），并定义 NoData、无效数据、查询失败、垂直夸张及边缘/采样策略；失败时不得静默改用未经声明的平面或另一 Surface。当前 Wave 2 CPU 合成测试不能证明真实 DEM 或 Vulkan 可见表面的净空，真实 Runtime/P4 仍独立验收。

此澄清限定 K-SPA-004 中“Editor Wheel 不执行 Terrain Query”的范围，不恢复 Cursor Anchor，不修改 ObservationCenter Authority，也不追认未验证的 Surface 等价关系。

### 2026-10-10 XYK Strengthen：起点入地时的 Zoom-Out 脱困

**Problem**：Editor Camera 的起点已经处于有效地形内部时，地形净空约束可能导致 Zoom-Out 无法移动。

**Root Cause**：旧的 `ConstrainDollyToTerrain` 在候选路径首次触发不安全判断时停止，保留 `lastSafe=start`，阻止相机脱困。

**Evidence**：`XYEPR2-NAV-SAFETY-INSIDE-001` 修复前 1 FAIL、修复后 1 PASS；Evidence Level 为 CPU 确定性回归，Commit=`5b464dcd219cf07d1569c56e5dbd05e6656beed2`，不等同于 Windows/Vulkan P3。

**Rule**：当相机起点已经处于已声明 Terrain Surface 的无净空状态时，普通 Editor Zoom-Out 应具有向外恢复能力。恢复过程必须保持 `ObservationCenter`、Zoom Anchor 和 Orbit Pivot 权威不变。相机开始向外移动不代表已经脱离地形；完整安全保证需要额外证明最终有效净空、路径状态及必要的失败处理。

**Known Limits**：64 段路径采样、NoData、Tile 边缘、高山脊、Render LOD 和真实 Vulkan 可见表面尚未完整证明。

**Suggested Action**：本条作为 K-SPA-004 的 STRENGTHEN 增量，不新建重复 Knowledge ID；继续为窄峰、边缘、NoData、真实 NASADEM/HGT 和 Vulkan 可视表面补充独立证据。

### Governance Supersede Record

- 事件：`EDITOR-ZOOM-AUTHORITY-SCOPE-SUPERSEDE-R1`
- 原因：产品上下文此前未区分 Editor 与 Gameplay，导致 capability rule 被误提升为全局 product authority。
- 决策：Scope Split / Supersede。K-SPA-003 的 Editor 全局 policy 解读由本条替代；其技术 capability 与历史事故证据保留。
- Effective Baseline：`4277b242391e273080c4c19b82c9acf53b348843`
- 批准：USER PRODUCT AUTHORITY + CHATGPT KNOWLEDGE AUDIT
- 日期：2026-10-05
- Version Event：NONE

### 替代证据与边界

- 用户报告 4277 (`fix: reconcile frozen orbit pivot contract`) 已通过 P4：DEM Import、DEM Orbit No-Refresh、DEM Orbit No-Flicker、Grid、ScaleBar、Pan 与 Navigation Chain 均 PASS。该验收事实来自用户授权/报告；本治理改动不伪称重新执行 P4。
- 用户明确决策：Editor Zoom / Orbit 共享稳定 Navigation Center；默认是 World Origin，Pan / Explicit Focus 后使用新的 ObservationCenter。
- 用户报告恢复候选 `fa55953a`、`d10adb31`、`8fb7e8ae`、`f75badb4` 均为 P4 FAIL；这些失败候选不用于定义本条合同。
- K-SPA-003 的 Screen XY payload、GroundPick SurfaceSource、完整 Screen→Surface capability 与有效命中不得被 fallback 静默覆盖等工程知识继续有效。

**关联 Knowledge**：K-SPA-001、K-SPA-002、K-SPA-003、K-VAL-002
**关联 DEC**：DEC-CAM-001

---

## K-ARCH-001 Composition Root 初始化顺序属于真实依赖合同

**状态**：Active
**优先级**：P0
**证据等级**：E1
**标签**：Composition Root、Initialization Order、Dependency、Startup
**适用范围**：Editor 组合根、Route/Service 装配、启动期依赖。

**确认时间**：2026-06-24 11:45（UTC+08:00，changelog 记录）；对应 Git Commit 时间为 2026-06-24 11:42:40（UTC+08:00）
**版本**：`v0.1.7.1-fix`
**Commit**：`359e3cee71f08b9a683753f089d53f01b4c5e7b2`
**来源**：`docs/archive/changelog/changelog-2026-06.md`、Git Commit `8.8-RZ-Fix1`

### 问题

某个服务字段“最终会被赋值”不代表构造期可以提前引用它。Composition Root 中的初始化先后关系实际上就是一张依赖图；顺序错误可在编译和大部分单测通过的情况下直接导致启动崩溃。

### 真实历史示例

`EditorShellComposition.Build()` 创建 `ProjectBootstrapRoute` 时传入 `ctx.HierarchyRoute`，但 `HierarchyRoute` 在之后才赋值。结果启动时 `hierarchyRoute` 为 null，用户看到退出码 `0xC0000005`，实际根因是 `NullReferenceException`。修复只是把 `HierarchyRoute` 初始化移到 `ProjectBootstrapRoute` 之前。

### 工程规则

若 `A` 构造需要 `B`：

```text
B = new B(...)
A = new A(B)
```

顺序就是合同。组合根不允许依靠 nullable 默认值、字段后赋值或“运行到使用点前应该已经初始化”作为隐式假设。

### 未来应用示例

新增 `RegionToolRoute` 依赖 `MapPickRoute` 与 `HistoryRoute` 时，必须先完成后两者构造再创建 RegionToolRoute；如果存在循环依赖，应重构边界，而不是把字段改成 nullable 并在运行时碰运气。

### 验证方法

- 启动冒烟必须属于组合根变更的正式验证；
- 对关键组合根可增加依赖非空合同测试；
- 构造签名优先接收已完成依赖，不在构造内部读取可能尚未赋值的共享 Context 字段。

### 注意

该历史 Commit 信息里曾记录旧时期 Warning 容忍口径；这只是历史事实，**不覆盖当前全解决方案 0W0E 门禁**。

**关联 Incident**：INC-2026-06-24-001

---

## K-XYC-001 自动化控制复用 Editor 相机权威，以执行后回读验证成功

**状态**：Active
**优先级**：P1
**证据等级**：E1（单轮代码与进程 E2E 证据；独立复跑待补）
**标签**：XYC、Agent Automation、Camera Authority、Runtime State、XYT
**适用范围**：XYC CLI、未来本机 MCP Adapter、Editor 自动化相机操作及独立测试实例。

**首次确认**：2026-10-14
**来源任务**：XYC-P0 / GitHub Issue #29
**证据 Commit**：`c16ec8b64abad9e79e204b6e19ef93ebfca44e3e`

### 已确认事实

XYC P0 在 `XuanYu.Editor.UI/Control/` 引入命令处理、标准输入输出 Host、相机 Adapter；`UiVmXycRuntime` 通过 UI Dispatcher 访问 `UiVm` 的相机状态，`WriteXycCamera` 应用 CameraState / ObservationCenter 并更新相机 Revision。Windows 进程 E2E 测试在真实 CLI → Editor App 子进程链中执行两次姿态修改，并检查再查询的坐标和 Revision。施工报告称 World.Tests 2302/2302 PASS；本次 XYK 审计核对了源码和提交，未独立运行 Windows 构建或测试。

### 工程知识

自动化 Agent 不应通过系统鼠标/键盘注入或直接写 Renderer 缓存代替 Editor 领域操作。控制 Adapter 应通过拥有写权限的正式执行线程和唯一状态所有者修改相机；命令的成功依据是执行后从同一实例的权威相机状态、ObservationCenter 与 Revision **重新读取** 的结果，而不是收到请求、发出输入或回显所要求的姿态。

### 验证方法与适用边界

- 相同实例中执行 `camera.inspect → camera.set-test-pose → camera.inspect`；至少两次不同姿态，检查实际 Position、Forward、ObservationCenter、Revision；非法输入和越权操作不得产生成功结果。
- `runtime.info` 应真实说明隔离模式、协议与渲染能力；当前 P0 声明 `windowed-editor-instance; offscreen=NOT-IMPLEMENTED`。
- 这一闭环只证明**命令/相机权威数据路径**；不证明 GPU Vulkan 图像正确、后台离屏渲染、Windows Native 输入完整性或用户 P4。GPU 视觉问题必须分别补真实 GPU 图像、Frame/Camera/Scene 身份及像素证据。
- P0 的 CLI 目前默认向测试子进程传递诊断写入参数，且测试路径使用 Windows `.exe`。默认只读与跨平台测试分层属于后续待解决的设计缺口，**不能把尚未实现的约束登记为已验证能力**。

**关联 Knowledge**：K-VAL-001、K-VAL-002、K-SPA-004。

---

## K-XYC-002 相机状态回读与窗口截图必须证明呈现帧关联

**状态**：Active
**优先级**：P1
**证据等级**：E1（2026-10-14 A1 本地运行报告；原始 E 盘截图与 manifest 待独立复核）
**标签**：XYC、Vulkan、CameraRevision、Presented Frame、Capture Freshness、Runtime Evidence
**适用范围**：XYC 自动化视口 A/B 测试、Editor Vulkan 窗口式截图、相机/比例尺/LOD/Orbit 视觉对照；不专门约束纯逻辑状态查询。

**首次证据日期**：2026-10-14
**来源任务**：XYC-R0-A1 / GitHub Issue #30 及其阶段报告
**运行案例**：`artifacts/diagnostics/xyc-r0-a1/065f0d7ae3dd45dda5b2291045810437/`（本机 ignored 诊断目录，未包含于远端提交）
**代码基线**：本地报告 `c16ec8b64abad9e79e204b6e19ef93ebfca44e3e`，包含未提交 A1 变更；本轮知识审计不能据远端基线声称新实现已验证。
**独立复核状态**：PENDING（相关 PNG 和 `run-manifest.json` 仅有本机路径；审计时尚未获取原始内容）

### 观察与尚未解释的差异

A1 报告：隔离 Editor 导入哈希核对的 NASADEM `n24e121`，以 XYC 指定 2 km 姿态并回读 CameraRevision 4，窗口 PrintWindow 截图显示 `2 km`；真实 Editor Orbit 后 CameraRevision 5、ObservationCenter 不变；20 km 状态回读 CameraRevision 6 且 ScaleBar=`20 km`，但其 PNG 与 Orbit PNG 的 SHA-256 完全一致，故**20 km 视觉证据无效**。当前尚无法确认具体根因属于截图文件复用、Windows 合成窗口缓存、采集时机、Present 关联缺失或真实渲染路径；不能据此确认 GPU 渲染器故障。

### 工程知识

**命令成功 ≠ 新渲染帧已呈现 ≠ 当前截图来自该帧。** CameraRevision/SceneRevision 回读仅证明控制路径及权威数据已更新；在视觉验收中必须说明截图与对应相机/场景状态和 GPU 帧之间的关联。对于 `PrintWindow`，必须声明 `WINDOWED / PrintWindow` 来源，不能冒称直接的 Vulkan GPU image readback 或用户 P4。

### 验证方法

1. 保留 `runId`、stage、requestId、相机 Revision、场景/Terrain Revision、视口尺寸、图像路径、图片 SHA-256、CaptureMethod 与具体捕获请求/确认时间；能取得时绑定 RenderSnapshot / PresentedFrameId 及消费的 CameraRevision；缺失事实明确为 `UNKNOWN`。
2. 当用例要求可见差异（例如 2 km 与 20 km 比例尺）时，对截图内容做可见目标校验，配合哈希/帧身份证据；在关联无法证明时保持 `PARTIAL`，不得通过固定等待毫秒或仅令帧计数变化强造 PASS。
3. **重复 PNG 哈希不是跨所有场景的通用失败条件**：真实画面可能合法完全相同。只有当预期画面语义应变化且证据仍不匹配时，才判该视觉验收用例证据无效。
4. 如果窗口式采集不能可靠建立来源链，则单独保留其窗口诊断用途，转用真实 Vulkan render-target→GPU copy/readback 证据进行更高等级验证。

### 适用边界

此条只强化 XYC 自动化**呈现与捕获证据的可归属性**；不改变 Editor Center Zoom/Orbit 权威、Renderer 深度语义或用户最终 P4 裁决，也**不构成新的开发全局门禁**。第一次证据只限用户提供的本地运行报告，后续取得图片与 manifest 后复核其实际内容。

### 2026-10-14 A1b 窗口式帧关联验证（STRENGTHEN，E1 不升级）

A1b 施工报告在真实 NASADEM `n24e121`、隔离 Editor 上完成 `20 km → 2 km → Editor Orbit → 20 km`。对四个目标 CameraRevision 均等待一致的 RenderSnapshot Revision 与 Vulkan Presented Frame，报告 `CaptureHold=True` 并在 `PrintWindow` 后释放；`QueuePresent=SUCCESS`。实际对应如下：

| 阶段 | Camera / RenderSnapshot Revision | Presented Frame | PNG SHA-256 前16位 |
|---|---:|---:|---|
| 初始 20 km | 4 / 4 | 889 | `9BD82F6C3838C73B` |
| 2 km | 5 / 5 | 891 | `7A6B58FA2B5412DB` |
| Editor Orbit | 6 / 6 | 893 | `054E2D13CB7612E7` |
| 最终 20 km | 7 / 7 | 895 | `FC77EAE378645F7C` |

四张图 SHA-256 各不相同，施工报告称逐张核对尺度与视角符合阶段预期。**本阶段结论仅为 WINDOWED 测试路径得到可信度更高的阶段对应证据，不是 OFFSCREEN PASS**；原始 PNG、`run-manifest.json` 和 `present.log` 位于本机 ignored 目录 `artifacts/diagnostics/xyc-r0-a1/8ae17cd86868403599a5ecc6adcfa648/`，本次 XYK 审计未独立读取，因此仍标注 E1 / 外部报告，待原件复核。

**机制边界：** `vkQueuePresentKHR` 成功与排队顺序不能直接证明 Windows DWM/PrintWindow 捕获的像素就是对应的 GPU 图像；捕获暂停、Revision 与呈现帧关联、图片语义核对可以增强窗口式诊断的可信度，但无法替代独立 Vulkan GPU readback、离屏 RenderTarget 或用户 P4。

**根因分离：** A1 原有 Orbit/20 km 重复 PNG 的根因仍为 UNKNOWN。A1b 另发现并修复测试脚本构造 20 km Pose 时浅拷贝连带修改 2 km Pose 的问题，不能倒推出原 A1 重复 PNG 由浅拷贝引起。相机姿态测试数据应使用互不共享可变嵌套对象的独立 Pose，并由四阶段回读/渲染结果检查其未串改。此处是**测试脚本缺陷的独立经验**，不是新产品相机根因。

**集成状态：** 本次 A1b 施工报告称全解 Build 0W0E、3562 PASS / 2 SKIP / 0 FAIL、World.Tests 2309/2309；施工代码在本机 `c16ec8b6` 的 Dirty 工作区，远端此时只有之前的 XYK 文档提交。不可将本条 XYK 记录写成 A1b 产品代码已 Commit/Push 或远端 CI 复跑通过。

### 2026-10-15 A2a GPU 独立读回实验（STRENGTHEN，E1 不升级）

本地执行 Agent 报告：在 RTX 3060 上启动三次独立进程，每次成功用**不依赖 Vulkan Surface/Swapchain 的独立 VkImage** 执行 GPU clear、image layout transition、Image→Buffer copy、Fence(5s) 等待和 host map，生成 PNG；4096 个已知像素断言均 PASS，实验项目 Build 0W/0E，报告设备资源释放正常。PNG SHA-256 为 `D274BCE646328E01303493B8DC79075B311CFB63D031E749CE2A79C65C5B2341`。

原始实验日志、PNG 和源代码位于本机 ignored `artifacts/xyc-r0-a2a-probe/`，用户报告时均**未集成到产品源码、未提交到 GitHub、未执行完整 XYT**；本次 XYK 审计未独立读取本机原件或重跑真实 GPU。工作基线 `c16ec8b6` 后方为远端三笔 XYK 文档提交（截至本证据报告），工作区有 A1/A1b owned Dirty 和两处 Foreign Dirty。此处只记录报告的单次实验结论，不升格为产品验收。

**适用知识及证据边界**：独立 VkImage 的 GPU 已知颜色图案 readback 可证明最小 Vulkan GPU→host→PNG 管线在该 RTX 3060 环境中可行；不能证明 XYE 现有 Terrain RenderPass 采用合法 TransferSrc ImageUsage、正确布局/屏障/format、与 Camera/RenderSnapshot/FrameId 关联，也不能证明真实 NASADEM Terrain 被渲染。实验未创建 Surface/Swapchain，只能证明**独立清屏读回实验不依赖窗口**，不能宣告基于 XYE Terrain RenderSnapshot 的 `TRUE_OFFSCREEN` 完成。未来接入前必须核查目标 Image 的 usage/format features、copy 执行点、GPU→host 同步与非 coherent memory invalidate（若适用），并验证真实地形像素内容、帧身份、重复运行后的资源回收。

**下一次复验点**：XYC-R0-A2 Issue #31；将实验模块通过现有 Renderer 写入范围安全集成后，在真实 `n24e121` RenderPass 输出 GPU-origin PNG，随后才在无窗口 Host 复用同一 Scene/Terrain 画面。A2a=`PARTIAL`，A2b=`OFFSCREEN_PARTIAL`；用户 P4=`PENDING`。

**关联知识**：K-XYC-001、K-REN-005、K-VAL-001、K-VAL-002。
