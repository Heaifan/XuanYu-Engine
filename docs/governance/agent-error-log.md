# Agent 错误记录库

> 路径：`docs/governance/agent-error-log.md`  
> 唯一职责：记录 AI Agent 在真实玄域引擎任务中实际犯过的错误、根因、影响、处置状态和关联防复发规则。

本文件不是普通 Bug 清单，不记录正常开发修改，不替代 `changelog.md`、测试报告、阶段计划或技术债 Backlog。

---

## 1. 写入权限

- **ChatGPT**：可创建 ERR、修订根因、关联 EXP、更新状态、关闭记录。
- **Codex / Gemini / 其他执行 Agent**：只读；可以报告候选错误、引用 ERR-ID、执行修复并提交验证证据，但不得创建、改写、关闭或删除正式 ERR。
- **用户**：拥有最终裁定权。

执行 Agent 不得通过修改本文件来降低、隐藏或重新解释自己的历史失误。

## 2. ID

格式：

```text
ERR-YYYYMMDD-NNN
```

同一天按发现顺序递增；ID 一经使用不得复用。

## 3. 错误类型

```text
LOGIC       逻辑错误
ARCH        架构违规
TEST        测试缺口 / 假通过
SCOPE       越界开发
GOVERNANCE  治理规则违规
REPORT      报告与事实不一致
REGRESSION  回归
UI          UI / 交互违背冻结规则
DATA        数据 / 状态 / 持久化错误
```

每个 ERR 只选择一个主类型；确有交叉影响时在正文说明，不为同一事件重复创建多条 ERR。

## 4. 严重度

```text
Critical  数据损坏、崩溃、无限递归、核心架构破坏、严重事实造假等
High      核心功能错误、严重回归、冻结规则破坏、关键测试失效等
Medium    局部功能错误、明显 UX 错误、非核心回归等
Low       轻微错误、维护性问题、低影响偏差等
```

严重度以实际影响和风险为依据，不以修复代码量为依据。

## 5. 状态

```text
已发现
已修复
已验证
```

- `已发现`：错误事实已确认，但尚未完成可靠修复。
- `已修复`：代码 / 文档修复已完成，但验证证据尚未满足关闭条件。
- `已验证`：修复已被适当门禁验证，可作为已关闭错误事实保留。

不得以“代码已改”直接跳到“已验证”。

## 6. 标准记录模板

```markdown
## ERR-YYYYMMDD-NNN

Agent：Codex
任务：任务 / 阶段名称
类型：LOGIC
严重度：High

错误：
简要说明 Agent 实际做错了什么。

根因：
说明为什么会犯错；必须是有证据支持的原因，未知时写“待确认”，不得编造。

后果：
说明真实影响、潜在风险或已造成的返工。

正确做法：
说明该事件本应如何处理。

经验规则：
EXP-LOGIC-001

发现方式：
ChatGPT Code Audit / 自动测试 / 用户真机验收 / 运行时诊断 / 其他真实来源

状态：
已发现 / 已修复 / 已验证
```

## 7. 经验规则关联

ERR 负责回答：

> 这次 Agent 到底犯了什么错，为什么？

可复用防复发规则统一进入：

```text
docs/governance/agent-experience-rules.md
```

ERR 正文只引用 EXP-ID，不在每条错误中重复维护一套近义经验规则。

## 8. 重复错误规则

同类错误再次发生时：

1. 新事件仍创建新的 ERR-ID，以保存真实发生事实；
2. 优先关联既有 EXP；
3. 在 EXP 中累计 `Occurrences` 与相关 ERR-ID；
4. 只有根因或防复发机制实质不同，才创建新的 EXP；
5. 不得为了增加“经验数量”制造近义规则。

## 9. 关闭门禁

ERR 进入 `已验证` 前至少必须满足：

- 错误事实与根因已经明确，或明确标注仍未知而不伪装关闭；
- 修复内容可追溯；
- 与风险等级匹配的验证已经真实执行；
- 若产生长期防复发价值，已关联或更新 EXP；
- 若适合机器防回潮，已评估 Regression Test / Static Check / Runtime Gate / Architecture Gate。

## 10. 禁止事项

禁止：

