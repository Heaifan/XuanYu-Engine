# XYE Core 5+100｜Scene 七文件逐成员审计 R1

- 日期：2026-10-24；分支：`审计XYE架构`；基线：`d410cd182b0bd547c917a0b9a4ebe16a5c3df4c9`。
- 生产范围：`XuanYu.Core/Scene` 的 Render（渲染）2份、State（状态）2份、Transform（变换）3份，共7份。
- 施工边界：只修改 Scene 中存在明确不变式缺口的 `SceneRenderSnapshot.cs`，其余六份源码原样保留；新增五份对位镜像测试，无调用方修改。
- 工程约束：Core 根目录模块警戒线为10；以下文件夹的直属子目录/源码文件≤5；每个生产和测试 C# 文件≤100行。只做 GitHub 静态审核，不编译运行。
- 术语：Entity（实体）、Snapshot（快照）、Render（渲染）、Preview（预览）、Commit（提交）、Transform（位置/旋转/缩放变换）、Gizmo（变换操纵器）、Record（记录类型）。

## 01 Render / ISceneRenderSnapshotSource.cs（渲染快照来源接口，8行）
- 声明接口 `ISceneRenderSnapshotSource`，没有自己实现函数，也没有字段/构造函数。
- 属性 `RenderSnapshot : SceneRenderSnapshot { get; }`：只读返回当前渲染快照。
- 事件 `RenderSnapshotChanged : Action<SceneRenderSnapshot>?`：快照变化时向订阅者推送新快照；实现类负责实际触发；问号代表允许无订阅者。
- 结论：接口合同明确，保持源文件不变。测试 `test_ISceneRenderSnapshotSource.cs` 覆盖属性与事件的一致性。

## 02 Render / SceneRenderSnapshot.cs（渲染状态快照，修改后60行）
- 类型：`public readonly record struct SceneRenderSnapshot`（只读值型记录）。
- 主构造 `SceneRenderSnapshot(SceneEntitySnapshot Entity, bool IsSelected=false, PreviewTransform? PreviewTransform=null, bool ShowMoveGizmo=false, IReadOnlyList<SceneEntitySnapshot>? RenderEntities=null, CameraState? Camera=null, bool ShowRotateGizmo=false, bool ShowScaleGizmo=false)`：打包实体、选择态、预览、Gizmo 和相机；主构造的8个参数也成为公开的init属性。
- 属性 `Entity`：主要/当前实体快照；`IsSelected`：是否标记选择。
- 属性 `PreviewTransform`：可选预览变换；`ShowMoveGizmo`：显示移动操纵器。
- 属性 `RenderEntities`：可选完整渲染实体集合；`Camera`：可选显式相机。
- 属性 `ShowRotateGizmo`：显示旋转操纵器；`ShowScaleGizmo`：显示缩放操纵器。
- 静态属性 `Empty`：含无效ID及单位变换的空快照；`TestEntityAtOrigin`：含编号1的原点测试实体。
- 计算属性 `HasEntity`：主实体ID是否有效，而不是 `RenderEntities` 是否非空。
- 计算属性 `Entities`：如果明确提供 `RenderEntities` 则原样返回；否则有有效主实体时返回仅含主实体的列表，否则为空列表。
- 计算属性 `CameraState`：有显式 `Camera` 就使用；否则回退到 `DefaultEditorCamera.Create(0)`。
- 计算属性 `RenderTransform`：主实体渲染变换；有效主实体有预览则取预览，否则取已提交变换。
- 计算属性 `RenderPosition`：`RenderTransform.Position`，用于视口中显示位置。
- 方法 `TransformFor(SceneEntitySnapshot entity)`：只对与有效主实体ID匹配的对象应用预览，其它对象返回自己的已提交变换。
- 方法 `PositionFor(SceneEntitySnapshot entity)`：同上，仅返回位置向量。
- 修复：旧逻辑可能让两个无效 `EntityId.None` 匹配，把预览套给无效实体。现 `RenderTransform`、`TransformFor`、`PositionFor` 必须确认 `HasEntity`；不改正常选中实体和渲染清单语义。
- 风险：`RenderEntities` 仅声明 `IReadOnlyList`，但外部仍可能持有并修改原集合；快照不是深冻结数据，非本轮修改。
- 记录类型由编译器合成标准值相等、`Equals`、`GetHashCode`、`Deconstruct`、`ToString` 和值复制成员；不应与手写业务函数混为一谈。

## 03 State / SceneEntitySnapshot.cs（实体状态快照，12行）
- 主构造 `SceneEntitySnapshot(EntityId EntityKey, string Name, string Type, CommittedTransform Transform)`：构建实体快照。
- 自动属性 `EntityKey`：稳定实体标识；`Name`：名称；`Type`：实体类型名称；`Transform`：已提交变换。
- 计算属性 `IsValid`：只判断 `EntityKey.IsValid`，不附带验证名称/类型/变换。
- 只读记录类型的值相等、解构等方法由编译器自动合成；无其它显式方法或字段。
- 处理：保持源文件不变，镜像测试明确锁定只按ID判定有效性的合同。

