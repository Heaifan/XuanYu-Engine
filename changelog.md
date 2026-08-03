# changelog

## 归档规则

- 每个自然月执行一次 changelog 归档（宪法第五十三条《月度归档》）。
- 当前自然月记录保留在本文件；已结束月份按自然月归档至 `docs/archive/changelog-YYYY-MM.md`（单一归档位置，不为每个轮次单独建文件）。
- 归档内容原则上原样迁移，保留版本、日期、验证与遗留事项；版本历史不丢失，按下方索引可定位。

## 历史归档索引

| 月份 | 归档文件 | 条目范围 |
|---|---|---|
| 2026-07 | `docs/archive/changelog-2026-07.md` | v0.2.1.1-rz ～ v0.2.23.0-rz |
| 2026-06 | `docs/archive/changelog-2026-06.md` | v0.1.1.5-rz ～ v0.1.8.10-fix |
| 2026-05 | `docs/archive/changelog-2026-05.md` | v0.1.1.1-rz ～ v0.1.1.4-rz |

---

## 2026-08（当前自然月）

## v0.2.24.13-fix
SHR-2026-08 阶段健康考核与治理收敛（2026-08-03，Commit 本轮落库为准）
- P0：宪法 2.0 独立入库（`docs: 生效玄域引擎AI开发宪法2.0`，b99e087，消除双事实源）；修复 arch-a-guard 5+100 行数统计漏检（PS 5.1 `Measure-Object -Line` 实测失真 109→96，改用 `[System.IO.File]::ReadAllLines` 确定性统计 + 8 样本门禁自验证，检查范围对齐宪法第十三条 .cs/.axaml/.js）；治理 3 个超限文件（WorldRotateTransformUiTests.R4R2.cs 109→66+Helpers 52、WorldToolStateHighlightUiTests.cs 105→85+Selection 26、Left.axaml 101→89+Left.Styles.axaml 16，真实拆分不压行）。
- P1：10 个 catch 逐处分类治理（B 类清理 best-effort 语义注释 3 处、C 类 UI 生命周期竞态注释 1 处、D 类 Gizmo 投影退化类型化+回退语义 3 处，另 3 处复核为正常业务处理不变）；dev-rules §17 失效"宪法第二十八章"引用改条款号+标题、版本规范"宪法第十六章"改第四十二条《版本一致性》、Editor.App=组合根/Editor.Win=Windows 平台宿主职责描述修正；changelog 月度归档 5/6/7 月 → `docs/archive/changelog-2026-{05,06,07}.md`（4436→200 行，含归档规则+历史索引）。
- P2：docs 分类框架落地（docs/archive/ 历史归档分类）；其余约 175 个历史文档平铺分类登记为渐进治理事项（每月一个逻辑簇，不阻断 MAP-A）。
- 验证：World.Tests/Editor.UI 快速编译 0 错误；修复后 arch-a-guard 全量 PASS（含自验证，此前同门禁对 3 个超限文件误报 PASS）；正式串行门禁见本轮最终报告。
- 治理：版本 v0.2.24.12-fix → v0.2.24.13-fix（五处同步）；未创建 Tag/Release；`IDEA.md` 已删除（无有效内容）。

## v0.2.24.12-fix
MAP-A-R1-D5-R1-F3-F1 世界原点屏幕空间标记 + 导航 Gizmo 移入 Vulkan Overlay Pass（2026-08-03 16:10:00，Commit 本轮落库为准）
- F3-A1（v0.2.24.11-fix）：**FAIL**。用户真机验收：
  1. 世界原点退化为黄色地面面片（旧实现贴 Z=0 世界空间面片，低角度透视被压扁成梯形）；
  2. 导航 Gizmo 真机零像素（Avalonia 覆盖层被 NativeControlHost 承载的 WS_CHILD 原生子窗口遮挡——airspace 问题，ZIndex/Margin/Opacity 均无效）。
- 修复（本版本，按用户指定方向——先调查层级后实现，不再调 XAML）：
  - **F3-F1-A 世界原点重写**（editor_world_origin.frag）：去掉射线求交与贴地投影；改为世界原点 (0,0,0) 投影到屏幕后画**恒定屏幕尺寸**的细十字线 + 小空心圆 + 中心点（蓝灰描边 #718096、中心淡金褐点 #C18A55、十字半长 8px/圆环半径 5px≈10~16 DIP）；相机后方/屏幕外 discard；深度保持原点平面深度（实体近则自然遮挡）；不再随视角压扁、不与地平线混同；
  - **F3-F1-B 导航 Gizmo → Vulkan 屏幕空间 Overlay Pass**：新增 editor_nav_gizmo.vert/.frag + ShaderBytecode.NavGizmoVert/Frag；新增 VulkanClearFrameOwner.NavGizmo.cs（80B push：cameraRight/Up/Forward + 视口 + DPI + gizmo 参数 + hover 索引）；CreateFullscreenPass 增加 depthTest 参数（Gizmo 用 DepthTest=Off/DepthWrite=Off）；GridPipelineSet 增加 NavGizmo 管线；DrawPlan 恒以 NavigationGizmo 收尾（RenderDrawKind 新增）；右上角 12 DIP 边距 88 DIP 区域；中心球 #CDD6DF + 三轴（X #C18A55/Y #5F87A7/Z #A9B8C7）+ 六端点（背向 40% Alpha 小点、朝向 100% 大点带 X/Y/Z 标签）+ 深度排序 + hover 高亮；
  - **F3-F1-C 命中走原生指针流**：Avalonia ViewGizmo/ViewNavigationGizmo 控件删除（UiRoot 移除引用）；VulkanNativeHost.NavGizmo.cs 在 OnNativePointerMessage 中先判右上角区域（视口→Gizmo 局部坐标），端点点击 → StandardViewResolver 标准视角命令，中心球/空白拖动 → 复用 UiVm 相机会话 Orbit（4 DIP 阈值区分点击/拖动）；CaptureLost/取消正常结束；控件区域外不截获（实体 Picking/框选/变换 Gizmo 不受影响）；导航不进入 Dirty/Undo；
  - DPI 链路：RenderProjection 增加 ViewportDpiScale；UiVm.UpdateViewportDpi（LayoutSync 调用）；RenderCameraProjection 增加 Right 计算属性。