- 把普通产品 Bug 全部塞入 ERR；
- 把正常需求变更记录成 Agent 犯错；
- 把推测写成根因；
- 删除历史 ERR 以隐藏失败；
- 因 Agent 自己已经修复就自行宣布 `已验证`；
- 用 ERR 替代 changelog、测试结果、事故证据或 Git 历史。

---

## 当前记录

## ERR-20260916-001

Agent：Codex（前序 Inspector 实现）
任务：INSPECTOR-2.0-R1 属性提交与 Recent 身份契约
类型：LOGIC
严重度：High

错误：
属性提交路径使用提交瞬间的 `_selectedMapGeometry` 解析目标，Recent 只按 `InspectorObjectKind` 存储；Entity 名称提交还通过统一入口递归回调自身。Selection 在编辑开始后切换时，提交可能修改错误对象，Recent 也会落入当前对象。

根因：
缺少由编辑行为持有的稳定 `ObjectKind + ObjectId + PropertyKey` 目标；提交、Recent 与当前 Inspector Identity 没有共享同一目标身份。Entity 专用入口没有下沉到领域 Rename Core，而是重新进入统一提交入口。

后果：
Road A 编辑后切换到 Road B 可观察到 Recent 错误归属；Entity 名称提交存在无限递归风险；原有仅覆盖 Road Happy Path 的测试未能发现跨 Selection 和 Entity 路径缺口。

正确做法：
在编辑行或 Entity 输入获得焦点时捕获 `InspectorEditTarget`；Commit 按目标 ID 执行；Recent 按 `ObjectKind + ObjectId` 隔离；统一入口只路由，Entity Rename Core 不得反向调用统一入口。

经验规则：
EXP-UI-001
EXP-LOGIC-001

发现方式：
ChatGPT Code Audit / 失败回归 / 自动测试

验证证据：
Inspector/Entity 专项回归 16/16；受影响 UI Build 0W/0E；ARCH-A、5+100、git diff --check PASS；修复提交 `a7472917866df357f3d3694b183d21672f7fbabc` 已推送并远端复核 0/0。

状态：
已验证


---

## ERR-20260925-001

Agent：Codex（Viewport 输入统一前序实现）
任务：WAVE-2.5 Production Input / Input Convergence
类型：ARCH
严重度：High

错误：
仓库已经存在 `ViewportInputRouter`、`GestureOwner`、Consumer 与 Lifecycle，并有对应测试，但真实 Native HWND 与 Avalonia Pointer 生产入口仍长期存在直接调用 `UiVm.*` 的 bypass。实现层把“架构类型存在、测试可实例化”误当成“生产输入已经统一”。

根因：
验收边界停留在 Router/Consumer 类型和局部测试，没有从真实平台 Source 反向审计完整生产链，也没有把“Real Platform Event → Adapter → Router → Arbitration → Owner → Consumer → Domain/Core”作为完成合同。

后果：
Camera、Picking、Gizmo、Region/Road/Marker 等输入仍可能由旧 Host 路径直接消费，Owner、Capture 与 Cancel 语义无法真正统一；后续必须通过 WAVE-2.5 E0/E5 再做生产接线审计和迁移。

正确做法：
任何 Input Architecture 完成声明都必须证明真实 Native/Avalonia Source 已接入统一 Adapter/Router，并审计旧 Host 是否仍直接调用业务 Consumer/UiVm。架构“存在”与生产“接线完成”必须分开验收。

经验规则：
EXP-ARCH-001
EXP-TEST-001

发现方式：
WAVE-2.5 E0 Production Input Audit / ChatGPT 架构复盘

验证证据：
当前仓库已有 `ProductionInputCompositionTests` 与 `E5ProductionInputFreezeTests`，并建立单一 Router/Lifecycle/Consumer Composition 与 WindowDeactivated 正式 Sink 约束；后续仍由正式输入收口门禁持续防回潮。

状态：
已验证

---

## ERR-20260925-002

Agent：Codex（Diagnostic 多轮实现）
任务：DIAG-R1 / Native Viewport Diagnostic
类型：UI
严重度：High

错误：
Diagnostic Highlight / Overlay 在 Native Viewport 上方一度成为实际输入阻挡层，导致诊断系统从“观察者”滑向第二套交互层，破坏 Viewport 原有 Pointer 所有权。

