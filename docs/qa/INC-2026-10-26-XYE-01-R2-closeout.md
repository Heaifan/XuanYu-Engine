# INC-2026-10-26-XYE-01 — R2 收口与关闭证据

**事故级别**：T0+  
**事故状态**：CLOSED  
**用户书面裁定日期**：2026-10-26  
**规范仓库**：`E:\MyDoc\project-VSCode\XuanYuEngine`  
**R2 基线**：`fa3baecc07a5c6b3a1b49a1abc19348b69fe4072`，分支 `codex/渲染器架构重建`  
**授权范围**：更新事故记录并归档证据；不提交或推送源码。

用户接受执行报告中的 R2 证据，裁定 R2 PASS / T0+ CLOSED。该裁定是书面审计结论，不表示用户独立读取了本地源码或原始日志。

## 关闭门槛与证据

### 1. 旧工作区链路

- 活跃脚本扫描中，已删除工作区入口及旧环境变量没有命中；相关 PowerShell 文件通过语法解析。
- XYC / XYT 只完成静态入口整改与解析，没有运行它们的 runner 或功能回归。
- Git 跟踪历史中保留的 changelog 记录没有为追求零搜索结果而改写。

### 2. Rebuild 与路径覆写护栏

`scripts/tests/test_xye_dotnet_reuse.ps1` 的安全拒绝矩阵通过：13 种 Rebuild 参数形式和 17 种输出/缓存路径覆写均在 dotnet 执行前拒绝；正常 Build、Restore、no-build Test 的帮助入口通过，拒绝测试没有启动构建进程。

覆盖了冒号、等号、空格分隔、大小写变化、多个 target，以及 `--artifacts-path`、`--output`、`-o` 和 MSBuild 输出/中间目录、NuGet 路径属性。该护栏约束 `xye-dotnet.ps1` 及转交给它的兼容入口；外部直接调用 SDK 仍可绕过。

### 3. World.Tests 构建恢复

目标：`XuanYu.World.Tests/XuanYu.World.Tests.csproj`，Debug，SDK `10.0.400`。

Restore 命令只启动一次。其标准输出为“正在确定要还原的项目… / 所有项目均是最新的，无法还原”，标准错误为空，Restore 进程退出码为 0。首个包装脚本因将正常输出文本与退出码混合判定而误报失败；修正返回值处理后，使用 `-RestoreCompleted` 继续，未重复 Restore。

随后正常 Build 只执行一次，成功生成 `XuanYu.World.Tests.dll`：0 警告、0 错误。没有执行 Rebuild、Clean 或递归删除 `bin/obj`。

受管日志位置及 SHA-256：

| 证据 | 文件 | SHA-256 |
|---|---|---|
| Restore stdout（86 bytes） | `obj/xye-build-logs/38c9137a1f754c67bfcf7a17e010ae3e.stdout.log` | `0A5293E62D3BB4D8819D8D749F51206D3AA424744E622337BEE96070823F245B` |
| Restore stderr（空） | `obj/xye-build-logs/38c9137a1f754c67bfcf7a17e010ae3e.stderr.log` | `E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855` |
| Build stdout（1295 bytes） | `obj/xye-build-logs/8aaae8e9be5d4dfe9389018b5b4bb57b.stdout.log` | `56C328312DE90B3F503A524960060872CCCCF4FFDE54E46DDB07F02F1B101390` |
| Build stderr（空） | `obj/xye-build-logs/8aaae8e9be5d4dfe9389018b5b4bb57b.stderr.log` | `E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855` |

构建身份：`E9A98D6B7594794A29853A578223E756AAFB32268F47797A38945E1009E57247`。构建清单：`XuanYu.World.Tests/obj/.xye-build-identity.json`，文件 SHA-256 `81F1598DEC021733C791591D60FB514D9A62526C471BB1458BD50F12DE2475FC`。

### 4. no-build 测试与构建复用

两组测试均经 `scripts/xye-dotnet.ps1` 执行，退出码为 0：

```text
test XuanYu.World.Tests/XuanYu.World.Tests.csproj --no-build --no-restore --configuration Debug --filter FullyQualifiedName~XyeDiagnosticPathsTests
通过 2，失败 0，跳过 0

test XuanYu.World.Tests/XuanYu.World.Tests.csproj --no-build --no-restore --configuration Debug --filter FullyQualifiedName~RendererEventSessionIdentityTests
通过 2，失败 0，跳过 0
```

之后两次以相同项目、配置和输入调用 `xye-build.ps1`，均观察到 `BUILD REUSED`；独立 PowerShell 进程中的复用检查退出码为 0。复用期间未执行 Restore 或 Build。

DLL：`XuanYu.World.Tests/bin/Debug/net10.0/XuanYu.World.Tests.dll`，长度 1,716,224 bytes，SHA-256：`1B60507829A1692EF7E1CC2CB983AFBE7E8748E0252194DC3B5138A7FBF34C7B`。该哈希与构建身份清单中的输出记录一致。

### 5. 并行写入守卫

`scripts/tests/test_xye_write_guard.ps1` 最终运行通过：同文件双 Owner 被拒绝、不同文件可并行登记、陈旧基线被拒绝、释放后允许重新登记。守卫是协作式范围控制；IDE、操作系统级写入和绕过守卫的文件操作不受它拦截。

## Dirty 与磁盘记录

R2 开始时记录了 88 项状态及各路径 SHA-256 / `MISSING` 标记，基线清单归档于 [R2 Dirty 基线 JSON](INC-2026-10-26-XYE-01-R2-dirty-baseline.json)。施工后 7 项与已授权 R2 文件相符变化，非预期变化为 0。该 R2 基线不弥补 R1 施工前 59 项 Dirty 缺少完整哈希记录的历史限制。

E 盘空闲字节：R2 基线 `111149801472`，收口后 `110302547968`，净变化 `-847253504` bytes。验收未新增任务级 `Builds`、`XYE-Workspace*`、`.xye-workspace` 或 NuGet 缓存目录；NuGet 全局缓存位置仍为 `C:\Users\Heai\.nuget\packages\`。空闲空间差值不单独归因于某一个文件集合。

## 永久保留的范围说明

- R1 曾错误放行 Rebuild 并造成测试 DLL 缺失；R2 用一次 Restore、一次正常 Build 恢复有效输出。
- R1 施工前 59 项 Dirty 无完整哈希基线，无法追溯证明完整保全。此限制永久保留。
- XYC / XYT 没有本轮运行时验收；相关功能回归保持未执行，转交各自模块正常任务。
- Codex 进程仍继承 `XYE_WORKSPACE_ROOT=E:\XYE-Workspace`，当时该路径不存在；受管入口和本次构建不依赖该变量。未修改用户级或系统级环境变量。
- 当前源码修改未提交或推送。事故关闭不代表正式版本发布、全解决方案回归或全工程功能验收完成。
- 事故关闭后不再为此重跑全解决方案构建，不引入额外工作区治理系统。

**最终裁定**：R2 PASS / T0+ CLOSED（按用户 2026-10-26 书面批准）。
