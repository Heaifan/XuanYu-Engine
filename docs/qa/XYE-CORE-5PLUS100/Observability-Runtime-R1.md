# XYE Core 5+100｜Observability/Runtime 静态审计 R1

- 日期：2026-10-24；基线：`5543a528bbbae014578b04f82d2dd6834764d20a`；分支：`审计XYE架构`。
- 本轮范围：`XuanYu.Core/Observability/Runtime` 两份生产文件（`CoreSelfTest.cs`、`XyeWorkspacePaths.cs`），及新增的独立镜像测试。
- Core 直属模块当前 7 个，符合已批准的 **10 个警戒线**（超过 10 个先评估是否拆分或编组）；二级以下保持每夹子文件夹/源码文件均≤5，单C#源码≤100行。
- 绝不把静态代码与测试源码的存在视为测试通过：Build NOT RUN、xUnit NOT RUN、Windows/Vulkan NOT RUN、Integration NOT PASS。

## 1. CoreSelfTest.cs（核心运行自检，71 行）

**全部显式方法与成员：**
- `CoreSelfTest.Run()`：执行 6 项核心自检，覆盖错误对象、结果对象、Vector3d、YawRotation、SimulationTime、EngineLogEntry；收集逐项结果并形成报告。
- `CoreSelfTest.Check(string,Func<bool>)`：封装单项检查，捕获异常并将异常消息转换为失败明细；静态私有方法。
- `CoreSelfTestItem(string,bool,string)`：创建具备名称、通过状态与文字细节的不可变检查项。
- `CoreSelfTestReport(IReadOnlyList<CoreSelfTestItem>)`：构造核心自检汇总结果。
- `CoreSelfTestReport.IsPassed`：判定自检是否通过。旧版 `Items.All(...)` 在空列表时返回 true，可能误报“无检查也通过”；改为 `Items.Count > 0 && Items.All(...)`，空报告不通过。
- 实际 Run 的 6 项自检内容、名称、调用方式与异常隔离机制均不变。

## 2. XyeWorkspacePaths.cs（探针目录和工作区边界，81 行）

**全部显式方法与成员：**
- `ResolveProbeDirectory(string?)`：解析或创建诊断目录；可从显式 Run ID / 环境变量选择路径；要求落在工程工作区范围内。
- `EnsureInsideWorkspace(string)`：对传入路径规范化并验证必须处在指定工作区内部；越界抛异常。
- `IsChildOf(string,string)`（新增 internal）：纯路径字符串判定，确保路径位于 root 的实际子级，拒绝同名前缀邻居和 `..` 跳出；不进行文件系统写入。
- `PathComparison`（新增私有静态属性）：Windows 不区分大小写；非 Windows 平台使用区分大小写比较。
- `GetRootDirectory()`：寻找工程根并解析 `XYE_WORKSPACE_ROOT`，要求根目录位于项目内部。
- `FindProjectRoot(string)`：沿父目录查找 `.git` 标识来定位仓库根。
- `IsWithin(string,string)`：判断工作区根路径相对于工程根的包含关系（可包括工程根本身）。
- 修复：之前所有平台使用 `OrdinalIgnoreCase`，在大小写敏感文件系统上可能误接纳仅大小写不同的路径；现在使用平台相符比较。
- 保留：`EnsureInsideWorkspace` 对工作区根本身依旧拒绝（只允许子目录），既有运行时输出路径与环境变量名不变。
- 边界：仅做**字面规范化路径**校验，未解决目录链接 / Windows junction 将真实位置指向工作区外的风险；这一项需独立设计和真实 Windows 安全验收，不在本轮不具备运行环境时扩大修改。

## 3. 新增镜像测试（均为 test_ 文件）

`XuanYu.Core.Tests/Observability/Runtime/test_CoreSelfTest.cs`（49 行）：
- `EmptyReportCannotPass()`：空检查列表不得视为自检通过。
- `FailedItemMakesReportFail()`：任何失败项均令汇总失败。
- `NonEmptySuccessfulReportPasses()`：至少一项且全部成功时通过。
- `CoreSelfTestListsSixChecks()`：核对核心自检具有六类预期检查。

`XuanYu.Core.Tests/Observability/Runtime/test_XyeWorkspacePaths.cs`（46 行）：
- `Workspace()`：构造测试用的路径字符串，无文件系统副作用。
- `AllowsChildDirectoriesWithoutWritingFiles()`：路径归属有效。
- `RejectsRootAndSamePrefixSibling()`：根自身与同名前缀目录不算子目录。
- `RejectsTraversalOutsideWorkspace()`：`..` 不得跨越工作区根。
- `UsesPlatformAppropriateCaseComparison()`：Windows/Linux 大小写策略合理。

## 4. 核查与收口

- 生产：Observability 下 `Logging` 三文件、`Runtime` 两文件，直接子目录数量2；Runtime 源码最大文件81行。
- 测试：Runtime 镜像子目录2文件，测试文件均≤100行；没有临时产物，也没有修改任何其它生产模块。
- 本轮静态审计完成；单元测试仅是已提交源代码，尚未编译或执行。
- Runtime 运行期及工作区路径边界完整验证：**PARTIAL / 未关闭**；不得宣称 Integration PASS。
