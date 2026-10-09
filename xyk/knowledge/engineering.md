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

**验证方法**：先执行 `scripts/xye-bootstrap.ps1`，记录 Resolver 选择的 SDK 路径/版本、当前分支、HEAD 与远端关系；正式 .NET 命令通过 `scripts/xye-dotnet.ps1` 或 Resolver 返回的绝对 `DOTNET_EXE` 执行。CanonicalRunResolver 已由 HANDOFF-BOOTSTRAP-R1 落地为机器 Gate；不得仅凭 PATH 或记忆宣布“无 SDK”或“运行的是某个输出目录”。

### 2026-09-28 强化：Version Consumer 必须服从正式 Resolver

产品版本唯一事实源为仓库正式 Version Source，并通过正式 Resolver 暴露给消费者。Guard、窗口标题、Changelog 等都只是消费者，不得反向把展示文本当成第二版本源。若 Guard 仍检查历史硬编码展示字符串，应修复 Guard 的取值路径，而不是修改正式 Version Source 去迎合旧检查。

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

### 2026-09-28 强化：Guard RED 不得通过放宽 Contract 修绿

Architecture Guard 报出冻结边界外文件、依赖或白名单违规时，必须先判断它属于 Guard 基础设施缺陷还是真实 Architecture Contract Violation。若是后者，禁止扩大 allowlist、增加临时 exception、弱化断言或关闭检查；应进入独立 Architecture Migration。只有经正式架构裁定改变 Contract，门禁才可随之更新。


---

## K-GOV-004 Governance Guard 必须共享显式 Bootstrap 且可独立执行

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：Governance、Architecture Guard、Bootstrap、Failure Propagation
**适用范围**：主 Guard、子 Guard、治理脚本、架构门禁与聚合验证。

### 已确认事实

2026-09-28 的治理基线审计确认：部分子 Guard 直接运行时缺少 `Get-SourceFiles`、`Read-Text`、断言函数、`Get-ProjectReferences` 与 failure sink，因为这些能力只由主 Guard 在运行期临时定义。相同治理历史还曾出现主/子 Guard failure scope 不一致、失败被重置或未完整上报的问题。

### 工程规则

公共治理能力必须进入唯一、显式、可复用的 bootstrap/module。任何子 Guard 都必须能够在不先运行父脚本的情况下独立加载依赖并得到与聚合执行一致的 PASS/FAIL。

主 Guard 聚合子 Guard 时必须完整传播失败；禁止 reset、覆盖、吞掉或局部隐藏 failure sink。

### 禁止做法

- 由主 Guard 临时定义公共函数，并假设子 Guard 永远只会被父脚本调用；
- 每个子 Guard 各复制一份同名 Helper；
- catch 后把治理失败降级为 warning；
- 为了让主 Guard 变绿而不呈现子 Guard 的真实失败。

### 验证

至少同时执行：主 Guard、每个子 Guard direct-run、Windows PowerShell 5.1、项目正式支持时的 pwsh，并验证失败传播一致。

**来源任务**：GOV-BASELINE-RECOVERY-R1 · 2026-09-28


---

## K-GOV-005 Candidate Tree Purity

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：Governance、Candidate Tree、Validation Tree、Fail Closed、Commit Eligibility
**适用范围**：C0、Forensic、Convergence、Integration、Build/Test/Acceptance、Commit 与 Release 裁决。

### 正式规则

1. **Candidate Tree** 是最终计划 Stage / Commit 的文件集合及其确定依赖状态。
2. **Validation Tree** 是实际参与 Build / Test / Acceptance 的工作树状态。
3. 如果 `Validation Tree != Candidate Tree`，必须输出 `CANDIDATE TREE MATCH = NO`。即使 Build、Test、Dedicated Gate 或 Manual Acceptance PASS，也不得推出 `COMMIT ELIGIBILITY = YES`、`RELEASE ELIGIBILITY = YES`、`PRODUCT CLOSED` 或 `ACCEPTANCE COMPLETE`。
4. Dirty dependency 只要参与编译、运行、测试或行为验证，即属于 Validation Tree；不能以“文件不是本 Lane 的”为理由忽略。
5. 合法解除方式只有：
   - 将 Dirty Dependency 独立正式收口，形成稳定 Commit Baseline 后重新验证；或
   - 在不包含该 Dirty Dependency 的真实 Candidate Tree 重新执行同等级 Gate 并 PASS。