- 验证：聚焦 NavigationGizmo/StandardViewResolver/ViewportChrome/OverlayContract 33/33；Core 258/258、World 435/435、WarCore 22/22；arch-a-guard PASS（含 5+100）；git diff --check OK；glslc 字节码三文件逐字 MATCH。
- 视觉冒烟：**未执行**（沿用用户决定，留真机验收）；请重点复验：原点不再贴地压扁（十字+空心圆+中心点）、右上角 Gizmo 可见且随相机旋转、六方向点击/拖动、顶底视图无滚转。
- 治理：版本 v0.2.24.11-fix → v0.2.24.12-fix（五处同步）；无新增依赖/项目；不创建 Tag/Release。

## v0.2.24.11-fix
MAP-A-R1-D5-R1-F3 视口黑边移除 + Blender 风格导航 Gizmo（2026-08-03 15:20:00，Commit 本轮落库为准）
- F3 问题（用户验收反馈）：
  1. 视口外层存在黑色粗边框和厚重圆角（两层深色容器：VulkanViewport.axaml `#0b1220`/`#31405d` + UiRoot 中央 `#101827`/Padding=18/圆角8/BoxShadow）；
  2. 右上角仍是白色占位块（ViewGizmo.axaml 3×3 按钮 + `#dce6f2` 圆角卡片），缺少正式导航 Gizmo。
- 修复（本版本）：
  - **F3-D1 去黑边**：VulkanViewport 与 UiRoot 中央容器改为浅灰 1 DIP 分隔（`#C9D2DC`）、无圆角、无 Padding、无深色背景、无 BoxShadow；Fallback 层改浅色 `#E8EEF5`；ClipToBounds 保留；
  - **F3-D2 Blender 风格导航 Gizmo**：替换白色占位为透明 88×88 覆盖层（右上 12 DIP）——中心球（`#CDD6DF`/描边 `#718096`）+ 三根世界轴 + 六正负端点 + X/Y/Z 标签；玄域低饱和配色（X `#C18A55` 淡金褐、Y `#5F87A7` 蓝灰、Z `#A9B8C7` 浅钢灰）；背向端点 40% Alpha 小圆点、侧向 78%、朝向 100% 大端点带标签；按深度升序绘制（背向先、朝向后）；轴正对相机时端点收缩中心无 NaN；控件完全透明无底板；
  - **F3-D3 交互**：点击六端点 → 标准视角命令（+X/-X/+Y/-Y/顶/底视图，保留 Pivot 与距离，Up 合同：±X/±Y=+Z、顶/底=+Y 防滚转）；中心球/空白拖动 → 复用 UiVm 相机会话 Orbit（同一灵敏度/俯仰限制/Pivot）；点击/拖动阈值 4 DIP；Hover 亮环 + Hand 光标；PointerCaptureLost 正常取消；控件 88×88 外不截获输入（实体 Picking/框选/变换 Gizmo 不受影响）；导航不进入 Dirty/Undo/场景文件；
  - 拆分职责（5+100）：ViewNavigationGizmo.cs（属性状态）/ .Layout.cs（投影纯数学）/ .Render.cs（绘制）/ .HitTest.cs（命中）/ .Input.cs（输入命令）；StandardViewResolver.cs（六方向解析 + 端点名映射）；UiVm.NavigationCamera 快照（相机变化统一通知 Gizmo）。
- 验证：聚焦 NavigationGizmo/StandardViewResolver/ViewportChrome 29/29；Core 254/254、World 435/435、WarCore 22/22；arch-a-guard PASS（含 5+100）；git diff --check OK；XAML 加载由构建编译验证。
- 视觉冒烟：**未执行**（沿用上轮用户决定——冒烟留用户真机验收 F3 十一项清单）；请用户重点复验：黑边消失、白色卡片消失、Gizmo 六方向与网格一致、点击/拖动、顶底视图无滚转。
- 治理：版本 v0.2.24.10-fix → v0.2.24.11-fix（五处同步）；无新增第三方依赖/项目；不创建 Tag/Release。

## v0.2.24.10-fix
MAP-A-R1-D5-R1-F2-R3-R2 背景颜色移到片元级（2026-08-03 14:10:00，Commit 本轮落库为准）
- F2-R3-A3（v0.2.24.9-fix）：**PARTIAL FAIL——中性灰参考地面 FAIL**。现象：截图下半部分仍是偏蓝背景（比天空略暗），未形成明确中性灰地面；网格线宽统一与 LOD 缩放观感 PASS。
- 根因（用户验收确认，与开发记录一致）：
  1. 颜色计算在顶点着色器 `backgroundVertex()`——全屏三角形仅 3 个顶点计算视线方向并判断天空/地面，中间像素全靠插值，地平线与灰地被插值冲淡成整片蓝灰渐变；
  2. 两个 smoothstep 参数反写（`smoothstep(0.0, -0.06, dir.z)` 与 `smoothstep(-0.06, -0.5, -dir.z)`，edge0 > edge1 未定义行为），第二个还有符号错误——地面远近基本不变化；
  3. 自动测试只能证明"灰色数字写进 Shader 且可编译"，不能证明颜色出现在视口地面区域。
- 修复（本版本）：
  - **背景颜色移到片元级**：`scene.vert` backgroundVertex 只输出全屏三角形位置与 NDC（哨兵 (2,2) 表示非背景分支）；`scene.frag` 每像素用 flat 传入的 `vInvViewProjection` 重建世界视线（invVP 每顶点算一次避免每像素求逆），`dir.z >= 0` 画天空、`dir.z < 0` 画灰色参考地面；
  - **smoothstep 方向修正**：`belowHorizon = 1 - smoothstep(-0.06, 0.0, dir.z)`、`groundNearness = smoothstep(0.06, 0.50, -dir.z)`——全部 edge0 < edge1，地平线过渡与地面远近（远处灰 → 近处深灰）正确；
  - **配色拉开对比**（用户建议）：天空顶部 `#A6C0DF` → 天空近地平线 `#B3C6DA` → 地平线 `#9CA6AF` → 远处地面 `#858B91` → 近处地面 `#747A80`；
  - 太阳圆盘/辉光保留（D1 合同 sunDirection 不变）；背景仍不写深度、不进地图/场景/拾取/碰撞；
  - 未触碰：网格 Shader、线宽 0.82、1/2/5 LOD、世界轴、原点、DrawPlan、地图、地形、相机。
