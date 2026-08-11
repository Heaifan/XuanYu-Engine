# MAP-DOC-A-R2-F2 · Dataset List State Sync + UI Refinement

状态：`READY FOR USER RE-ACCEPTANCE`，不是 `CLOSED`。范围只覆盖 Dataset 创建成功后的列表发布、六类 type 的 UI 展示、自动 ID 与 Dataset 页布局；不改 F1 Create/Manifest 事务、Dataset schema、Geometry、Feature、AI、Asset、Layer、Renderer、Picking。

## T01～T03 基线与取证

| TODO | 证据 | 结论 |
|---|---|---|
| F2-T01 | 本地/远端均为 `3a7a403`，`0/0`；工作区仅保留 `_tmp_blind_rows/` | 基线干净，版本 `v0.2.26.12-fix` |
| F2-T02 | 真机窗口用新 ID `222` 创建后显示“数据集创建成功：222（road）”，列表仍为空 | 创建结果已成功发布，UI 列表没有同步可见 |
| F2-T03 | `CreateAsync` 更新 Registry；VM 调用 `RefreshDatasetProjectionAsync`；刷新原地改写同一个 `List` 后通知 `ItemsSource` | UI 绑定可能继续持有旧列表实例，形成成功反馈与空列表分裂 |

## T04～T08 实施与验证

- F2-T04：Dataset projection 改为新数组快照，创建/解除注册/打开/保存均经同一刷新通知链；空态数量消费当前投影。
- F2-T05：新增 UI-only `MapDatasetTypePresentation`；六类内部值映射为中文，Manifest 与 Dataset JSON 仍保存 `road` 等内部值。
- F2-T06：Registry 生成 `<internal-type>-<6 lowercase hex>`；同时检查 Manifest ID 与 `data/<id>.json` 文件/目录，最多 16 次，耗尽失败且不覆盖。
- F2-T07：移除 ID TextBox；ComboBox 展示中文；列表使用中文主类型、ID 副行、状态；保留解除注册和空态。
- F2-T08：F2 focused `39/39 PASS`，覆盖刷新、空态、失败不增行、六类映射、JSON 内部值、碰撞、有限重试、重开与多数据集。

## T09～T10

T09 solution build `0 Warning / 0 Error`；Core.Tests `339/339`、WarCore.Tests `22/22`、World.Tests `1217/1217`、F2 focused `17/17`；ARCH-A、5+100、版本与 `git diff --check` 均通过。T10 在最终门禁通过后 commit、push，并保留真机待验状态。

## 仍待用户验收

F2-M01～M08 以及 R2-M02/M03 的补验必须在真实编辑器窗口完成；自动测试、编译和本轮真机取证均不替代用户最终验收。
