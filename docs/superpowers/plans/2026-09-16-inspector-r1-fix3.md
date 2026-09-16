# INSPECTOR-2.0-R1-FIX3 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 修复 Inspector 属性提交、Dataset/Feature 身份、权威值回显、状态语义、错误文案和版本证据链，使 R1 逻辑合同闭合。

**Architecture:** 保留现有 `InspectorPanel` 与 `InspectorPropertyDescriptor` 主路径。将 Dataset 与具体 Feature 分成不同 Inspector 身份；统一提交入口负责 Recent MRU，领域提交只负责领域状态；属性值直接读取当前选中领域对象。

**Tech Stack:** C#/.NET 10, Avalonia, xUnit, PowerShell, Git。

**Spec:** 用户提供的 INSPECTOR-2.0-R1-FIX3 审计反馈（2026-09-16）。

## Global Constraints

- 不修改本轮已冻结的 Inspector 视觉和 XYUI 图标。
- 每个手写 `.cs` / `.axaml` 不超过 100 行。
- dotnet 命令严格串行；解决方案完整构建一次；测试使用 `--no-build`。
- 不删除或弱化既有测试；不宣称真机验收完成。
- 精确暂存本轮文件，提交并推送正式分支。

### Task 1: Recent MRU contract

**Files:**
- Modify: `XuanYu.Editor.UI/Vm/Inspector/UiVm.InspectorNavigation.Commands.cs`
- Modify: `XuanYu.Editor.UI/Vm/Inspector/UiVm.EntityInspector.cs`
- Test: `XuanYu.World.Tests/UiRuntime/InspectorPropertyMutabilityTests.cs`

- [ ] 写失败测试：Road 名称成功提交后，Recent 分类包含 `Road.Basic.Name`。
- [ ] 运行定向测试确认因未记录 MRU 而失败。
- [ ] 统一 `CommitInspectorProperty` 记录一次提交结果，并移除 Entity 内部重复记录。
- [ ] 运行测试确认通过且不重复记录。

### Task 2: Dataset/Feature identity

**Files:**
- Modify: `XuanYu.Editor.UI/Vm/Inspector/InspectorSelection.cs`
- Modify: `XuanYu.Editor.UI/Vm/Inspector/InspectorDescriptors.cs`
- Modify: `XuanYu.Editor.UI/Vm/Inspector/UiVm.InspectorFeatureEditing.cs`
- Test: `XuanYu.World.Tests/UiRuntime/InspectorPropertyMutabilityTests.cs`

- [ ] 写失败测试：只选中 Road Dataset 时不得生成具体 Road Feature 的可编辑名称属性，且提交返回 false。
- [ ] 运行测试确认失败。
- [ ] 增加 Dataset 身份并为 Dataset 提供独立描述，禁止 Dataset 走 Feature 名称提交链。
- [ ] 运行测试确认 Dataset 与 Feature 身份隔离。

### Task 3: Authoritative values and status

**Files:**
- Modify: `XuanYu.Editor.UI/Vm/Inspector/InspectorDescriptors.cs`
- Modify: `XuanYu.Editor.UI/Vm/Map/UiVm.MapGeometryEditing.Inspector.cs`
- Modify: `XuanYu.Editor.UI/Right/InspectorPanel.axaml.cs`
- Test: `XuanYu.World.Tests/UiRuntime/InspectorPropertyMutabilityTests.cs`

- [ ] 写失败测试：Road/Region/Marker 名称值直接等于模型 `DisplayName`；失败提交后属性行恢复模型原值。
- [ ] 运行测试确认失败。
- [ ] 添加明确的 Feature 名称取值；失败提交后刷新 Inspector 属性绑定。
- [ ] 将状态值改为真实 `可见/锁定` 语义，保持只读。
- [ ] 运行定向回归。

### Task 4: Feature validation copy and version evidence

**Files:**
- Create/Modify: `XuanYu.World/Map/MapFeatureNameRules.cs`
- Modify: `XuanYu.Editor/MapEditing/MapEditSession.FeatureNames.cs`
- Modify: `XuanYu.World.Tests/MapEditing/MapEditSessionValidationTests.cs`
- Modify: `run.bat`
- Modify: `XuanYu.Editor.UI/Win/UiWin.axaml`
- Modify: `XuanYu.Editor.UI/Vm/Scene/UiVm.SceneDocument.cs`
- Modify: `XuanYu.World.Tests/UiTokens/UiCanonicalVersionContractTests.cs`

- [ ] 写失败测试：Feature 名称非法时显示“要素名称”而不是“图层名称”；四处版本值一致。
- [ ] 运行测试确认失败。
- [ ] 分离通用名称规则与 Feature 文案；统一 canonical version。
- [ ] 运行版本/校验测试。

### Task 5: Formal verification and delivery

- [ ] 运行 Inspector 定向测试、Core、WarCore、XYUI 相关测试。
- [ ] 运行一次完整解决方案构建、World 全量测试、ARCH-A、`git diff --check`。
- [ ] 更新必要的审计/变更文档，精确暂存。
- [ ] Commit、Push，并核对本地与远端 SHA、ahead/behind、工作区状态。
