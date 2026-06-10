# FluidWarfare 命名规则

创建时间：2026-06-10

## C# 命名

使用标准 C# 命名风格。

| 项目 | 规则 |
|---|---|
| 命名空间 | `FluidWarfare.*` |
| 类、结构体、枚举、记录 | PascalCase |
| 方法、属性 | PascalCase |
| 私有字段 | `_camelCase` 或 camelCase |
| 接口 | `I` + PascalCase |
| 文件 | 与主类型名称一致 |

## 禁止的 C# 前缀

C# 代码中不得使用以下前缀：

```text
cls_
fuc_
var_
obj_
str_
int_
```

不得使用旧命名空间或旧项目名：

```text
BingWuChangShiEngine
Bwc.*
```

## 数据与资源文件前缀

数据和资源文件允许使用领域前缀。

| 前缀 | 含义 |
|---|---|
| `cfg_` | 配置 |
| `dat_` | 数据表 |
| `scn_` | 场景 |
| `rpl_` | 回放 |
| `log_` | 日志 |
| `mesh_` | 网格 |
| `tex_` | 贴图 |
| `mat_` | 材质 |
| `shd_` | Shader 源文件 |
| `spv_` | 编译后的 Shader |
| `loc_` | 本地化 |

示例：

```text
game_data/scn_phase1_test_field.json
```

## 中文化规则

FluidWarfare 遵循“机器读英文，人类读中文”。

保持英文的内容：

1. 命名空间。
2. 类名。
3. 方法名。
4. 文件名。
5. 测试方法名。
6. `EngineError.Code`。
7. 程序内部枚举名。

使用中文的内容：

1. `EngineError.Message`。
2. 异常 message。
3. 日志 message。
4. 编辑器提示。
5. 导出提示。
6. 验收结果。
7. 文档说明。
8. 开发任务说明。

错误代码示例：`Core.InvalidArgument`。

错误信息示例：`参数无效。`。
