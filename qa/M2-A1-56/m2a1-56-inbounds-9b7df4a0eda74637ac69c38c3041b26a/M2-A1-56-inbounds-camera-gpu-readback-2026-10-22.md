# M2-A1 #56：范围内机位与 GPU 地形读回

日期：2026-10-22（Asia/Shanghai）
结论：本轮范围内对照通过；原始 `OutOfBounds` 来自原测试相机 XY 位于地形范围之外。没有修改地形查询、坐标转换、LOD 或 Vulkan 产品代码。
候选：HEAD `cccd49ce4caabb0101f0f57cf91a81e2b7802dc6`，工作树 `DIRTY=YES`。进程为 `XuanYu.Editor.App` Release，静默隐藏 HWND swapchain，设备 NVIDIA GeForce RTX 3060，视口 776×650。

## 对照设置和结果

输入 DEM：`artifacts/diagnostics/wp2-p4-r3/n24e121.hgt`，样点 3601×3601。瓦片范围为纬度 24–25、经度 121–122；按 `TerrainWorldPlacement` 映射得到约 X=0–101,296 m、Y=0–111,319 m。HGT 中心样点为 1,528 m，中心机位 `(50,648.213, 55,659.745)` 落在瓦片范围内。

此前失败位置 `(166,030.766, -88,319.077, 87,915.294)` 明确在范围外；`scene.inspect` 对此报告 `OutOfBounds`。同一 DEM 下：

| 姿态 | 相机位置 (m) | 相机修订 | 地表查询 | GPU 呈现帧 | 结果 |
| --- | --- | ---: | --- | --- | --- |
| 俯视 | (50,648.213, 55,659.745, 12,000) | 4 | `Valid`, renderSurfaceZ=1596.193 | FrameId 1019, ImageIndex 0 | 有真实 GPU PNG |
| 斜视 | (50,648.213, 54,159.745, 3,528) | 5 | `Valid`, renderSurfaceZ=1668.288 | FrameId 1021, ImageIndex 0 | 有真实 GPU PNG |

两姿态均返回 `terrainId=n24e121`, `terrainRevision=1`，present 成功。侧车文件分别将 NVIDIA GeForce RTX 3060、Vulkan swapchain GPU readback、帧号、相机修订、图像索引、776×650、格式和 PNG SHA-256 对齐。图像中可见绿色起伏地形及网格；斜视帧另有 200 m 比例尺。

## 证据

- `requests.jsonl`、`replies.jsonl`、`stdout.raw.log`、`stderr.log`：协议原始请求/回复及进程日志。所有有效请求成功，包含 `camera.set-test-pose`、`capture.arm`、轮询 `scene.inspect` 和 `capture.release`。
- `gpu-frame-00001019-camera-00000004.png` 与同名 `.json`：俯视真实 GPU 读回；SHA-256 `76903809C7C2C107481831636714071F55D8C8F5812FA2C3921AB85A9180C0B9`。
- `gpu-frame-00001021-camera-00000005.png` 与同名 `.json`：斜视真实 GPU 读回；SHA-256 `E37D77D456B3CDDD2163E18A7C4A829CF08F043DC293624A1D6D3003B526B964`。
- `ground-chain.log`、`terrain.log`、`present.log`、`viewport.log`：对应运行诊断。

本轮第一次采集脚本把空 `args` 错误序列化成数组，导致相机设置被协议拒绝；该失败运行的原始记录保留在相邻 `m2a1-56-inbounds-60cffd8f47804da1aac2a0f91f369346` 目录，未用来作产品结论。修正采集器后，本报告所列 run 的全部请求成功。

## 裁定边界

本结果支持先修正 #56 原测试机位，不支持修改地形查询或坐标转换实现。此处的 `surfaceQuery` 是相机 XY 处的垂直地表查询；不是独立的任意屏幕中心射线命中 API。两张 swapchain 图像直接证明 GPU 输出中存在该地形，但本次只完成定向机位/读回对照，#56 的完整 DEM 证据链仍需将需求侧指定相机、同帧元数据和归档链对齐后再裁定。它不构成 #54 多轮交换链换代/关闭/异常恢复的综合验收，也不替代用户对真实 UI 的产品验收。

本会话未提交、推送或更改 GitHub Issue。原有工作区边界回归与 Arch-A 既有 Dirty 项目引用阻碍的裁定不变。
