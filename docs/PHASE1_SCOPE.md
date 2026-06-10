# FluidWarfare Phase 1 范围

创建时间：2026-06-10

Phase 1 用于证明最小跨平台闭环。

## 范围内

1. Windows Editor 可以创建并保存简单场景 JSON。
2. 场景包含地面、红方编队标记、蓝方编队标记、相机数据、ECS 数据和基础模拟数据。
3. Windows Runtime 读取 JSON，运行固定 Tick，并渲染场景。
4. Android Runtime 读取同一份场景数据，并渲染同一个基础场景。
5. Exporter 可以打包 Windows 输出和 Android APK 输出。
6. ECS Inspector 可以显示基础实体和组件数据。

## Phase 1 里程碑

1. 旧仓库审计与工程骨架。
2. Core 基础类型。
3. ECS-lite。
4. Simulation 时钟与固定 Tick Runner。
5. 场景数据读取。
6. 渲染抽象。
7. Windows Vulkan 最小渲染路径。
8. Windows Runtime。
9. Avalonia Editor 初版。
10. Android Runtime 模板。
11. Exporter 初版。

## 明确不做

真实战斗、AI、空军、炮兵、万人同屏、复杂地形、高度图、DEM、战争迷雾、PBR、阴影、骨骼动画、粒子系统、联网、Mod、蓝图编辑器、完整地图编辑器、复杂 ECS 可视化，以及 Avalonia 内嵌 Vulkan 视口。
