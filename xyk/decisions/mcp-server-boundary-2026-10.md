# DEC-XYE-MCP-SERVER-20261001｜MCP 服务端接口与控制权边界

**状态**：ACTIVE（已批准设计，不等于实现验收）｜**用户裁定**：2026-10-01（北京时间）｜**范围**：XYE MCP R1 / Editor Application Interface。

## 正式裁定
1. R1只做MCP Server，**不做 MCP Client**；Client以后另立任务，不得偷偷进入本批范围。
2. MCP Host 是Editor进程外可替换的通信适配层；Editor内部用Bridge，只允许从Application正式入口调度能力，不能依赖/绕过内部World/UI实现细节。协议实现不是Core长期依赖，稳定的Application Contract才是长期资产。
3. 可附着已有正在运行的Editor，但**不得重置或破坏用户现场**。需要明确UI写线程、Scene/Workspace等权威Owner与资源级隔离；多Agent并发应受资源作用域/所有权约束，禁止用共享全局锁任意篡改另一个Agent事务。
4. MCP可做自动驾驶、运行压力测试与语义控制，但**不得改动引擎源码**；代码编辑只交给经授权的Coding Agent。MCP业务写需另设显式能力权限、会话/Revision和事务边界。
5. 测试严格走XYT分层：Semantic Test、UI Contract Test、Avalonia Input Pipeline、真实产品窗口/OS输入、P4必须区分；Synthetic/Representative/Live Product结果不得跨级继承。内部API测试PASS不等于真实UI PASS。

## 与后续XYC的关系
2026-10-14批准的XYC语义控制长期目标与本DEC兼容：XYC是面向Agent的产品语义控制能力，MCP只是一种可替换协议适配器。10/01的MCP Server-only为**当时R1任务限制**，不能将其泛化为玄域永远禁止其他协议或未来Client。

## 验收边界
此DEC源自用户对设计条款的逐项批准，**不能推出MCP Host+Bridge已开发完成、已连接真实Editor或真机P3/P4已通过**；任何实现门禁需附独立产品Commit/Source Tree/XYT结果。本DEC不写入XYE产品分支。
