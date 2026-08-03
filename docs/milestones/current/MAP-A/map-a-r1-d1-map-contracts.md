# MAP-A-R1-D1：.xymap 地图合同冻结

版本：v0.2.24.0-rz
日期：2026-08-02
类型：合同冻结文档（D1 只读核查 + 冻结，零产品代码）

## 结论（冻结裁定）

- **坐标系（方案 B，用户裁定）**：`.xymap` 语义与世界轴直写，不引入映射层。
  `X`=地图横向（世界 X）、`Z`=高度（世界 Z=Up）、`Y`=地图纵向（世界 Y）。
  与官方坐标合同 `docs/architecture/world-a-r0-coordinate-contract.md`（Z-Up、XY 水平、X×Y=Z）完全一致。
  地图范围：`X ∈ [-Width/2, Width/2]`、`Y ∈ [-Depth/2, Depth/2]`；高度沿世界 Z。
- **查询合同**：输入世界 X、Y（水平面坐标），输出地表 Z 高度；地图外/未加载返回失败。
- **`.xyscene` 升级 v4**：新增可选 `mapReference`（mapId + assetPath 项目相对路径）；保存固定写 v4，加载兼容 v1–v4。
- **地表采样唯一源**：`MapSurfaceSampler` 唯一实现，World 查询与 Render 网格生成共用（禁止双公式）。
- **sampler 归属建议**：`XuanYu.Core/Map/`（纯函数、零依赖；Core 可被 World/Editor/UI/Render 全链引用）。
  等待 D1 域类型实施时由架构守卫复核，冲突即停下报告。

## 代码审计结论（2026-08-02 只读核查）

- SceneDocument：`.xyscene` 由 `SceneStorageService` 读写（原子保存=临时文件+`File.Move(...,true)`；camelCase；
  大小写不敏感读取）；`SceneDocumentValidator` 严格校验（错误码 BrokenJson/UnsupportedFormat/UnsupportedSchema/…），
  当前接受 v1–v3；`SceneDocumentMapper` 保存固定写 v3；加载走两阶段候选
  （`SceneDocumentLoadTransaction.BuildCandidateAsync`），失败整场失败、原场景不变；
  会话 Dirty 由 `SceneDocumentSession`（revision 对比）+ History revision 驱动。
- World Snapshot：`SceneStateOwner`（ISceneRenderSnapshotSource）→ `SceneRenderSnapshot`（Core/Scene，含实体+相机+gizmo，
  无地图字段）→ `SceneRenderProjectionAdapter.TryCreate` → `RenderProjection` → `RenderDrawPlan.GetFrameDrawPlan`
  → `VulkanClearFrameOwner.RecordDraw`。全库无任何地图类型。
- 渲染地面：无限灰网格 = `RenderDrawKind.EditorGrid`（252 顶点，scene.vert `gridVertex()`，±10 米 21×21 线，绘制于 z=0 平面）；
  天空 = `EditorBackground` + 深度不写第二管线 `_skyPipeline`（WORLD-D 成品，直接复用）；
  光照 = 固定方向光+半球环境光（shader 硬编码常量，不随文档变化）。
- 右侧模块：`Right.axaml` TabControl = 检查器/调试/偏好/模式 四个 Tab；MAP-A §7.1 收为「检查器 + 地图编辑器」。
- 版本源五处一致 `v0.2.23.0-rz`；HEAD cbb694b = origin tip，ahead/behind 0/0；
  唯一偏差 untracked `IDEA.md`（已知，未处理）；`XuanYu.Editor.Avalonia/` 为本地残留 bin 目录（git 不可见，未处理）。

## .xymap Schema v1（冻结）

```json
{
  "schemaVersion": 1,
  "mapId": "21e4a2d34d4a4a1eb2539eac76d412a8",
  "name": "TestBattlefield",
  "sizeMeters": { "width": 2000.0, "depth": 2000.0 },
  "coordinateSystem": { "unit": "meter", "upAxis": "Z", "origin": { "x": 0.0, "y": 0.0, "z": 0.0 } },
  "surface": { "kind": "GentleHillsV1", "baseHeightMeters": 0.0, "amplitudeMeters": 12.0, "wavelengthMeters": 400.0, "seed": 1 },
  "environment": { "skyPreset": "ClearDayV1", "sunDirection": { "x": -0.35, "y": -0.55, "z": 0.75 }, "sunIntensity": 1.0, "ambientIntensity": 0.35 },
  "layerReferences": []
}
```

### 字段语义（冻结）

