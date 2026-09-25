# Diagnostic Auto ID Fix 1

## 目标

修复 Inspector 动态切换触发的诊断自动 ID 重复，避免诊断系统把编辑器 UI 线程打死。

## 方案

1. `DiagnosticAutoBinder` 从当前 VisualTree 收集所有既有 DebugId，不再依赖已被清空的 Registry。
2. `DiagnosticRegistry.Rebuild` 先在临时索引中完成校验和 AUTO ID 冲突消解，再一次性替换正式索引。
3. `XYE.AUTO.*` 重复时分配稳定后缀；人工声明 ID 重复仍抛异常，保留测试约束。
4. `UiRoot.RefreshDiagnosticRegistry` 改为 Bind 后 Rebuild，不再先清空再扫描。

## 验证

- 动态新增两个 XYIconButton，二次刷新后 ID 唯一且都在 Registry。
- AUTO ID 重复自动修复；人工固定 ID 重复仍失败。
- 现有 UiRoot / Diagnostic 专项测试和受影响项目编译。
- `git diff --check`、5+100、工作区范围检查。
