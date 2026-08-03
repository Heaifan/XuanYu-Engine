# ARCH-C-R5 Transform Session 封版记录

版本：v0.2.17.26-rz
日期：2026-07-20

## Entry Gate

审计确认旧 `VulkanRenderSession.UpdateScene` 每次场景快照变化都会执行 `Present Stop -> CommandBuffer 重录 -> Present Start`。该路径只能服务低频提交，不能复用于 PointerMove。

R5 建立的最小高频通道为：UI 发布最新只读渲染快照，`VulkanClearFrameOwner` 以单槽覆盖合并，Present 线程在 fence 安全点消费最新快照并重录命令；不停止 Present、不堆积请求、不输出逐帧日志。未扩展 Swapchain、Pipeline 或 Vulkan 资源种类。

## 状态合同

```text
CommittedTransform      SceneStateOwner 正式事实
TransformStartSnapshot  Begin 时的实体与起始 Transform
PreviewTransform        PointerMove 临时渲染位置
```

PointerMove 不修改 Scene 或 SpatialIndex。MouseUp 先校验当前 Interaction Session，再让 Transform Session 失效并最多正式提交一次。Escape、`WM_CANCELMODE`、失焦、CaptureLost 和 Host Detach 均取消 Session、丢弃 Preview；迟到 MouseUp 因 Session 已失效而被忽略。

首版拖动把鼠标位移投影到 R4 已显示轴的屏幕方向，再只写对应世界 X / Y / Z 分量。不包含 Rotate、Scale、Local Space、吸附、多选、平面拖动或 Undo。

## 自动验证

- Core tests：覆盖 X/Y/Z 轴约束、垂直鼠标运动不产生位移、Preview 与正式 Scene/SpatialIndex 隔离、一次 Commit、Cancel 与迟到 Commit、渲染 Preview 覆盖不替换正式 Transform。
- 高频边界：PointerMove 不写普通日志、不刷新 Inspector/Diagnostics；渲染请求单槽覆盖。
- 真机回传：用户确认拖动正常；日志显示 X / Y / Z / Z / X 五次真实 Move 会话均进入 Begin -> Commit -> End，Position 只沿对应轴变化；拖动期间无 Present Stop / Start；日志栏 Resize 后 Swapchain 自愈到 1248x478 并恢复 Present；正常关闭按 Present -> Pipeline -> ClearFrame -> RenderSession -> Swapchain -> Device -> Surface -> Instance 释放。
- Cancel 真机回传：Session=2 执行 Escape Cancel，Session=3 执行 `WM_CANCELMODE` Cancel，Session=4 执行 Escape Cancel 后松手；三次取消后的 Position 均保持 `Vector3d(0, 0, -2.604)`，且没有旧 Session 的提交捕获或正式 Position 改写。

## 禁止项确认

- [x] 未实现 Undo、Rotate、Scale、Local Space、Snapping、多选。
- [x] 未加入地平面、原点、世界轴、天空盒或 Gizmo UX 美化。
- [x] 未新增依赖、项目或 Vulkan 资源类型。
