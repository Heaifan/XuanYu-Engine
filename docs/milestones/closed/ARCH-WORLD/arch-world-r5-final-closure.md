# ARCH-WORLD R5 最终收口报告

版本：v0.2.19.8-rz
分支：`refactor/ARCH-WORLD-R5-render-contract`
范围：Render Projection 合同、相机显式化、日志降噪、R5 CLOSED 收口

## 一、最终裁定

**ARCH-WORLD R5 = CLOSED。**

R5-R0A 已完成只读审计，R5-R1 已完成显式 Render Projection 最小合同实装；用户真机验收确认启动、首帧、查看全部、聚焦选中对象、Gizmo 显示、Preview、Commit、Escape Cancel、Resize、日志栏展开/收起、Vulkan 关闭释放链全部通过。

本收口轮只处理一个非功能阻断项：高频调试日志降噪。该问题不否决 R5-R1，不代表 Render Projection 回退，也不要求重跑已通过的真机功能测试。

## 二、已关闭的 R5 目标

- `Render.Vulkan` 不再引用 `SceneRenderSnapshot` / `ISceneRenderSnapshotSource` / `DefaultEditorCamera`。
- `Render.Abstractions` 自持最小 `RenderProjection` 合同，不引用 `Core.Scene` / World / Editor.UI。
- Preview 最终位置、Gizmo 可见性与位置、显式相机投影均在 Editor/UI 组合边界完成。
- 缺相机时返回明确失败原因，并跳过当前帧提交；不再静默生成默认编辑器相机。
- R5 守卫由 `scripts/arch-a-guard-render.ps1` 锁定，防止 Render 边界回退。

## 三、真机验收证据

| 项 | 裁定 | 证据 |
|---|---|---|
| 启动与首帧 | PASS | 10 个实体正常显示，日志记录首帧 Present 成功 |
| 查看全部 / 聚焦 | PASS | 全部实体入镜；选中测试实体09后相机与检查器一致 |
| Gizmo / Preview / Commit | PASS | 三轴工具显示正确，拖动预览同步，松开后正式坐标提交 |
| Escape Cancel | PASS | 日志记录会话取消，坐标恢复到原始值 |
| Resize / 日志栏展开收起 | PASS | 多次真实尺寸变化后 Swapchain 自愈恢复 |
| Vulkan 释放链 | PASS | Present 泵、Pipeline、Framebuffer、Swapchain、设备、Surface、Instance 依序释放 |
| 默认相机后门 | PASS | 未发现默认相机、相机缺失、跳过帧或 Fatal |

## 四、日志降噪收口

上传日志中存在高频调试噪声：`PublishSceneRenderSnapshot` 约 228 条、命令缓冲录制开始约 232 条、命令缓冲录制结束约 232 条。

本轮最小处理：

- `UiVm.Scene` 不再逐次输出 `PublishSceneRenderSnapshot`，改为首次、实体数变化和每 100 次摘要输出。
- `VulkanClearFrameOwner.Commands` 不再逐次输出命令缓冲录制开始/结束，改为重录成功后的低频摘要。
- 失败路径仍由既有 `Ok(...)`、Fatal 和异常日志记录；没有吞错、没有降低错误可见性。
- 不修改 Render Projection、Scene、World、Gizmo、Picking、Selection、Camera 或 Vulkan 生命周期。

## 五、剩余状态

功能阻断：无。
R5 遗留：无阻断项。
后续建议：进入 ARCH-WORLD 后续阶段规划，优先处理测试程序集分层债务与下一阶段游戏领域闭环入口，不在 R5 内继续扩范围。
