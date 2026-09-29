# Engineering 工程知识

## K-VAL-001 用户运行产物必须与验证产物一致

**状态**：Active
**优先级**：P0
**证据等级**：E1
**标签**：Validation、Artifact、Runtime、CopyToOutput、False Positive
**适用范围**：Editor.App、Render.Vulkan、Shader Bytecode、Native DLL、资源复制、任何存在多级 Build/Copy/Embed 的运行链。

**首次确认**：2026-08-10 16:51:42（UTC+08:00）
**版本**：`v0.2.25.18-stab`
**Commit**：`06b26e9`
**来源**：`changelog.md` / MAP-A-R3-D2-F1 STAB-5A

### 问题

源码、单元测试和目标项目 Build 全部正确，并不自动证明用户启动的程序正在运行这些新产物。只要中间存在输出复制、Shader 内嵌、应用打包、固定启动目录或旧进程未退出，验证对象就可能与用户运行对象分叉。

### 根因

验证链曾默认“源码 HEAD = 用户运行程序”，却没有显式证明：

```text
Git HEAD
→ Build Artifact
→ Copy / Embed 后 Artifact
→ Editor.App 实际输出目录
→ 用户启动的 EXE / 已加载模块
```

每一级都一致。

### 工程规则

运行时问题只有在“验证对象”和“用户实际运行对象”被证明一致后，自动验证结果才可以用于真机结论。涉及跨项目复制或生成产物时，交付验证必须把产物身份链作为一等证据。

### 禁止做法

- 仅凭 `Build 0W0E` 或测试 PASS 宣布真机问题已修复。
- 修改 Shader 源文件后只检查 GLSL 编译，不检查内嵌字节码和 App 输出。
- 用户仍运行旧 Editor 进程时，直接把新源码结果当成真机结果。
- 看到自动探针 PASS 后跳过运行目录/模块版本核对。

### 正确做法

1. 记录当前 Git HEAD。
2. 确认目标项目 Build 输出的时间/Hash/版本。
3. 确认 Copy/Embed 阶段已经把新产物送到最终 App 输出目录。
4. 确认用户启动的 EXE 路径与预期一致。
5. 必要时记录运行进程加载模块路径、版本或探针值。
6. 只有这条链闭合，才进入真机功能验收。

### 真实历史示例

`v0.2.25.18-stab` 修复比例尺 Native Overlay 时，源码和自动验证已经体现新行为，但 App 输出副本未同步，造成“测试对象正确、用户运行对象仍旧”的假验证。该版本明确把“修复 App 输出副本未同步导致的假验证”写入 changelog，并在真机重启编辑器后确认视口内悬浮 `100 m` 控件可见。

### 未来应用示例

若修改 `scene.vert`：

```text
scene.vert
→ glslc
→ ShaderBytecode.Vert.cs
→ XuanYu.Render.Vulkan Build
→ XuanYu.Editor.App 输出复制
→ 用户运行进程
```

即使 Shader 合同测试 PASS，只要 `ShaderBytecode.Vert.cs` 或 App 输出目录仍旧，就必须判定“产物链未闭合”，不能宣告 Shader 真机修复完成。

### 验证方法

- 版本/Hash/时间戳对照；
- App 启动路径确认；
- 运行时探针返回版本/关键状态；
- 必要时彻底退出旧 Editor 再重启；
- 真机视觉/输入重验。

### 边界

纯算法库且测试进程直接加载当前 Build 输出时，链路可能很短，但仍要证明测试加载的 DLL 是本次 Build 的产物。

**关联 Incident**：INC-2026-08-10-004
**关联 Knowledge**：K-VAL-002、K-NATIVE-001

---

## K-VAL-002 UI / Native 功能必须采用分层验收

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：Acceptance、Headless、Runtime UI、Real Machine、Native HWND
**适用范围**：Avalonia UI、NativeHost、Vulkan Viewport、布局、可见性、命中、拖拽、DPI。

**关键确认**：2026-08-09 19:42:41（UTC+08:00）
**版本**：`v0.2.24.50-fix`
**Commit**：`60fd339`
**来源**：`changelog.md` / MAP-A-R2-D5-F5

### 问题

静态合同或 ViewModel 测试可以证明属性和值，但不能证明真实视觉树完成 Measure/Arrange 后的宽度、裁剪、Z-order、DPI、Pointer 命中和 Native HWND 遮挡都正确。历史上出现过“静态检查全绿，真机冷启动仍错位”的情况。

