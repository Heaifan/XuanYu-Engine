# XYE-R2 检查点：Muse 独立测试交接

## 范围与代码身份

本交接用于独立验证开发进度检查点，不是 Release、产品最终验收或 P3/P4 PASS。

| 修复 | 版本 | 产品提交 |
|---|---|---|
| Camera FarPlane after terrain constraint | `v0.3.0.11-fix` | `95a3bab1e91c7ca1053e993bcead18c9854c616a` |
| SessionGeneration for render resource updates | `v0.3.0.12-fix` | `43a80d456ff9a73cbbd9d9251f05df248e6d133b` |

两项代码提交在同一分支顺序提交；本文件随 QA/证据提交单独记录。Muse 开始前先 Bootstrap，记录实际 HEAD、branch、dirty、SDK 和 `./scripts/resolve-version.ps1`；测试报告必须绑定实际 SHA。不要假定运行中的旧 App 对应当前 HEAD。

## 环境与串行运行顺序

仓库：`E:\MyDoc\project-VSCode\XuanYuEngine`。SDK Resolver：`scripts/resolve-dotnet.ps1`，正式命令使用 `scripts/xye-dotnet.ps1`。期望版本源为 `v0.3.0.12-fix`。所有 Build/Test 串行执行；本轮独立验证不得修改产品、测试断言、XYT 映射、版本、Changelog 或 XYK，也不得 Commit/Push。

1. `./scripts/xye-bootstrap.ps1`
2. `./scripts/resolve-version.ps1`
3. `./scripts/xye-dotnet.ps1 build ./XuanYu.Engine.slnx --no-restore -m 1 -nr false -p:BuildInParallel=false -p:UseSharedCompilation=false`
4. `./scripts/xye-dotnet.ps1 build-server shutdown`
5. 依次运行下列完整正式测试（每项退出后再开始下一项）：

```powershell
./scripts/xye-dotnet.ps1 test ./XuanYu.Core.Tests/XuanYu.Core.Tests.csproj --no-build --no-restore --logger "console;verbosity=minimal"
./scripts/xye-dotnet.ps1 test ./XuanYu.World.Tests/XuanYu.World.Tests.csproj --no-build --no-restore --logger "console;verbosity=minimal"
./scripts/xye-dotnet.ps1 test ./XuanYu.WarCore.Tests/XuanYu.WarCore.Tests.csproj --no-build --no-restore --logger "console;verbosity=minimal"
./scripts/xye-dotnet.ps1 test ./xyui/avalonia/tests/XYUI.Avalonia.Tests/XYUI.Avalonia.Tests.csproj --no-build --no-restore --logger "console;verbosity=minimal"
```

6. 分别复跑 Camera/Surface Authority 与 Generation 专项：

```powershell
./scripts/xye-dotnet.ps1 test ./XuanYu.World.Tests/XuanYu.World.Tests.csproj --no-build --no-restore --filter "FullyQualifiedName~XYEPR2NavigationSafetyFarPlaneTests|FullyQualifiedName~XYEPR2SurfaceAuthority" --logger "console;verbosity=minimal"
./scripts/xye-dotnet.ps1 test ./XuanYu.World.Tests/XuanYu.World.Tests.csproj --no-build --no-restore --filter "FullyQualifiedName~XYEPR2Generation" --logger "console;verbosity=minimal"
```

7. 架构与 whitespace 检查：`./scripts/arch-a-guard.ps1`、`./scripts/arch-a-guard-warcore.ps1`、`git diff --check`。
8. 检查 XYT 映射状态（此 runner 当前预期仍可能报告 `REVIEW_REQUIRED`，不要改映射或将其写成 PASS）：

```powershell
./scripts/governance/xyt-runner.ps1 -DiffRange HEAD -AgentTests 'FullyQualifiedName~XYEPR2NavigationSafetyFarPlaneTests','FullyQualifiedName~XYEPR2SurfaceAuthority','FullyQualifiedName~XYEPR2Generation'
```

保留每条命令的完整 stdout/stderr、退出码、测试计数、开始/结束时间及实际 SHA。不得因为 focused PASS 删除或过滤完整套件的失败。

## 已有验证证据

提交前 Parent 的检查点结果：完整方案 Build `0 Warning / 0 Error`；World `2271/2271`、WarCore `22/22`、XYUI `708/708`；FarPlane/Surface Authority/Generation 合计 `12/12`；ARCH-A、ARCH-A-WarCore、`git diff --check` 通过。`.11-fix` 和 `.12-fix` 版本源下各自重建通过，Camera/Surface Authority 为 `6/6`，Generation 为 `6/6`。Muse 必须独立复跑，不得引用这些结果代替自己的证据。