根因：
Diagnostic 的承载、Placement、Highlight 与 Native/Avalonia Airspace 问题被连续局部修补，但没有始终把“Diagnostic 必须输入透明、不得成为 Owner/Consumer”作为第一约束；观察能力与交互承载边界混在一起。

后果：
开启 Diagnostic 后可能影响真实 Viewport 点击/悬浮/拖拽，诊断工具本身改变被诊断系统行为；同时扩大 Popup/Window/Native ownership 复杂度。

正确做法：
Diagnostic 只能 Observer：允许观察 Routed/Native Pointer、读取 HWND/Bounds、绘制不抢输入的 Highlight；不得抢 Capture、标记生产 Pointer handled、建立第二套 Gesture Owner，或为了显示诊断卡长期扩建 Native UI Ownership。

经验规则：
EXP-UI-002
EXP-ARCH-001

发现方式：
用户真机验收 / Diagnostic Viewport 回归 / Code Audit

验证证据：
已有提交 `edc2acc41ae43f64ac09b4ab6111aa4942c3533b`（`fix(diag): remove viewport highlight input blocker`）及 Diagnostic Viewport input passthrough 回归；本条长期规则仍需由后续 DiagnosticInputTransparency Gate 固化。

状态：
已修复

---

## ERR-20260925-003

Agent：Codex
任务：MAP-REGION-SNAP-R1 Native Alt
类型：LOGIC
严重度：High

错误：
将 Win32 鼠标消息 `wParam` 的 `0x0020` 解释为 Alt。该位实际是 `MK_XBUTTON1`，不是 Alt；因此真实 Alt 可能无法取消吸附，而鼠标侧键 XBUTTON1 反而可能被误判为 Alt。

根因：
没有从 Win32 平台合同确认按钮位含义，把熟悉的数值/假设直接带入实现；对应测试也最初重复了同一错误前提。

后果：
Native/Vulkan Region Snap 的 Alt 临时取消语义失真，并产生“错误实现 + 错误测试 = 全绿”的假安全感。

正确做法：
Win32 Alt 必须从真实键盘状态（如 `VK_MENU`）或平台 Adapter 获取；鼠标 `wParam` 只解释其正式定义的 mouse key state。平台编码必须在 Adapter 边界正规化。

经验规则：
EXP-TEST-001

发现方式：
ChatGPT Code Audit

验证证据：
`5e758f63e29aca4ec30890b0999ac93778e967e5` 将 Alt 独立为 `AltDown`，使用 `GetKeyState(VK_MENU)`，并新增 XBUTTON1 不得识别为 Alt 的回归；当轮 Snap 专项曾报告 25/25 PASS。

状态：
已验证

---

## ERR-20260925-004

Agent：Codex
任务：MAP-REGION-SNAP-R1-REVALIDATE Avalonia Alt
类型：TEST
严重度：High

错误：
Avalonia Alt 回归最初直接构造 `AvaloniaKeySample(0x12, ...)`，把 Win32 `VK_MENU` 值伪装成 Avalonia 键值，绕过了真实 `Key.LeftAlt / Key.RightAlt` 到 Editor Key 的映射，因此测试可以 PASS，而真实 Avalonia Alt 仍可能不触发 Router 刷新。

根因：
测试从“期望的统一结果”开始，而不是从权威平台输入开始；Adapter 本身没有被真实平台 Enum 契约覆盖。

后果：
形成平台边界假阳性，掩盖真实 Avalonia Alt 与 Router `0x12` 语义不一致的问题。

正确做法：
平台边界回归必须从真实平台 Enum / Message Contract 开始，再经过 Adapter → Unified Model → Router → Consumer。禁止用手工构造的“理想统一值”证明 Adapter 正确。

经验规则：
EXP-TEST-001

发现方式：
ChatGPT Code Audit

验证证据：
生产代码已将 `Key.LeftAlt / Key.RightAlt` 正规化为 Editor `0x12`，测试也已改用真实 Avalonia 键值；但当前工作环境的实际 Build/专项测试尚未完成，因此不提升为“已验证”。

状态：
已修复

---

