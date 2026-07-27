# WORLD-B-R5 Scale Gizmo 缩放变换闭环报告

版本：v0.2.20.18-rz
日期：2026-07-27 22:32:12
分支：feat/WORLD-B-editor-interaction
阶段：WORLD-B-R5：Scale Gizmo 缩放变换闭环

## 裁定

本轮完成 R5 自动门禁实现，状态为等待真机验收。不得宣布 R5 CLOSED，不得启动 F6 / WarCore。

本轮只实现实体自身 TRS 的 X/Y/Z 局部分量缩放与中心 Uniform 等比缩放，不实现负缩放、镜像、多选、Pivot、吸附、数值输入、Local/Global 切换、父子传播或世界空间剪切。

## 实现变化

- Core 新增 Scale Gizmo 纯函数：轴/手柄身份、屏幕空间恒定尺寸、布局投影、命中测试和指数倍率拖动解算。
- Editor 复用 `TransformSession`，新增 `BeginScale` 与 `ScaleHandle`，Preview / Commit / Cancel 仍走既有生命周期。
- UI 在“缩放”工具下先尝试 Scale Gizmo 命中；未命中时保留 R4 的直接 Picking 切换实体逻辑。
- Render Projection 新增 Scale Gizmo 可见性、世界轴长和朝向字段。
- Vulkan shader 绘制三轴端方块与中心等比方块，按实体 Rotation 对齐局部轴。
- RenderProjection 创建改读 Preview 后的 `RenderTransform`，保证预览缩放时实体、轮廓和 Gizmo 同步。

## 自动验证

```text
dotnet build .\XuanYu.Engine.slnx -m:1 -nr:false -p:BuildInParallel=false -p:UseSharedCompilation=false
结果：10 项目 0W0E

dotnet test .\XuanYu.Core.Tests\XuanYu.Core.Tests.csproj --no-build --no-restore
结果：125 passed / 0 failed / 0 skipped

dotnet test .\XuanYu.World.Tests\XuanYu.World.Tests.csproj --no-build --no-restore
结果：160 passed / 0 failed / 0 skipped

git diff --check
结果：PASS

5+100
结果：PASS
```

首次沙箱内 build 因 NuGet 网络权限 NU1301 失败，未进入 C# 编译；授权非沙箱构建后通过。

## 真机验收清单

#### 测试项目：显示 Scale Gizmo

- 序号：01
- 路径：运行 `run.bat` → 中央视口 → 左侧“层级” → 顶部“缩放”
- 输入 I：
  - 选择任意可见实体。
- 过程 P：
  1. 点击顶部工具栏“缩放”。
  2. 拉近、拉远相机。
  3. 查看三轴端方块和中心方块。
- 输出 O：
  - X/Y/Z 三轴与中心柄清晰可见。
  - Gizmo 位于实体中心，尺寸随相机距离基本稳定。
  - 浅蓝白轮廓仍存在，且不异常巨大或过小。

#### 测试项目：缩放闭环

- 序号：02
- 路径：顶部“缩放” → 中央视口 → 顶部“撤销”“重做”
- 输入 I：
  - 当前已选中一个实体并显示 Scale Gizmo。
- 过程 P：
  1. 分别拖动 X/Y/Z 控制柄。
  2. 拖动中心等比控制柄。
  3. 对一次缩放执行“撤销”和“重做”。
  4. 拖动中按 Escape 后再松开鼠标。
- 输出 O：
  - 单轴只改变对应 Scale 分量，Uniform 三轴同倍率。
  - Preview 实时变化，松手只提交一次。
  - Undo 一次恢复，Redo 一次重现。
  - Escape 恢复原 Scale，延迟 MouseUp 不提交。

#### 测试项目：缩放工具内切换实体

- 序号：03
- 路径：顶部“缩放” → 中央视口 → 右侧“检查器”
- 输入 I：
  - 当前选择实体 A，场景内还有实体 B。
- 过程 P：
  1. 保持“缩放”工具激活。
  2. 点击实体 B。
  3. 不切换工具，立即拖动 B 的缩放柄。
- 输出 O：
  - 轮廓、Gizmo 和检查器立即切到 B。
  - 第一次拖动只缩放 B，A 的 Scale 不变。

## 最终状态

R5 自动门禁实现完成，等待用户真机验收。真机通过前不得宣布 WORLD-B-R5 CLOSED。
