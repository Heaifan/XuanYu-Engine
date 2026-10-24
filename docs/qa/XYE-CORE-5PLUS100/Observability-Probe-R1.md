# XYE Core 5+100：Observability/Probe 子文件夹审计 R1
- 分支：`审计XYE架构`，基线：`dff7b57d83a877ba629d3ad06e5aee2292c75bfe`。
- 范围：`XuanYu.Core/Observability/Probe` 全部 8 个生产文件（Capture、Frames、Owner），及对应 Core.Tests 镜像测试。
- 禁止越界：不修改 Runtime、Replay、World、Camera、Editor、Vulkan 或外部调用方。
- 交付合同：新增/整理测试均为 `test_` 前缀，生产和测试源码均不超过 100 行；审计列出所有显式方法、主要属性及中文解释。
## Capture（像素编码职责）
**VulkanPngEncoder.cs（PNG编码，原样保留）**
- `EncodeBgra(int,int,ReadOnlySpan<byte>)`（BGRA像素编码PNG）
- `EncodeRgba(int,int,ReadOnlySpan<byte>)`（RGBA像素编码PNG）
- `Encode(int,int,ReadOnlySpan<byte>,bool)`（验证尺寸、行转换和压缩，私有）
- `WriteChunk(Stream,string,ReadOnlySpan<byte>)`（写PNG分块及CRC，私有）
- `Crc32(ReadOnlySpan<byte>)`（计算CRC32，私有）
- 字段：`Signature`（PNG签名）；没有发现必须在本轮修改的明确缺陷
## Frames（帧与身份记录职责）
**ViewportPresentedFrame.cs（呈现帧记录，原样保留）**
- 主构造 `ViewportPresentedFrame(long,uint,long,bool)`（记录呈现帧信息）
- 属性：`FrameId`（帧号）、`ImageIndex`（图像索引）、`CameraRevision`（相机版本）、`CaptureHeld`（捕获保持标志）
- 没有需要手写的其它函数
**ViewportProbeFrame.cs（相机与矩阵证据）**
- `CameraSnapshot(string,CameraState,Vector3d,ViewProjectionState?)`（记录相机快照）
- `CpuGpuSnapshot(CameraState,Vector3d,Matrix4x4)`（记录摄像机及传入的GPU矩阵参数）
- `FormatCpuGpuEvidence(long,Vector3d,string)`（形成不冒充 GPU 实测的证据文本，新增内部函数）
- `MatrixHash(Matrix4x4)`（生成简化矩阵哈希，私有）
- 字段：`_lastGpuFrame`（上次帧）、`_lastGpuHash`（上次哈希）
- 修改：未真正读取 GPU 侧矩阵就输出 `CpuGpuCameraMatch=YES` / `CpuGpuRenderOriginMatch=YES` 属于不实声明。现改为 `UNKNOWN`，保留 `CpuCameraRevision` 并使用 `GpuMatrixArgumentHash` 标注输入参数
**ViewportProbeIdentity.cs（探针构建身份）**
- `Current()`（从环境变量与程序集提取当前身份）
- `Format(string,bool,string,string)`（格式化 HEAD、Dirty、Probe 和 Build）
- 常量：`ProbeName`（探针名称）
- 修改：仅拆分过长源码行，不改语义
**ViewportProbeInput.cs（输入与相机日志）**
- `BeginWheel(double,double,double,string,string,string)`（开启滚轮事件）
- `Operation(string)`（记录操作）
- `EndWheel(long,bool)`（结束滚轮事件）
- `CameraWriter(string,CameraState,Vector3d,CameraState,Vector3d)`（记录相机与 Pivot 变化）
- 修改：拆分长行，保持日志输出字段和顺序。极端差分数值溢出风险已记录，未扩大本轮修复
## Owner（探针状态与截图归档职责）
**ViewportProbe.cs（探针状态拥有者）**
- `Initialize()`（初始化目录和程序集取证）
- `BeginFrame()`（开始帧并分配ID）
- `SetWheel(long)`（设置滚轮ID）
- `NewWheel()`（分配滚轮ID）
- `NewNativeInput()`（分配原生输入ID）
- `SetNativeInput(long)`（设置输入ID并返回原值）
- `RestoreNativeInput(long)`（恢复输入ID）
- `RefreshLoadedAssemblies()`（重新采集程序集信息）
- `Log(string,string)`（输出分类诊断日志）
- `AssemblyPath(string)`（获取程序集路径）
- 属性：`Enabled`（是否启用）、`CurrentFrameId`（当前帧号）、`CurrentWheelEventId`（滚轮ID）、`CurrentNativeInputId`（原生输入ID）
- 修改：拆分超长日志行；未更改同步与写文件行为
**ViewportProbe.Present.cs（呈现帧控制，原样保留）**
- `NextPresentedFrameId()`（分配呈现帧ID）
- `ArmFrameCapture(long)`（挂起后续匹配帧）
- `ReleaseCapturedFrame()`（释放保持的捕获帧）
- `FrameSubmitted(long,uint,long)`（记录提交）
- `FramePresented(long,uint,long)`（记录呈现并通知等待方）
- `WaitForPresentedFrameAsync(long,TimeSpan)`（等待指定之后的新帧）
- 属性：`LastPresented`（最近帧记录）、`LastPresentedFrame`（最近帧ID）、`LastPresentedCameraRevision`（相机版本）
- 待验：PresentedSignal 潜在积压、多等待者与 30秒 CaptureHold；不能在无 Vulkan 回归时擅改这一同步路径
**ViewportProbe.GpuCapture.cs（GPU 读回与 PNG 归档）**
- `LogGpuReadbackCapability(string,string,bool)`（记录GPU读回能力）
- `IsCaptureTargetFrame(long,long)`（判断目标帧）
- `EncodeGpuPixels(int,int,string,ReadOnlySpan<byte>)`（按图像格式选择编码器，新增内部函数）
- `WriteGpuCapture(long,uint,long,int,int,string,ReadOnlySpan<byte>)`（保存PNG、SHA-256与JSON证据）
- 字段：`_gpuDeviceName`（设备名称）
- 修改：仅四种已知 BGRA / RGBA 像素格式可编码；其它格式抛出 NotSupportedException，避免误编码文件
## 镜像测试与全部显式函数
**Capture/test_VulkanPngEncoder.cs**
- `EquivalentBgraAndRgbaProduceSamePng()`（同色像素编码结果一致）
- `RejectsInvalidDimensionsOrPixelLengths()`（拒绝非法尺寸与像素长度）
**Frames/test_ViewportProbeFrame.cs**
- `DoesNotClaimUnmeasuredCpuGpuAgreement()`（不虚报 CPU/GPU 一致）
**Frames/test_ViewportProbeIdentity.cs**
- `Identity_contains_head_dirty_probe_and_build()`（保留原有身份回归）
- `Identity_formats_clean_source()`（验证非 Dirty 身份格式）
- 原 `XuanYu.Core.Tests/Diagnostics/ViewportRuntimeProbeTests.cs` 的测试迁移至此，原文件删除
**Owner/test_ViewportProbeGpuCapture.cs**
- `UsesBgraEncoder(string)`（验证两种BGRA格式）
- `UsesRgbaEncoder(string)`（验证两种RGBA格式）
- `RejectsUnsupportedFormatBeforeWritingFiles()`（拒绝未知像素格式）
## 状态与边界
- 静态审计与逐函数清单已完成；只修 Probe 内的证据真实性和像素格式判定。
- 未修复：磁盘 I/O 失败对调用链的影响、极端相机差分、简化哈希碰撞、帧等待信号积压。需运行证据后处理
- Build：NOT RUN；xUnit：NOT RUN；Windows Vulkan / GPU PNG 回放：NOT RUN；Integration：NOT PASS
- 只有远端 Git HEAD/Tree 和本轮文件清单再次核对后，才能报告已完成上传
