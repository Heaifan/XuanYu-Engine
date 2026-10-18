# XYC-P2-A-R1 实施报告

## 实施结果

XYC 新增受限的场景实体控制面，复用正式 SceneStateOwner、World 变换边界和 Editor History。`AddCubeEntity()` 的前后 World Entity 集合差异用于登记新 EntityId；测试状态同时回读实体类型、完整 Transform、Hierarchy 节点、RenderSnapshot EntityId、Inspector 投影和 History 深度。

位置写入只允许本会话创建的测试实体与一个固定事务阶段。位置通过 `TransformCommand` 对 `IWorldTransformMutationContract` 提交，成功后调用正常 `RecordTransformHistory` 并发布 SceneRenderSnapshot。Undo/Redo 使用现有 `TryUndoFromShortcut` / `TryRedoFromShortcut` 路径，并对实体存在性、完整 Transform、History/Redo 深度进行后置核验。

没有调用 `UiVm.RunCommand`、没有直接写 World/Scene backing store、没有保存项目、没有删除用户实体。创建差异找不到恰好一个 Cube 时会返回失败，拒绝通过 ID 顺序猜测。

## 验证概览

- 纯 Candidate Release Build：Editor App PASS，XYC CLI PASS，0 warning / 0 error。
- Candidate XYC/P1-A/P1-B 相关 Scoped Tests：29/29 PASS。
- Candidate 真实 Windows Runtime：闭环 PASS；最终测试实体、Hierarchy 和渲染快照实体清单为空；Editor/CLI exit code 0。
- Candidate Silent Host Release Runtime：隐藏 HWND、Swapchain attach、Vulkan Present 和资源释放 PASS；无前台焦点；exit code 0。
- `git diff --check`：通过（仅工作区外线文件报告既存 CRLF/LF 警告，无 whitespace error）。
- 完整 Candidate `FullyQualifiedName~Xyc` 曾执行：34 PASS、8 FAIL。失败测试依赖纯 archive 未包含的未跟踪 WP2 文件 `artifacts/diagnostics/wp2-p4-r3/n24e121.hgt`；Scoped Candidate 不纳入该外部 fixture，29 个相关测试均通过。

## 限制与剩余项

- 真实截图是生产 Window 的 TOP 裁切，证明 Editor Runtime 采集路径及窗口隔离属性，不作为视口 Cube 几何像素证据。本轮没有宣称 Vulkan 视口像素验收。
- P2-A6 中请求重放、错误 Session、无写入授权、无效/不存在 EntityId、过期 Revision、空 History、重复创建、越权 Entity 与超界坐标已覆盖。活动交互、异常半提交、错误 SceneId、进程关闭后请求等专门故障注入未全部做完。
- Candidate Build 与 Runtime 在独立 Release archive 完成；正式版本推进和 Commit/Push 未完成。工作树的共享 `Directory.Build.props` 由其他 Owner Dirty 修改，不能作为 XYC 版本提交；开发宪法没有为本轮给出可据以推断的新编号。本任务整体为 PARTIAL。
- 未开始 P2-B。

## XYK CANDIDATE（未写入）

**候选主题：** 场景自动化写操作要绑定会话实体、预期修订与明确事务阶段，并在每步从权威 World/Scene 和 Editor History 回读。

**依据：** 本轮 Candidate Runtime 证明 EntityId 同时贯穿 World、Hierarchy、RenderSnapshot、Inspector；TransformCommand 写入后由生产 History Undo/Redo 验证 T0/T1，最后撤销创建并恢复空集合。

**边界：** “命令成功”不能只依据调用返回；必须核实 SceneId/Revision、实体类型与完整 Transform、History/Redo 深度、投影状态和最终恢复。该条目前仅作知识候选，不写入 XYK。

## 代码结构审查

8 个 Candidate 源码/测试文件均不超过 100 行。职责拆分为 API 模型、命令路由、Runtime Dispatcher 适配、UiVm 场景事务和守卫、协议测试。`git archive` Candidate 加明确 8 文件 overlay，未带入任何其他开发线 Dirty；Tree Identity 见 `candidate-identity.json`。

## R2 收口补充

- NASADEM fixture SHA-256 `BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1` 与源代码登记值一致；外部素材只用于 Candidate 临时测试环境。最终完整 XYC 测试 46/46 PASS。
- 补足活动 Transform/Drawing、错误 SceneId、无效 Redo、无写权限真实 Editor 会话与提交后观察者故障注入。测试核验真实 UiVm/World、History、Redo、事务阶段和 Interaction 状态。
- 故障注入发现并修复 Transform commit 后 observer 抛错造成的 World 已更新但 History/XYC 阶段未完成问题；修复只允许在权威 World 精确等于目标 Transform 时补齐 History。最终真实 Candidate 已重跑生产场景链、TOP Capture 与 Silent Host。
- 产品版本仍为 `0.3.0.24-fix`。本轮不建立 Version Event，不修改 `Directory.Build.props`。
- Remaining: P1-B 四主题/分辨率保真阈值矩阵未针对更新后的 UI DLL 重跑；P2-A 全范围运行/事务、安全链已验证。最终整体状态依据正式 commit/push 与所有 Candidate 门禁核验。

## R2 验收裁定

- 最终 Candidate Build/Test/Runtime 与 Tree Identity 均已按最终源码重新生成。P1-B 四主题/分辨率保真矩阵在 UI DLL `33C41F7F...348C29` 上全部达到用户已批准的逐组门槛；不是只沿用旧构建截图。
- `FullyQualifiedName~Xyc`: 46 PASS / 0 FAIL；含 P0/P1-A/P1-B、P2-A 命令和 4 项事务注入。App、CLI Release builds 0 warning / 0 error。
- Candidate real Windows Editor 完成场景写入闭环，未授权真实会话拒绝写入，Silent Host Present/资源退出通过。详细 manifest、PNG、sidecar 均在 evidence。
- 所有 R2 门槛满足后，此任务可独立提交。提交版本不变为 `0.3.0.24-fix`；Commit SHA/正式 Git Tree/Remote HEAD 以 Parent 完成并核实后回填。
