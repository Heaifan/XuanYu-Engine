# XYC-P2-A-R1 预检

## 仓库基线

- 仓库：`E:\MyDoc\project-VSCode\XuanYuEngine`
- Branch：`recovery/editor-center-navigation-r1`
- HEAD：`d0335c52b0c9938277f571cc58d11c4d038f3b43`
- Origin tip：`d0335c52b0c9938277f571cc58d11c4d038f3b43`（`git ls-remote` 实测）
- Ahead / Behind：`0 / 0`
- Bootstrap：`scripts/xye-bootstrap.ps1` = READY
- .NET：Resolver `scripts/resolve-dotnet.ps1`，SDK `10.0.400`
- 开发宪法版本：3.5；第34条规定产品版本只依产品版本源维护，不设独立 Version Event 台账。

## 所有权与 Dirty

预检时工作区已存在多个 WP2、地图/地形、渲染、XYUI 及 QA 文档 Dirty。它们均未由本任务改写、暂存或覆盖。唯一与版本归属直接相关的外线 Dirty 是 `Directory.Build.props`：HEAD 中正式版本为 `0.3.0.24-fix`，工作树内容为 `0.3.0.25-fix`。该文件不属于 XYC-P2-A，且修改归属未获本轮授权。

本轮代码 Ownership 限定为：

- 修改：`XuanYu.Editor.UI/Control/XycCommandProcessor.cs`
- 新增：`XuanYu.Editor.UI/Control/XycSceneContracts.cs`
- 新增：`XuanYu.Editor.UI/Control/XycSceneCommandProcessor.cs`
- 新增：`XuanYu.Editor.UI/Control/UiVmXycRuntime.Scene.cs`
- 新增：`XuanYu.Editor.UI/Vm/Control/UiVm.XycScene.cs`
- 新增：`XuanYu.Editor.UI/Vm/Control/UiVm.XycScene.Guards.cs`
- 修改测试 Fake：`XuanYu.World.Tests/Control/FakeRuntime.cs`
- 新增：`XuanYu.World.Tests/Control/XycSceneCommandTests.cs`
- 新增本目录交付文档、脚本与运行证据。

未创建第二 Worktree。独立 Candidate 由正式 HEAD 的 `git archive` 快照加上述 8 个源码/测试文件物化；Candidate 不含工作树中的 WP2、XYUI、地图、地形、Vulkan 或版本文件 Dirty。

## 施工前架构核验

- 空白生产 XYC Editor 使用真实 `UiWin`，无文档路径，`SceneStateOwner(seedInitialEntity:false)`；默认实体数和 History 深度为 0。
- `UiVm.AddCubeEntity()` 经 `SceneStateOwner` 建立 Cube，并推入 `AddEntityHistoryEntry`，更新正式选择、Hierarchy 与文档状态。
- 位置提交复用 `TransformCommand` + `IWorldTransformMutationContract`，成功后走 `RecordTransformHistory` 和 `PublishSceneRenderSnapshot`。
- `TryUndoFromShortcut` / `TryRedoFromShortcut` 进入 Editor History Owner 与正式历史应用路径。
- 旧 `CommitTestEntityPosition()` 不写正常 Transform History，未用于本功能。
- 所有场景写入由 UI Dispatcher 串行执行；XYC Session 采用独立 Editor 实例，写操作必须显式启用 `--diagnostic-write`。

## 治理结论

版本源 Dirty 与 P2-A 功能版本归属冲突。依开发宪法不得恢复旧 FEATURE Event；本轮不猜测新版本，也不把 `0.3.0.25-fix` 归入 XYC。因此未执行正式 Commit / Push，整体状态为 PARTIAL。

## R2 收口复核

- HEAD/Remote：`d0335c52b0c9938277f571cc58d11c4d038f3b43`，fetch 后一致，分支 `recovery/editor-center-navigation-r1`，Ahead/Behind `0/0`。
- SDK bootstrap：State READY，Resolver `scripts/resolve-dotnet.ps1`，SDK `10.0.400`。
- 正式版本源 HEAD 为 `0.3.0.24-fix`；工作树 `Directory.Build.props` 的 `0.3.0.25-fix` 属 WP2 外线 Dirty。按用户裁决继续使用正式版本，不提交版本文件，不建立 Version Event。
- R2 Ownership：9 个 XYC 源码/测试文件及 `docs/qa/XYC-P2-A-R1/`。另制作 UTF-16LE Changelog Candidate，仅从正式 HEAD 增加 P2-A 条目；工作树 Changelog 的 WP2 Dirty 保留，不混入 Candidate。
- NASADEM fixture 外部 SHA 与策略登记一致，只在 Candidate 临时测试快照提供；无 fixture 被 Git Index/正式候选收录。
- Candidate Release App/CLI 0 warning/0 error，完整 XYC 46/46 PASS，P2-A6 注入测试 4/4 PASS，真实生产 Editor 场景链/无写权限会话、P1-B 四组合和 Silent Host 均按最终 UI DLL 重新取证。
