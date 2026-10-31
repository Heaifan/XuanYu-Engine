# 十月历史知识交叉审计 R4：相机ID与真实GPU证据

执行日期：2026-10-31。知识范围：XYK独立分支；产品源码改动：NONE；本轮没有执行Build/Restore/Test。

## 纠错：K-SPA-004主旨被缩窄
10/05的产品知识在 docs/knowledge/architecture.md 中已将K-SPA-003定义为Gameplay/显式Screen-Anchored技术能力，K-SPA-004定义为Editor / Gameplay Zoom Authority上下文分离，DEC-CAM-001是获准决策。10/10候选相机只读净空和陷地Zoom-Out脱困是K-SPA-004的局部强化，不是独立的新主题。
R2在xyk/main只查旧独立索引，错误用K-SPA-004直接表示只读净空。此次保持历史R2事件追加不删，创建K-SPA-003与DEC-CAM-001、UPDATE K-SPA-004，把原主旨和附加规则一并恢复。
原始产品正文：https://github.com/Heaifan/XuanYu-Engine/blob/71fc8c365fcdd83166b9d57fdd123fe81316d618/docs/knowledge/architecture.md
10/10 CPU强化：3bbf6fa382dc00c3156ed744330e57de598e8aee；未重测完整Vulkan P3/P4。

## 10/17 XYC Vulkan已存在真实实验数据
提交4df10e80bdf9f1d86a11da2a1b42029e3d363a4d包含隐藏Win32 HWND / Swapchain相关实现；产品知识提交71fc8c365fcdd83166b9d57fdd123fe81316d618的R2报告记录RTX3060、776x650、Present/GPU Readback和CameraRevision绑定，14/14与0W0E，但DIRTY=YES。不能推断可见UiWin、完整UI输入或WP2 P4验收。
R8报告：https://github.com/Heaifan/XuanYu-Engine/blob/1ca3b515a1d4f7b86cad2d9add0cbd45686d6c8c/docs/qa/XYC-WP2-P4-R8/XYC-WP2-P4-R8-RUN1.md
R8记录513/1025/2049与9张GPU PNG的FrameId、CameraRevision、SHA；2049固定2km姿态ScaleBar无效。采样密度与Auto LOD同时变化，不能定论Auto LOD是主因；理论全格三角形数不是实际GPU提交量。R8局部5/5与0W0E，Arch-A仍报告抽象层反向依赖Vulkan，真实产品视觉未CLOSED。

## 防复发
任何新建/重定义知识ID必须查独立xyk/main与可验证的遗留产品知识。旧事件追加保留，不能静默改写历史。产品提交和知识提交身份分开，不以实验报告冒充本轮重新执行的GPU结果。