### 工程规则

UI/Native 功能至少区分以下证据层：

```text
L1 Static Contract
→ L2 Pure Logic / Headless
→ L3 Runtime UI（真实控件实例、Measure/Arrange）
→ L4 Real-machine Visual / Input
```

上层不能被下层替代。L1 PASS 只能证明 L1；没有 L4 证据时，不得把真机验收项写成 PASS/CLOSED。

### 禁止做法

- `IsVisible == true` 就宣称用户能看到控件。
- XAML 文本包含正确 Grid 列定义，就宣称冷启动布局正确。
- Headless 能点击就宣称 Vulkan HWND 上方的 Native Overlay 真能点击/可见。
- 自动验证完成后提前 CLOSED，等待用户真机只是“形式”。

### 正确做法

对每个验收项明确它属于哪一层；若问题涉及真实窗口、DPI、GPU/Native 混合、实际鼠标操作，计划中必须保留 L4。Runtime UI 能自动化的内容尽量前移到 L3，降低真机返工，但不冒充 L4。

### 真实历史示例

`v0.2.24.50-fix` 为 LayerPanel 首次建立 Avalonia.Headless Runtime UI Gate，覆盖冷启动和增层后的宽度稳定性。此前多轮静态合同和业务测试未阻止真机错位；F5 在 Runtime UI 7/7 与用户真机 8/8 PASS 后才收口。

### 未来应用示例

新增“视口右下角比例尺”时：

- L1：确认绑定、样式、位置参数存在；
- L2：确认尺度算法；
- L3：确认控件实际 Measure/Arrange 后尺寸与坐标；
- L4：确认 Vulkan 视口前方真实可见、DPI 正确、滚轮/点击不受影响。

### 验证方法

验收报告必须注明每项证据层级；任何“Visual Regression NOT ENABLED / Real-machine PENDING”都应显式保留，不能省略成总 PASS。

**关联 Incident**：INC-2026-08-09-001、INC-2026-08-10-004
**关联 Knowledge**：K-VAL-001、K-UI-001、K-NATIVE-001

### 2026-08-12 追加：同一视觉入口必须覆盖真实业务路由分支

同一按钮不代表同一运行路径。条件路由的验收矩阵必须按 `Visual Entry × Domain Branch × Runtime Host` 展开；普通 Layer 删除 PASS 不能推导 Dataset-backed 解除注册也使用同一确认窗口。若自动测试证明新实现存在、真机却毫无变化，先确认用户操作是否实际进入该实现。

Runtime Probe 应只记录入口、路由、分支、宿主和生命周期等决定性状态；使命完成后删除，不进入正式产品日志。

**关联 Incident**：INC-2026-08-12-001
**关联 Lesson**：L-VAL-001

---

## K-GOV-001 历史唯一身份以 Commit Hash 为准

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：Git、Versioning、Traceability、Changelog
**适用范围**：版本追溯、事故定位、AI 交接、验收基线、回滚、报告引用。

**审计标识**：`SHR-2026-08-R2`
**历史审计时间**：2026-08（原始审计注记未登记具体日与时分，禁止补造）
**涉及版本**：`v0.2.16.2-rz`、`v0.2.17.8-rz`、`v0.2.20.19-fix`
**Commit**：跨多个历史提交，无单一 Commit
**来源**：`changelog.md` 顶部“历史审计注记”

### 问题

人类可读版本号曾被重复分配，日期顺序也存在历史非单调。若把版本号当作唯一身份，就可能在事故复盘、AI 接手或回滚时定位到错误代码状态。

### 已确认历史事实

SHR-2026-08-R2 记录：7 月归档内至少 3 组版本号被分配给两个不同轮次：

- `v0.2.16.2-rz`
- `v0.2.17.8-rz`
- `v0.2.20.19-fix`

另登记 18 处版本号与日期顺序非单调。归档不篡改历史，冲突时按 Commit Hash 追溯。

### 工程规则

版本号用于人类阅读和发布语义；Commit Hash 才是代码历史的唯一身份。任何交接、验收基线和事故报告，在可行时必须同时记录：

```text
branch + version + commit + local/remote relation
```

### 禁止做法

- 只写“基线 v0.x.x.x”而不写 Commit。
- 遇到重复版本号时根据日期猜哪个是“正确版本”。
- 为了让历史看起来整齐而重排或改写已归档 changelog。

### 正确做法

