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
