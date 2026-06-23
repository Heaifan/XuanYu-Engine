# 8.7.8C-1 — GameProjectLoader 拆分审计

审计日期：2026-06-23
目标文件：`FluidWarfare.Project/Loading/GameProjectLoader.cs`
目标行数：392 行

---

## 1. 当前文件状态

| 维度 | 值 |
|------|-----|
| **文件路径** | `FluidWarfare.Project/Loading/GameProjectLoader.cs` |
| **行数** | 392 行 |
| **类型** | `public static partial class` — 纯静态，无实例状态 |
| **依赖目录** | `Loading/` 当前 2 文件（Loader + LoadResult），余量 +3 |
| **白名单状态** | ✅ 在债务路线图 A 类中（未列入代码宪法白名单） |

## 2. 职责拆解

### 职责 A：Project manifest JSON 读取 + 顶层校验（~46 行）

```
LoadFromDirectory() 中：
  28-38  File.ReadAllText + JsonSerializer.Deserialize<ProjectManifestDto>
  39-51  JsonException / NotSupportedException / null 捕获
  53-63  SchemaVersion 校验（null + 版本号）
  65-73  ProjectId + DisplayName 非空校验
```

### 职责 B：Content folder JSON 解析（~106 行）

子方法及行数：
| 方法 | 行数范围 | 行数 | 职责 |
|------|----------|------|------|
| `LoadContentFolders` | 131-169 | 39 | 解析 JSON 数组，外层循环 |
| `LoadContentFolder` | 171-237 | 67 | 单个 folder 对象：name→displayName→description→contentKind→extensions |
| `LoadAllowedExtensions` | 239-276 | 38 | 解析 `allowedExtensions` 数组 + 正则校验 |

合计 **~144 行**，是文件最重的部分。

### 职责 C：目录存在性校验 + 未声明目录检测（~30 行）

```
LoadFromDirectory() 中：
  87-96   检查声明目录在磁盘上是否存在
  99-106  枚举根目录，标记未声明的一级目录
```

### 职责 D：结果构建 + 工具方法 + 正则（~80 行）

| 品类 | 位置 | 行数 |
|------|------|------|
| 成功结果构建 (`GameProjectInfo`) | 120-128 | 9 |
| `FindAllUndeclaredDirectories` | 281-292 | 12 |
| `GetString` / `GetBoolean` | 294-305 | 12 |
| `Fail` / `FailResult` / `WithReport` | 307-327 | 21 |
| `GeneratedRegex` 3 个 | 329-336 | 8 |

### 内部 record 类型（4 个，~54 行）

| 类型 | 行数 | 用途 |
|------|------|------|
| `ProjectManifestDto` | 6 | JSON 反序列化 DTO |
| `ContentFoldersLoadResult` | 22 | 文件夹列表解析中间结果 + Fail 工厂 |
| `ContentFolderLoadResult` | 14 | 单个文件夹解析中间结果 + Fail 工厂 |
| `AllowedExtensionsLoadResult` | 9 | 扩展名解析中间结果 + Fail 工厂 |

### 职责饼图（估算）

```
职责 A：JSON 读取 + 顶层校验   46 行  (12%)
职责 B：Content folder 解析    144 行 (37%)  ← 最重
职责 C：目录校验                30 行  (8%)
职责 D：结果构建 + 工具 + 正则   80 行  (20%)
内部 record 类型                  54 行  (14%)
注释/空行/using                  38 行  (10%)
```

## 3. 调用点

| 位置 | 用途 |
|------|------|
| `ProjectBootstrapRoute.cs:22` | Editor 启动时加载 SampleProject |
| `GameProjectLoaderTests.cs` (24 个测试方法) | 单元测试全覆盖 |
| `SampleProjectSmokeTests.cs:12` | 冒烟测试验证真实 SampleProject |

调用签名：`GameProjectLoader.LoadFromDirectory(string projectDirectory)` → `GameProjectLoadResult`

**调用链：** EditorShell → ProjectBootstrapRoute → GameProjectLoader → (内部) GameContentFileScanner

GameProjectLoader 不直接依赖任何 Vulkan 或 Render 类型，**边界非常清楚**。

## 4. 外部依赖类型

| 类型 | 位置 | 行数 |
|------|------|------|
| `GameProjectLoadResult` | `Loading/GameProjectLoadResult.cs` | 11 |
| `GameProjectInfo` | `Metadata/GameProjectInfo.cs` | 12 |
| `GameContentFolderInfo` | `Content/GameContentFolderInfo.cs` | 10 |
| `GameContentFileScanner` | `Content/GameContentFileScanner.cs` | — |
| `ProjectValidationReport` | `Validation/ProjectValidationReport.cs` | — |
| `ProjectValidationIssue` | `Validation/ProjectValidationIssue.cs` | — |

