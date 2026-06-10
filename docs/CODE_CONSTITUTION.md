# FluidWarfare 代码宪法

创建时间：2026-06-10

本文档定义 FluidWarfare 的代码硬性规则。

## 单一职责

每个文件、类和方法都应该只有一个清晰存在理由。

不要在同一个实现单元里混合 UI、数据读取、模拟、渲染、平台适配、日志和领域计算。

职责不清时，先拆清领域边界，再写实现。

## 文件长度

普通代码文件建议不超过 100 行。

复杂基础设施文件在职责单一时可以接近 150 行。

超过 150 行前必须先提出拆分方案。

超过 200 行的文件必须有明确理由。

超过 300 行的文件默认禁止。

## 目录规模

一个目录通常应少于 7 个文件。

超过 10 个文件时必须按领域或职责拆分二级目录。

不得创建泛化的 `utils`、`helpers`、`managers` 或 `processors` 目录。

目录拆分必须按领域概念进行，而不是按文件编号进行。

## 明确命名

名称必须描述具体职责。

可接受示例包括：

1. `ScenarioEntityLoader`
2. `CombatContactResolver`
3. `MoraleDeltaCalculator`
4. `VulkanSwapchain`

## 平台隔离

Core、ECS、Simulation、World、Combat、AI、Data 和 Render 抽象项目不得依赖 Windows、Android、Avalonia 或具体 Vulkan 实现。

平台相关代码只能位于 Runtime、Editor、Exporter 或具体渲染后端。

## 旧仓库隔离

FluidWarfare-old 只作为历史参考。

新引擎代码不得复制旧代码、旧 OpenGL 实现、旧工程布局或旧前缀命名。

旧仓库中的思想可以被重新表达。

旧仓库中的实现不得被直接迁移。