6. 禁止用局部定向测试代替真实 Candidate Tree、用历史 PASS 继承当前 Candidate，或在 Candidate Tree Mismatch 状态推进版本。

### 本轮 Incident Evidence

本条由 `XYK-C0-GOVERNANCE-SEDIMENT-R1` 写入；以下数字仅是本次 Incident Evidence，不是永久固定值：`XYE Dirty Paths = 68`、`XYE OwnDirty = 53`、`XYUI known dirty = 15`、`True ForeignDirty = 0`、`Candidate Tree Match = NO`、`Confirmed Current-Wave Regression = 4`、`Unresolved Unknown = 11`。

### 验证与关联

所有顶层 C0 / Forensic / Convergence / Integration 报告必须使用 Reporting Contract 输出本条状态；任一 Mismatch 必须 fail-closed。

**来源任务**：XYK-C0-GOVERNANCE-SEDIMENT-R1 · 2026-09-28

---

## K-VAL-003 Headless Runtime UI 必须单一生命周期并进入真实 Visual Tree

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：Validation、Avalonia.Headless、Runtime UI、Platform Lifecycle、Visual Tree、Pointer、HitTest
**适用范围**：Avalonia.Headless、UiRuntime、Diagnostic、Popup/ContextMenu、Pointer、Button、TranslatePoint、Visual 坐标与任何需要真实事件路由的自动化 UI 测试。

**确认时间**：2026-09-28（UTC+08:00）
**来源任务**：XYE-C0-LAST-MILE-FINAL-R1
**产品证据 Commit**：`516c83f9da2e7be129e2f9e6560bf12958347fad`

### 已确认事实

同一测试进程中若先由 ModuleInitializer 执行 `BuildAvaloniaApp().SetupWithoutStarting()`，随后再创建 `HeadlessUnitTestSession`，会形成重复 Platform Bootstrap。此时 Window、Layout、Bounds 与可见性仍可能正常，但 MouseDown/MouseUp 无法进入正常 Routed Pointer / Button Click 链，制造“产品输入坏了”的假故障。

A/B 对照中，仅移除额外 `SetupWithoutStarting()`、让 `HeadlessUnitTestSession` 独占生命周期，即使同一 Minimal Button、同一 Harness 与同一 Cluster B 从 FAIL 全部转为 PASS。

### 工程规则

1. `HeadlessUnitTestSession` 是 Headless Runtime UI 的唯一 Avalonia Platform Lifecycle Owner。
2. Platform Stub / Test Service 可以在 Session 内安装，但不得因此再次执行 `Setup()` 或 `SetupWithoutStarting()`。
3. 依赖 Pointer、HitTest、Popup、`TranslatePoint`、Visual 坐标或 Routed Event 的测试，目标必须挂入真实 `Window/TopLevel/Visual Tree`。
4. 进入真实树后必须完成 `Show → Layout → Dispatcher drain`，再进行输入或坐标验证。
5. 脱离 Visual Tree 的直接对象调用只能证明局部逻辑，不能作为 Runtime UI / Production Route 的等价证据。

### 防复发 Gate

- `HeadlessInputInfrastructureTests.Official_headless_button_click_executes`
- `HeadlessInputInfrastructureTests.Harness_headless_button_click_executes`
- Diagnostic Probe / Overlay 专项回归
- World Full Gate

### 验证证据

正式收口结果：Minimal Button PASS、Harness PASS、Cluster B 3/3 PASS、Diagnostic Probe/Overlay 26/26 PASS、World 2125/2125、XYUI 701/701、Core 448/448、Build 0W0E、ARCH-A PASS、5+100 PASS。

**关联 ERR**：ERR-20260928-002
**关联 Experience**：EXP-TEST-001
**关联 Knowledge**：K-VAL-002

---

