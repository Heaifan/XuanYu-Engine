# XYE Core 子文件夹审计｜Observability / Logging｜R1

- 审计单元：整个 `XuanYu.Core/Observability/Logging` 子文件夹（3 个生产文件）。
- 测试目录：`XuanYu.Core.Tests/Observability/Logging`；测试文件统一 `test_` 前缀。
- 基线：`02026e1c536d0fd991eb1498710219a05293fc0e`；分支：`审计XYE架构`。
- 限制：本轮不修改 Camera、World、Editor、Vulkan 及任何调用方。

## 逐文件清单

### EngineLogEntry.cs（引擎日志条目）
- `EngineLogEntry(double, EngineLogLevel, string, string)`（创建日志对象的私有构造函数）
- `Create(double, EngineLogLevel, string, string)`（校验后创建日志条目）
- `ToDisplayString()`（生成带中文等级的显示消息）
- `ToString()`（生成日志字符串）
- `ContainsLogLevelPrefix(string)`（识别正文开头已附带的等级标签）
- 属性：`SimulationSeconds`（模拟秒数）、`Level`（日志级别）、`Category`（分类）、`Message`（正文）。
- 风险及改动：只拒绝正文开头的等级标签（含前导空格），不再误拒正文中间的普通标签文本；明确拒绝未知级别。

### EngineLogLevel.cs（日志等级枚举）
- 无显式函数；枚举值：`Trace`（追踪）、`Info`（信息）、`Warning`（警告）、`Error`（报错）、`Critical`（严重）。
- 无需修改。

### EngineLogLevelExtensions.cs（日志等级扩展）
- `ToChineseLabel(EngineLogLevel)`（获取日志等级中文标签）。
- 常量：`TraceChineseLabel`（追踪）、`InfoChineseLabel`（信息）、`WarningChineseLabel`（警告）、`ErrorChineseLabel`（报错）、`CriticalChineseLabel`（严重）。
- 风险及改动：不再把未知等级默默当作信息等级，而是明确拒绝。

## 测试文件与函数

### test_EngineLogEntry.cs
- `CreatesEntry()`（校验日志字段及显示格式）
- `RejectsInvalidTime()`（拒绝非法模拟时间）
- `RejectsBlankFields()`（拒绝空分类或正文）
- `RejectsLevelPrefix()`（拒绝正文开头重复等级标签）
- `AllowsEmbeddedLabel()`（允许正文中间出现标签文本）
- `RejectsInvalidLevel()`（拒绝无效等级值）

### test_EngineLogLevelExtensions.cs
- `MapsChineseLabels(EngineLogLevel, string)`（验证五种等级中文映射）
- `RejectsUnknownLevel()`（拒绝未知等级值）

## 状态与风险
- 三份生产文件保持原有职责；没有改动任何日志调用方。
- 已静态核对文件清单、修改内容、单文件行数、测试函数及公开 API。
- 编译：NOT RUN；测试：NOT RUN；业务集成：NOT PASS。不得将静态审计等同于运行验收。
