# 玄域引擎知识索引

> 最后治理更新：2026-10-31
> 使用方法：先按任务域定位必须读取项，再读取对应正文；禁止默认把全部知识无差别塞入实现上下文。

## 任务域预检映射

MEDIUM / HIGH 任务，以及下表已登记任务域，开始设计或写入前执行 Knowledge Preflight。

Knowledge Preflight 有两个 Phase：`PLANNING` 与 `EXECUTION`。如果计划或实现与 ACTIVE P0 Knowledge 冲突，必须 `STOP`；只有显式 Governance Supersede、更新原条目状态并提供替代证据后才能继续。无匹配时输出 `Loaded: none`，不得为了形式制造无关知识。

| 任务域 | 典型触发 | 优先读取 |
|---|---|---|
| 通用验证 / 交付 | Build、测试、真机、产物、Git 基线 | K-VAL-001～K-VAL-003、K-GOV-001～K-GOV-003；EXP-GOVERNANCE-001；再检查其他相关 ACTIVE EXP |
| 架构 / 状态所有权 | 分层、Composition Root、Workspace、事实源 | K-ARCH-001、K-ARCH-002；EXP-ARCH-001；`decisions/` 中相关 DEC；其他相关 ACTIVE EXP |
| 空间 / 几何 | Camera、Screen↔World、Geometry、Snap、Topology | K-SPA-001、K-SPA-002、K-GEO-001、K-GEO-002；相关 DEC / EXP |
| Rendering / Native | Overlay、Depth、Grid、NativeHost、Vulkan、Shader、Texture、GPU Text、Vector Stroke | K-REN-001～K-REN-005、K-NATIVE-001～K-NATIVE-002、L-REN-001～L-REN-003、L-NATIVE-001；EXP-ARCH-001；其他相关 ACTIVE EXP |
| Input | Pointer、Capture、手势 Owner、真实生产输入接线、平台输入 | K-INP-001～K-INP-004；EXP-TEST-001；其他相关 ACTIVE EXP |
| UI / Inspector | Layout、Measure/Arrange、Inspector、冻结交互、稳定属性编辑目标、Diagnostic | K-UI-001、K-DIAG-001、K-VAL-003（Headless/Runtime UI 时）；相关 `decisions/`；EXP-UI-001、EXP-UI-002、EXP-TEST-001；其他 UI 类 ACTIVE EXP |
| Data / Save / Asset | 保存、加载、覆盖、资源归一化、异步确认 | K-DATA-001～K-DATA-003、K-ASSET-001、K-ASSET-002；DATA 类 ACTIVE EXP |
| Performance | Preview、Commit、高频路径 | K-PERF-001；相关 ACTIVE EXP |
| Global Migration / Refactor | Reverse-Z、Projection、Renderer、Depth Policy、Input Architecture、Persistence Migration、Workspace Architecture | 相关领域 K/L；EXP-GOVERNANCE-002；EXP-ARCH-001 |
| Agent 历史错误 | 当前任务命中已知错误模式 | `xyk/incidents/agent-error-log.md` + `xyk/experience/rules.md` 中命中的 ACTIVE EXP |
| Diagnostic / Viewport | Diagnostic、Viewport、NativeControlHost、Vulkan、Popup、Pointer、Capture、Input Router | K-VAL-001、K-VAL-002、K-NATIVE-001、K-NATIVE-002、K-INP-001～K-INP-004、K-DIAG-001、K-GOV-003、L-VAL-001、L-NATIVE-001、L-TEST-001；EXP-ARCH-001、EXP-TEST-001、EXP-UI-002、EXP-GOVERNANCE-001 |

任务若横跨多个域，只加载与当前 Scope 直接相关条目，不机械全文读取。

---

## 当前知识条目

