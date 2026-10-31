# 2026-10-10～31 XYK 正式补录审计（R1）

**Owner**：ChatGPT；**分支**：xyk/main；**性质**：知识资产写回（无产品源码修改、无 Build/Restore/Test）。所有日期按北京时间。
**溯源层级**：远端 Commit SHA=已上传某分支、非必然 main/已合并；用户/Agent 报告=工程主张、未验证的动态结果不升格；产品 Commit 不明记 -。

## 已处理：逐条去重
| Action | 条目 | 领域 | 裁定说明 |
|---|---|---|---|
| CREATE | K-GOV-007 / 008 / 009 / 010 | 工程治理 | 工作区白名单、有界构建、XYK 强制审计、分层 Gate |
| CREATE | K-ARCH-003 | 架构 | 严格三级、5+100、test_ 与依赖方向 |
| CREATE | K-WORLD-001 | World | 变更 Candidate 隔离与父子不变量 |
| CREATE | K-GEO-004 | Terrain/Camera | raw DEM/显示 LOD 与观察中心 |
| CREATE | K-REN-006 / 007 | Rendering | 多帧资源归属与性能证据层 |
| CREATE | K-UI-003 | XYUI | 编号索引与 Typography 不变量 |
| CREATE | EXP-GOVERNANCE-006 | Agent 防复发 | 越界 STOP |
| CREATE | L-GOV-001 / 002 | Lessons | 磁盘事故、静态迁移不等于功能 |
| CREATE | ERR-20261026-001 | Incident | T0+ 工作区事故 |
| CREATE | DEC-XYE-ARCH-5X5X5-R1 | Decision | 用户批准目录与命名 |
| STRENGTHEN | K-GOV-005, K-VAL-001/002, K-XYT-EXEC-001 | Governance/Tests | 候选身份、运行测试归因 |
| STRENGTHEN | K-GEO-003, K-REN-005, K-INP-001, K-UI-002 | Terrain/Render/Input | 失败显式性与真机层 |
| STRENGTHEN | EXP-GOVERNANCE-005, EXP-TEST-001 | Agent | Lane、真机证据等级 |

## 当期已审但不得误报为正式完成
| 主题 | 裁定 | 证据/剩余缺口 |
|---|---|---|
| Renderer M2 10/21～23 | NONE（额外产品 CLOSED 主张） | [6dd62073](https://github.com/Heaifan/XuanYu-Engine/commit/6dd62073e0c91a925000c6297d873020dd0df36c) 存在；但 A1/B1 Arch-A/DEM fixture、A2 GPU 双帧、C Editor 采样与后续本地 Tree 须另核对 |
| M1/GPU Present | NONE（全帧性能达标） | 1419 对 Present 的样本不能替代 GPU P50/P95、Shader/动态地形、输入可见延迟 |
| Core 10/24～25 | NONE（每个文件改名新建卡） | [84d49d43](https://github.com/Heaifan/XuanYu-Engine/commit/84d49d437d6a59060c18d37fc72b2d4594fb07ac) 与 [bfaa3342](https://github.com/Heaifan/XuanYu-Engine/commit/bfaa33425abd51325544c743168c764805376a83)；知识已归并 DEC/K-ARCH-003 |
| Vulkan 10/25～28 | NONE（总体验收通过） | 静态审计/局部测试不消除 R1-01、R3-04、SES-03 PARTIAL；Shader 源码—字节码必须与 Tree 配对 |
| World R1/R2 10/28～30 | NONE（整体 CLOSED） | [b8ae01f9](https://github.com/Heaifan/XuanYu-Engine/commit/b8ae01f92f942074c81a1da70c2fc8220c3870b5) 仅证明 R2-A；World R1 静态整理 104/104 不代替 Integration |
| WarCore R1 10/29 | NONE（新的知识卡和远端产品 PASS） | 未定义 UnitKind 的拒绝已报告，27/27，用户给出的本地 957e021c 尚未在本轮核对对应远端 |
| XYUI R2 10/30 | NONE（最终 Git 发布完成） | 用户认可 Gallery 视觉及 708/708；最后一批报告未暂存/推送，不能假称 remote |
| Editor R1 10/30～31 | NONE（全模块完成） | MapEditSession、MapDocument 保存/回滚、构建门禁与静态路径迁移不能混认 |
| T0+ 磁盘 10/26 | UPDATE（并入统一事故事实） | 事故 R2 CLOSED，产品构建/上传状态不因此变成 CLOSED |
| 10/28 Native Input P3 | NONE（运行验收） | 实际 HWND、OS Message、Owner/Capture/Consumer 与产品产物身份未提供真机 PASS |
| Dynamic Terrain SSE | NONE（实机性能结论） | CPU Select 和 Conservative Frustum 建议，不代表 Vulkan 真实画面/性能 Gate |
| Linux/通用 Agent | NONE（功能实现） | 仍是目标和设计评审，不是已交付 Linux 控制接口 |
| Blender 花瓶工坊 | REJECT（非 XYK 的 XYE/XYUI 范围） | 不混入本仓工程治理知识 |

## 审计限制
10/10～31 的部分原始 Codex 报告和本地 Dirty 没有可验证的远端产品 SHA。本轮已覆盖**当前可取得的主要决策、事故和工程报告**，但不声称完成了每份未上传报告、每个本地文件的逐字复核；后续补证必须对既有 ID UPDATE/STRENGTHEN，并补记事件，不得重复 CREATE。