- 验证：聚焦 83/83；Core 225/225、World 435/435、WarCore 22/22；arch-a-guard PASS（含 5+100）；glslc SceneVert/SceneFrag 逐字 MATCH；git diff --check OK。
- 视觉冒烟：**未执行**（沿用上轮用户决定——冒烟留用户真机验收）；本轮修复点明确（顶点→片元 + smoothstep 方向），请用户按 F2-R3-A3 复验，重点：默认斜视（天空/灰地/地平线分离）、压低视角（地平线平滑、无硬切线）、有地图（地图覆盖灰地）。
- 治理：版本 v0.2.24.9-fix → v0.2.24.10-fix（五处同步）；无新增文件/依赖；不创建 Tag/Release。

## v0.2.24.9-fix
MAP-A-R1-D5-R1-F2-R3 网格线宽统一 + Unity 风格灰色参考地面（2026-08-03 13:05:00，Commit 本轮落库为准）
- F2-R2-A2（v0.2.24.8-fix）：**FAIL**。现象：缩放时 Fine/Coarse 使用不同线宽（0.70px vs 1.00px）且重合处直接相加，部分网格线看起来忽粗忽细、层级交界出现明暗脉冲；编辑器参考地面整体偏蓝，与天空层次不足。
- 修复（本版本）：
  - **R3-A 唯一像素线宽**：删除 FineWidthPixels/CoarseWidthPixels 双宽度，统一 `GRID_LINE_WIDTH_PX = 0.82`（硬合同 0.78~0.90，≤1.0；世界轴保持 1.25px > 网格）；
  - **R3-A 非累加合成**：`gridAlpha = max(fineContribution, coarseContribution)`（禁止 fine+coarse 相加 → 无双重 Alpha、无粗黑线）；颜色按贡献加权归一化混合（total 仅用于颜色，Alpha 仍为 max）；
  - **R3-A 配色收敛**：Fine `#5D6670` α0.16 / Coarse `#525C67` α0.24（差 0.08 ≤ 0.10，克制深浅差防"深色=更粗"错觉）；
  - **R3-B 中性灰参考地面**：scene.vert backgroundVertex 程序化背景扩展为 天空顶部 `#9DBBE0` → 天空近地平线 `#AEC4DC` → 地平线混合区 `#9DA5AD` → 远处地面 `#8B9299` → 近处地面 `#7B8289`；地平线过渡按视线方向 dir.z（[-0.06,0] 柔和混合），地面远近按 dir.z ∈ [-0.06,-0.5] 轻微渐变；不写深度、不进地图/场景/拾取/碰撞，地图与实体自然覆盖；
  - 未触碰：ReferenceGridScale 1/2/5 选级、48px 目标、相机求交、LOD 权重、方向性抗摩尔纹、深度偏移、世界轴/原点架构、地图/地形/光照。
- 验证：聚焦 ReferenceGrid/VisualStyle/ShaderContract/DrawPlan 82/82；Core 224/224、World 435/435、WarCore 22/22；arch-a-guard PASS（含 5+100，ShaderBytecode.Vert.cs 保持原 120 词/行紧凑格式 76 行）；glslc 字节码 GridFrag/SceneVert 逐字 MATCH；git diff --check OK。
- 视觉冒烟：**未执行**（用户选择跳过，图像待用户真机验收 F2-R3-A3 十项清单——不得视为 PASS）。
- 治理：版本 v0.2.24.8-fix → v0.2.24.9-fix（五处同步）；新增 ReferenceGridVisualStyleTests.cs；无新增第三方依赖/项目；不创建 Tag/Release。

## v0.2.24.8-fix
MAP-A-R1-D5-R1-F2-R2 统一网格尺度与轴线修复（2026-08-03 11:40:00，Commit 本轮落库为准）
- F2-A1（v0.2.24.7-fix）：**FAIL**。现象：逐屏幕位置 LOD 导致横向密度分区（近 0.1/中 1/远 10 单位并存）；近处摩尔纹与灰色叠块；世界轴出现楔形；网格 Shader 与独立 WorldAxes Pass 存在轴线重复绘制。
- 修复（本版本）：
  - 取消逐 Fragment LOD——每帧 CPU 由视口中心射线与 Z=0 求交（中心±1px 世界距离取 max）得参考世界每像素，整帧统一 Fine/Coarse 层级；
  - 1/2/5 十进制序列（0.01/0.02/0.05/0.1/0.2/0.5/1/2/5/10…），目标 48px/格，对数域相位 + smoothstep 互补交叉淡化（FineWeight+CoarseWeight≈1，边界旧 Coarse=新 Fine 无缝）；
  - 求交失败回退：中心 → 视口偏下 60% → 上一帧合法尺度（禁止重置为 1）；
  - 方向性抗摩尔纹：X/Y 各自按单元屏幕间距淡出（<6px 隐藏、6~12 渐入、>12 正常）；
  - 轴线单一事实源：网格 Shader 删除 X/Y 轴与原点绘制；新增独立 WorldAxes 全屏 Pass（金 X=世界 Y=0、蓝 Y=世界 X=0，各自方向导数固定 1.25px 屏幕宽度）与 WorldOrigin 全屏 Pass（琥珀原点标记 ≤4px 半径）；三个 Pass 开关（ShowGrid/ShowWorldAxes/ShowOrigin）完全独立；
  - 深度偏移有界化：clamp(fwidth(depth)×0.5, 1e-7, 2e-5)；
  - DrawPlan 顺序（方案 12）：背景 → 地形(MapBounds) → 网格 → 原点 → 世界轴 → 实体填充 → 轮廓 → Gizmo。