| ID | 类型 | 分类 | 标题 | 优先级 | 证据 | 首次关键证据 | 状态 |
|---|---|---|---|---|---|---|---|
| K-VAL-001 | Knowledge | Engineering | 用户运行产物必须与验证产物一致 | P0 | E1 | v0.2.25.18-stab · 2026-08-10 16:51:42 · 06b26e9 | Active |
| K-VAL-002 | Knowledge | Engineering | UI/Native 功能必须分层验收 | P0 | E2 | v0.2.24.50-fix · 2026-08-09 19:42:41 · 60fd339 | Active |
| K-VAL-003 | Knowledge | Engineering | Headless Runtime UI 必须单一生命周期并进入真实 Visual Tree | P0 | E2 | XYE-C0-LAST-MILE-FINAL-R1 · 2026-09-28 · 516c83f9 | Active |
| L-VAL-001 | Lesson | Engineering | 修复存在但真机完全不变时先证明运行时实际路由 | P0 | E1 | MAP-DATA-A-R2-F2-F2-F1 · 2026-08-12 · 3d53de0 | Active |
| L-NATIVE-001 | Lesson | Rendering | Native/Avalonia UI 问题连续两次局部 Placement 修复失败后审查 Airspace 与 Ownership | P0 | E2 | Diagnostic + Viewport R1 · 2026-09-25 | Active |
| L-TEST-001 | Lesson | Engineering | 测试假绿与迁移路径链验证（Test、AXAML、MSBuild Manifest） | P0 | E2 | Region Snap R1 · 2026-09-25；Editor BUILD-01-R2-001 / BUILD-02 · 2026-10-31 | Active |
| K-GOV-001 | Knowledge | Engineering | 历史唯一身份以 Commit Hash 为准 | P0 | E2 | SHR-2026-08-R2 · 涉及 3 组重复版本 | Active |
| K-GOV-002 | Knowledge | Engineering | 治理成果必须建立自动防回潮门禁 | P1 | E3 | 8.8-0 · 2026-06-23 23:09:45 · 4c4d82c | Active |
| K-SPA-001 | Knowledge | Architecture | 大地图 Screen↔World CPU 链使用双精度并做往返验证 | P0 | E2 | v0.2.25.12-rz · 2026-08-10 12:20:03 · 0594c4c | Active |
| K-SPA-002 | Knowledge | Architecture | 斜视 Metric 具有方向性，失败保持上一合法状态 | P1 | E1 | v0.2.25.17-stab · 2026-08-10 · c307c66 | Active |
| K-ARCH-001 | Knowledge | Architecture | Composition Root 初始化顺序属于依赖合同 | P0 | E1 | v0.1.7.1-fix · 2026-06-24 11:45 · 359e3ce | Active |
| K-ARCH-002 | Knowledge | Architecture | 产品模式持续膨胀时先建立 Workspace 边界 | P1 | E1 | MAP-A → EDITOR-A · 2026-08-11 · 6724079 | Active |
| K-REN-001 | Knowledge | Rendering | Editor Overlay 不得用世界坐标偏移制造视觉层级 | P0 | E2 | v0.2.25.13-rz · 2026-08-10 13:37:23 · ef12f4b | Active |
| K-REN-002 | Knowledge | Rendering | 共面 Overlay 应由独立 Depth Policy 与 Draw Order 表达 | P0 | E2 | v0.2.25.15-stab · 2026-08-10 14:22:43 · 751da52 | Active |
| K-REN-003 | Knowledge | Rendering | Background / Sky 必须具有明确且独立的 Depth 语义 | P0 | E2 | v0.2.21.21-fix · 2026-08-01 16:56:53 · e0a994a | Active |
| K-REN-004 | Knowledge | Rendering | Editor World Reference Grid 必须独立于 MapGround | P0 | E3 | v0.2.25.28-fix → .29-fix · 2c57893 / 6154078 | Active |
| K-REN-005 | Knowledge | Rendering | 图形功能必须以语义正确的最终像素和 Runtime Visual Gate 收口 | P0 | E1 | MAP-VECTOR-VISUAL-R1 · 2026-09-26 · 0212fea4→ed3e0df5 | Active |
| L-REN-003 | Lesson | Rendering | Contract PASS 但画面错误时必须验证几何与 Coverage 的语义方向 | P0 | E1 | MAP-VECTOR-VISUAL-R1 · 2026-09-26 | Active |
| K-NATIVE-001 | Knowledge | Rendering | Native Overlay 必须验证真实 HWND 层级与绘制状态 | P0 | E2 | v0.2.25.18-stab · 2026-08-10 16:51:42 · 06b26e9 | Active |
| K-NATIVE-002 | Knowledge | Architecture | Native↔Avalonia 坐标必须显式跨空间转换 | P0 | E2 | Diagnostic + Viewport R1 · 2026-09-25 · screen-space 修复 | Active |
| L-REN-002 | Lesson | Rendering | 双精度回退必须发生在第一次降精度之前 | P0 | E2 | F1-FAR-SAFE-01 · 2026-08-11 | Active |
| L-REN-001 | Lesson | Rendering | 连续参数修补失败必须重新审查承载架构 | P0 | E3 | GRID-RW-2A/B · c1451df / 2c57893 / 6154078 | Active |
| L-ARCH-001 | Lesson | Architecture | 跨越完整交互链的产品切片必须先拆清 Workspace 边界 | P1 | E1 | MAP-A → EDITOR-A · 2026-08-11 · 6724079 | Active |
| K-INP-001 | Knowledge | Input | 同一 Pointer 手势必须只有一个实时 Owner | P0 | E2 | v0.2.25.9-fix · 2026-08-10 11:48:28 · d621755 | Active |
| K-INP-002 | Knowledge | Input | Win32 Mouse Capture 必须统一管理完整释放生命周期 | P0 | E2 | v0.1.8.10-fix · 2026-06-26 · 8d6e7fd | Active |
| K-INP-003 | Knowledge | Input | Input Router 只有接入真实生产 Source 才算完成 | P0 | E2 | WAVE-2.5 E0/E5 · 2026-09-24 | Active |
| K-INP-004 | Knowledge | Input | 平台输入编码必须在 Adapter 边界正规化 | P0 | E2 | Region Snap R1 · 2026-09-25 | Active |
| K-DIAG-001 | Knowledge | UI | Diagnostic 必须保持观察者与输入透明 | P0 | E2 | Diagnostic + Viewport R1 · 2026-09-25 | Active |
| K-GOV-003 | Knowledge | Engineering | 当前仓库入口/Resolver 高于 Agent 历史环境记忆 | P0 | E2 | SDK resolver audit · 2026-09-25 | Active |
| EXP-GOVERNANCE-003 | EXP | Engineering | Process Version Telemetry Must Advance | P0 | E2 | GLOBAL-PROCESS-VERSION-GOVERNANCE-R1 · 2026-09-28 | Active |
| K-UI-001 | Knowledge | UI | 冷启动错位/操作后恢复优先检查 Measure/Arrange 与命中热区 | P0 | E2 | v0.2.24.49-fix → .50-fix · 2026-08-09 · 60fd339 收口 | Active |
| K-DATA-001 | Knowledge | Data | 覆盖保存必须采用可回滚 Staging 事务 | P0 | E3 | v0.2.21.24-rz · 2026-08-02 14:10:00 · e089325 | Active |
| K-DATA-002 | Knowledge | Data | Load 必须 Candidate→Commit，结构失败与资源失败分级 | P0 | E3 | v0.2.21.25-rz · 2026-08-02 15:30:00 · cafe400 | Active |
| K-DATA-003 | Knowledge | Data | 异步危险确认必须捕获稳定对象身份并在确认后重新验证 | P0 | E1 | MAP-DATA-A-R2-F2-F2-F1 · 2026-08-12 · 3d53de0 | Active |
| K-ASSET-001 | Knowledge | Data | 数据归一化/Bake 后必须同步归一化相关元数据 | P0 | E2 | v0.2.21.23-fix · 2026-08-02 12:45:00 · a9c1ec6 | Active |
| K-ASSET-002 | Knowledge | Data | 确定性资源创建失败必须按 Key+Revision 负缓存 | P1 | E2 | v0.2.21.23-fix · 2026-08-02 12:45:00 · a9c1ec6 | Active |
| K-PERF-001 | Knowledge | Performance | Preview 高频路径与 Commit 重路径必须分离 | P0 | E2 | v0.1.8.7-fix · 2026-06-25 00:18 · 26f2006 | Active |
| K-GEO-001 | Knowledge | Architecture | 可编辑几何能力契约与 Snap/Topology 边界 | P0 | E1 | MAP-DATA-A-R2-F3-E1 · 2026-08-13 · 本轮提交 | Active |
| K-GEO-002 | Decision | Architecture | R2 收口并以 Point Consumer 作为下一验证形态 | P0 | R2 | MAP-DATA-A-R2-CLOSEOUT · 2026-08-13 · 6a3d5b8 | Active |
| K-GEO-003 | Knowledge | Architecture | 地理对象显式绑定 Ground Surface，Terrain 查询失败不得静默回退 | P0 | E2 | Ground Authoring R1 · 2026-09-29 · 403fcdf3→67ca6fd5 | Active |
| DEC-XYE-XYUI-DUAL-LANE | Decision | Governance | XYE / XYUI Dual-Lane Development Model | P0 | R1 | XYE-XYUI-DUAL-LANE-R1 · 2026-09-27 | Active |