## 04 State / SceneTransformCommitResult.cs（变换提交结果，9行）
- 主构造 `SceneTransformCommitResult(EntityId EntityKey, CommittedTransform Before, CommittedTransform After, bool Changed)`：表示一次提交结果。
- 自动属性 `EntityKey`：目标实体ID；`Before`：提交前变换；`After`：提交后变换；`Changed`：调用方声明是否发生变更。
- 无显式自定义方法、静态属性或字段；普通记录方法由编译器合成。
- 风险：`Changed` 不是根据 `Before != After` 自动计算的，调用方需维持一致性。源文件不改，测试记录该合同。

## 05 Transform / CommittedTransform.cs（已提交变换，55行）
- 常量 `MinimumScale = 0.0001`：各轴缩放最小允许值。
- 构造 `CommittedTransform(Vector3d position)`：只设位置，旋转取零、缩放取各轴1。
- 构造 `CommittedTransform(Vector3d position, Vector3d rotation, Vector3d scale)`：逐项验证有限数值以及最小缩放，再保存。
- 只读属性 `Position`：三维位置；`Rotation`：三维旋转参数；`Scale`：XYZ缩放。
- 静态属性 `Identity`：单位变换，零位置/旋转及1倍缩放。
- 方法 `WithPosition(Vector3d)`：返回替换位置的新变换，保留旋转和缩放。
- 方法 `WithRotation(Vector3d)`：返回替换旋转的新变换，保留位置和缩放。
- 方法 `WithScale(Vector3d)`：返回替换缩放的新变换，保留位置和旋转。
- 私有方法 `Validate(Vector3d,string,string)`：拒绝NaN/Infinity等非有限分量。
- 私有方法 `ValidateScale(Vector3d)`：拒绝任意轴缩放小于最小值。
- 风险：值类型的 `default(CommittedTransform)` 绕过构造验证，Scale为零；应使用 `Identity` 作业务默认值。本轮不改变已有结构和默认行为。

## 06 Transform / PreviewTransform.cs（预览变换，19行）
- 构造 `PreviewTransform(Vector3d position)`：用给定位置创建默认旋转/缩放的预览变换。
- 构造 `PreviewTransform(CommittedTransform transform)`：直接接收完整已提交变换作为预览值。
- 属性 `Transform`：预览所携带的完整变换；`Position`：从 `Transform.Position` 派生的位置。
- 类型 `readonly record struct`，编译器合成值相等等标准方法；无其它显式方法/字段。
- 保持不变。与变换会话提交/撤销属于上层协作，不归入此文件。

## 07 Transform / TransformStartSnapshot.cs（变换开始快照，8行）
- 主构造 `TransformStartSnapshot(EntityId EntityKey, CommittedTransform Transform)`：保存操作开始时的目标ID和原始变换。
- 自动属性 `EntityKey`：起始实体ID；`Transform`：起始变换。
- 无显式其它函数/字段；编译器合成只读记录的值相等、解构等标准成员。
- 保持不变。

## 08 镜像测试文件与方法（均以 test_ 命名）
- `Render/test_ISceneRenderSnapshotSource.cs`：`Source.Publish(...)`（测试伪实现推送）、`PropertyAndEventPublishTheSameSnapshot()`（属性和通知一致）。
- `Render/test_SceneRenderSnapshot.cs`：`Entity(...)`（构造测试实体）、`EmptyHasNoEntitiesAndIdentityTransform()`（空快照）、`TestEntityAtOriginHasOneEntity()`（测试实体）、`RenderEntitiesOverridePrimaryFallback()`（显式渲染清单优先）、`PreviewAppliesOnlyToPrimaryId()`（预览ID匹配）、`InvalidPrimaryIdNeverReceivesPreview()`（无效实体保护）、`ExplicitCameraOverridesDefault()`（显式相机及Gizmo开关）。
- `State/test_SceneStateContracts.cs`：`EntityValidityUsesIdOnly()`（实体合法性）、`TransformCommitRecordsBeforeAfterAndChangedFlag()`（提交记录数据）、`CommitResultDoesNotCalculateChanged()`（Changed由调用方提供）。
- `Transform/test_CommittedTransform.cs`：`IdentityHasZeroPositionRotationAndUnitScale()`（单位变换）、`PositionConstructorUsesIdentityRotationAndScale()`（单参构造）、`WithMethodsModifyOnlyTheirRequestedComponent()`（三个With函数保持其它轴不变）、`RejectsNonFiniteTransformComponents()`（数值校验）、`RejectsScaleBelowMinimum()`（最小缩放）、`AcceptsMinimumScale()`（临界值）。
- `Transform/test_PreviewTransform.cs`：`PositionConstructorPreservesCommittedTransformRules()`（单参预览）、`TransformConstructorKeepsAllComponents()`（完整变换预览）、`StartSnapshotExposesEntityAndStartingTransform()`（起始快照）。

## 09 结构门禁与结论
- 生产目录：Render 2/5、State 2/5、Transform 3/5；镜像测试：Render 2/5、State 1/5、Transform 2/5。
- 生产七份均≤100行，新测试五份均≤100行；只改一份生产文件，不修改命名空间、公共方法签名、调用方或产品版本。
- Scene 本轮静态审计及成员清单完成；Build NOT RUN、xUnit NOT RUN、Windows GPU NOT RUN、Integration NOT PASS。
- 上述快照集合深冻结、default变换无效与提交结果的一致性问题为已知合同风险，延后至对应业务模块验证。