- 验证：聚焦 ReferenceGrid/WorldAxes/DrawPlan/Shader 合同测试 73/73；Core 215/215、World 435/435、WarCore 22/22；arch-a-guard PASS（含 5+100）；glslc 字节码四文件逐字一致；git diff --check OK。
- 视觉冒烟：**仅完成启动冒烟，图像待用户验收**（本环境无 computer_use 工具无法读取编辑器截图，按宪法不得写视觉 PASS）。启动冒烟实测：编辑器进程启动后存活 72.9s 无崩溃、无 Vulkan 会话回滚（三全屏 Pass 管线创建成功）；F2-A2 三张截图（默认斜视/拉近/拉远）待用户执行。
- 治理：版本 v0.2.24.7-fix → v0.2.24.8-fix（五处同步）；新增 ReferenceGridScale.cs（纯数学）、WorldAxes/WorldOrigin Shader + 字节码、GridPipelineSet.cs、GridScale.cs、ShaderContractTests/ReferenceGridScaleTests；无新增第三方依赖；不创建 Tag/Release。
- F2-A2 真机验收清单已交付（9 项：默认斜视/拉近/拉远/平移/环绕/独立开关/实体遮挡/有地图无地图/窗口尺寸），待用户执行。

## v0.2.24.7-fix
MAP-A-R1-D5-R1-F2 无限参考网格稳定性修复（2026-08-03 10:40:00，Commit 本轮落库为准）
- 任务目标：修复截图中"普通网格几乎不可见、只剩两条坐标轴"问题——稳定的缩放自适应层级、普通网格可见、远处无闪烁/地平线无噪声、有地图时网格不受地图边界裁剪；不修改天空/光照/地形/视角 Gizmo/地图编辑器/Schema。
- 根因（代码调查，非计划推测）：
  1. **线宽公式参数反转（主因）**：`gridLine` 内 `smoothstep(vec2(0.5), edge, f)` 中 `edge = 0.5 - d×linePixels/2 < 0.5`，edge0 > edge1 属 GLSL 未定义行为，实际线宽 = `1/d - linePixels` 像素——远处 d→0 时线宽爆炸为数十像素宽的淡带，近处趋近 0，普通网格视觉上消失；
  2. **层级目标间距 20px 过小**：desiredStep = worldMetersPerPixel×20，量化后细格屏幕间隔平均仅 ~8px，过密成噪声，且权重窗口 0.25~0.75 互补导致细格常被完全压掉（仅剩 0.18 基础 α）；
  3. **地图矩形内 discard**：F2A 为规避 Z-fighting 在 shader 内按地图矩形裁剪网格，有地图时视野内网格全部消失，违背"无限网格不受有限地图边界裁剪"；
  4. **坐标轴过强**：α0.78/2.5px 压过网格，且 X/Y 颜色与方案相反。
- 修复（editor_reference_grid.frag 重写 + DrawPlan 顺序调整）：
  - 线宽改用方案 4.6 标准公式 `1 - smoothstep(w-0.5, w+0.5, 像素距离)`：细 0.75px / 主 1.10px / 轴 1.35px，屏幕恒定不再随距离爆炸；
  - 目标间距 36px/格（`worldMetersPerPixel×36`，合法层级 0.1~10000 钳制）；细格权重 `1-smoothstep(0.5,1.0,phase)` 1→0、主格加深权重 `smoothstep(0.0,0.5,phase)` 0→1；
  - **跨级透明度连续**：主格线位置是细格子集，细格基础 α0.20 + 主格加深 α0.18，同组线跨级时从主格 0.18 平滑过渡为细格 0.20（差 ≤0.02），不跳格不闪烁；
  - **移除地图矩形 discard**：网格为无限参考平面，不再按地图裁剪；共面稳定改由 `gl_FragDepth = depth - max(fwidth(depth)×1.5, 1e-7)` 像素级深度偏移实现（实体/凸起地形仍正常遮挡，符合方案八）；
  - 配色按方案：细格 #566A82 α0.20、主格 #344A63（基础 0.20+加深 0.18）、X 轴 #AD8550 α0.62（世界 Y=0 线）、Y 轴 #557C9E α0.62（世界 X=0 线）、原点 #D1AE69 α0.70；坐标轴 1.35px 不再抢眼；
  - 掠射角淡出窗口 0.015~0.080（方案七）；距离淡出保持 0.45~0.75 far、gridMaxDistance=far×0.75（基于 far 约定，未硬编码米数）；
  - **DrawPlan 顺序修正**：网格从"天空之后"移到"地形/实体之后、轮廓/Gizmo 之前"（RenderDrawPlan.GetFrameDrawPlan），实体可遮挡网格、平坦地形上经深度偏移稳定显示；有/无地图、有/无实体均保留网格；
  - PushConstant 从 192B 缩为 160B（移除 mapParams/mapParams2，40 float），vert/frag/C# 三处同步，管线 maxPushConstantsSize 校验同步；
  - 相机相对坐标：本轮保持绝对 float32（与实体同机制，地图 2000m 量级内精度足够）；大尺度世界相机相对化属全局渲染原点架构问题，按方案九不强行扩围，另行登记。
- 测试：`ReferenceGridAdaptiveTests` 重写（×36 目标、两级相邻+权重区间、跨级 α 差 ≤0.02 连续性、phase=0.5 峰值 0.38、距离/掠射角曲线）；`ReferenceGridDrawPlanTests` 新增 4 组合（有/无地图×有/无实体）+顺序断言（实体后、Gizmo 前）+关闭开关缺席；`ViewportAssistDrawPlanTests`/`MapRenderDrawPlanTests` 顺序断言同步；Core 189/189、World 435/435、WarCore 22/22；arch-a-guard PASS；glslc 字节码逐字一致（GridVert 336、GridFrag 1379 词）。
- 治理：版本 v0.2.24.6-rz → v0.2.24.7-fix（五处同步）；无新增项目/依赖；不创建 Tag/Release。
- 状态：MAP-A-R1-D5-R1-F2 自动门禁全绿，真机验收待用户执行（IPO 清单见报告）；通过前不宣布 D5-R1 CLOSED，不进入 D5-R2。

