# FluidWarfare 变更日志

本文档记录项目的重要变更。

## 0.0.1-dev - 2026-06-10

### Milestone 2.x 中文化补丁

#### 修改

1. 明确 FluidWarfare 的人类可读文本默认使用中文。
2. 保留机器识别用 Code、类名、方法名、命名空间为英文。
3. 将 Core 已有异常提示中文化。

### 新增

1. 创建初始解决方案骨架。
2. 创建顶层模块目录和资源目录规划。
3. 创建项目宪章、架构说明、AI 开发规则、代码宪法、命名规则、Phase 1 范围和旧仓库考古报告。
4. 创建 `file-tree.md`，作为项目结构地图。
5. 为当前空目录创建 `.gitkeep` 占位文件。
6. 创建 `.gitattributes`，固定 Markdown、解决方案、C# 和 JSON 文件使用 LF 行尾。
7. 创建 `docs/MILESTONE1_PUBLIC_VALIDATION.md`，记录公开 GitHub Raw 验收命令与旧目录核对命令。
8. 创建 `FluidWarfare.Core` 纯 C# 类库项目。
9. 创建 `FluidWarfare.Tests` xUnit 测试项目。
10. 创建 `CoreSmokeTests` 最小冒烟测试。
11. Milestone 2.2：新增 `EntityId` 值对象。
12. Milestone 2.2：新增 `EntityId` 单元测试。
13. Milestone 2.3：新增 `TimeStep` 时间步长值对象。
14. Milestone 2.3：新增 `SimulationTime` 模拟累计时间值对象。
15. Milestone 2.3：新增 `TimeStep` 与 `SimulationTime` 单元测试。
16. Milestone 2.4：新增 `Vector3d` 3D 坐标与向量值对象。
17. Milestone 2.4：新增 `YawRotation` 水平朝向角值对象。
18. Milestone 2.4：新增 `Vector3d` 与 `YawRotation` 单元测试。
19. Milestone 2.4：固定核心坐标约定，XZ 为地面平面，Y 为高度。
20. Milestone 2.4：固定朝向约定，0 度朝 +Z，90 度朝 +X。
21. Milestone 2.5：新增 `EngineError` 错误值对象。
22. Milestone 2.5：新增 `EngineResult` 操作结果值对象。
23. Milestone 2.5：新增 `EngineError` 与 `EngineResult` 单元测试。
24. Milestone 2.5：固定结果语义，Success 不携带错误，Failure 必须携带有效错误。
25. Milestone 2.5：固定错误文本规则，Code 使用英文，Message 使用中文。
26. Milestone 2.5：固定分层规则，【报错】【信息】等日志等级前缀不写入 `EngineError.Message`。
27. Milestone 2.6：新增 `EngineLogLevel` 日志等级枚举。
28. Milestone 2.6：新增 `EngineLogEntry` 日志记录值对象。
29. Milestone 2.6：新增日志等级中文前缀映射。
30. Milestone 2.6：新增 `EngineLogLevel` 与 `EngineLogEntry` 单元测试。
31. Milestone 3.1：新增 `FluidWarfare.Editor.Windows` Avalonia 编辑器项目。
32. Milestone 3.1：将 Editor 项目加入 `FluidWarfare.sln`。
33. Milestone 3.1：Editor 引用 `FluidWarfare.Core`。
34. Milestone 3.2：新增 `FluidWarfare Editor` 主窗口。
35. Milestone 3.2：新增顶部菜单、项目面板、3D 视口占位、检查器、日志面板。
36. Milestone 3.2：日志面板显示中文 `EngineLogEntry` 输出。

### 修改

1. 将 `docs/` 下所有 Markdown 文档正文改为中文。
2. 修复 `docs/` 文档和 `file-tree.md` 的 Markdown 排版，使标题、段落、表格、列表和代码块独立换行。
3. 将 Markdown 文件重写为 UTF-8 无 BOM 与 LF 行尾，方便 GitHub Raw 公开验收。
4. 使用 Python 以 `newline="\n"` 重新写入 `.gitattributes`、`file-tree.md` 和所有 `docs/*.md`。
5. 将 Core 与 Tests 项目加入 `FluidWarfare.sln`。
6. Milestone 2.3.1：修复 `TimeStep` 默认值边界。
7. Milestone 2.3.1：修复 `SimulationTime.Advance` 对 `default(TimeStep)` 的处理。
8. Milestone 2.3.1：稳定 `TimeStep` / `SimulationTime` 的 `ToString` 输出。
9. Milestone 2.5.1：修复 `EngineResult` 默认值语义，明确 `default(EngineResult)` 为无效结果。
10. Milestone 2.5.1：调整 `EngineResult.IsFailure`，只有携带有效 `EngineError` 的失败结果才返回 true。
11. Milestone 2.5.1：确认日志等级前缀统一使用【】。
12. Milestone 2.5.2：统一日志等级前缀符号为【追踪】【信息】【警告】【报错】【严重】。
13. Milestone 2.6：固定日志分层规则：Message 只保存正文，显示输出时再添加等级前缀。
14. Milestone 2.6.1：修复日志等级前缀统一校验，确认 `EngineLogLevel` 与 `EngineLogEntry` 只使用【】。

### 删除

1. 删除由 .NET SDK 默认模板临时生成的 `FluidWarfare.slnx`。
