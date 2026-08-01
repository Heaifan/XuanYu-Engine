# WORLD-C-R4-D0：GLB 资产合同、依赖与场景 Schema 冻结

## 结论

- 批准：`SharpGLTF.Core` 1.0.6，落点 `XuanYu.Editor`，仅允许导入边界消费第三方类型。
- 拒绝：`AssimpNet` / `AssimpNetter`，原因是 Assimp 原生库引入 native binary；`glTFast` / `UnityGLTF`，原因是 Unity 包体系和运行时模型不适合作为玄域独立 .NET 编辑器依赖。
- 不新增 `.csproj`。不把 GLB 依赖引入 Core、World、Render.Abstractions、Render.Vulkan 或 Editor.UI。
- D0 不实现导入 UI、Vulkan 模型 Draw、资产浏览器、贴图系统或精确 Mesh Picking。

## 代码审计

- SceneDocument：当前 `.xyscene` 由 `SceneStorageService` 读写；保存固定写 `schemaVersion=2`，加载接受 v1/v2；JSON 严格验证后映射为候选 Snapshot；保存使用同目录临时文件后 `File.Move(..., true)` 替换；Save As 由 UI 传入目标路径，成功后 `SceneDocumentSession.MarkSaved` 切换当前路径；Dirty 来源是 History revision 与保存 checkpoint。
- Entity / World：实体真相是 `GlobalWorld -> EntityRegistry`；正式身份是 `EntityId`，不再使用 `EntityKey` 术语扩张；Transform 来自 `WorldEntitySnapshot.Transform`；Bounds 由实体局部 `Extent` 平移到 `GlobalPosition` 后进入 `WorldQuery`。
- Render：当前 `RenderProjection` 只携带实体类型、TRS、选择态与 Gizmo/辅助层；Cube 由 `RenderDrawPlan` 固定顶点数进入 Vulkan 程序化绘制；尚无稳定资产资源缓存入口，D1 不得直接把第三方 GLB 类型穿透到 Render。
- Picking：视口入口为 `ViewportPickingService.Pick`，候选来自 `SceneStateOwner.RaycastSpatial`；首期模型只允许世界 Bounds 命中，不扫描所有三角形。
- History：Transform 使用 `TransformHistoryEntry`；添加/删除/重命名使用 `SceneHistoryEntry` 和 `WorldEntitySnapshot` 恢复。模型导入归属 Add Entity 历史；资源重定位需要新增资产级历史条目；Target Switch 仍只改变 Selection，不进入 History。

## 资产身份

- `AssetId` 是稳定、可序列化、同一场景内唯一的资产身份，格式为 `asset_` + 32 位十六进制。
- `AssetId` 与源文件名、实体名、绝对路径、GPU Buffer、第三方 GLB 对象均无关。
- `ModelAssetId` 是实体对资产的引用；`EntityId` 仍只表示场景实体。

```text
AssetId -> 一个托管 GLB 资产
EntityId -> 一个场景实体
ModelAssetId -> 实体对 AssetId 的引用
```

## 资源目录

```text
<SceneName>.xyscene
<SceneName>.xyassets/
└─ models/
   └─ <AssetId>/
      └─ source.glb
```

- `.xyassets` 与 `.xyscene` 同级。
- 原始文件名只作为显示信息。
- 场景运行只依赖托管相对路径。
- 路径验证单一入口为 `SceneAssetPathPolicy`；禁止 `..`、盘符、UNC、反斜杠和规范化后逃逸。

## Schema 草案

下一版保存 Schema 建议升为 v3；v1/v2 继续可读。

```text
Assets[]
  AssetId
  Kind = ModelGltf
  RelativePath = models/<AssetId>/source.glb
  DisplayName
  ImporterVersion

Entities[]
  Id
  Name
  EntityType
  Transform
  ModelAssetId
```

- 旧场景没有 `Assets` 时仍可打开，打开后不自动 Dirty。
- 真实保存时才写入新 Schema。
- 单个资产 Missing/Failed 不导致整个场景加载失败。
- SceneDocument 不保存运行时加载状态、顶点、索引或 GPU 数据。
- 非法路径只让对应资产失效，不允许任意读盘。

## 坐标转换

- 玄域 World Space：右手系，`+Z Up`，XY 水平。
- glTF 2.0 按右手 `+Y Up` 输入，导入层一次性转换为玄域坐标。
- 位置和法线映射：`(x, y, z) -> (x, -z, y)`。
- 映射行列式为 `+1`，不产生镜像，三角形绕序不反转。
- Node Matrix/TRS 转换为 `C * M * C^-1` 语义；平移使用同一向量映射，旋转基向量随坐标基变换，非均匀缩放保留在转换后的局部轴。
- Bounds 在坐标转换后由导入层生成局部 Bounds。
- Pivot 作为节点局部原点保留，只转换其坐标基。
- D1 测试资产必须覆盖非对称三轴、平移 Node、旋转 Node、非均匀缩放、Pivot 偏移和正反面绕序。

## 运行时状态

运行时模型资产状态允许：

```text
Unloaded
Loading
Ready
Missing
Failed
```

- 这些状态不写入 `.xyscene`。
- `Missing` 表示托管文件不存在。
- `Failed` 表示文件存在但解析、验证或 GPU 建立失败。
- Missing/Failed 不自动删除实体。

## Picking 与 Bounds

首期流程固定为：

```text
世界空间候选查询 -> 模型实体世界 Bounds 命中 -> 选择实体
```

- 资产导入产生局部 Bounds。
- 实体 Transform 产生世界 Bounds。
- Frame Selected 与 Picking 使用同一 Bounds 事实。
- 缺失占位体使用独立固定 Bounds。
- 不实现精确 Mesh Picking。

## Save As

事务顺序冻结为：

```text
候选目标目录 -> 写入新场景文件 -> 复制托管资源 -> 完整验证 -> 最后切换当前文档路径
```

- 失败时当前文档仍指向旧场景。
- 旧 `.xyscene` 与旧 `.xyassets` 不受影响。
- 不留下被误认为成功的新场景。
- R4 首期允许复制场景登记的全部资产，不强制清理未引用资产。

## D1 精确范围

- 允许修改：`XuanYu.Editor/Assets/*`、`XuanYu.Editor/SceneDocument/*`、`XuanYu.World/WorldEntitySnapshot.cs` 及必要 partial、`XuanYu.World/Scene/*`、`XuanYu.Render.Abstractions/*`、相关 World/Core 测试与文档。
- 禁止修改：导入 UI、文件选择器、Vulkan 模型 Draw、贴图系统、资产浏览器、项目系统、父子模型层级、精确三角形 Picking、WarCore。