## K-XYT-MAP-001 正式测试选择必须经过 Change → Ownership → Capability → Required Tests
**状态**：Active　**优先级**：P0　**证据等级**：E3  
**适用范围**：XYT 快速验证、模块收口、全局收口。
- 必跑测试必须从实际变更出发，经文件归属和能力映射生成，不得由 Agent 凭感觉挑选。
- 未知映射必须进入 `REVIEW_REQUIRED`，不得静默当作通过。
- Agent 可以追加验证，但不能删减系统生成的 Required Tests。
**证据**：XYT-C · `0d75987cff361cefd3ccd4864ea9c17fe17f3581` · 2026-09-29。

## K-XYT-MAP-002 Agent 可以增加测试，但不得删除 Required Tests
**状态**：Active　**优先级**：P0　**证据等级**：E3  
若 Agent 认为某个 Required Test 不适用，必须走 Review / Governance 修改映射；不得在当前任务里自行移除、降级或绕过。
**证据**：XYT-C · `0d75987cff361cefd3ccd4864ea9c17fe17f3581` · 2026-09-29。

## K-XYT-EXEC-001 执行失败根因在证据不足时必须保持 UNKNOWN
**状态**：Active　**优先级**：P0　**证据等级**：E3  
FAIL、TIMEOUT、FLAKY 或 Harness 异常只说明执行结果；完成根因分类前必须保持 `UNKNOWN / UNCLASSIFIED`，不得自动升级成产品根因。应先完成 Failure Sweep，再区分 Product / Harness / Oracle / Environment / Unknown。
**证据**：XYT-D · `673dae7f02c4d9c43a5a5fc83b1952233471f4d5` · 2026-09-29。

### 2026-10-09 强化：独立自测与嵌套全套入口须分别认定

在 XYE-GOVERNANCE-SIMPLIFICATION-R1 / C2 中，XYT 独立 Runtime 自测报告 PASS，但顶层 `XYT/tests/xyt.selftest.ps1` 经「全局收口 → xyt-runtime.selftest.ps1」嵌套执行时持续无进展，未获得父进程退出码及测试后的快照核对。该轮顶层测试状态只能记为 **INCOMPLETE / 未确认 PASS**；等待链根因仍为 **UNKNOWN**。不可将独立子测试 PASS 推断为父级全套 PASS，也不可据此推断产品故障。

**执行规则**：独立运行与父 Runner 嵌套运行是不同的验证上下文。发现嵌套等待时，先只读保留父子进程 PID、命令行、时间、最后输出和退出状态；排查并发共享文件/构建输出、子进程等待和结果捕获，但不得在证据不足时认定任何一种为根因。若未拿到退出码与结束快照，则保持全套 INCOMPLETE；明确终止/超时须分别按实际事实报告。稳定最终集成树后重验受影响范围；验证窗口内相关文件变化时，依 K-XYT-EVID-001 判断证据是否需重验，不默认全量重跑。

**证据边界**：C2 交接报告，2026-10-09 18:12 启动的顶层自测，基线 `79476c4c48dac03efbda2338da3298d3cf2a97b2`；该报告仅有启动前 XYT 快照，缺少退出状态和结束快照。此条记载施工报告观察，不声称 ChatGPT 已独立复跑 Windows 测试。待 Parent 查明等待链并取得最终验证后复核。

### 2026-10-09 Parent 后续核验：顶层 XYT 自测正常结束

Parent 补齐了 C2 交接时缺失的结束证据：`XYT/tests/xyt.selftest.ps1` 最终退出码为 0；运行前后 37 个 XYT 文件的 SHA256 均为 `959803C315097C5A491E30A864C22DAD4CFDA327A7CDEDF9A10DD9B23BD4CB52`，报告中的当前 XYT 快照一致。独立 Runtime harness 再次退出码 0。子 PowerShell 依次验证通过、失败、超时、崩溃、无 GUI、流读取超时和证据写入故障；合成超时用例约 15.4 秒。当前证据不支持“死锁”判断。

**结论更新**：此前 C2 交接时的 `INCOMPLETE / UNKNOWN` 是当时正确的暂定记录；补齐退出码和相同 XYT 快照后，本次顶层 XYT 自测改判为 **PASS（据 Parent 报告）**。这不代表全部产品测试 PASS，也不代表真实 P3 Runtime 或 P4 验收已完成。Parent 同时报出 Core、XYUI 各一项产品测试失败，尚需独立核查。

