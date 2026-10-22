# #55 VUID-07987 修复验证证据

- 产品版本：v0.3.0.28-fix
- 产品修复提交：7535d99057e4f954a45d222fd7641bdcdc79f267
- 分支：codex/m2-a1-vuid-fix-20261022；推送目标：origin/渲染器2.0
- 验证源码身份：HEAD=7535d99057e4f954a45d222fd7641bdcdc79f267；DIRTY=YES（候选内保留的未跟踪运行/诊断材料）；构建来源 Tree=c93cfa9989469efd54baa196fbd1647261121899

## 变更

- 分析描边管线 PushConstantRange 允许 Vertex|Fragment；普通覆盖物仍为 Vertex-only。
- 分析描边绘制使用描边管线布局及 Vertex|Fragment 推送；普通覆盖物继续使用原布局和 Vertex-only 行为。
- 新增 `VulkanVectorOverlayPushConstantContractTests` 两项合同测试。

## 定向测试

命令：`xye-dotnet test XuanYu.World.Tests\XuanYu.World.Tests.csproj --no-restore --configuration Release --filter FullyQualifiedName~VulkanVectorOverlayPushConstantContractTests -p:DefaultItemExcludes=**/VulkanOffscreenDeviceTests.cs`
结果：2/2 PASS，0 failed，0 skipped；dotnet 子进程退出码 0；外层任务 RunId `799de7c2c6fb44f680d18800ca70ffab`，Manifest `COMPLETED / 0`。

测试排除了已有编译故障文件 `VulkanOffscreenDeviceTests.cs`。因此这是定向通过，不代表 World.Tests 全量通过。

## RTX 3060 / Vulkan Validation

- RunId：`m2a1-55-validation-confirm-20261022`
- GPU：NVIDIA GeForce RTX 3060；Vulkan API 1.4.341。
- Loader 日志确认 `VK_LAYER_KHRONOS_validation` 插入 Instance 与 Device；进程退出码 0。
- 检查日志中 `VUID-07987` 计数 0，Vulkan Validation Error/Warning/VUID 消息计数 0。
- `runtime.info` 与 `scene.inspect` 成功；Hidden HWND swapchain attach / Present 成功；NASADEM `n24e121` 导入完成、terrain revision=1；`session.close` 返回 closed=true，应用与 CLI 退出码 0。
- 当前场景中心 Surface Query 为 `OutOfBounds`，属于 #56 已记录的相机/查询范围问题，不作为 #55 管线修复通过依据。
- 本次没有产生 GPU 像素读回或截图；不得把 Present 成功描述成地形图像证据。

## SHA-256

- ``focused-test.log``: ``24CCD9C656F2699A55A86F523639B7FCBB7B95A31C0D10D4899C1FEFC9B7B30B``
- ``runtime-replies.jsonl``: ``6838CFD2A693F5CBB4B9EBD08C1548EC1B04A47BEEC20C47F6526A62B8853891``
- ``runtime-summary.json``: ``674F88CE01BE67B1F6D0600B03F073548A8B823165546CEC3A7C7BD48BEF5E5F``
- ``vulkan-validation.log``: ``576E949B618D9D060E494FA7E7A2A1DA91719B524300AF5A97905D0F6004E5AC``
