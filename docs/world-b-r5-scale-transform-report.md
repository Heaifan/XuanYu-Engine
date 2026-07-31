# WORLD-B-R5 Scale Gizmo 缩放变换闭环报告

版本：v0.2.20.19-fix
日期：2026-07-31
分支：feat/WORLD-B-editor-interaction
阶段：WORLD-B-R5：Scale Gizmo 缩放变换闭环正式收口

## 裁定

v0.2.20.18-rz 已被真机验收退回：Scale Gizmo 在视口中过大，明显遮挡实体和场景；中心 `Uniform` 整体等比缩放手柄不可发现、不可理解或难以稳定命中。

v0.2.20.19-fix 已完成 R5-R1 自动门禁修复。用户已完成真机重新验收并裁定通过，`WORLD-B-R5` 正式 CLOSED。

本轮只记录用户真机验收结论与阶段收口，不重复执行已通过的 R5 真机测试，不新增 `WORLD-B-R6`，不启动 WarCore。

本轮仍只实现实体自身 TRS 的 X/Y/Z 局部分量缩放与中心 Uniform 等比缩放，不实现负缩放、镜像、多选、Pivot、吸附、数值输入、Local/Global 切换、父子传播或世界空间剪切。

## R5-R1 实现变化

- `ScaleGizmoScreenSize` 将 Scale Gizmo 主体屏幕轴长从 90 DIP 缩小到 63 DIP，约为原尺寸 70%。
- 端点手柄视觉尺寸从 11 DIP 缩小到 8 DIP，中心 Uniform 视觉尺寸调为 15 DIP。
- 新增 `CenterHitRadiusDip=12`，保证中心整体缩放普通鼠标可稳定命中。
- `ScaleGizmoHitTester` 改为中心 Uniform 核心区先裁决，X/Y/Z 轴段从中心核心区外参与命中。
- `scene.vert` 保持现有 Scale Gizmo 252 顶点绘制结构，随 63 DIP 轴长整体缩小，并将中心白色 Uniform 立方体半径调为 `L*0.15`。
- `ShaderBytecode.Vert.cs` 已由 glslc 从正式 `scene.vert` 重新生成。
- `UiVm.ScaleGizmo.cs` 补充低频“缩放开始捕获 Entity=... Handle=...”日志。
- `UiVm.MoveGizmoLogging.cs` 在缩放 Commit / Cancel 中补充 `Handle`、最终 Scale 和取消 Reason。

## 保持不变

- X 手柄只改变 Scale.X。
- Y 手柄只改变 Scale.Y。
- Z 手柄只改变 Scale.Z。
- Uniform 使用同一倍率改变 Scale.X / Scale.Y / Scale.Z，并保持原始比例。
- Preview 不绕过权威链写入最终状态。
- Commit 仍通过 `TransformSession → SceneStateOwner → GlobalWorld`。
- Escape 恢复捕获前 Scale，不提交历史记录。
- 一次完整拖动只产生一次历史记录。
- Undo / Redo 恢复完整 Scale。
- 缩放工具内切换实体后，旧实体不接受延迟缩放。

## 自动验证

```text
dotnet build-server shutdown
结果：PASS

dotnet build XuanYu.Engine.slnx --no-restore -m:1 -nr:false -p:UseSharedCompilation=false
首次结果：0 warning / 2 errors
阻断原因：Avalonia BuildServices 写 C:\Users\Heai\AppData\Local\AvaloniaUI\BuildServices\buildtasks.log 被拒绝
授权后结果：10 项目 0 warning / 0 error

dotnet test XuanYu.Core.Tests\XuanYu.Core.Tests.csproj --no-build -m:1 -nr:false
结果：129 passed / 0 failed / 0 skipped

dotnet test XuanYu.World.Tests\XuanYu.World.Tests.csproj --no-build -m:1 -nr:false
结果：163 passed / 0 failed / 0 skipped
```

后续 `arch-a-guard`、`git diff --check`、5+100、最终 build-server shutdown、commit/push 与远端 tip 复核结果以本轮最终执行回复为准。

## 真机验收结论

以下结论来自用户真机验收结果，不是自动测试替代证据：

- Scale Gizmo 尺寸调整符合预期，不再大面积遮挡实体和场景。
- X、Y、Z 单轴缩放有效，只改变对应 Scale 分量。
- 中心 Uniform 等比缩放有效，可稳定发现和命中。
- 实体、选中轮廓和 Gizmo 在 Preview 过程中同步。
- Commit 生效。
- Escape Cancel 生效，延迟 MouseUp 未发现阻断问题。
- Undo / Redo 生效，一次拖动对应一次历史记录。
- 工具状态和目标切换没有发现阻断问题。
- 用户已完成真机验收，不需要再次执行重复验收。

## 已覆盖的 R5 IPO 项

R5 真机复验已覆盖控件尺寸、X 单轴、Y 单轴、Z 单轴、Uniform 整体缩放、撤销重做、Escape 取消、目标切换与工具状态。本轮不再保留待执行验收卡。

## 最终状态

WORLD-B-R5 CLOSED。Scale Gizmo 缩放变换闭环已完成，WORLD-B 不需要 R6；下一步只允许进行 WORLD-B 总收口，并从收口后的准确 HEAD 创建 WORLD-C 分支。