- `schemaVersion`：固定 1；非 1 拒绝。
- `mapId`：**32 位十六进制，无前缀**（D2 §5.2 口径，如 `21e4a2d34d4a4a1eb2539eac76d412a8`；独立类型 `MapId`，创建后保持稳定）；保存后不变。
- `name`：非空白；长度 1–128。
- `sizeMeters.width/depth`：`100 ≤ v ≤ 10000`，有限数；越界拒绝，不自动截断。
- `coordinateSystem`：unit 固定 "meter"；upAxis 固定 "Z"；origin 固定 0/0/0（World Origin=地图中心）。
- `surface.kind`：仅 `Flat` / `GentleHillsV1`；未知类型拒绝。
- `surface.baseHeightMeters`：有限数（默认 0）。
- `surface.amplitudeMeters`：有限数且 `≥ 0`（默认 12）。
- `surface.wavelengthMeters`：有限数且 `> 0`（默认 400）；Flat 时忽略但必须合法。
- `surface.seed`：任意 int（默认 1）；同一 (seed, x, y) 永远同一高度，禁止随机。
- `environment.skyPreset`：仅 `ClearDayV1`；未知拒绝。
- `environment.sunDirection`：有限、非零、指向光源方向（光射来方向，Z 分量 > 0 朝上，R1 不强制单位化）。
- `environment.sunIntensity`：有限数且 `> 0`（默认 1）。
- `environment.ambientIntensity`：有限数且 `≥ 0`（默认 0.35）。
- `layerReferences`：必须存在且为空数组（R1 不建图层）；缺失或非空拒绝。

### 文件规则（冻结）

- 保存路径：`Maps/<MapName>/map.xymap`；不创建 layers/data/preview/cache/environment.xyenv。
- 写入：同目录临时文件 → 完整验证候选 → 原子替换（`File.Move(..., true)`）；失败保留原文件并清理临时文件。
- 候选对象完整验证通过后才替换当前地图；禁止部分读取污染当前地图状态。
- 错误反馈：明确错误码 + 中文消息（参照 SceneDocument 错误码风格：BrokenJson/UnsupportedSchema/InvalidSize/UnknownSurfaceKind/…）。

## .xyscene mapReference 合同（v4 冻结）

- `.xyscene` 新增可选字段 `mapReference { mapId, assetPath }`（JSON 小写键）。
- `assetPath` 必须是项目相对路径（如 `Maps/TestBattlefield/map.xymap`）；禁止绝对路径/盘符/`..`/UNC/反斜杠逃逸
  （复用 `SceneAssetPathPolicy` 同款策略或独立 `MapAssetPathPolicy`，冻结时二选一，倾向复用策略函数）。
- 场景不复制地图数据（尺寸/地表/天空/完整 JSON 均禁止）；唯一数据源 = map.xymap。
- 旧场景（无 mapReference）正常打开，地图状态为空。
- 引用缺失/损坏：场景主体仍可进入，地图状态显示「引用失效」，视口不生成伪地图，日志给路径+原因；
  禁止自动创建同名空地图/自动替换默认地图/静默忽略。
- MapId 不匹配（引用 mapId ≠ 文件 mapId）：明确报告。

## 模块责任边界（冻结）

- Editor.UI：地图编辑器界面、地图命令、文件选择、Dirty 展示、错误反馈。不得负责地表算法。
- World：当前地图世界状态、地图边界、地表高度查询（输入 X/Y → 输出 Z）、地图快照。不得依赖 Editor.UI。
- Render：消费地图渲染快照，生成有限地表网格、天空、基础光照、边界显示。不得直接读 .xymap。
- Storage（Editor/MapDocument）：读取、验证、保存、原子替换。不得操作渲染器。
- 依赖方向：`Editor → World / Storage`；`Render ← World Snapshot`。
- 禁止：Render 读编辑器状态、World 调 UI、Editor.UI 依赖 Vulkan 类型。

## 测试清单（冻结目标）

- 地图数据：合法创建；width=0/负 depth 拒绝；未知 surface kind 拒绝；非法光照参数拒绝；MapId 保存不变。
- 存储：Round-trip 一致；损坏 JSON 不污染当前地图；错误 SchemaVersion 拒绝；缺字段拒绝；保存失败保留原文件；临时文件清理。
- 地表查询：Flat 固定高度；GentleHillsV1 确定性；同坐标多次一致；地图内成功；地图外失败；卸载后失败。
- 场景引用：旧场景可打开；引用可保存恢复；项目相对路径；地图数据未复制；缺失文件明确失败；MapId 不匹配明确报告。
- 渲染快照：尺寸/参数正确进入快照；卸载后清空；切换地图无残留。

## 风险与止损（冻结）

- 光照膨胀 → 只做方向光+环境光+Lambert；需要阴影/PBR 立即停。
- 存储膨胀为通用资产框架 → 只建 MapDocument/MapStorageService/MapValidation。
- 查询与画面不一致 → World 与 Render 强制复用同一 MapSurfaceSampler。
- 双文档状态复杂 → 地图独立保存、场景独立保存引用；不实现跨文件联合事务。
- 尺寸精度问题 → R1 限制 100–10000；优先保证 2000×2000 默认正确。