| K-GOV-004 | Knowledge | Engineering | Governance Guard 必须共享显式 Bootstrap 且可独立执行 | P0 | E2 | GOV-BASELINE-RECOVERY-R1 · 2026-09-28 | Active |
| K-GOV-005 | Knowledge | Engineering | Candidate Tree 与 Validation Tree 不一致时必须 Fail-Closed | P0 | E2 | XYK-C0-GOVERNANCE-SEDIMENT-R1 · 2026-09-28 | Active |
| K-GOV-006 | Knowledge | Engineering | Windows PowerShell 5.1 中文脚本必须保留兼容编码 | P0 | E2 | Handoff CI · 2026-10-01 · 33afb893 | Active |
| EXP-GOVERNANCE-004 | EXP | Engineering | Governance RED 先分类，禁止“修绿” | P0 | E2 | GOV-BASELINE-RECOVERY-R1 · 2026-09-28 | Active |
| DEC-XYK-KNOWLEDGE-PLANE | Decision | Governance | XYE / XYUI / XYK 三位一体与 XYK 远端知识平面 | P0 | R1 | XYK-FOUNDATION-R1 · 2026-09-28 | Active |

| K-UI-002 | Knowledge | UI | 拖拽反馈必须保持 Pointer Grab Point 不变量 | P1 | E2 | XYUI4 DragFeedback / Coordinate Audit · 2026-09-28 | Active |
| K-XYT-T2-TERRAIN-CONTEXT-001 | Knowledge | UI | 上下文派生状态必须绑定真实 Workspace 与 Context | P0 | E2 | XYT-T2-T-A-FIX-A-TERRAIN-CONTEXT · 2026-09-30 · 115b4ea9 | Active |
| K-XYT-MAP-001 | Knowledge | Engineering | 正式测试选择必须经过 Change→Ownership→Capability→Required Tests | P0 | E3 | XYT-C · 0d75987c · 2026-09-29 | Active |
| K-XYT-MAP-002 | Knowledge | Engineering | Agent 可以增加测试但不得删除 Required Tests | P0 | E3 | XYT-C · 0d75987c · 2026-09-29 | Active |
| K-XYT-EXEC-001 | Knowledge | Engineering | 执行失败根因在证据不足时保持 UNKNOWN | P0 | E3 | XYT-D · 673dae7f · 2026-09-29 | Active |
| K-XYT-AUDIT-001 | Knowledge | Engineering | 测试名称与目录不能授予证据等级 | P0 | E2 | XYT-G Legacy Audit · 2026-09-29 | Active |
| K-XYT-WIT-001 | Knowledge | Engineering | Regression Witness 必须由同一测试形成 RED→GREEN | P0 | E3 | XYT-J · 9e4c1a3d · 2026-09-29 | Active |
| K-XYT-EVID-001 | Knowledge | Engineering | 证据失效必须按 Capability 隔离 | P0 | E2 | XYT-K · 32af17e9 · 2026-09-29 | Active |
| K-XYT-CLOSE-001 | Knowledge | Engineering | 集成合同 READY 不等于产品收口 | P0 | E2 | XYT-L/FINAL · 18842310/475ab10d | Active |
| K-XYT-P3-001 | Knowledge | Engineering | P3 必须走仓库权威构建和真实运行入口 | P0 | E2 | XYT-H · ccd3dbef · 2026-09-29 | Active |
| K-XYT-HARNESS-001 | Knowledge | Engineering | 预期子进程失败不得污染父级 Selftest 退出码 | P0 | E2 | INC-2026-09-29-003 | Active |
| K-HANDOFF-001 | Knowledge | Engineering | Stale Commit Mutex 必须有受控恢复路径 | P0 | E2 | XYT-L2/Handoff · 2026-09-29 | Active |
| K-XYT-P3-002 | Knowledge | Engineering | P3 构建准备与 Runtime Probe 必须分开计时 | P1 | E2 | XYT-H2/H3 · 2026-09-29 | Active |

