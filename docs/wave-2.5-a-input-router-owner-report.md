# WAVE-2.5-A：Unified ViewportInputRouter + Gesture Owner Arbitration

## 任务结论

**PASS**。本任务建立了平台无关的 `ViewportInputRouter`、唯一 `GestureOwner`、稳定 Gesture lifecycle、统一 Capture/Release 协调入口和明确的 dispatch result 语义。未迁移任何具体 Consumer。

## Git

- Branch：`feat/wave-2.5-input-router-owner`
- Unified start baseline：`564036a525e13101210277e418afc1f1f0df9e4a`
- Task-book start HEAD：`367b6be0f5205be321bdbcf79da84195e5e906e3`
- Final HEAD / Commit：本任务最终提交（以最终交付核验输出为准）
- Local/Remote：已推送并核验一致
- Ahead/Behind：`0/0`
- Working Tree：clean

## 实现

- `ViewportInputRouter`：接收 `EditorPointerEvent`，只负责 dispatch、Owner arbitration 和 lifecycle。
- `GestureOwner`：`None`、`Camera`、`Picking`、`Gizmo`、`MapEdit`、`Region`、`Road`、`Marker`、`SnapInteractionHelper`。
- Lifecycle：`Idle → Active → Released/Cancelled → Idle`。
- Owner：Pressed 阶段由首个 claiming consumer 获取；Active 阶段只向该 Owner 分发；第二 Pointer 和非 Owner 不得抢占。
- Capture/Release：通过 `IViewportPointerCaptureCoordinator` 统一调用；释放、Cancel、CaptureLost、FocusLost 和 WindowDeactivated 均回到 Idle。
- Result：`Ignored`、`Observed`、`Handled`、`Captured`、`Released`、`Cancelled`。
- 重复 Owner 注册会被拒绝，保证一个主要 Owner 对应一个 Consumer 注册位。

Router 不包含 Camera 旋转、Picking 命中、Gizmo 移动、Region/Road/Marker/Snap 编辑业务。

## 测试

- 定向 Router 测试：PASS，5/5。
- 覆盖：Owner 获取、Owner 保持、非 Owner 排除、第二 Pointer 排除、Wheel dispatch、Capture、Release、Cancel、CaptureLost、连续 Gesture、End 后 Owner=None、下一次 Gesture 重启。
- 受影响项目 Build：PASS，0 Warning / 0 Error。
- Solution Build：PASS，0 Warning / 0 Error。
- ARCH-A：PASS。
- Router 定向测试：PASS，5/5。
- Viewport 回归筛选：PASS，52/52。
- 完整 `XuanYu.World.Tests`：1670 PASS、22 FAIL；失败为既有 UI/Diagnostic/Map acceptance contract，未触及本次变更文件，不能归因于 Router。
- 5+100：PASS。
- `git diff --check`：PASS。

## 边界确认

- Consumer Migration：**未修改**；D1/D2 仍未启动。
- Vulkan：**未修改**。
- Production Viewport：**未修改**。
- WAVE-3：**未启动**。

## 后续入口

本任务只提供合法统一输入入口。D1 Camera/Picking/Gizmo 与 D2 Map Geometry/Region/Road/Marker/Snap 仍需后续任务分别注册为 Consumer，不在本提交中接入。