**经验**：嵌套 PowerShell 自测可能包含耗时的串行合成故障用例。判定卡死前，应检查父子进程、用例耗时、退出码和测试前后关联文件哈希；后续证据能够更新先前状态，但不得抹掉历史观察。

**来源**：XYE-GOVERNANCE-SIMPLIFICATION-R1 Parent 集成恢复报告，2026-10-09。ChatGPT 未独立复跑 Windows 测试。

### 2026-10-09 治理精简正式收口：基线复现与间歇性失败必须区分

本轮产品治理精简 Parent 于 `7cc8be331496e1cb750142aaad4e7c5195919635` 提交 131 项治理/XYT 改动，未修改所述两项产品测试路径。Parent 提供了两种**不同**的测试失败证据：

- **Core（已在旧基线复现）**：`EmptyWorldBaselineContractTests.New_scene_returns_to_empty_world_without_map_roundtrip` 在本轮测试中断言 `Assert.False()` 失败、实际为 `true`；在本轮施工前干净基线 `79476c4c48dac03efbda2338da3298d3cf2a97b2` 的同项复测得到同样失败。可归为**本轮之前已存在的失败**，但并不表示问题已修复。
- **XYUI（间歇性失败）**：`XYUI3FinalNavigationTests.Bottom_navigation_primary_action_floating_hit_target_does_not_change_destination` 全套运行时浮动按钮命中断言失败；干净基线上的聚焦运行通过，本轮聚焦重跑 3/3 通过，并有此前全套运行间歇失败记录。分类为**已有间歇性失败**，尚不能确定其触发条件或稳定根因；聚焦 PASS 不得冲销全套 FAIL。

**可复用规则**：集成失败归因要保留测试身份、实际断言、命令/模式、测试对象版本与快照、干净施工前基线的同项结果、历史复现以及本轮依赖变更情况。**基线复现**才有直接证据支持“本轮前已失败”；**全套失败而聚焦通过**说明执行上下文敏感或不稳定，不能自行断言已修复、当前回归或产品代码根因。测试文件未改不能单独证明失败是历史遗留。无足够证据继续标注 UNKNOWN，后续以复现与真实 P3/P4 结果重验；不得把局部 PASS 宣布为产品全套 PASS。

**证据边界**：以上 Core、XYUI 运行及本地 clean 状态据 2026-10-09 Parent 最终核验报告；ChatGPT 核实了 GitHub 产品提交及分支 HEAD，未独立执行 Windows 测试。本次产品 P3 Runtime 未运行，P4 人工验收待完成。

## K-XYT-AUDIT-001 测试名称和目录不能授予证据等级
**状态**：Active　**优先级**：P0　**证据等级**：E2  
`Runtime`、`Vulkan`、`Performance`、`Integration`、`Real` 等名称不能自动获得更高证据等级。证据等级只能由测试实际跨越的验证边界决定；改名不改变正式证据等级。
**证据**：XYT-G Legacy Audit · 2026-09-29；扫描 696 个测试源文件、2552 个测试方法，确认存在降级/重命名需求及 P3 真空。

---

## K-XYT-WIT-001 Regression Witness 必须由同一测试形成 RED→GREEN
**状态**：Active　**优先级**：P0　**证据等级**：E3  
有效 Witness 必须绑定同一 `TestId`、明确的修复前/后 Commit 与前后证据。修复前 PASS、TestId 不一致为 INVALID；缺旧证据或身份不完整为 INCOMPLETE。没有旧状态证据不得升级为 RED_CONFIRMED。Witness 不替代 P3/P4。
**证据**：XYT-J · `9e4c1a3de93c569bd24352867001c6c2ad10b869` · 2026-09-29。

## K-XYT-EVID-001 证据失效必须按 Capability 隔离
**状态**：Active　**优先级**：P0　**证据等级**：E2  
证据应绑定 Commit、Version、Capability、TestId、TestSet 和证据等级。真实失效条件命中时只能从 VALID 变为 REVALIDATION_REQUIRED，不能直接推导 PRODUCT FAIL；重验应生成最小充分计划，不默认全量重跑。
**证据**：XYT-K · `32af17e9dbe076fbd76dde331c751fe3479b0abf` · 2026-09-29。

