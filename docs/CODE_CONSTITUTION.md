# XuanYu Engine 代码宪法

创建时间：2026-06-10
最后更新：2026-06-24（品牌更名为 XuanYu Engine）

本文档定义 XuanYu Engine 的代码硬性规则（历史代号 FluidWarfare）。

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

不得创建泛化的工具、辅助、总管或处理器目录。

目录拆分必须按领域概念进行，而不是按文件编号进行。

## 明确命名

名称必须描述具体职责。

可接受示例包括：

1. `ScenarioEntityLoader`
2. `CombatContactResolver`
3. `MoraleDeltaCalculator`
4. `VulkanSwapchain`

## 中文化规则

机器读取的标识保持英文，人类读取的文本使用中文。

必须使用中文的内容包括：

1. `EngineError.Message`
2. 异常 message。
3. 日志 message。
4. 编辑器提示。
5. 导出提示。
6. 验收结果。
7. `file-tree.md`。
8. `CHANGELOG.md`。
9. 开发任务说明。

必须保留英文的内容包括：

1. 命名空间。
2. 类名。
3. 方法名。
4. 文件名。
5. 测试方法名。
6. `EngineError.Code`。
7. 程序内部枚举名。

示例：`Core.InvalidArgument` 作为错误代码保持英文，`参数无效。` 作为错误信息使用中文。

## 平台隔离

Core、ECS、Simulation、World、Combat、AI、Data 和 Render 抽象项目不得依赖 Windows、Android、Avalonia 或具体 Vulkan 实现。

平台相关代码只能位于 Runtime、Editor、Exporter 或具体渲染后端。

## 旧仓库隔离

FluidWarfare-old 只作为历史参考。

新引擎代码不得复制旧代码、旧 OpenGL 实现、旧工程布局或旧前缀命名。

旧仓库中的思想可以被重新表达。

旧仓库中的实现不得被直接迁移。

## Vulkan 返回值规则

除规范明确返回 void 的 Vulkan 函数外，所有 Result 必须被保存并分类处理。
禁止直接丢弃 VkResult。

## 两阶段枚举规则

所有 count → array 的 Vulkan 枚举必须处理 Incomplete，并设置有限重试次数。

## 等待规则

Editor UI 线程不得使用 ulong.MaxValue 等无限 Vulkan 等待。
所有 Fence / Acquire 等待必须有上限和超时处理。

## native 资源规则

每个 native 创建入口必须在构造时具备对应销毁能力。
禁止先创建资源、后续再"可选赋值"销毁函数。

## Swapchain 唯一入口

vkCreateSwapchainKHR 与 vkDestroySwapchainKHR 只能由 Scene3D Swapchain 模块调用。

## 生命周期测试

新增 Vulkan 资源类型时，必须同时增加：
1. 创建成功测试
2. 部分失败清理测试
3. Dispose 幂等测试
4. 创建/销毁数量不变量测试
