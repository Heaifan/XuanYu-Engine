# 2026年10月历史对话｜未证实候选登记（非正式Knowledge条目）

下列项目记录为CANDIDATE/NEEDS EVIDENCE，**不授予ACTIVE知识条目编号，不代表已发生事故，也不代表产品已修复**。

## C-SESSION-INVARIANTS｜2026-10-07
14项Bug审计归因之一：Session Bootstrap可能不原子，建议在Start/New/Reset/Load共用Normalize→Validate→Prepare→Atomic Commit唯一InstallMap及EmptyWorld合法语义。来源Claude/ChatGPT架构静态分析；需要产品树、各入口和失败注入。处理：审查K-ARCH-002/K-DATA-002，必要时创建新Session合同，未核实前不造K。

## C-INTERACTION-CONSISTENCY｜2026-10-07
14 Bug中Interaction生命周期多Owner，ToolChanged/Direct Gizmo/Router之间可能终止不一致。已有K-INP-001/K-INP-003与本次R2强化；需真实OS输入及重现用例，不重复新建卡。

## C-CAMERA-SAFETY｜2026-10-07
Camera可能没有明确Surface Authority。旧K-GEO-003、K-SPA-002/004已覆盖治理边界；要以相机/近地真实Vulkan Trace与固定DEM确定是哪一段链路失败，不凭14 Bug归因宣称根因已被证明。

## C-RENDER-GENERATION｜2026-10-07～09
CPU/GPU双Authority与Stale-Rejection Generation/Sequence可能冲突；历史建议禁用长整型最大值强行覆盖旧包，并使生命周期/资源代际一致。需要测试旧/新场景交错、Present与Buffer异常；不能把分析直接提升为K-REN-008 ACTIVE。

## C-VULKAN-CREATE-ROLLBACK｜2026-10-27
静态审计指出InstanceOwner/SurfaceOwner在原生句柄创建成功后，如日志或Owner包装阶段抛异常，catch可能返回失败而未回收已创建句柄。应定义事务边界为「Native Handle成功创建→Owner成功交付」，在故障注入中证实回滚。**尚未实测实际泄漏，故无ERR ID和已确认K ID**。该轮原报告明确待用户裁决且未入库，本次仅保留待审候选。

## C-WORLD-REPLACE-GATE｜2026-10-29
EntityRegistry.Replace clear→逐条写入跨多个容器可能中途失败。当前已强化K-WORLD-001为所需事务合同；R2-B实际双缓冲预建、Generation短锁提交、失败无污染仍需进一步提交与测试，不能加一张重复K-WORLD-002。

## C-XYK-CHANGELOG-PRODUCT-WORKTREE｜2026-10-31
用户明确要求从下一轮Codex起，在实际产品仓中根据已有约定建立专用XYK候选变更记录，追加任务日期、Source Tree、证据、风险、去重与处理状态，不与产品发布CHANGELOG混用。XYK Owner本轮已更新K-GOV-009；但当前只写入xyk/main的治理知识，没有擅自修改产品工作树或宣称下一轮Codex脚本已接线。

## C-GAMEPLAY-DEMO-AND-LOGISTICS｜2026-10-03、10-11、10-15、10-30
用户提出2026年最小可玩Demo、班/排级战术与后勤/支援梯队价值。可以作为Product/Design未来决策审阅，但尚未确认独立战斗经济/增援公式与引擎真实执行代码；不混入Renderer工程结论。年度Demo优先级不代表已完成关卡和战斗闭环。

## 追查缺口
10月2、6日缺乏足够独立候选原文；10月其他部分对话仅能查到摘要、非完整逐句内容。补齐未推送的Codex草案/产品Dirty与真实GPU/Editor验收后重新审查，特别K-SPA-004、K-XYC-002、K-PERF-002的产品实现边界。