| K-GOV-007 | Knowledge | Governance | 仓库内可写边界与缓存根控制 | P0 | E2 | 2026-10-22～30 | Active |
| K-GOV-008 | Knowledge | Governance | 单次受控 Build/Restore 与禁止自动重试 | P0 | E2 | 2026-10-30 | Active |
| K-GOV-009 | Knowledge | Governance | XYK 候选逐轮去重审计与 Git 事件闭环 | P0 | E2 | 2026-10-30 | Active |
| K-GOV-010 | Knowledge | Governance | 静态/Lane/Candidate/Integration/发布分层 | P0 | E2 | 2026-10-23～30 | Active |
| K-ARCH-003 | Knowledge | Architecture | 5+100、严格三级、test_ 与依赖方向 | P0 | E2 | 2026-10-24～30 | Active |
| K-WORLD-001 | Knowledge | Architecture | WorldState Candidate 先验证后发布 | P0 | E2 | 2026-10-29 · b8ae01f | Active |
| K-GEO-004 | Knowledge | Architecture | raw DEM 与 LOD/观察中心分离 | P0 | E2 | 2026-10-21～23 | Active |
| K-REN-006 | Knowledge | Rendering | 在途帧/图像/资源 Fence 生命周期解耦 | P0 | E2 | 2026-10-23 · 6dd6207 | Active |
| K-REN-007 | Knowledge | Rendering | 性能 Schema 不等于真实 GPU 采样 | P0 | E2 | 2026-10-23 · 6dd6207 | Active |
| K-UI-003 | Knowledge | UI | XYUI 组件编号及 Typography 语义不变量 | P0 | E2 | 2026-10-30 · a88a06a | Active |
| L-GOV-001 | Lesson | Governance | 无界构建副本导致磁盘事故 | P0 | E2 | 2026-10-26 | Active |
| L-GOV-002 | Lesson | Governance | 目录迁移 STATIC 不代表产品完成 | P0 | E2 | 2026-10-30 | Active |
| DEC-XYE-ARCH-5X5X5-R1 | Decision | Architecture | 严格三级、5+100、test_ 与 XYUI 编号 | P0 | R1 | 2026-10-24～30 | Active |

