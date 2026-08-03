# FluidWarfare 项目宪章

创建时间：2026-06-10

## 项目定位

FluidWarfare 是一个面向 RTS、RTT、大战略态势推演和大规模编队作战的自研 C# 3D 战略战术引擎。

本项目不是 Unity、Unreal、Godot 或 MonoGame 的通用替代品。

FluidWarfare 的目标是服务于战争模拟、战术推演、态势表达、可解释战斗、可回放模拟和编辑器驱动的场景制作。

## 目标类型

FluidWarfare 优先面向以下类型：

1. RTS，即时战略。
2. RTT，即时战术。
3. GSG，大战略态势。
4. Total War 式编队战斗。
5. HOI4 式战略态势表达。

## 核心原则

1. 3D 原生。
2. 代码优先。
3. 数据驱动。
4. AI 友好。
5. 战斗可解释。
6. 推演可回放。
7. ECS 数据可检查。
8. 编辑器与运行时分离。
9. Windows 开发，Windows 与 Android 发布。

## 第一阶段目标闭环

Phase 1 需要证明最小跨平台闭环。

该闭环包含以下步骤：

1. Windows Editor 创建简单 3D 场景。
2. Windows Editor 将场景保存为 JSON。
3. Windows Runtime 读取并渲染该场景。
4. Android Runtime 读取同一份场景数据。
5. Exporter 能打包 Windows 与 Android 两个运行时目标。

## 仓库规则

所有新代码、新文档和新工程结构都写入新的 FluidWarfare 仓库。

旧仓库 FluidWarfare-old 只作为只读历史资料，不继续承载新引擎开发。

## 中文化规则

FluidWarfare 面向中文开发者，所有人类可读文本默认使用中文。

必须使用中文的内容包括：异常 message、错误 message、日志 message、编辑器提示、导出提示、验收结果、`file-tree.md`、`CHANGELOG.md` 和开发任务说明。

必须保留英文的内容包括：命名空间、类名、方法名、文件名、测试方法名、`EngineError.Code` 和程序内部枚举名。

错误代码用于机器判断、搜索、日志筛选、测试断言和跨语言扩展，因此保持英文。

## 第一阶段边界

Phase 1 只追求最小闭环。

真实战斗、AI、复杂地形、PBR、联网、Mod 和完整地图编辑器都不进入第一阶段。
