# XYE Core｜Foundation 五个子文件夹静态收口 R1

- 日期：2026-10-24
- 分支：`审计XYE架构`
- 基线 Commit：`71c34c471878f8fde9ce97460d4336e2a25d3e76`
- 本轮唯一新增源码：`XuanYu.Core.Tests/Foundation/Identity/test_EntityId.cs`
- 生产范围：`XuanYu.Core/Foundation` 下五个三级子目录；不改动任何生产代码及调用方
- 核查办法：读取完整远端 Git tree 与八份 Foundation 源码；检查现有六份镜像测试的路径，补齐 Identity 独立测试
- 验证范围：STATIC REVIEW；编译 NOT RUN，xUnit NOT RUN，Windows/Vulkan NOT RUN，Integration NOT PASS

## 一、生产源码和镜像测试目录

| 子目录 | 生产文件及行数 | 镜像测试文件数 | 单夹≤5 / 单文件≤100 |
| --- | --- | --- | --- |
| Identity（身份） | `EntityId.cs` 30 | 1（本轮新增） | 静态 PASS |
| Math（数学） | `Vector3d.cs` 93，`YawRotation.cs` 71 | 2 | 静态 PASS |
| Properties（程序集信息） | `AssemblyInfo.cs` 3 | 0（无业务函数） | 静态 PASS |
| Results（错误与结果） | `EngineError.cs` 41，`EngineResult.cs` 45 | 2 | 静态 PASS |
| Time（时间） | `SimulationTime.cs` 60，`TimeStep.cs` 49 | 2 | 静态 PASS |

总数：五个生产子文件夹、八份生产 C# 文件；七份独立镜像测试文件。
测试源码文件名统一 `test_`，各镜像子文件夹直接文件数均不超过五个。

## 二、Identity 完整公开合同与审查

**生产文件：`XuanYu.Core/Foundation/Identity/EntityId.cs`（30 行）**

- `EntityId(int value)`：私有构造函数；禁止绕过验证构造正整数实体身份。
- `FromInt(int value)`：用正整数创建 EntityId；零和负整数抛 `ArgumentOutOfRangeException`。
- `ToString()`：有效身份显示 `EntityId(N)`，无效身份显示 `EntityId(None)`。
- `Value`：只读整数编号。
- `IsValid`：只有 `Value > 0` 才有效。
- `None`：编号为零的无效哨兵；与 `default(EntityId)` 等价。
- 类型合同：`readonly record struct`；相同编号采用值相等性，不依赖对象引用。
- 裁定：方法命名、长度、职责与边界验证清晰，本轮不修改生产文件。

**本轮新增：`XuanYu.Core.Tests/Foundation/Identity/test_EntityId.cs`（64 行）**

- `DefaultAndNoneAreInvalid()`：检查默认值与 None 语义、值与文本表示。
- `PositiveIntegerCreatesValidId(int)`：覆盖 1、42、`int.MaxValue`，校验构造、只读值及文本。
- `NonPositiveIntegerIsRejected(int)`：覆盖 0、-1、`int.MinValue` 与参数名。
- `SameValueHasValueEquality()`：验证两个相同编号的值相等、哈希一致及相等操作符。
- `DifferentValuesRemainDistinct()`：不同编号不能相等，有效 ID 不等于 None。

没有删除/迁移其它测试，避免未经定位就移动跨模块依赖测试。

## 三、其它 Foundation 子文件夹边界

- Math（数学）：`Vector3d` 负责双精度三维向量；`YawRotation` 负责围绕 Z 轴的偏航表示。已有镜像测试，不在本轮修改。Math 仍存在待调用方审计的极端数值使用风险，不能因为静态整理而宣告所有上层算法可靠。
- Properties（程序集属性）：`AssemblyInfo.cs` 仅声明 `InternalsVisibleTo("XuanYu.Core.Tests")`；不需要为了文件对称而建立无意义的测试源码。
- Results（操作结果）：`EngineError` 与 `EngineResult` 各已有 `test_` 镜像测试，结构静态符合规则。
- Time（模拟时间）：`SimulationTime` 与 `TimeStep` 各已有 `test_` 镜像测试，结构静态符合规则。

## 四、收口结论

- Foundation 五个三级子文件夹：**静态目录/文件/测试镜像整理已完成**。
- 单子文件夹文件数、单源码文件行数、Identity 的独立测试覆盖均已静态核对。
- 本轮只新增 Identity 测试及此份审计记录：生产二进制与业务调用方源码未更改。
- **运行验收未关闭**：所有新增测试仍需在 Windows .NET 10 环境执行，运行结果不可推断为 PASS。
- 下一步仍按原顺序审其它 Core 目录，调用方代码到各自目录再处理。