历史冲突保留原文，加审计注记；所有未来正式记录优先写 Commit。需要还原历史时从 Commit 查看真实 tree，而不是从版本字符串推断。

### 真实历史示例

若两个不同历史条目都写 `v0.2.17.8-rz`，它们不能被视为同一状态。必须取各自 Commit Hash 才能确定真实文件树和实现内容。

### 未来应用示例

Codex 接手一个“基于 v0.2.30.4-fix 修复”的任务时，如果用户给了 Commit `abc1234`，而本地相同版本号对应另一个 Commit，必须停在 Git 基线核对，不得因版本字符串相同直接开始开发。

### 验证方法

- `git rev-parse HEAD`
- `git rev-parse origin/<branch>`
- `git log --decorate --oneline`
- 报告记录 ahead/behind、worktree、stash 状态

**关联 Incident**：INC-2026-08-10-005（历史版本追溯风险）
**关联 Knowledge**：K-VAL-001

### K-GOV-001 追加：双身份模型（2026-09-28）

Process Version 是开发过程统计身份，Commit SHA 是历史唯一身份；二者必须同时记录。Commit Hash 唯一不意味着 Version 可以不递增。禁止只记录 Version、只记录 SHA、事后合并多个 Fix Event、隐藏返工次数，或重用已有正式 Version 表示新的正式状态。正式 Build/Acceptance Identity 还必须记录 Branch 与 Dirty 状态。

**关联规则**：K-VAL-001、EXP-GOVERNANCE-003。

### K-VAL-001 追加：产物双身份闭环（2026-09-28）

用户运行产物与验证产物必须绑定相同 Process Version + Commit SHA；Dirty 产物只能作为明确命名的 Runtime Probe，不能直接成为正式 Product Acceptance Baseline。

---

## K-GOV-003 当前仓库入口/Resolver 高于 Agent 历史环境记忆

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：Repository Authority、SDK、Build、Run、Toolchain、Resolver
**适用范围**：SDK、Build、Run、Toolchain、Output Path、验收入口。

涉及工具链或运行入口时，事实优先级固定为：

```text
Repository Current Files → Current Machine Resolver → Git Current State → Agent Memory
```

必须先核对 `run.bat`、`scripts/resolve-dotnet.ps1` 和当前 Git 状态。历史记忆只能帮助寻找入口，不能作为最终环境结论。

**验证方法**：先执行 `scripts/xye-bootstrap.ps1`，记录 Resolver 选择的 SDK 路径/版本、当前分支、HEAD 与远端关系；正式 .NET 命令通过 `scripts/xye-dotnet.ps1` 或 Resolver 返回的绝对 `DOTNET_EXE` 执行。`CanonicalRunResolver` 已由 HANDOFF-BOOTSTRAP-R1 落地为机器 Gate；不得仅凭 PATH 或记忆宣布“无 SDK”或“运行的是某个输出目录”。

---

## K-GOV-002 治理成果必须建立自动防回潮门禁

**状态**：Active
**优先级**：P1
**证据等级**：E3
**标签**：Architecture Gate、Regression、Governance、5+100
**适用范围**：架构债务清理、依赖边界、白名单收口、长期规则。

**确认时间**：2026-06-23 23:09:45（UTC+08:00，由 Git Commit 时间 2026-06-23T15:09:45Z 换算）
**历史版本标识**：`8.8-0`（该时期使用旧历史编号，不伪造当前 SemVer 映射）
**Commit**：`4c4d82c0f508535e8c472f882084ac8008722dd5`
**来源**：Git Commit `8.8-0 — 架构防回潮门禁`

### 问题

一次性清理架构债务只能证明某个 Commit 很干净；如果规则只存在于文档和人的记忆里，后续开发很容易重新引入同类债务。

### 工程规则

当一项治理成果具有明确机器可判定条件时，收口动作应包括“把成果转成自动门禁”。治理完成的定义不是“现在没有问题”，而是“以后重新出现时机器会阻止合入”。

### 真实历史示例

8.7.8 大规模白名单债务收口后，`8.8-0` 新增：

- `ProductionWhitelist_OnlyApproved`
- `GlobalUsings_Max100Lines`
- `EditorShellContext_Max95Lines`
- `EditorShell_NotInWhitelist`
- `DirectoryWhitelist_RemainsZero`

这些测试把“已清理”转成“不可静默回潮”。

### 未来应用示例

如果正式冻结“Editor.UI 不得引用 Render.Vulkan”，仅写入架构文档不够；应在 ARCH Gate 中扫描项目引用或 namespace，违规直接失败。

