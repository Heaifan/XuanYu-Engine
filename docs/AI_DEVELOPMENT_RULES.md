# FluidWarfare AI 开发规则

创建时间：2026-06-10

本文档约束 Codex 以及所有参与本仓库开发的人类或 AI 助手。

## 范围纪律

1. 每次只处理一个里程碑或一个小子任务。
2. 编辑前列出计划新增或修改的文件。
3. 不得超出当前任务范围。
4. 只要新增、删除、重命名或移动文件与目录，就必须更新 `file-tree.md`。
5. 如果本次没有结构变化，回复中必须说明本次无需更新 `file-tree.md`。

## 代码纪律

1. 优先使用小文件，每个文件只承担清晰职责。
2. 不创建 `Part1`、`Part2` 或编号拆分文件。
3. 避免 `GameManager`、`CommonUtils`、`DataProcessor`、`SystemHelper` 等含糊名称。
4. 只使用 `FluidWarfare.*` 命名空间。
5. 不使用 `BingWuChangShiEngine`、`Bwc.*`、`cls_` 或 `fuc_` 命名。

## 技术纪律

1. 不引入 Unity、Unreal、Godot 或 MonoGame。
2. Vulkan 代码只能写在 `FluidWarfare.Render.Vulkan`。
3. Avalonia 依赖只能写在 `FluidWarfare.Editor.Windows`。
4. 不复制旧仓库代码。
5. 不迁移旧 OpenGL 实现。

## 验证要求

核心模块实现时必须配套聚焦测试。每个里程碑结束时都要给出变更清单和验收方式。
