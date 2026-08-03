# WORLD-A-UI-R1 Display Cleanup

版本：`v0.2.18.20-fix`

## 当前裁定

本轮为 `WORLD-A-R3-R2` 前的 UI 可读性修正轮，不进入 Picking 正式接线，不改变 SpatialIndex / WorldQuery 算法，不引入 Organization Graph、ECS、场景保存、层级拖拽重排或公共 API 中文重命名。

## 完成项

| 项 | 结果 |
| --- | --- |
| 日志列顺序 | 底部日志表格改为 `时间 / 级别 / 来源 / 模块 / 消息 / 详情` |
| 运行模块显示 | `LogEntry.ModuleText` 输出中文模块，兼容旧 `CategoryText` |
| 普通日志中文化 | Selection、Picking、Move、History、Scene、NativeHost、Vulkan 低频生命周期主文案清理 |
| 树形 UI | Project Tree / Hierarchy Tree 补分支线、统一行高、缩进、悬停与选中态 |
| SVG Path 图标 | Region、Camera、Ground、Entity、Script、Build 等节点使用矢量路径图标 |
| 中文显示映射 | 测试实体、Region、Activity、Inspector 字段使用中文显示文本 |
| 治理同步 | 开发宪法与 dev-rules 追加日志模块名、时间首字段、树形 UI 与中文显示映射规则 |

## 禁止项确认

- 未接入 `WORLD-A-R3-R2` Picking 正式主链。
- 未修改 SpatialIndex / WorldQuery 算法。
- 未新增 Organization Graph、ECS、场景保存或层级拖拽重排。
- 未重命名内部 C# 标识、实体 key 或公共 API。

## 验证

| 验证项 | 结果 |
| --- | --- |
| 残留文本扫描 | `SampleProject`、旧测试实体名、`RecordCommandBuffers`、`ApplySelection`、`旧 extent`、`chosen extent`、`generation=`、`Selection=` 等运行时可见残留扫描无命中 |
| 文件计数 | `rg --files -g "!*bin*" -g "!*obj*"` = 414；`file-tree.md` = 414 |
| SVG XML | `docs/world-a-ui-r1-display-cleanup.svg` 可被 XML 解析 |
| 5+100 | `.cs` / `.axaml` 无新增超 100 行文件 |
| 构建 | `dotnet build .\XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false -maxcpucount:1`：7 项目，0 warning / 0 error |
| 测试 | `dotnet test .\XuanYu.Engine.slnx --no-restore --no-build -p:UseSharedCompilation=false -maxcpucount:1`：149 passed / 0 failed / 0 skipped |
| 守卫 | `scripts/arch-a-guard.ps1`：PASS；`git diff --check`：PASS |
| 真机启动 | `run.bat` 启动窗口标题为 `玄域引擎编辑器 v0.2.18.20-fix` |
| 真机 UI | Project Tree / Hierarchy Tree 显示分支线与矢量图标；右侧项目检查器显示 `玄域示例项目`；实体 Inspector 显示 `测试实体 04`、`最小场景实体`、`实体编号(4)`、`区域 0,0,0` |
| 真机日志 | 展开日志表格后可见列顺序为 `时间 / 级别 / 来源 / 模块 / 消息`；模块列显示 `Vulkan桥接`，Vulkan 设备日志正文不再带 `【VulkanDevice】` 旧模块标签 |

## Git 证据

- 实现提交：`7437ddde82464115d6c8a083e17f7d9d47b10470`
- 实现提交父节点：`df141f7ed23221360bfd339d482643d9f8584f2c`
- 证据回填：本节由后续文档证据提交记录，避免自引用 hash 悖论。