| K-DRAW-001 | Knowledge | Input | Drawing Session Snapshot 唯一事实源 | P0 | E2 | 2026-10-04 · 518a0849 | Active |
| K-DRAW-002 | Knowledge | Input | 绘制Accepted/Rejected/Blocked及Reason | P0 | E2 | 2026-10-04 | Active |
| K-SPA-003 | Knowledge | Spatial | Screen to Surface Anchored Zoom capability remains scoped | P0 | E1 | 2026-10-03; context split 2026-10-05 | Active |
| K-SPA-004 | Knowledge | Spatial | Editor and Gameplay Zoom Authority context split; clearance is 10/10 addendum | P0 | E1 | 2026-10-05; baseline 4277b242 | Active |
| DEC-CAM-001 | Decision | Spatial | Editor center navigation versus Gameplay anchored navigation | P0 | Design | 2026-10-05; baseline 4277b242 | Active |
| K-XYC-001 | Knowledge | Automation | XYC语义命令使用Editor权威写线程及Revision | P0 | E2 | 2026-10-14 · 6b19798 | Active |
| K-XYC-002 | Knowledge | Automation | 自动化截图需同帧身份和GPU完成证据 | P0 | E2 | 2026-10-14～17; 71fc8c36静默GPU取证 | Active |
| K-XYC-003 | Knowledge | Automation | 写命令成功须核对World Projection History | P0 | E2 | 2026-10-18～19 · fa666cb | Active |
| K-PERF-002 | Knowledge | Performance | 动态SSE须使用Renderer视锥和SourceRevision | P0 | E2 | 2026-10-18 | Active |
| DEC-XYE-TERRAIN-HYDROLOGY-20261003 | Decision | Terrain | Base DEM只读和水文Graph局部增量更新 | P0 | Design | 2026-10-03～04 | Active |
| DEC-XYE-EMPTY-WORLD-20261003 | Decision | Rendering | 逻辑ReferencePlane与可见Grid独立 | P0 | Design | 2026-10-03 | Active |
| DEC-XYE-XYC-CONTROL-20261014 | Decision | Automation | XYC后台语义控制与权限边界 | P0 | Design | 2026-10-14 | Active |

| DEC-XYE-MCP-SERVER-20261001 | Decision | Automation | MCP Server-only、外置Host和Editor Bridge、源码控制权限 | P0 | Design | 2026-10-01 | Active |