## v0.2.24.6-rz
MAP-A-R1-D5-R1-F2/F2A Blender 风格自适应参考网格（2026-08-03 00:30:55，Commit 909b6fd 之后待收口）
- 任务目标：废弃 42 条世界空间粗四边形网格，改为独立全屏 Pass + 片元解析世界 Z=0 平面，实现 Blender 式无限自适应参考网格；只动网格，不处理 Gizmo/天空/取景。
- 独立渲染管线：新增 `editor_reference_grid.vert/.frag` + `VulkanGraphicsPipelineOwner.Grid.cs`（独立 192B PushConstant，创建时校验设备 maxPushConstantsSize；DepthTest=On/LessOrEqual、DepthWrite=Off、AlphaBlend=On）+ `VulkanClearFrameOwner.Grid.cs`（VP/InvVP/相机/视口/far/地图参数填充）；`RenderDrawKind.EditorGrid` → `EditorReferenceGrid`（顶点数 252→3 全屏三角形）；scene.vert 移除 gridVertex 与 -10.5 魔法分支；DrawAssist 不再处理网格。
- 自适应分级：`desiredStep = worldMetersPerPixel × 20`（合法层级 0.1/1/10/100/1000/10000，钳制 0.1~10000）；只混合相邻两个十进制层级，权重和=1，平滑交叉淡入（细格 1px α0.18、主格 2px α0.32）。
- 淡出：距离淡出 0~45% far 完整 / 45~75% 平滑 / >75% 隐藏；掠射角淡出 abs(dot(N,V))<0.03 隐藏 / 0.03~0.12 淡入 / >0.12 完整。
- 主轴与地图：X 轴（世界 Y=0，#5A7FA3 α0.78）、Y 轴（世界 X=0，#B68B54 α0.78）、原点标记（#D1AE69 α0.85），屏幕恒定 ~2.5px 贯穿可见平面；地图矩形内逐片元 discard（feather=像素×1.5 或 0.05），地图外网格继续显示，卸载后完整恢复。
- 配色（玄域浅色编辑器，禁高饱和/荧光/红绿工程轴）：细格 #7E8FA1 α0.18、主格 #607487 α0.32。
- 测试：`ReferenceGridRayIntersectionTests`（G1 射线求交 7 项）+ `ReferenceGridAdaptiveTests`（层级选择/权重和/钳制/淡出曲线/裁切，28 项）+ `ReferenceGridDrawPlanTests`（有/无地图网格存在+顺序）；Core 183/183、World 435/435、WarCore 22/22；arch-a-guard PASS；glslc 字节码逐字一致（GridVert 348、GridFrag 1559、scene.vert 7864 词）。
- 已知基线说明：909b6fd 的 scene.frag 源码（78 词透传版）与内嵌 ShaderBytecode.Frag.cs（113 词 F1 版）本身不一致（基线遗留，本轮未触碰 scene.frag/其字节码，超出 F2A 冻结范围）。
- 治理：版本 v0.2.24.5-rz → v0.2.24.6-rz（五处同步）；无新增项目/依赖；不创建 Tag/Release。
- 状态：MAP-A-R1-D5-R1 网格专项真机验收通过（用户授权收口 push）；后续 Gizmo/天空/取景按 F2 纪律单独轮次处理。

## v0.2.24.5-rz
MAP-A-R1-D5-R1 视口参照与导航（2026-08-02 22:57:00，Commit 2fdf470 之后待收口）
- 任务目标：按用户最新真机裁定修正视口参照与导航——视觉无限参考网格、地图外网格延伸、右上角视角 Gizmo 真实可见、程序化天空渐变、自动取景屏幕占用率 65~75%。
- 视觉无限 EditorReferenceGrid：`scene.vert gridVertex` 重构——网格重心跟随相机（worldPosition.xy 对齐间距）、间距按相机高度分级 0.1/1/10/100/1000/10000 米、线长覆盖 step×12、主次线分级宽度；`RenderDrawPlan` 取消 HasMap 时移除 EditorGrid（地图存在时网格保留，地图矩形由 shader 裁切避免穿透地表与 Z-Fighting，卸载后网格继续存在）；`VulkanClearFrameOwner.DrawAssist` EditorGrid 分支传相机位置 + 地图半宽/半深（entityScale.xy 复用，push constant 128B 不扩容）。
- 视角 Gizmo 真实可见：根因是 ViewGizmo 位于 VulkanViewport Grid 内被嵌入 Win32 原生窗口遮挡——移至 `UiRoot.axaml` 视口 Border 外层 Grid（Avalonia 覆盖层，位于原生渲染窗口之上），六方向按钮 + 当前朝向琥珀描边。
- 程序化天空增强：天顶饱和蓝 (0.22,0.45,0.85) → 地平线更雾白 (0.88,0.92,0.97)，pow 0.55→0.35 渐变更快集中；仍为独立 Sky Pipeline（DepthTest/Write=Off、Z-Up 读 dir.z、只依赖相机旋转）。
- 地图自动取景：`FrameMapAllWithCenter` 改为按目标屏幕占用率（垂直投影约 70%，透视补偿 ×1.55，实测 d≈2850 时占用率≈69%、最大视锥角 28.5°<30°）计算距离，地图不再过小；新增 `WorldCameraFramingOccupancyTests`（NDC 投影包围盒 65%~80%）。
- 世界坐标轴颜色：X=浅蓝灰 (0.55,0.62,0.70)、Y=冷钢蓝 (0.42,0.52,0.64)、Z=柔和琥珀 (0.78,0.66,0.42)，禁止高饱和红绿轴。
- ShaderBytecode：glslc -O 重新生成（8762 词，83 行）逐字比对一致。
- 测试：`WorldCameraFramingOccupancyTests` 新增（占用率 65~75%）；`MapRenderDrawPlanTests.With_map_grid_kept_and_bounds_added` 更新（D5-R1 需求变更：网格保留而非移除）；World 435/435、Core 148/148、WarCore 22/22；arch-a-guard PASS。
- 治理：版本 v0.2.24.4-rz → v0.2.24.5-rz（五处同步）；无新增项目/依赖；不创建 Tag/Release。
- 状态：MAP-A-R1-D5-R1 真机人工验收待用户执行；通过后进入 D5-R2 真实参数编辑。

