# AI 整合指令 — a051352 Gizmo 完整鼠标交互修复

## 目标

将 `patch/` 下的文件按相对路径覆盖到 FluidWarfare 仓库，完成右上角 Blender 风格导航 Gizmo 的真实鼠标操作，并修复 Resize 后 Overlay 绘制位置与 CPU HitTest 不一致的问题。

## 一、基线确认

在仓库根目录执行：

```powershell
git rev-parse --short HEAD
git status --short
```

预期基线为 `a051352`，工作区应干净。若 HEAD 已向后推进，先逐文件合并，不要直接覆盖后续人工修改。

## 二、复制文件

可在本修复包目录执行：

```powershell
.\apply_patch.ps1 -RepoRoot "E:\MyDoc\project-VSCode\fluidwarfare"
```

或者手工将 `patch/` 中全部文件按相对路径覆盖到仓库。

新增文件：

```text
FluidWarfare.Render/ViewportNavigation/ViewportNavigationPressResult.cs
```

其余文件均为完整替换版本。

## 三、必须理解的根因

Overlay Pipeline 的 Viewport 是创建时固定的。原 `Resize()` 没有重建 Overlay 资源，导致：

```text
GPU Overlay：旧尺寸
CPU Layout/HitTest：新尺寸
```

因此 Gizmo 会从右上角偏到顶部中间，并且 `GetPresentedNavigationLayout()` 因尺寸不一致返回 `null`，鼠标操作全部失效。

本补丁在 Swapchain Resize 时同步事务式重建 Overlay Shader/Pipeline/Layout/VertexBuffer，并在成功 Present 后发布与实际画面一致的 Overlay Snapshot。

## 四、构建与测试

```powershell
dotnet restore FluidWarfare.sln
dotnet build FluidWarfare.sln -warnaserror
dotnet test FluidWarfare.sln --no-build
```

要求：

```text
0 errors
0 warnings
全部测试通过
测试数量高于 492
```

如果测试数量仍为 492，说明新增测试文件没有放到正确目录或未被测试项目包含。

## 五、真实 GUI 验收

```powershell
dotnet run --project FluidWarfare.Editor.Windows --no-build
```

必须逐项操作，不允许只凭 build/test 宣称通过。

### 1. 位置与 Resize

- 启动后 Gizmo 位于 Vulkan 视口右上角；
- 最大化、恢复、连续 Resize 后仍位于右上角；
- 不得再次出现在顶部中间；
- Resize 后按钮和轴端仍可点击。

### 2. 六轴端

依次点击：

```text
+X -X +Y -Y +Z -Z
```

要求：切换到相应正交视图，点击不选择后方实体，不产生地面落点日志。

### 3. 拖动操作

- 左键拖动导航球内部空白区域：Orbit；
- 左键拖动手掌：Pan；
- 左键拖动放大镜：Zoom；
- 拖出视口后仍因 Mouse Capture 持续操作；
- 松开、失焦或 CaptureLost 后立即停止。

### 4. 按钮

- 靶心：有实体时聚焦所选；无实体时显示全部；
- 投影：透视/正交切换，画面尺寸不应突然剧烈跳变；
- 正交模式下滚轮和放大镜拖动必须实际缩放。

### 5. 防穿透

将实体移到 Gizmo 后方并点击 Overlay：

```text
实体选择不变化
Ground Cursor 不变化
不输出“地面落点”日志
```

### 6. Picking 回归

在自由透视、自由正交、六标准视图、Pan/Zoom/Orbit 和 Resize 后分别点击三个样例实体，必须全部命中。

### 7. 按需帧

停止操作 10 秒：

```text
Frame 编号不增长
日志不增长
CPU 占用回落
```

## 六、代码扫描

```powershell
git grep -n "GizmoYaw\|GizmoPitch\|OverlayCameraState" -- "*.cs"
git grep -n "ViewportNavigationPressResult" -- "*.cs"
git grep -n "VulkanOverlayResources.TryCreate" -- "*.cs"
```

要求：

- 不存在独立 Gizmo 相机状态；
- PressResult 贯穿 NativeHost → Panel → EditorShell；
- Overlay Resources 在 Session Start 和 Resize 路径都有创建调用。

## 七、文档与提交

同步：

```text
CODE_CONSTITUTION.md
docs/CHANGELOG.md
file-tree.md
```

代码宪法至少增加：

```text
1. Overlay 绘制与 HitTest 必须使用同一 Presented Layout。
2. Resize 后必须使 Overlay Pipeline/Viewport 与新 Swapchain Extent 对齐。
3. Overlay 命中必须优先于世界 Picking。
4. 一次性 Overlay 点击不得无意义捕获鼠标；拖动操作必须捕获鼠标。
```

最后：

```powershell
git add .
git commit -m "fix: complete interactive viewport navigation gizmo"
git push origin main
git status --short
```

最终报告必须包含新 SHA、实际 GUI 截图、测试数量、Resize 后 Gizmo 坐标、六轴点击结果、四按钮结果和防穿透结果。