### 禁止做法

- 清理完白名单后删除报告，却不加回归守卫。
- 把机器可判定的硬边界只留在自然语言规范中。
- 为赶进度临时放宽 Gate，任务结束后不恢复。

### 验证方法

治理任务关闭前回答：

1. 哪个历史坏状态现在被禁止？
2. 哪个自动测试/脚本会在它回来时失败？
3. 门禁是否已经进入正式串行验证链？

**关联 Knowledge**：K-GOV-001、K-VAL-002


---

## K-GOV-004 ChatGPT 审计通过后默认立即写回 XYK

**状态**：Active
**优先级**：P0
**证据等级**：E1
**标签**：XYK、Knowledge Writeback、Audit、Monthly Review、Governance
**适用范围**：所有带有 `KNOWLEDGE / EXPERIENCE AUDIT HANDOFF` 的正式施工任务。

**首次确认**：2026-09-29
**来源**：XYT 建设阶段用户治理裁决。

### 工程规则

施工 Agent 只提交事实与 Candidate Lessons；ChatGPT 完成 `SEARCH EXISTING → MATCH → UPDATE / STRENGTHEN / CREATE / NO DEPOSIT` 审计后，凡结论为 `CREATE / UPDATE / STRENGTHEN`，默认立即写回权威 XYK、同步索引并提交上传，不再逐条等待用户二次批准。

`NO DEPOSIT` 不写入；`CONFLICT / UNCERTAIN` 或涉及重要 Canonical 废止、互相矛盾的长期规则时，必须通报用户裁决。

月度巡检负责去重、合并、降级、SUPERSEDE / RETIRE 与自动化升级，不作为首次入库的前置条件。历史条目不得无痕删除。

### 目的

工程经验是由真实返工、事故和开发时间换来的资产。默认先保全事实与经验，再在周期性巡检中做瘦身，避免因“等以后整理”而丢失知识。

---

## K-XYT-MAP-001 正式测试选择必须经过 Change → Ownership → Capability → Required Tests

**状态**：Active
**优先级**：P0
**证据等级**：E3
**标签**：XYT、Test Mapping、Minimum Sufficient Set、Capability
**适用范围**：XYT 快速验证、模块收口、全局收口。

**首次确认**：2026-09-29
**Commit**：`0d75987cff361cefd3ccd4864ea9c17fe17f3581`
**来源**：XYT-C Minimum Sufficient Test Set。

### 工程规则

正式必跑测试不得只由 Agent 主观挑选。XYT 必须从实际 Change/Diff 出发，结合文件 Ownership、Capability Mapping，生成 Required Tests；未知映射进入 `REVIEW_REQUIRED`，不得静默当作 PASS。

### 验证 / 自动化

`scripts/governance/xyt-runner.ps1` 与对应 Selftest 已建立机器化链路。

---

## K-XYT-MAP-002 Agent 可以增加测试，但不得删除 XYT Required Tests

**状态**：Active
**优先级**：P0
**证据等级**：E3
**标签**：XYT、Required Tests、Agent Boundary、False Green
**适用范围**：所有由 AI / Agent 发起或扩展的 XYT 测试计划。

**首次确认**：2026-09-29
**Commit**：`0d75987cff361cefd3ccd4864ea9c17fe17f3581`

### 工程规则

Agent 可以基于额外风险追加验证，但不得删除、降级或绕过 XYT 根据固定映射生成的 Required Tests。若认为 Required Test 不适用，必须进入 Review / Governance 变更，而不是在本轮计划中自行移除。

---

## K-XYT-INC-001 T0 默认局部隔离，不默认全局停线

**状态**：Active
**优先级**：P0
**证据等级**：E3
**标签**：XYT、Incident、T0、Parallel Development、Lock
**适用范围**：XYT T0 事故响应。

**首次确认**：2026-09-29
**Commit**：`bac12aec`

### 工程规则

T0 的目标是尽快阻止污染扩散并保护开发效率。污染范围可可靠圈定时，只锁事故能力、所有权文件及真实依赖链；不相关的并行任务继续运行。

---

## K-XYT-INC-002 DEPENDENCY-UNCERTAIN 必须上报用户，不得擅自扩大锁定或放行

**状态**：Active
**优先级**：P0
**证据等级**：E3
**标签**：XYT、Dependency Lock、Uncertain、User Decision
**适用范围**：T0-LOCK 依赖传播与并行任务恢复。