## 已知问题与平台限制

- Core 全量：`503 PASS / 2 SKIP / 1 FAIL`，失败为 `EmptyWorldBaselineContractTests.New_scene_returns_to_empty_world_without_map_roundtrip`（`Assert.False` 实际为 True）。已有干净归档 `79476c4c48dac03efbda2338da3298d3cf2a97b2` 中同一失败的记录；归档测试与 `NewBlankScene` 源码 SHA-256 与当前一致，且这些文件在归档后没有提交修改。请保留本轮实际失败输出，标记为已证实的基线失败，不要跳过测试或声称 Core PASS。
- XYT runner 因路径 Owner/Capability 映射缺项返回 `REVIEW_REQUIRED`（退出码 2）。可见争议覆盖 `Directory.Build.props`、Render Abstractions/Vulkan、World.Tests 和部分文档路径。全套测试已经实际运行不等于 runner 映射 PASS；不要在本独立测试任务里更改 XYT 权威。
- P3 当前仅有真实 App/ HWND/ Native Viewport、Vulkan 模块与 RTX 3060 枚举，以及窗口截图；没有 Operator-generated Move/Down/Up 链，也没有 LogicalDevice、Swapchain、Present 证据。状态 `PARTIAL / NOT PASS`。截图与运行身份见 `docs/qa/XYE-R2-WP3-NATIVE-P3.md` 和 `docs/qa/evidence/XYE-R2-WP3-C/20261010-run01/`。
- 用户 P4 `PENDING`。Muse 不得把自动测试或截图分析改写成用户验收。

## 中文 IPO（真实 Windows Runtime，单独时段）

这部分不是本轮自动化测试的替代品。只在 Parent 分配独占 Runtime 时段后执行；使用由目标 Commit 新构建的 App，先记录 EXE/DLL 路径、SHA-256、版本、Commit、进程 PID、主 HWND、Viewport 子 HWND、DPI、Vulkan Device/Swapchain/Present。当前已打开窗口来自检查点构建前的产物，不能直接当作目标提交身份。

| 步骤 | 输入与操作 | 观察/期望 | 证据 |
|---|---|---|---|
| 1 | 启动目标 Commit 的 Editor，导入有已知高程变化的 DEM，记录当前 `ObservationCenter` 与相机状态。 | EXE、App DLL、进程模块与 Commit/版本对应；主窗及 Native Viewport 有效。 | 运行身份、设备信息、截图。 |
| 2 | 使用 Editor Wheel 普通 Dolly，多次向地形方向缩放，覆盖约束前后各一帧。 | 相机最终位置保持安全净空；FarPlane 按最终受约束位置覆盖 `ObservationCenter`；`ObservationCenter` 不因 Wheel 改变。 | 输入序列、前后相机/中心/FarPlane、日志及可见画面。 |
| 3 | 继续做 Orbit、Pan、普通 Terrain refresh/LOD 更新。 | Orbit 使用冻结 Pivot；Wheel/Orbit 不重选中心；刷新/LOD 不隐式修改中心。 | 前后 Pivot/Center 与视口截图。 |
| 4 | 打开 Map A 后切换到新的 Map Session/Map B；快速连续发布较新的修改，再尝试延迟旧 Session 更新。 | 新 Session 的低 sequence 更新可被接受；旧 generation 更新被拒绝。仅此项不能证明 GPU Buffer 已释放。 | Generation/Sequence、资源键、帧/日志证据。 |
| 5 | 清空 Map/切换 Terrain Context，再建立另一张地图。 | 旧 Surface 实际清除，新 Surface 正确重建并显示；必须观察 Vulkan 资源清除/创建与 Present，CPU policy 结果不够。 | Device/Swapchain、资源 Dispose/Create、帧与截图。 |
| 6 | 若任一步出现 FAIL，冻结版本、输入、日志和截图；不得临时修产品或改测试断言。 | 另建 Root Cause 与最小 RED 复现，回交 Parent。P4 仍待用户裁定。 | 可复核失败包。 |

## Muse 交付格式

按实际执行输出 Bootstrap/HEAD、版本、提交 SHA、每条 Build/Test/XYT/ARCH 命令退出码、计数和日志位置；逐项区分 PASS、FAIL、REVIEW_REQUIRED、NOT RUN、UNKNOWN。明确列出 Core 基线失败、XYT 映射争议、P3/P4 限制。只提供独立结果和修复候选，不修改共享源码、不 Commit/Push。