所有依赖类型均在同一项目（`FluidWarfare.Project`）内，无跨项目耦合。

## 5. 现有测试覆盖

测试文件：`GameProjectLoaderTests.cs`（613 行）

| 测试类别 | 测试数 | 覆盖内容 |
|----------|--------|----------|
| 成功路径 | 3 | 正常加载 + contentFiles + 内容目录 |
| 目录/文件缺失 | 2 | 目录不存在、manifest 不存在 |
| JSON 格式错误 | 1 | 无效 JSON |
| 顶层字段校验 | 4 | ProjectId/DisplayName/SchemaVersion 缺失/不支持 |
| Content folder 校验 | 8 | folderName/displayName/description/contentKind 缺失 |
| Extension 校验 | 3 | 格式错误 |
| 目录校验报告 | 4 | 目录不存在、未声明目录、嵌套目录、扩展名拒绝 |

**测试覆盖率高，可以作为拆分的信心锚点。**

## 6. 风险点

| 风险 | 级别 | 说明 |
|------|------|------|
| **Content folder 解析超重** | 低 | ~144 行需要分到 2 个文件（主解析 + extensions） |
| **内部 record 类型迁移** | 低 | 4 个 record 需要分配到新文件，小心命名冲突 |
| **正则表达式迁移** | 低 | 3 个 `[GeneratedRegex]` 需要跟随职责迁移 |
| **GameContentFileScanner 不在此文件** | 无 | Scanner 已是独立文件，无需移动 |
| **ProjectBootstrapRoute 调用** | 无 | 签名不变，调用方不改 |
| **测试** | 低 | 只需更新 using/namespace 引用，不改测试逻辑 |

## 7. 推荐拆分方案

### 拆成 4 文件（Loading/ 目录 2→5 文件，≤5 不超限）

| # | 新文件 | 预计行数 | 职责 |
|---|--------|----------|------|
| 1 | `GameProjectLoader.cs` **（门面）** | ≤100 | `LoadFromDirectory()` 主编排 + 成功结果构建 + 目录校验 |
| 2 | `GameProjectManifestReader.cs` **（新增）** | ≤80 | 读取 manifest.json + 反序列化 + SchemaVersion/ProjectId/DisplayName 校验 + `ProjectManifestDto` |
| 3 | `GameProjectFolderParser.cs` **（新增）** | ≤100 | `ContentFolders` 数组 + 单个 folder 解析 + 查重 + name/displayName/description/contentKind 正则校验 + 内部 record 类型 |
| 4 | `GameProjectExtensionParser.cs` **（新增）** | ≤60 | `allowedExtensions` 数组解析 + 正则校验 + `AllowedExtensionsLoadResult` |

保留现有 `GameProjectLoadResult.cs`（11 行，不动）。

### 门面逻辑示意

```
GameProjectLoader.LoadFromDirectory(dir)
  → if (!Directory.Exists(dir)) → Fail
  → manifest = GameProjectManifestReader.Read(dir)
  → if (manifest has errors) → Fail
  → folders = GameProjectFolderParser.Parse(manifest.ContentFolders)
  → if (folders has errors) → Fail
  → 校验目录存在性 + 未声明目录
  → scanResult = GameContentFileScanner.Scan(...)
  → 构建 GameProjectLoadResult (成功或失败 + 报告)
```

### 拆分后 Loading/ 目录文件

```
FluidWarfare.Project/Loading/
  ├── GameProjectLoadResult.cs      (11 行，不动)
  ├── GameProjectLoader.cs          (≤100 行，门面)
  ├── GameProjectManifestReader.cs  (新增，≤80 行)
  ├── GameProjectFolderParser.cs    (新增，≤100 行)
  └── GameProjectExtensionParser.cs (新增，≤60 行)
```

合计 5 文件 = 目录上限 ✅

## 8. 结论

| 维度 | 结论 |
|------|------|
| **是否可以拆** | ✅ **可以拆** |
| **推荐方案** | **4 文件**（门面 + ManifestReader + FolderParser + ExtensionParser） |
| **风险** | **低** — 纯数据解析，无 Vulkan/无窗口/无 Render 依赖 |
| **调用方改动** | **无** — `LoadFromDirectory()` 签名不变 |
| **Loading/ 目录** | 5 文件 = 上限，不超限 |
| **下一轮行动** | 8.7.8C-2 — 执行拆分 |
| **建议先** | 创建 3 个新文件，逐个提取职责，最后精简门面 |