## 分类文件

- xyk/decisions/camera-zoom-context-2026-10.md — DEC-CAM-001与K-SPA-003/004纠错
- xyk/governance/conversation-audit-2026-10-r4.md — 10/17 Vulkan取证与跨分支编号冲突


- `xyk/decisions/mcp-server-boundary-2026-10.md`：10月1日MCP Server-only与控制权边界
- `xyk/governance/conversation-audit-2026-10-r3.md`：Editor R1H-R2三条候选及10月1日MCP补证


- `xyk/knowledge/2026-10-conversation-recovery.md`：10月历史聊天新增Drawing、XYC、SSE、只读净空知识
- `xyk/governance/conversation-audit-2026-10-r2.md`：10月逐日追查与证据分级
- `xyk/governance/conversation-candidates-2026-10-r2.md`：未证实候选单独登记
- `xyk/decisions/terrain-hydrology-2026-10.md`：Terrain与Hydrology架构裁定
- `xyk/decisions/empty-world-2026-10.md`：空世界与ReferencePlane边界
- `xyk/decisions/xyc-control-2026-10.md`：XYC设计边界

- `xyk/knowledge/2026-10-backfill.md`：2026-10-10～31 新知识 K-GOV/K-ARCH/K-WORLD/K-GEO/K-REN/K-UI
- `xyk/experience/backfill-2026-10.md`：EXP/L-GOV-001～002 与 EXP 强化
- `xyk/incidents/backfill-2026-10.md`：ERR-20261026-001
- `xyk/decisions/architecture-layout-2026-10.md`：DEC-XYE-ARCH-5X5X5-R1
- `xyk/governance/backfill-2026-10-r1.md`：审计覆盖、待验收项和证据边界

- `engineering.md`：K-VAL-001～K-VAL-003、K-GOV-001～K-GOV-006、K-XYT-MAP-001～002、K-XYT-EXEC-001、K-XYT-AUDIT-001、K-XYT-WIT-001、K-XYT-EVID-001、K-XYT-CLOSE-001、K-XYT-P3-001～002、K-XYT-HARNESS-001、K-HANDOFF-001
- `architecture.md`：K-SPA-001、K-SPA-002、K-ARCH-001、K-ARCH-002、K-GEO-001、K-GEO-003；R2 Closeout / Point Foundation 见 K-GEO-002
- `rendering.md`：K-REN-001～K-REN-005、K-NATIVE-001～K-NATIVE-002
- `input.md`：K-INP-001～K-INP-004
- `ui.md`：K-UI-001～K-UI-002、K-XYT-T2-TERRAIN-CONTEXT-001、K-DIAG-001
- `data.md`：K-DATA-001、K-DATA-002、K-DATA-003、K-ASSET-001、K-ASSET-002
- `performance.md`：K-PERF-001
- `incidents.md`：代表性事故记录与映射
- `lessons.md`：L-ARCH-001、L-REN-001～L-REN-003、L-VAL-001、L-NATIVE-001、L-TEST-001 及后续复盘
- `decisions/`：已批准并仍有长期约束价值的 DEC
- `decisions/xye-xyui-dual-lane-development-model.md`：XYE / XYUI One Workspace / Dual Lane 决策
- `decisions/xyk-knowledge-plane.md`：XYE / XYUI / XYK 三位一体与 XYK 远端知识平面
- `xyk/incidents/agent-error-log.md`：Agent 真实错误事实
- `xyk/experience/rules.md`：去重后的防复发经验规则

### Knowledge Preflight helper

`tools/governance/knowledge-preflight.ps1` 是只读定位工具。它接受 `-Phase Planning|Execution`、`-Domain` 与 `-Keywords`，只输出索引命中、候选 ID 和对应文件路径，不替 Agent 读取正文、判断冲突或执行 Supersede。`knowledge-preflight.selftest.ps1` 固定验证 Rendering / Grid / Depth 的关键候选与无匹配查询。

> 2026-10-31 Coordinator Review：`XYK-CAND-20261031-EDITOR-BUILD-01-R2-001` → **STRENGTHEN `L-TEST-001`**，正式新增段落在 `docs/knowledge/lessons.md` / `xyk/experience/lessons.md`，审计回执 `xyk/governance/review-editor-build-01-r2-001.md`。产品本地候选 Changelog 仍为 PENDING / 未提交，不可视作自动同步。
