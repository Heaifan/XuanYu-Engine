# FluidWarfare 项目宪章

创建时间：2026-06-10

FluidWarfare 是一个面向 RTS、RTT、大战略态势推演和大规模编队作战的自研 C# 3D 战略战术引擎。

本项目不是 Unity、Unreal、Godot 或 MonoGame 的通用替代品，而是专门服务于战争模拟、战术推演、态势表达、可解释战斗、可回放模拟和编辑器驱动场景制作的专用引擎。

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

Phase 1 需要证明最小跨平台闭环：Windows Editor 创建简单 3D 场景并保存为 JSON，Windows Runtime 读取并渲染该场景，Android Runtime 读取同一份数据，Exporter 能打包两个运行时目标。

## 仓库规则

所有新代码、新文档和新工程结构都写入新的 FluidWarfare 仓库。旧仓库 FluidWarfare-old 只作为只读历史资料，不继续承载新引擎开发。