## v0.2.24.4-rz
MAP-A-R1 D4 视觉收口 + D5 正式地图编辑器/场景引用（2026-08-02 22:32:36，Commit 5fcd02b 之后待收口）
- 任务目标：把 D4 真机视觉缺陷收口（程序化天空、视角 Gizmo、正式地图编辑器、场景地图引用），完成 MAP-A-R1 功能闭环；D4/D5 各轮独立提交推送。
- EDITOR-VIEW-R1 视角 Gizmo：`UiVm.ViewGizmo.cs` + `ViewGizmo.axaml`——视口右上角 3×3 网格六方向按钮（顶/底/前/后/左/右）+ 中心当前朝向琥珀描边；Z-Up 坐标合同冻结（顶=+Z 看向 -Z、前=-Y 看向 +Y 等）；保持观察中心（选中实体→地图中心→原点）与距离只改朝向；浅蓝灰主体、白字，无红绿蓝三轴配色；不建第二套 CameraState；测试 3 项（六方向朝向/中心距离保持/选择保持）。
- D5-A 正式地图编辑器：右侧一级「地图编辑器」Tab（与检查器平级）——地图资产区（名称/路径/MapId/尺寸/状态）+ 新建/打开/保存/卸载/聚焦五命令；复用 D2 MapDocumentOwner/MapStorageService 与 D3 WorldMapStateOwner；打开失败保持原地图；第二排「加载测试地图/卸载地图」临时按钮已删除；测试 4 项。
- D5-B 场景地图引用：`.xyscene` schema v3→v4 新增可选 `mapReference{mapId, assetPath}`（只存引用不复制地图数据）；旧场景无引用正常打开；缺失/损坏时场景主体打开 + 显示「引用失效」+ 路径原因 + 不自动建默认地图；保存附加引用、打开自动加载；测试 4 项 + schema v4 断言更新。
- 验证结果（D6 最终门禁）：全解决方案强制重编译 0 error / 1 既有 warning（xUnit2013，非本轮引入）；Core 148/148、World 434/434、WarCore 22/22；arch-a-guard PASS；glslc 字节码 8293 词逐字一致；git diff --check PASS；5+100 本轮文件全过（3 个既有超限文件非本轮范围，守卫口径 PASS）。
- 治理：版本 v0.2.24.3-rz → v0.2.24.4-rz（五处同步：changelog/file-tree/UiVm.SceneDocument.cs/UiWin.axaml/run.bat）；无新增项目/依赖；不创建 Tag/Release。
- 状态：MAP-A-R1-D4/D5 真机人工验收待用户执行（IPO 清单见报告）；全部通过后 MAP-A-R1 CLOSED，进入 MAP-A-R2 区域与图层。

