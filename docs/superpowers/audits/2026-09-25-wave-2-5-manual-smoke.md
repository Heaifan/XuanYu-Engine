# Wave 2.5 Manual Smoke Acceptance Record

**状态**：`WAITING FOR HUMAN INPUT`  
**日期**：2026-09-25  
**基线**：`e1d411daf26c7b86ba6211f60266a249836e43a5`  
**任务分支**：`audit/wave-2-5-manual-smoke`  
**唯一入口**：仓库根目录 `run.bat`

## 证据边界

- 本记录只接受真人通过 `run.bat` 启动后的真实 UI 操作、可观察状态、日志和截图。
- 构建成功或 Editor 启动成功只能记录为启动证据，不得升格为输入 PASS。
- 未由真人完成并留下证据的项目必须保持 `NOT VERIFIED`。
- 不使用 Headless、合成输入、自动化点击或测试替代真人验收。
- 每个项目需要检查：无双点击、无双滚轮、无 stuck capture、无 stuck modifier。

## 启动证据

| 项目 | 结果 | 证据 |
|---|---|---|
| 从仓库根目录执行 `run.bat` | `NOT VERIFIED` | 待真人提供启动日志/截图 |
| Build | `NOT VERIFIED` | 待 `run.bat` 日志；不等价于输入 PASS |
| Editor 可见并进入可操作状态 | `NOT VERIFIED` | 待真人提供截图与观察记录 |

## 人工 IPO 清单

记录格式：`I` 输入、`P` 过程、`O` 可观察输出。结果只能填写 `PASS`、`FAIL` 或 `NOT VERIFIED`。

| 序号 | 路径 | 输入 I | 过程 P | 输出 O | 结果 | 证据 |
|---:|---|---|---|---|---|---|
| 1 | Camera Orbit | 在视口按住中键并拖动，释放 | 重复至少两次，确认一次按下只产生一次会话 | 相机绕轨道旋转；释放后 capture 结束，无残留 | `NOT VERIFIED` | 待日志/截图 |
| 2 | Camera Pan | 在视口按住约定平移键并拖动，释放 | 重复至少两次 | 相机平移；无跳变、无 stuck capture | `NOT VERIFIED` | 待日志/截图 |
| 3 | Camera Wheel | 在视口滚动一次，再分次滚动 | 区分单次滚轮与连续滚轮 | 缩放步数与实际滚轮次数一致，无双滚轮 | `NOT VERIFIED` | 待日志/截图 |
| 4 | Picking 单击 | 左键单击一个可命中对象 | 只单击一次，不连点 | 对象被选中一次；无重复选择/双击行为 | `NOT VERIFIED` | 待日志/截图 |
| 5 | Gizmo Move | 选择 Move 工具，拖动 Move 轴 | 按下、拖动、释放各一次 | 对象只沿目标轴移动；capture 正常结束 | `NOT VERIFIED` | 待日志/截图 |
| 6 | Gizmo Rotate | 选择 Rotate 工具，拖动旋转环 | 按下、拖动、释放各一次 | 对象旋转；无重复提交或残留 capture | `NOT VERIFIED` | 待日志/截图 |
| 7 | Gizmo Scale | 选择 Scale 工具，拖动缩放柄 | 按下、拖动、释放各一次 | 对象缩放；无重复提交或残留 capture | `NOT VERIFIED` | 待日志/截图 |
| 8 | Marker 放置 | 进入 Marker 工具后左键单击一次 | 只放置一个点 | 只新增一个 Marker；无双点击导致的重复点 | `NOT VERIFIED` | 待日志/截图 |
| 9 | Region 开始/节点/完成 | 进入 Region 工具，单击开始并继续单击节点，按约定方式完成 | 至少三个节点，完成一次 | 草稿状态、节点数和完成后的 Region 可观察且正确 | `NOT VERIFIED` | 待日志/截图 |
| 10 | Region 取消 | 进入 Region 工具并添加节点，按 Escape 取消 | 取消后观察工具和草稿 | 草稿消失、工具回到就绪；无 stuck capture/modifier | `NOT VERIFIED` | 待日志/截图 |
| 11 | Road 开始/节点/完成 | 进入 Road 工具，单击开始并继续单击节点，按约定方式完成 | 至少两个节点，完成一次 | 草稿状态、节点数和完成后的 Road 可观察且正确 | `NOT VERIFIED` | 待日志/截图 |
| 12 | Road 取消 | 进入 Road 工具并添加节点，按 Escape 取消 | 取消后观察工具和草稿 | 草稿消失、工具回到就绪；无 stuck capture/modifier | `NOT VERIFIED` | 待日志/截图 |
| 13 | FocusLost / Alt+Tab | 开始一次可捕获交互后切换窗口或 Alt+Tab | 返回 Editor，重复一次 | 当前交互被取消；返回后可重新开始，无 stuck capture | `NOT VERIFIED` | 待日志/截图 |
| 14 | Shift/Ctrl/Alt | 在 Region/Road 或对应交互中分别按住并释放 Shift、Ctrl、Alt | 覆盖 modifier 中断、吸附抑制或组合输入路径 | modifier 释放后不残留；Alt 等约定效果可观察 | `NOT VERIFIED` | 待日志/截图 |

## 证据记录区

### 真人操作信息

- 操作者：`待填写`
- 操作时间：`待填写`
- OS / 显示器 / DPI：`待填写`
- Editor 进程与窗口状态：`待填写`

### 日志

```text
待真人粘贴 run.bat 启动及操作期间的相关日志。
```

### 截图

待真人提供截图文件路径，并在上表“证据”列逐项关联。

## 当前结论

`NOT VERIFIED`。当前只有仓库基线和验收模板证据；没有真人 UI 操作证据，因此不得宣称输入链路通过或 CLOSED。
