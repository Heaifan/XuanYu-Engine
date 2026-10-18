# P2-A6 失败与事务安全验证补充（R2）

## 原始 8 项失败的堆栈审计

重跑证明，先前“8 项全部由缺失 DEM fixture 引起”的归因不准确：

- **1 项** 是真实 fixture 缺失：`XycTestDemPolicyTests.Allows_only_registered_real_dem_with_expected_hash` 在 `XycTestDemPolicy.Resolve` 的 `File.OpenRead` 抛 `FileNotFoundException`，从 `XycTestDemPolicyTests.cs:13` 调用。独立日志：`evidence/diagnostics/no-fixture-dem-tests.log`。
- **7 项** 是测试目标配置错误：只构建 Release 后，E2E/静默窗口测试从 Release test output 的硬编码路径查找 `XuanYu.Editor.Control.Cli/bin/Debug/net10.0` 和 `XuanYu.Editor.App/bin/Debug/net10.0`。堆栈分别落在 `XycProcessE2ETests.cs:16-17`、`XycCliGuardE2ETests.cs:12-52`、`XycSilentWindowDecisionTests.cs:43`，具体失败与调用堆栈在 `evidence/diagnostics/release-with-fixture-before-debug-build.log`。

处理方式是仅在纯 Candidate 临时目录补建其测试要求的 Debug CLI 与 Editor App，并在正确路径提供已核验的 DEM fixture。旧 42 项 XYC 集随后 42/42 通过。最终新增四项事务测试后，Release `FullyQualifiedName~Xyc` 为 **46/46 PASS**。

受控移走 fixture 后重跑完整当前集合为 45 PASS / 1 FAIL；唯一失败仍为上述 DEM Policy FileNotFound，其余 45 项通过。素材恢复后 SHA-256 再核验通过。故缺失素材本身不会导致 8 项失败。

## 外部 DEM fixture

工作区登记素材 `artifacts/diagnostics/wp2-p4-r3/n24e121.hgt` 的 SHA-256 为 `BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1`，与 `XycTestDemPolicy.SourceSha256` 一致。只复制到纯 Candidate 临时快照的相同相对路径；不进入产品源码 Candidate、Candidate Git Tree 或提交。完整测试结果、原始错误输出和素材状态见上述 diagnostics 日志及 Candidate manifest。

## P2-A6 逐项结论

| 检查 | 结果 | 独立核验 |
|---|---|---|
| 活动 Transform Interaction 时写入 | PASS | UiVm 交互快照为活动态；返回 `HISTORY_CONFLICT`；Scene/World、History、Redo 与阶段前后相同 |
| 活动 Drawing Interaction 时写入 | PASS | Editor Authoring Owner 处于绘制事务；返回 `HISTORY_CONFLICT`；World、History、阶段未变化 |
| 错误 SceneId | PASS | 返回 `SCENE_MISMATCH`；实体/History/阶段不变 |
| 无写权限真实会话 | PASS | 真实 Editor 子进程未传 `--diagnostic-write`；创建命令返回 `UNAUTHORIZED`；World、History、阶段仍为 0；退出码 0 |
| 无效 Redo 阶段 | PASS | 本测试实体处于非 Redo 阶段；返回 `INVALID_STAGE`；状态不变 |
| Transform 提交中观察者抛异常 | PASS（修复后） | 注入 `RenderSnapshotChanged` observer 异常；World 目标 Transform 已提交后，XYC 补齐 Undo History；Stage=2、Undo/Redo=2/0、Interaction=Idle，实体位置一致 |
| 异常中途提交无半状态 | PASS（修复后） | 重新读取 World 精确等于目标 Transform 后才补 History 并完成事务；若后置事实不符，原异常继续传播，不合成成功 |

故障注入仅存在于测试订阅者，不进入正常用户流程。最终事务测试专用日志见 `evidence/diagnostics/transaction-safety-tests.log`。
