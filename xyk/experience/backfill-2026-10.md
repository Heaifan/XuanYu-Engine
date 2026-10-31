# XYK｜2026年10月经验与教训补录

## EXP-GOVERNANCE-006 越界写入疑点必须 Fail-Closed
**ACTIVE｜Occurrences：2（2026-10-22 边界事件、2026-10-26 T0+ 磁盘事故，非同一机制）。**
触发：仓库外目录、NuGet/临时缓存真实可写路径不明、Build/Restore 重试导致多份工作区、清理白名单不符。
规则：立即 STOP，仅做只读定位，保留实际进程参数、源/目标路径和占用数据；疑似外部读取与已证实写入要区分。经 Owner 批准后才能对固定路径做单次受控操作；禁用无限重试和全盘清理。
关联：K-GOV-007、K-GOV-008、ERR-20261026-001。

## L-GOV-001 无界构建副本会把测试治理变为磁盘事故
**ACTIVE｜P0｜E2｜2026-10-26。**
错误前提：重复 Candidate、每批 Build、失败自动 Restore 都是无成本的验证。实际会增加 .xye-workspace/Artifacts/包缓存，且测试产物身份可能偏离当前源码。止损：冻结写入，列出所有合法可写根与磁盘差额，分类清理，再建立一次性构建与产物复用。T0+ 事故关闭≠产品集成全部通过。

## L-GOV-002 静态文件迁移完成不等于系统行为验证通过
**ACTIVE｜P0｜E2｜2026-10-24～30。**
Core/World/Vulkan/XYUI/Editor 迁移若仅完成数量、目录深度、5+100 静态检查，应报告 STATIC ONLY；Build、Test、GPU 与真实 UI 各自独立验收。不得以局部移动/改名成功，抹去 Arch-A、DEM、QueueWaitIdle 或范围外全仓 RED。
关联：K-ARCH-003、K-GOV-010、EXP-GOVERNANCE-005。

## 现有 EXP 强化
**EXP-GOVERNANCE-005**：M2 A/B/C 不同源码候选、XYUI R2 用户视觉 CLOSED 但最新 Git 未固化，Lane PASS/视觉 PASS 都不等于最终 Integration 与远端发布。
**EXP-TEST-001**：Native P3 必须用真实 Win32 HWND、消息、Owner/Capture/Consumer、结果与产物身份关联；Headless 合成、CPU/GPU 探针互不替代。
