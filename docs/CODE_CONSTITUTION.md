# XuanYu Engine 代码宪法

最后更新：2026-09-16

本文档定义 XuanYu Engine 的代码硬性规则；若与 `docs/玄域引擎_AI开发宪法.md` 冲突，以 AI 开发宪法为准。

## 5+100 红线

所有手写 `.cs`、`.axaml`、`.js` 单文件必须 ≤ 100 行。

这是不可豁免硬红线：

- 不存在“职责单一即可超过 100 行”的例外；
- 不存在“临时审计豁免”；
- 不存在“复杂基础设施文件”豁免；
- 历史债务不产生新增超限权利；
- 不得用压缩格式、多语句挤一行、`partial`、无意义转发类或伪装生成文件规避。

复杂性必须通过职责分解解决。

## 单一职责

每个文件、类和方法都应只有一个清晰存在理由。

不得在同一个实现单元里混合 UI、数据读取、模拟、渲染、平台适配、日志和领域计算等不相关职责。

职责不清时，先拆清领域边界，再写实现。

## 明确命名

名称必须描述具体职责。

可接受示例：

1. `ScenarioEntityLoader`
2. `CombatContactResolver`
3. `MoraleDeltaCalculator`
4. `VulkanSwapchain`

禁止泛化的 `Manager` / `Helper` / `Utils` 式职责容器在没有明确领域边界时继续膨胀。

## 中文化规则

机器读取标识保持英文，人类读取文本使用中文。

必须使用中文：

1. `EngineError.Message`
2. 异常 message
3. 日志 message
4. 编辑器提示
5. 导出提示
6. 验收结果
7. `file-tree.md`
8. `changelog.md`
9. 开发任务说明

必须保留英文：

1. 命名空间
2. 类名
3. 方法名
4. 文件名
5. 测试方法名
6. `EngineError.Code`
7. 程序内部枚举名

## 平台隔离

Core、World、WarCore、Simulation、Data 和 Render 抽象不得依赖 Windows、Android、Avalonia 或具体 Vulkan 实现。

`Editor.UI` 不得直接持有或控制 Vulkan 对象和生命周期；具体 Vulkan 实现只位于 Vulkan 后端或正式组合根。

## 权威事实源

长期状态只能存在一个可写权威所有者。

UI、Renderer、Inspector、Snapshot、缓存和索引只能读取、投影或维护派生事实，不得反向成为领域真源。

## 高频链路

PointerMoved、Hover、DragPreview、RenderFrame、Resize、Picking 等高频主链不得执行可预见的大规模 O(N) 全量扫描、普通日志洪泛、持久化、Undo Commit、重型 Inspector 刷新或重建全部世界数据。

## Vulkan 返回值规则

除规范明确返回 void 的 Vulkan 函数外，所有 `VkResult` 必须保存并分类处理，禁止直接丢弃。

## 两阶段枚举规则

所有 count → array 的 Vulkan 枚举必须处理 `Incomplete`，并设置有限重试次数。

## 等待规则

Editor UI 线程不得使用 `ulong.MaxValue` 等无限 Vulkan 等待；Fence / Acquire 等待必须有上限和超时处理。

## Native 资源规则

每个 native 创建入口必须同时具备对应销毁能力，禁止先创建资源、后续再可选补齐销毁函数。

## Swapchain 唯一入口

`vkCreateSwapchainKHR` 与 `vkDestroySwapchainKHR` 只能由正式 Swapchain 模块调用。

## 生命周期测试

新增 Vulkan 资源类型时，必须覆盖：

1. 创建成功；
2. 部分失败清理；
3. Dispose 幂等；
4. 创建 / 销毁数量不变量。