**首次确认**：2026-09-29
**Commit**：`bac12aec`

### 工程规则

依赖关系无法可靠判定时，状态必须为 `DEPENDENCY-UNCERTAIN`。系统不得因为求稳无限扩大锁定，也不得为了赶进度擅自放行；必须立即通报用户，由用户最终裁决。

---

## K-XYT-INC-003 USER OVERRIDE 是正式治理能力，局部授权不等于解除 T0-LOCK

**状态**：Active
**优先级**：P0
**证据等级**：E3
**标签**：XYT、User Override、Incident、Authority
**适用范围**：T0-LOCK、DEPENDENCY-LOCK。

**首次确认**：2026-09-29
**Commit**：`bac12aec`

### 工程规则

用户拥有最终解释权与决定权，可以显式授权某个任务、文件或操作在锁定期间放行。Override 必须记录允许范围和原因；默认是局部豁免，不自动解除整个 T0-LOCK。

---

## K-XYT-INC-004 上游解锁后先做轻量依赖变更检查再自动恢复

**状态**：Active
**优先级**：P1
**证据等级**：E3
**标签**：XYT、Auto Resume、Dependency、Incident Recovery
**适用范围**：被 DEPENDENCY-LOCK 连带暂停的并行任务。

**首次确认**：2026-09-29
**Commit**：`bac12aec`

### 工程规则

上游 T0-LOCK 解除后，下游任务不应长期等待人工重新批准。先检查接口、行为契约、共享文件与依赖证据是否改变；无影响则自动恢复，有影响只继续锁定真正受影响的任务。

---

## K-XYT-EXEC-001 执行失败的根因在证据不足时必须保持 UNKNOWN

**状态**：Active
**优先级**：P0
**证据等级**：E3
**标签**：XYT、Execution、Root Cause、UNKNOWN、False Product Failure
**适用范围**：XYT Executor、Failure Sweep、Timeout、Flaky 与失败分类。

**首次确认**：2026-09-29
**Commit**：`673dae7f02c4d9c43a5a5fc83b1952233471f4d5`
**来源**：XYT-D Test Execution Engine。

### 工程规则

执行层只报告它实际观察到的执行结果。FAIL、TIMEOUT、FLAKY 或 Harness 异常在完成根因分类前必须保持 `UNKNOWN / UNCLASSIFIED`；不得因为测试红灯直接升级为 PRODUCT ROOT CAUSE。

### 正确做法

先完成 Failure Sweep，保留独立测试继续执行；再把稳定失败分类为 Product / Harness / Oracle / Environment / Unknown。UNKNOWN 是合法状态，不是需要被“补齐”为 Product 的空字段。

### 验证 / 自动化

XYT-D Executor Selftest 已覆盖 Stable FAIL、FLAKY、TIMEOUT、BLOCKED_BY、Failure Sweep 与 UNKNOWN root cause。


---

## K-XYT-AUDIT-001 测试名称与目录不能授予证据等级，证据等级必须由真实验证边界决定

**状态**：Active
**优先级**：P0
**证据等级**：E1
**标签**：XYT、Legacy Test、Evidence Classification、Runtime、False Green
**适用范围**：所有旧测试清库、测试命名、P0~P4 登记、Runtime/Integration/Performance 类测试。

**首次确认**：2026-09-29
**Commit**：待补证
**来源**：XYT-G Legacy Test Clean-Room Audit。

### 已确认事实

XYT-G 扫描 696 个测试源文件、2552 个测试方法。审计快照中：P0=281、P1=284、P2=131、P3=0；另有 37 项需要 DOWNGRADE、95 项需要 RENAME，P4 明确为 MISSING。

### 工程规则

测试类名、目录名、文件名中出现 `Runtime`、`Vulkan`、`Swapchain`、`Churn`、`Performance`、`Integration`、`EndToEnd`、`Real`、`Native`，都不能自动获得更高证据等级。P0~P4 只由测试实际跨越的验证边界决定。

源码字符串检查、反射结构检查、纯逻辑、Headless/UI VM 测试即使名字含 Runtime，也不得登记为 P3。P3 必须具备真实应用/窗口/Native/Vulkan/GPU/Present 等其目标能力所要求的真实运行边界。

### 防复发

旧测试完成清库后，Registry 应保存经审计的 P-Level；测试新增或改名不得自行提升 P-Level。若测试实现边界未改变，仅修改名称不能改变正式证据等级。

**关联 Incident**：INC-2026-09-29-002
