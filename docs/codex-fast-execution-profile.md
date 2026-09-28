# Codex Fast Execution Profile

## 目的

本文件将 CODEX FAST EXECUTION PROFILE 转化为本仓库可执行的工作规则，适用于 XYUI Runtime、Rules & Architecture 任务。

核心流程：

```text
AUDIT ONCE
→ DESIGN ONCE
→ IMPLEMENT ONCE
→ VERIFY ONCE
→ CLOSEOUT ONCE
```

目标不是减少真实验证，而是避免在没有新证据时重复审计、重复设计、重复构建和重复规划。

## 一、开工冻结

每轮最多冻结 3 个目标包，不按单个 Control 建立十几个变化中的 TODO。

以 XYUI-3 Round 1 为例：

```text
TARGET A · Menu Runtime Family · 3.01～3.04
TARGET B · Navigation Runtime · 3.05
TARGET C · Sidebar Runtime · 3.06
```

开工时只完整读取一次：

- `AGENTS.md`
- `docs/dev-rules.md`
- XYUI 双 Agent 规范
- 本轮 Canonical / Runtime Contract / Design Spec

读取后立即提炼为：

```text
OWNERSHIP
ALLOWED FILES
FORBIDDEN FILES
PUBLIC CONTRACT
TEST MATRIX
```

只有出现真实规则冲突时，才回查具体章节；不得为了安全感反复读取整份长文档。

## 二、一次性审计

审计范围仅包含：

- 本轮 Controls
- 本轮 Tests
- 直接依赖的 XYUI Runtime
- 必要的 Gallery Contract

审计输出固定为：

```text
EXISTING · KEEP · PATCH · ADD · DELETE
```

审计完成后标记：

```text
AUDIT FROZEN
```

实施阶段禁止无目标全仓搜索。只有以下证据允许重新定向调查：

```text
Compile Failure
Test Failure
Contract Conflict
```

## 三、Contract First

编辑代码前一次性冻结：

```text
Public Types
Properties
Events
Commands
State Ownership
Composition
Interaction
Files
Tests
```

Contract 冻结后直接选择满足要求的最小正确实现，不因普通内部实现细节反复重新规划，也不向用户请求不必要的选择。

Runtime Contract 应尽早交给并行 Agent，至少包含：

```text
CONTROL
TYPE
PROPERTY
EVENT
STATE
VARIANT
LIMITATION
```

如果后续没有公共 API 变化，不重复编写 Contract。

## 四、批量实现与 Existing Runtime 优先

同一目标包的直接相关文件一次完成，不采用以下低效循环：

```text
写一个控件 → Build → 写下一个控件 → Build → 回头重构
```

正确顺序是：

```text
Audit
→ 冻结共享 Runtime
→ 批量完成目标包 A
→ 批量完成目标包 B
→ 批量完成目标包 C
→ 统一 Compile
```

已有实现满足以下条件时优先 `PATCH`：

```text
语义正确
结构正确
API 基本正确
```

不得以“统一风格”或“未来可能需要”为理由重写正确 Runtime、提前建立复杂 Framework 或顺手整理无关文件。

## 五、验证门禁

正常路径：

```text
Implementation Complete
→ Runtime Project Build / Compile Check
→ Targeted Runtime Tests
→ 失败则只修复失败相关路径
→ Final Full Solution Build
→ Full Tests --no-build
→ ARCH-A
→ 5+100
→ git diff --check
```

约束：

- dotnet 命令严格串行。
- Runtime 开发期间优先使用最小项目 Build 和定向测试。
- 完整解决方案 Build 每轮最终阶段只运行一次。
- 没有代码变化时不得重复运行同一个 Build、Test 或 ARCH-A。
- 单个测试失败时，只定位、修改并重跑失败测试或相关测试；恢复通过后再进入最终全量门禁。
- 同一问题最多进行两个修复循环；仍失败则停止并报告证据，不继续无边界试错。

## 六、双 Agent 合流

允许 Codex 与 Gemini 共享 Canonical Working Tree，但必须满足：

1. Runtime / Presentation 严格所有权隔离。
2. 禁止处理另一 Agent 的未提交文件。
3. 施工阶段只跑各自局部门禁。
4. Full Integration Gate 只在双方 `HANDOFF READY` 后运行。
5. 另一 Agent 施工中的失败不得进入本 Agent 调试链。

完整集成测试必须等待：

```text
PRESENTATION IMPLEMENTED
HANDOFF READY
```

如果失败文件属于另一 Agent 的 Ownership，或发生在另一 Agent 仍施工中的文件：

```text
指出文件
→ 指出测试与失败证据
→ 通知 Owner
→ STOP INVESTIGATION
```

不得为了让自己的门禁变绿而修改另一 Agent 的文件、弱化测试或替对方深入调试 Presentation。

## 七、文档与落库

代码和测试稳定后，只做一次文档同步：

```text
Code Stable
→ Tests Stable
→ changelog
→ 基于 git ls-files 重建 file-tree
→ Audit / Contract
→ commit
→ push
→ STOP
```

禁止在代码尚未稳定时反复修改 `changelog.md` 或 `file-tree.md`。`file-tree.md` 必须以 `git ls-files` 为事实源，不进行大量手工逐项修补。

最终报告只保留：

```text
TARGET A · PASS / FAIL
TARGET B · PASS / FAIL
TARGET C · PASS / FAIL
Changed Files
Public Contract
Tests
Build
ARCH-A
5+100
diff-check
Gemini Integration
Commit
Push
Blockers
Status
```

所有施工任务最终报告还必须附加：

```text
==================================================
KNOWLEDGE / EXPERIENCE AUDIT HANDOFF
==================================================
CHATGPT KNOWLEDGE AUDIT REQUIRED
```

Codex 仅提交事实、Root Cause、Changed Files、Tests、Evidence、Known Risk 和 Candidate Lessons；正式知识判断由 ChatGPT 审计 AI 完成，并遵循 `SEARCH EXISTING → MATCH → UPDATE / STRENGTHEN → CREATE`。

## 八、XYUI Round 1 标准执行序列

```text
1. Audit 01～06 Runtime
2. Freeze 3 Targets：Menu Family / Navigation / Sidebar
3. Freeze Runtime Contract
4. Bulk Implement Menu Family
5. Bulk Implement Navigation
6. Bulk Implement Sidebar
7. Runtime compile
8. Targeted tests
9. Contract handoff / confirm Gemini
10. Gemini HANDOFF READY 后执行 Full Build、Full Tests、ARCH-A、5+100、diff-check
11. 一次同步 changelog、file-tree、audit
12. commit / push
13. STOP
```

本 Profile 的完成标准是：在已冻结 Contract 下，用最小正确实现一次完成 Runtime，并用一次最终技术门禁确认，而不是证明已经考虑过所有可能性。
