# FluidWarfare 变更日志

本文档记录项目的重要变更。

## 0.0.1-dev - 2026-06-10

### 新增

1. 创建初始解决方案骨架。
2. 创建顶层模块目录和资源目录规划。
3. 创建项目宪章、架构说明、AI 开发规则、代码宪法、命名规则、Phase 1 范围和旧仓库考古报告。
4. 创建 `file-tree.md`，作为项目结构地图。
5. 为当前空目录创建 `.gitkeep` 占位文件。
6. 创建 `.gitattributes`，固定 Markdown、解决方案、C# 和 JSON 文件使用 LF 行尾。
7. 创建 `docs/MILESTONE1_PUBLIC_VALIDATION.md`，记录公开 GitHub Raw 验收命令与旧目录核对命令。

### 修改

1. 将 `docs/` 下所有 Markdown 文档正文改为中文。
2. 修复 `docs/` 文档和 `file-tree.md` 的 Markdown 排版，使标题、段落、表格、列表和代码块独立换行。
3. 将 Markdown 文件重写为 UTF-8 无 BOM 与 LF 行尾，方便 GitHub Raw 公开验收。
4. 使用 Python 以 `newline="\n"` 重新写入 `.gitattributes`、`file-tree.md` 和所有 `docs/*.md`。

### 删除

1. 删除由 .NET SDK 默认模板临时生成的 `FluidWarfare.slnx`。
