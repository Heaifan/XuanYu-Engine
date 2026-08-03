# WORLD-C-R4-D3：真实 GLB 导入闭环报告

版本：`v0.2.21.22-rz`
状态：自动验证 PASS；真机验收待用户执行（本报告不含真机结论）

## 一、范围与目标

把 D1 GLB 导入与 D2 静态模型渲染接入真实编辑器操作：

```text
选择 GLB → GltfStaticModelImporter 导入 → 静态模型实体 → Catalog 绑定
→ 层级树出现实体 → 视口显示真实模型 → 选择/移动/旋转/缩放复用既有主链
```

D3 不修改 `.xyscene`（Schema 与持久化属 D4），不建立资产浏览器、贴图、FBX/OBJ、拖放导入。

## 二、实现

### World（`XuanYu.World`）
- `WorldEntityType` 新增 `StaticModel`；`WorldEntityTypes.StaticModel = "StaticModel"`。
- `SceneStateOwner.StaticModel.cs`：`AddStaticModelEntity(name, transform, extent)`。extent 取模型 LocalBounds，供 Picking / 空间查询使用；World 只把静态模型当作普通世界实体，不接收 AssetId、RenderKey、GLB 路径或 GPU 资源。

### Editor（`XuanYu.Editor`）
- `SceneStaticModelBinding(EntityId, AssetId, SourcePath)`：实体 → 资产最小绑定。
- `SceneStaticModelCatalog`：`Bind / TryGetByEntity / TryGetByAsset / Remove / Clear / Snapshot / Revision / Changed`；Snapshot 按 AssetId.Value 稳定排序；同一 EntityId 最多一个绑定；对外不暴露可变 Dictionary。
- `StaticModelAuthoringService`：导入事务组合。顺序：路径校验（非空 / `Path.GetFullPath` / 存在 / `.glb` 大小写不敏感）→ `GlbImportService.ImportFile` → `AssetId.New()` → `SceneStateOwner.AddStaticModelEntity` → `catalog.Bind`。回滚：导入失败不建实体；实体创建失败不写 Catalog；绑定失败删除已建实体。
- 架构裁决：Editor 不引用 Render.Abstractions（`arch-a-guard-editor.ps1` 强制 Editor 只允许 Core/World），故 Binding 不含 RenderKey；稳定 `RenderStaticModelKey` 由 UI 层按 `AssetId.Value` 派生（同一资产共享同一 Key，满足 D0「同一资源多实体共享」）。此点与原始计划的 §2.4 建议（Binding 含 RenderKey）不同，原因见上。

### Editor.UI（`XuanYu.Editor.UI`）
- `SceneRenderProjectionAdapter.TryCreate` 增加 `SceneStaticModelCatalog?` 与 `IReadOnlyDictionary<AssetId, RenderStaticModelResource>?` 输入；`StaticModel` 实体按 EntityId 查询绑定并携带正确 RenderKey；绑定缺失（导入事务中间帧）跳过该实体，不让整帧投影失败；`RenderProjection.StaticModels` 来自缓存资源快照（按 Key 排序）。
- `UiVm.RenderProjection.cs`：生产路径移除 `D2StaticModelDemo.Apply`，改经 Catalog 派生。
- `UiVm.StaticModelImport.cs`：`ImportStaticModel(path)`；成功后选择新实体、刷新层级/检查器、发布 RenderProjection、低频日志（实体/资产/路径/顶点/索引）、Dirty=true。
- UI 入口：顶部菜单「文件 → 导入 GLB」（`Top.axaml`），文件选择器过滤 `*.glb` 单选（`UiWin.SceneCommands.cs`）；取消选择不建实体、不改 Dirty、不写错误日志、不产生 Undo。
- Undo/Redo：`AddEntityHistoryEntry` / `DeleteEntityHistoryEntry` 携带可选 `SceneStaticModelBinding`；恢复实体时重新绑定、移除实体时解除绑定；新建/打开场景清空 Catalog 绑定。
- `D2StaticModelDemo.cs` 按宪法 §十三「删除文件须下一轮执行」保留文件、移除生产调用；待用户批准后删除。

## 三、验证（全部真实执行）

| 门禁 | 结果 |
|---|---|
| 串行 build 10 项目（-m:1 串行） | 0 warning / 0 error |
| Core Tests | 145/145 PASS |
| World Tests | 235/235 PASS（基线 216 + D3 新增 19） |
| `scripts/arch-a-guard.ps1` | PASS |
| glslc 编译 scene.vert / scene.frag | PASS |
| `.xyscene` JSON 校验 | PASS |
| `git diff --check` | PASS |
| 守卫口径 5+100（tracked 手写 459 文件） | 0 超线 |

## 四、D3 自动测试清单

- `WorldCR4D3AuthoringServiceTests`：合法 GLB 导入实体+绑定；缺失文件/非 GLB/损坏 GLB 失败不建实体；重复导入独立绑定；实体类型与 Bounds。
- `WorldCR4D3CatalogTests`：Snapshot 排序；同实体不重复绑定；Remove/Clear；Revision 递增；Changed 事件。
- `WorldCR4D3ProjectionTests`：投影使用 Catalog RenderKey；资源不串绑其它实体；无绑定 StaticModel 实体被跳过。
- `WorldCR4D3StaticModelUiTests`：导入添加实体+Dirty；失败导入状态不变；Undo/Redo 绑定保持；删除实体解绑；生产投影不再引用 D2StaticModelDemo。

## 五、真机验收入口（待用户执行）

按 `docs/world-c-r4-d3-static-model-authoring-report.md` 上方计划章节二的 IPO 卡：

1. 导入 GLB（真实模型显示，无三角/立方体占位，层级与检查器同步）。
2. 取消导入（无实体、无 Dirty 变化、无错误日志）。
3. 静态模型选择（视口 / 层级 / Inspector 同步）。
4. 静态模型变换（移动/旋转/缩放 + Undo/Redo，无残影）。
5. 多模型独立绑定（两模型互不串绑）。

## 六、当前状态

```text
WORLD-C-R4-D0：COMPLETE
WORLD-C-R4-D1：COMPLETE
WORLD-C-R4-D2：COMPLETE
WORLD-C-R4-D3：自动验证 PASS（等待真机验收）
WORLD-C-R4：IN PROGRESS
WORLD-C：未完成
```

未获用户真机确认前，不宣布 D3 PASS / WORLD-C-R4 CLOSED / WORLD-C CLOSED。