## K-XYT-CLOSE-001 集成合同 READY 不等于产品收口
**状态**：Active　**优先级**：P0　**证据等级**：E2  
只要真实 Runtime、Incident/Witness、关键证据重验或 P4 用户结论仍未满足，下游必须保持 Pending/Blocked。Fake/Fixture 集成通过不能升级成真实 Runtime 或产品通过。
**证据**：XYT-L/FINAL · `18842310` / `475ab10d` · 2026-09-29。

## K-XYT-P3-001 P3 必须走仓库权威构建和真实运行入口
**状态**：Active　**优先级**：P0　**证据等级**：E2  
P3 不能由 Headless 补齐。必须先经仓库权威 Resolver / Build / Run 得到身份明确的产物，再进入 App、窗口、Vulkan、Swapchain、Present 等目标真实边界。目标 EXE 未生成时优先判定 Harness/Prerequisite，不得直接判产品根因。
**证据**：XYT-H · `ccd3dbef94864466ec2a7fc73c4ee6074e5809b2` · 2026-09-29。

## K-XYT-HARNESS-001 预期子进程失败不得污染父级 Selftest 退出码
**状态**：Active　**优先级**：P0　**证据等级**：E2  
故意验证错误路径的子进程可以非零退出，但父级在全部断言通过后必须显式返回成功；真正断言失败必须保持非零。机器 Gate 同时检查可读结论和 Process Exit Code。
**证据**：INC-2026-09-29-003。

## K-HANDOFF-001 Stale Commit Mutex 必须有受控恢复路径

### 2026-10-01 强化：交接同步不得自动丢弃本地内容

当本地 `Ahead=0 / Behind>0` 时：
- working tree clean：只允许 `merge --ff-only <upstream>`；
- working tree dirty：必须返回 `DIRTY_BEHIND_REMOTE` 并保留全部 tracked / untracked 内容；
- 禁止 Handoff 自动执行 `reset --hard`、`clean -fd`、stash、merge commit 或 rebase；
- 无法 fast-forward 时必须阻断并交给人工处理。

**实现证据**：`a846728cfd27dead3bd458abb41ba6ea6e607120`。
**状态**：Active　**优先级**：P0　**证据等级**：E2  
Commit Mutex 不能被普通 Agent 随意抢锁或删除；过期锁恢复必须证明原 Owner 已失效、当前基线可审计、工作区不被改写，并记录恢复事件。用户或 Coordinator 保留最终授权权。
**证据**：XYT-L2 Convergence / Handoff Audit · 2026-09-29。

## K-XYT-P3-002 P3 构建准备与 Runtime Probe 必须分开计时
**状态**：Active　**优先级**：P1　**证据等级**：E2  
Build Preparation 与 Runtime Probe 分开报告。身份一致的已验证产物可以复用；身份不一致才重建。各 P3 Capability 拥有独立时间预算，一个 TIMEOUT 不得覆盖另一个已经成立的 PASS。
**证据**：XYT-H2/H3 · 2026-09-29。

---

## K-GOV-006 Windows PowerShell 5.1 中文脚本必须保留兼容编码
**状态**：Active　**优先级**：P0　**证据等级**：E2  
**适用范围**：需要同时支持 Windows PowerShell 5.1 的 `.ps1/.psd1` 治理脚本。

### 工程规则
- 脚本含中文或其它非 ASCII 文本且要被 Windows PowerShell 5.1 直接解析时，必须保留 UTF-8 BOM。
- Agent / GitHub API 重写文本文件时不得无意去掉 BOM。
- 改动后至少执行一次 `powershell.exe` 5.1 自测；只在 `pwsh` 通过不算兼容性证明。
- 若明确采用无 BOM UTF-8，则脚本内容必须保持 ASCII-only，或由启动层显式使用兼容解码路径。

### 事故证据
2026-10-01 修改 `tools/handoff/handoff.ps1` 时 BOM 被移除，GitHub Windows Runner 的 PowerShell 5.1 在中文脚本中产生解析失败；恢复 BOM 后解析问题消失。

**修复证据**：`33afb8936061efc01c242ddeb8bd8f4d8119c92c`。

