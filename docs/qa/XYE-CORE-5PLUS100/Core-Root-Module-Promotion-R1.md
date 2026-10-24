# XYE Core 根目录模块提升 R1（Probe / Replay）

- 日期：2026-10-24；目标分支：`审计XYE架构`；Parent：`ee0a303c8d9f78747214f625bb7c327ea64d735f`。
- 用户批准：Core 根目录可拥有超过五个直属模块；**10 个为架构警戒线，超过 10 个须先评估拆分/编组，不得自动继续堆叠**。直属二级及以下文件夹仍执行每夹至多五个直接子文件夹、至多五个直接源码文件，单源码文件最多100行（5+100）。
- 方案：不再增设中间包装目录；把 `Probe` 和 `Replay` 从 `XuanYu.Core/Observability` 提升为 `XuanYu.Core` 直属模块。
- 提升后 Core 直属模块：`Editing`、`Foundation`、`Observability`、`Probe`、`Replay`、`Scene`、`Space`（**7/10**）。
- 原 `Observability` 仅保留 `Logging` 和 `Runtime`；没有把文件平铺到 Core 根目录。

## 物理目录迁移

| 旧路径 | 新路径 | 生产文件 | 对应测试文件 |
| --- | --- | ---: | ---: |
| `XuanYu.Core/Observability/Probe/Capture` | `XuanYu.Core/Probe/Capture` | 1 | 1 |
| `XuanYu.Core/Observability/Probe/Frames` | `XuanYu.Core/Probe/Frames` | 4 | 2 |
| `XuanYu.Core/Observability/Probe/Owner` | `XuanYu.Core/Probe/Owner` | 3 | 1 |
| `XuanYu.Core/Observability/Replay/Mesh` | `XuanYu.Core/Replay/Mesh` | 4 | 2 |
| `XuanYu.Core/Observability/Replay/Options` | `XuanYu.Core/Replay/Options` | 3 | 3 |
| `XuanYu.Core/Observability/Replay/Sse` | `XuanYu.Core/Replay/Sse` | 4 | 4 |

- 共迁移 19 个生产源码文件、13 个镜像测试源码文件。
- 生产源码保留**完全相同 Git blob SHA**，因此生产公共命名空间、类名、方法签名和实现一律不变。
- 镜像测试文件从 `XuanYu.Core.Tests/Observability/{Probe|Replay}` 同步迁移到 `XuanYu.Core.Tests/{Probe|Replay}`，并仅调整测试源文件中的 `namespace` 前缀；保持原测试类、函数和断言，不顺带修复其它模块。
- `XuanYu.Core.csproj` 和 `XuanYu.Core.Tests.csproj` 使用 SDK 默认 Compile glob，未发现写死上述旧路径的显式 Compile 引用，不修改项目文件。
- 旧路径已保存在这份映射中；**历史 QA 记录保持历史路径而不改写既有证据**。其它可能包含硬编码旧文件路径的非源码工具/文档，需要 Windows 集成环境后专门核查。

## 5+100 门禁

- Core 根目录：7 个直属模块，低于10个警戒线。
- `Probe`：Capture 1、Frames 4、Owner 3；测试镜像为 1/2/1。
- `Replay`：Mesh 4、Options 3、Sse 4；测试镜像为 2/3/4。
- `Observability`：Logging 3、Runtime 2。
- 源码行数：迁移代码内容不变，原已静态核查所有迁移源码不超过100行。
- 候选源码静态目录结构：PASS；编译 NOT RUN；xUnit NOT RUN；Windows Vulkan NOT RUN；Integration NOT PASS。

## 风险与待核查

- 单纯 Git 文件树移动不改变 C# 编译语义，但不能取代实际编译/运行确认。
- 不更新生产命名空间 `XuanYu.Core.Diagnostics` / `XuanYu.Core.Space.Diagnostics`，以避免外部调用方改名。
- 上述仅确认 Core 两个项目文件无显式旧路径，不能推断全部脚本、工具和历史文档都没有旧路径引用。
- 本次不修改 `Directory.Build.props` 版本、不动其它生产模块或历史报告，也不将 Candidate 静态整理标成 Integration PASS。
