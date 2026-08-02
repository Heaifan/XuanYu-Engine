# WORLD-C-R4-D4-R1：`.xyassets` 托管资源事务内核报告

版本：`v0.2.21.24-rz`
状态：自动验证 PASS；本轮无用户可见功能，不单独真机验收

## 一、范围与目标

按 D0 冻结合同实现独立、可测试、可回滚的托管资源事务内核：

```text
运行时 GLB SourcePath → 托管目标规划 → 复制到同目录临时 staging
→ 验证全部托管文件 → 激活新 <SceneName>.xyassets
→ 后续场景保存成功时 Complete / 失败时 Rollback
```

**采用 D0 `.xyassets` 托管模型,禁止外部 `..` 引用。** 本轮只实现事务内核;Schema v3 尚未接线,正式保存入口尚未接线,加载、占位和弹窗尚未开始。

## 二、冻结目录与相对路径

```text
<SceneName>.xyscene
<SceneName>.xyassets/
└─ models/
   └─ <AssetId>/
      └─ source.glb
```

相对路径固定 `models/<AssetId>/source.glb`,复用 `SceneAssetPathPolicy.ModelSourceRelativePath` 生成、`IsSafeRelativePath` 校验(禁止 `..`/盘符/UNC/反斜杠/逃逸)。

## 三、实现

### 数据契约(`XuanYu.Editor/Assets/`)
- `HostedSceneAsset(AssetId, SourcePath, RelativePath, StagedPath, FinalPath)`:托管资产项;SourcePath 是 D3 导入时记录的规范化绝对路径,本轮不改其运行时语义。
- `SceneAssetHostingPlan(SceneFilePath, AssetRootPath, StagingRootPath, BackupRootPath, Assets)`:资产按 `AssetId.Value` 稳定排序;绝对路径均经 `Path.GetFullPath`;规划阶段不写磁盘。
- `SceneAssetHostingError`:14 个明确错误码,复用现有 `SceneDocumentResult<T>` 模式,不重复创建错误框架。

### Planner
- 场景路径校验:非空、扩展名 `.xyscene`、目录存在、文件名非空。
- 资产校验:AssetId 合法、SourcePath 非空/绝对/存在/非目录/可读、扩展名 `.glb`(大小写不敏感)。
- 去重:同 AssetId + 同源去重只复制一次;同 AssetId + 异源拒绝 `AssetSourceConflict`(不得后者覆盖前者);同源 + 异 AssetId 允许。
- 路径安全:RelativePath 经 `SceneAssetPathPolicy`;FinalPath 经 `TryResolveManagedPath` 防逃逸。

### 事务状态机
```text
Prepared → Activated → Completed
Prepared → RolledBack
Activated → RolledBack
Failed(任意失败)
```
禁止:Completed/RolledBack 后继续操作、重复 Activate/Complete/Rollback;错误调用返回明确失败。

- `Prepare`:创建唯一 staging 根(`.<SceneName>.xyassets.staging-<Guid>`),按 AssetId 顺序 `File.Copy(overwrite:false)`,复制后校验存在 + 长度一致 + 路径在 staging 内;失败删除整个 staging,不创建/不修改正式 assetRoot。本轮不做内容 Hash(D0 未冻结 Hash 合同)。
- `Activate`:目标不存在 → `Directory.Move(staging, assetRoot)`;目标已存在 → 先移旧目录为 backup(`.<SceneName>.xyassets.backup-<Guid>`),再移入 staging,第 3 步失败自动恢复旧目录。
- `Complete`:仅 Activated 且 staging 已清时执行;删除 backup;删除失败返回错误、保留证据、不声称 Completed。
- `Rollback`:Prepared 删 staging;Activated 无旧根删新根;Activated 有旧根删除新根 → 恢复 backup → 清理 staging;恢复失败返回 `RollbackFailed` 并保留 backup。**旧数据安全优先于清理整洁。**

## 四、验证(全部真实执行)

| 门禁 | 结果 |
|---|---|
| 串行 build 10 项目 | 0 warning / 0 error |
| Core Tests | 145/145 PASS |
| World Tests | 全量 PASS(含 D4-R1 新增 28 项) |
| `scripts/arch-a-guard.ps1` | PASS |
| glslc 编译 scene.vert / scene.frag | PASS |
| `git diff --check` | PASS |
| 守卫口径 5+100 | 0 超线 |

## 五、测试清单(`XuanYu.World.Tests/Assets/`)

- `WorldCR4D4HostingPlannerTests`:单资产目录、多资产按 AssetId 排序、同 AssetId 同源去重、同源异 AssetId 允许、中文与空格场景名。
- `WorldCR4D4HostingPlannerRejectTests`:同 AssetId 异源拒绝、非 .glb 拒绝、缺失源拒绝、相对路径拒绝、非法 AssetId 拒绝。
- `WorldCR4D4HostingTransactionTests`:Prepare 复制与长度校验、不碰既有 assetRoot、失败清理 staging、Activate 无旧根/有旧根、重复 Activate 拒绝。
- `WorldCR4D4HostingCompleteTests`:Complete 删备份留根、未 Activate 拒绝、重复 Complete 拒绝、Rollback 后 Activate 拒绝。
- `WorldCR4D4HostingRollbackTests`:Prepared 回滚、无旧根 Activated 回滚、有旧根回滚恢复旧内容、重复回滚拒绝、Completed 回滚拒绝。
- `WorldCR4D4HostingSaveAsTests`:同一批资产规划到两个场景产生两个独立 .xyassets 根,独立激活互不干扰。

测试全部使用独立临时目录(`Path.GetTempPath()` 下唯一子目录,结束后清理),AssetId 使用确定性值,不依赖随机排序、Dictionary 遍历顺序、线程时序或固定睡眠。

## 六、边界与后续

- 本轮未触碰:`XuanYu.Core`、`XuanYu.World` 生产代码、`Render.Abstractions`、`Render.Vulkan`、`Editor.UI`、`Editor.App`、SceneDocument JSON/Validator/Mapper、Shader。
- Schema v3 JSON、SceneDocument Entity 的 `ModelAssetId`、正式保存/另存为接线、场景加载、Missing/Failed 状态、占位 Bounds、错误弹窗均属后续轮次(R2 起)。
- R2 可直接使用事务结果生成 v3 字段:`AssetId`、`Kind=ModelGltf`、`RelativePath`、`DisplayName`、`ImporterVersion`、`ModelAssetId`。

## 七、当前状态

```text
WORLD-C-R4-D3：COMPLETE
WORLD-C-R4-D4-R1 自动验证：PASS
WORLD-C-R4-D4：IN PROGRESS
WORLD-C-R4：IN PROGRESS
```

不声明 D4 COMPLETE / R4 CLOSED / WORLD-C CLOSED。
