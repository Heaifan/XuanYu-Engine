# DEC-XYE-ARCH-5X5X5-R1｜架构目录和命名

**ACTIVE｜用户裁定日期：2026-10-24～30。**

1. C# 文件≤100行；末级目录≤5文件；采用严格三级目录，不得加无穷第四层来躲避规则。SRP、中文诊断、0W0E仍然适用。
2. 一级目录越少越好但必须职责清晰；一个一级中二级数量超过10为拆编警戒，不是固化的绝对错误。
3. 简短可检索文件名；测试独立模块统一 test_ 前缀，按被测职责归档；改名须同步所有消费者、测试及文件树。
4. Core / World / Render.Abstractions 不可反向依赖具体 Vulkan 或 Editor 实现。
5. XYUI1～XYUI4 和 XYUIx-编号-组件名是稳定索引，不能因为重组改为纯功能名称。

**依据**：用户在 Core/World/XYUI 审计的明确裁定；GitHub [84d49d43](https://github.com/Heaifan/XuanYu-Engine/commit/84d49d437d6a59060c18d37fc72b2d4594fb07ac)、[bfaa3342](https://github.com/Heaifan/XuanYu-Engine/commit/bfaa33425abd51325544c743168c764805376a83)、[a88a06aa](https://github.com/Heaifan/XuanYu-Engine/commit/a88a06aa05608bfc279d9028650749c83bae62e7)。

**边界**：架构裁定不等于任何源码迁移自动通过构建或全仓 Integration。