## v0.2.24.3-rz
MAP-A-R1-D4 有限地表渲染与自动取景（2026-08-02 21:46:52，Commit 9d1f2c9 之后待收口）
- 任务目标：让地图以可观察、可编辑的战场方式出现在视口——有限地表网格、缓丘明暗、程序化天空、地图边界、加载后斜上方自动取景；D4 真机修复收口。
- D4 主体（基线 9d1f2c9 已含）：`MapTerrainMeshBuilder`（唯一采样源 MapSurfaceSampler 的渲染侧消费方，4225 顶点/24576 索引，CPU 数值差分法线 + 预计算亮度）、`MapBoundsMeshBuilder`（48 顶点琥珀色边界线）、`RenderDrawPlan` 地图绘制（EditorBackground 天空 → WorldOrigin/Axes → MapBounds 地形+边界 → EntityFill → Gizmo，HasMap 时移除 EditorGrid）、`RenderProjection.Map` 携带 `MapRenderSnapshot` 传播链、shader kind=-14 地表 / -15 边界分支、F1 临时加载/卸载按钮、F2 绘制顺序修复（地表在天空之后）。
- F3 真机修复（本轮）：Lambert 方向语义与 D1 合同对齐——`sunDirection` = 指向光源方向（Z>0 朝上），`MapTerrainMeshBuilder.Brightness` 不再取反（修复前 toLight 指向地面下方，平面 ndl=-0.75→0，地表只剩环境光 0.35，视觉为灰蒙暗绿）；`WorldMapState` 默认 SunDirectionZ 同步 +0.75；`MapRenderSnapshot`/`MapDocumentWorldBridge` 注释同步合同语义。
- F4 可读性（本轮）：`EditorCameraFraming.FrameMapAllWithCenter` 地图取景 45° 斜上方俯视（Forward.Z=-0.707，完整容纳四角 + 安全边距）；`Brightness` 合成降为 `ambient×0.3×hemi + sun×0.85×ndl`（clamp [0,1]），避免全部顶点被 shader 钳制同色，缓丘受光/背光差 ≈0.086 肉眼可辨；scene.vert 天空顶部加深蓝 (0.45,0.56,0.74)、地平线更雾白 (0.88,0.90,0.94)，ShaderBytecode 由 glslc -O 重新生成并逐字比对。
- 测试（XuanYu.World.Tests/Map/ 与 /World/）：`MapTerrainBrightnessTests` 新增（Flat 亮度稳定∈[0.5,0.9]、缓丘明暗差>0.03、方向光贡献>0.05）；`WorldCameraFramingTests` 新增（45° 俯视 + 四角完整容纳）；`MapTerrainMeshBuilderTests` 亮度断言按 F4 合成公式更新。
- 治理：版本 v0.2.24.2-rz → v0.2.24.3-rz（五处同步：changelog/file-tree/UiVm.SceneDocument.cs/UiWin.axaml/run.bat）；无新增项目/依赖；ShaderBytecode 为生成物，行数 78（≤100 守卫口径通过）；第二排「加载测试地图/卸载地图」为 D4 临时验收入口，D5 移入右侧「地图编辑器」一级模块。
- F5 程序化天空（用户真机截图裁定：D4 视觉验收 FAIL 后追加，D4 保持 IN PROGRESS，15c9a0e 保留不回滚）：重建 Unity/Godot 风格程序化天空——天顶清晰蓝 (0.28,0.50,0.85) → 地平线浅蓝雾白 (0.78,0.87,0.96) → 地平线以下轻微大气泛光 (0.42,0.48,0.56)；上半球渐变改用 pow(dir.z, 0.55) 集中；新增最小太阳圆盘（方向与 D1 合同 sunDirection 一致，仅圆盘+微弱辉光，无耀斑/体积光）；ClearColor 改为浅蓝失败回退 (0.35,0.55,0.80)，不再用灰色掩盖天空失败；天空失败日志保留（ShaderModule/PipelineLayout/GraphicsPipelines 三处明确记录）；绘制仍为独立 Sky Pipeline（DepthTest/Write=Off、先于地表、只依赖相机旋转不依赖平移，Z-Up 读 dir.z）。
- 验证结果（F5 追加）：串行 build 12 项目 0 error / 1 warning（既有 xUnit2013）；Core 148/148、World 423/423、WarCore 22/22；arch-a-guard PASS；glslc 重新生成 ShaderBytecode（8293 词，79 行）逐字比对一致，新天空色与太阳常量全部在字节码中。
- D5-A 正式地图编辑器（用户真机裁定后追加，独立轮次）：右侧一级模块新增「地图编辑器」Tab（检查器之后，与检查器平级）——地图资产区（名称/路径/MapId/尺寸/状态：未加载/已保存/未保存）+ 新建/打开/保存/卸载/聚焦五命令；新建默认 TestBattlefield 2000×2000，复用 D2 MapDocumentOwner/MapStorageService（候选加载+原子保存）与 D3 WorldMapStateOwner，无第二套系统；打开失败保持原地图不变；第二排「加载测试地图/卸载地图」临时按钮已删除；基础地表/环境编辑组留 D5 后续补齐。新增 UiVm.MapEditor.cs（文档状态+命令，100 行）、UiWin.MapCommands.cs（.xymap 文件选择器）、MapEditorPanel.axaml（面板，Right.axaml 引用）；测试 UiMapEditorTests 4 项（新建入 World+Dirty、保存/打开 Round-trip、卸载清空、打开失败不污染）。
- 验证结果（D5-A 追加）：串行 build 12 项目 0 error；Core 148/148、World 430/430（含 D5-A 新增 4 项）、WarCore 22/22；arch-a-guard PASS（5+100 全过）；git diff --check PASS。
- D5-B 场景地图引用（独立轮次）：`.xyscene` schema v3 → v4，新增可选 `mapReference{mapId, assetPath}`（只存 mapId + 相对场景目录路径，不复制地图尺寸/地表/环境参数）；旧场景无 mapReference 正常打开；Validator 校验 mapId 合法 + 路径安全（非法拒绝），无效引用场景主体仍打开、地图编辑器显示「引用失效」+ 路径原因、不自动创建默认地图。新增 MapReference.cs、SceneDocumentValidator.MapReference.cs（校验拆分，Validator 保持 100 行）；SceneDocumentJson/Mapper/Snapshot 双向映射；UiVm.SceneDocumentMapRef.cs（保存附加引用 + 打开解析加载）；测试 SceneMapReferenceTests 4 项（保存携带/打开恢复/旧场景兼容/缺失失效）。
- 验证结果（D5-B 追加）：串行 build 12 项目 0 error；Core 148/148、World 434/434（含 D5-B 新增 4 项 + schema v4 断言更新 3 处）、WarCore 22/22；arch-a-guard PASS；git diff --check PASS。
- 状态：MAP-A-R1-D4 真机人工验收待用户执行（IPO 清单见报告）；验收通过后 D4 CLOSED，进入 MAP-A-R1-D5 正式地图编辑器与场景引用。

## v0.2.24.2-rz
MAP-A-R1-D3 World 地表能力（2026-08-02 18:24:41）
- 任务目标：把地图文档转化为 World 可查询的确定性地表能力——有限边界、唯一地表采样器、世界 X/Y → 地表 Z、加载/切换/卸载、最小渲染快照；本轮不渲染、不做 UI 与场景引用。
- 新增 `XuanYu.Core/Map/`：`MapSurfaceKind`（Flat/GentleHillsV1）、`MapSurfaceSampler`（唯一采样源：Flat 固定高度；GentleHillsV1 双正交正弦叠加，相位由 seed 固定派生，输出 [base−amp, base+amp]，纯算术确定性）、`MapRenderSnapshot`（供 D4 Render 消费的最小快照：尺寸+地表参数+MapId，卸载后 Empty）。
- 新增 `XuanYu.World/Map/`：`WorldMapState`（纯数据+有限边界判断+高度查询；世界 X 横向/Y 纵向/Z 高度，Z-Up 直写无映射层；闭区间边界，边界点属于地图；地图外不钳制不返回虚假零高度）、`WorldMapStateOwner`（当前地图状态：Load/Unload/Switch、TryGetSurfaceHeight(X,Y,out Z)、BuildRenderSnapshot）。
- 桥接：`XuanYu.Editor/MapDocument/MapDocumentWorldBridge.ToWorldState`（MapDocument → WorldMapState，字符串 kind → 枚举映射，对齐 SceneDocumentWorldBridge 模式）。
- 测试（XuanYu.World.Tests/Map/，新增 4 文件 32 项）：Flat/GentleHills 确定性（同坐标多次一致、200 点扫描）、幅度范围、seed/位置差异；边界闭区间（中心/四边/角在内，外 0.001 米拒绝）；Owner 加载/切换/卸载/快照清空不残留；桥接字段完整与端到端查询一致。
- 治理：版本 v0.2.24.1-rz → v0.2.24.2-rz（五处同步）；无新增项目/依赖；Core 新增纯数学 Map 类型（非 Scene/World/Picking/Gizmo 禁区）；World → Core 仅、Editor 桥接不反向依赖。
- 验证结果：串行 build 12 项目 0 error / 1 warning（既有 xUnit2013）；Core Tests 145/145；World Tests 411/411（含地图新增 32 项）；WarCore Tests 22/22；arch-a-guard PASS；glslc PASS；git diff --check PASS；5+100 全仓扫描 PASS（守卫口径与 wc 均 ≤100）。
- 状态：MAP-A-R1-D3 完成（无 UI/视口，验收以自动测试为准），等待批准后进入 MAP-A-R1-D4 有限地表、天空和光照。