## ERR-20260925-005

Agent：Codex / ChatGPT 前序环境判断
任务：MAP-REGION-SNAP-R1-REVALIDATE 环境门禁
类型：GOVERNANCE
严重度：Medium

错误：
判断当前机器没有 .NET SDK 时，优先使用历史记忆中的固定 SDK 路径并检查系统 PATH，没有先读取并执行仓库权威入口 `run.bat → scripts/resolve-dotnet.ps1`，从而错误宣布环境 BLOCKED。

根因：
把 Agent 历史记忆当成当前机器事实，优先级高于 Repository Current Files 与当前 Resolver；同时忽略项目已经专门解决多机器/多盘符差异的环境发现机制。

后果：
把本可继续执行的 Build/Test 错误标记为 NO SDK，浪费开发轮次，并可能在私人电脑 D 盘、工作电脑 E 盘等环境间持续产生错误判断。

正确做法：
涉及 SDK、Build、Run、Toolchain、Output Path、Acceptance Entry 时，事实优先级固定为：
`Repository Current Files → Current Machine Resolver → Git Current State → Agent Memory`。
玄域引擎必须先读取/执行 `run.bat` 与 `scripts/resolve-dotnet.ps1`；只有 Resolver 真实失败后才允许报告 NO SDK。

经验规则：
EXP-GOVERNANCE-001

发现方式：
用户纠正 / ChatGPT Repository Audit

验证证据：
已确认当前 `run.bat` 调用 `scripts/resolve-dotnet.ps1`；Resolver 支持 `XUANYU_DOTNET`、repo-local SDK、A:～Z: 的 `MyApp\sdk-dotnet` / `DevTools\dotnet` 与 PATH。自动 CanonicalRunResolver Gate 尚未建立。

状态：
已修复


---

## ERR-20260926-001

Agent：Codex（工作站交接执行）/ ChatGPT（任务交接设计）
任务：XYE-WORKSTATION-REHOME-V0300-R1 / Toolchain Handoff
类型：GOVERNANCE
严重度：Medium

错误：
仓库已存在 K-GOV-003、EXP-GOVERNANCE-001、ERR-20260925-005，且 run.bat 已通过 scripts/resolve-dotnet.ps1 解析多机器 SDK，但本次工作站交接仍未先执行 Repository Resolver，而把 PATH 中无法直接调用 dotnet 误判为“.NET SDK 未安装”，从而错误将版本基线任务标记为 BLOCKED。

根因：
交接流程仍停留在“知识库有规则、任务书人工提醒”的层级，没有把 Repository Bootstrap 和 Canonical Toolchain Resolver 做成所有 Build/Test/Run 之前的强制机器入口；同时旧 docs/dev-rules.md 示例仍使用裸 dotnet build/test/restore，给执行 Agent 留出了绕过 Resolver 的空间。

后果：
在 v0.3.0.0-r1 版本基线已经完成代码修改后产生一次假环境阻断，延迟 Build/Test、Commit 和 Push；若不机器化，会在不同电脑、不同盘符环境间持续重复同类错误。

正确做法：
所有 XYE 新会话、新机器、交接任务先执行 scripts/xye-bootstrap.ps1；所有正式 .NET 命令通过 scripts/xye-dotnet.ps1 或 scripts/resolve-dotnet.ps1 返回的绝对 DOTNET_EXE 执行。只有 Canonical Resolver 本身失败后，才允许报告 SDK / Toolchain BLOCKED。

经验规则：
EXP-GOVERNANCE-001

发现方式：
用户纠正 / ChatGPT Repository Audit / HANDOFF-BOOTSTRAP-R1 复盘

验证证据：
scripts/resolve-dotnet.ps1 实际解析到 D:\MyApp\sdk-dotnet\dotnet.exe，SDK 10.0.400；scripts/xye-bootstrap.ps1 返回 READY；Canonical Toolchain Contract T1–T7 PASS；Dogfood Build/Test PASS；完整 Solution Build 0 Warning / 0 Error；版本、治理、Terrain 专项 24/24 PASS。治理提交 1b29f197 与 ee17a281 已推送，最终远端为 ee17a281ffd3326253501cd3ec35c4df4effb996。

状态：
已验证