## v0.2.24.1-rz
MAP-A-R1-D2 .xymap 地图存储闭环（2026-08-02 18:15:25）
- 任务目标：地图资产可靠创建、严格校验、保存、关闭并重新读取；本轮不渲染、不查询、不做 UI 与场景引用。
- 新增 `XuanYu.Editor/MapDocument/`：`MapDocument`（SchemaVersion/MapId/Name/SizeMeters/CoordinateSystem/Surface/Environment/LayerReferences）、`MapId`（32 位十六进制，创建后稳定）、`MapSize`/`MapCoordinateSystem`/`MapSurfaceDefinition`/`MapEnvironmentDefinition`/`MapVector3` 值对象、`MapDocumentValidator`（结构化 Issue 校验：尺寸 100–10000、坐标 Z-Up 米制零原点、地表仅 Flat/GentleHillsV1、环境参数有限非负、layerReferences 必须为空、未知类型拒绝）、`MapDocumentResult<T>`（对齐 SceneDocumentResult 模式）。
- 存储闭环：`MapJsonSerializer`（严格 JSON：字段大小写敏感 + 未知字段拒绝 + JsonPropertyName 固定 camelCase + JsonPropertyOrder 确定性输出 + UTF-8）、`MapJsonMapper`、`MapStorageService`（候选加载=解析→验证→成功才返回；原子保存=同目录临时文件→完整写入→File.Move 替换→失败清理并保留旧文件）、`MapDocumentOwner`（CurrentMap/CurrentPath/IsDirty 最小状态机：New→Dirty、Load→Clean、Modify→Dirty、Save→Clean、Unload→清空；失败不污染）。
- 路径合同：`Maps/<MapName>/map.xymap`；不存绝对路径；目录按需创建。D1 合同修正：`mapId` 口径更新为纯 32 位十六进制（无 `map_` 前缀，D2 §5.2 明确），docs/map-a-r1-d1-map-contracts.md 已同步。
- 测试（XuanYu.World.Tests/Map/，新增 9 文件）：MapId 格式/稳定性、尺寸边界与拒绝、坐标合同、地表/环境参数、图层引用空约束、JSON Round-trip 与确定性、大小写/未知字段/类型/损坏拒绝、候选加载失败不污染、原子保存与临时文件清理、Owner 状态链闭环。
- 治理：版本 v0.2.24.0-rz → v0.2.24.1-rz（五处同步）；无新增项目/依赖；不触碰 SceneDocument、WarCore、渲染与 UI。
- 验证结果：串行 build 12 项目 0 error / 1 warning（既有 xUnit2013）；Core Tests 145/145；World Tests 379/379（含地图新增 65 项）；WarCore Tests 22/22；arch-a-guard PASS；glslc PASS；git diff --check PASS；5+100 全仓扫描 PASS（守卫口径与 wc 均 ≤100）。
- 文件级验收（临时目录真实文件）：首次保存→重新读取 Round-trip 全字段一致（mapId/尺寸/坐标/地表/环境）；损坏 JSON 拒绝且不替换；保存失败无临时文件残留、不破坏旧文件。
- 状态：MAP-A-R1-D2 完成（无 UI/视口，验收以自动测试 + 真实文件检查为准），等待批准后进入 MAP-A-R1-D3 World 地表能力。

## v0.2.24.0-rz
MAP-A-R1-D1 地图合同冻结（2026-08-02 17:42:55）
- 任务目标：只读核查现有 SceneDocument / World Snapshot / 渲染地面 / 右侧模块结构后，冻结 `.xymap` 第一版 Schema 与 `.xyscene` mapReference 合同；本轮零产品代码，不重构旧代码。
- 坐标裁定（用户拍板，方案 B）：`.xymap` 语义与世界轴直写——X 横向（世界 X）、Z 高度（世界 Z=Up）、Y 纵向（世界 Y），与官方坐标合同 WORLD-A-R0（Z-Up、XY 水平）一致；不引入映射层；查询合同为「输入世界 X/Y 水平面坐标 → 输出地表 Z 高度」。
- 合同冻结（docs/map-a-r1-d1-map-contracts.md）：`.xymap` schemaVersion=1，mapId=`map_`+32hex，尺寸 100–10000 米，surface 仅 Flat/GentleHillsV1（确定性采样），environment 仅 ClearDayV1 + 方向光/环境光；保存路径 `Maps/<Name>/map.xymap`，原子替换，候选完整验证；`.xyscene` 升 v4 增可选 `mapReference{mapId, assetPath}`（项目相对路径，场景不复制地图数据），旧场景兼容，引用缺失明确报「引用失效」。
- 核查事实：无限灰网格=RenderDrawKind.EditorGrid（252 顶点，scene.vert gridVertex，±10 米 21×21 线，z=0 平面）；天空=EditorBackground+深度不写第二管线（WORLD-D 成品，直接复用）；光照=shader 硬编码固定方向光+半球环境光；右侧模块 Right.axaml=检查器/调试/偏好/模式四 Tab（MAP-A 收为检查器+地图编辑器）；全库无任何地图类型；版本源五处一致。
- 治理：新里程碑 MAP-A（模块 24），新分支 feat/MAP-A-map；版本 v0.2.23.0-rz → v0.2.24.0-rz（五处同步）；基线 HEAD cbb694b = origin tip，ahead/behind 0/0；已知偏差 untracked `IDEA.md` 与残留 `XuanYu.Editor.Avalonia/` bin 目录未处理。
- 状态：MAP-A-R1-D1 合同冻结完成，等待批准后进入 D1 域类型编码（MapId/MapDocument/MapSurfaceDefinition/字段验证）。
