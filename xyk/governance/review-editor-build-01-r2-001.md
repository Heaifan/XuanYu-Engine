# XYK 正式审核回执｜EDITOR-BUILD-01-R2-001

**审核日期**：2026-10-31（北京时间）；**Coordinator**：ChatGPT / XYK；**正式裁定**：ACCEPT — `STRENGTHEN`；**知识 ID**：`L-TEST-001`（原条目保持 ACTIVE/E2，不新建知识）。

## 候选身份与去重

- Candidate ID：`XYK-CAND-20261031-EDITOR-BUILD-01-R2-001`。
- 原始候选位置：产品本地工作树 `docs/knowledge/xyk-changelog.md`，`BUILD-01-R2-001` 与 `BUILD-02` 段；交接时状态 PENDING / GIT UNCOMMITTED。GitHub 现有远端产品 HEAD 不包含该本地文件，Coordinator **未直接读取本地 Candidate 文件或独立重跑 Build**。
- 去重依据：已有 `L-TEST-001` 及其 2026-10-31 R2“源路径/测试发现数防空跑”补证，但没有 `.csproj ApplicationManifest` 等 MSBuild 路径属性；新事实同属迁移导致的错误平台/构建前提，适合 STRENGTHEN，不另建 `L-TEST-002`。

## 事实和证据等级

- 施工报告：Editor.UI 首次 Release Build 因旧 `ApplicationManifest` 路径触发 `CS1926`；将 `.csproj` 值指向 `Infrastructure/Resources/app.manifest` 后，Editor/UI Release Build 报告 0W0E。
- 独立可核对的旧基线：产品分支 `codex/world-r1-migration-20261028` HEAD `a88a06aa05608bfc279d9028650749c83bae62e7` 的 `XuanYu.Editor.UI/XuanYu.Editor.UI.csproj` 仍配置 `<ApplicationManifest>app.manifest</ApplicationManifest>`，旧 `XuanYu.Editor.UI/app.manifest` 存在，而新位置不在该远端基线。
- 该对照仅能证明远端旧基线状态，不能替代本地 BUILD-01/BUILD-02 日志，也不能证明产品修复已经提交/推送。0W0E 是已交接报告，不是本次独立复现；SourceProductCommit=`-`。

## 正式知识规则

物理路径迁移后，不能只检查 C# 编译文件枚举、测试发现与 AXAML URI；还要检查 `.csproj`、导入的 `.props/.targets` 和 MSBuild 路径型属性或项目项（含 `ApplicationManifest`）。普通项目相对路径按项目目录解析，导入文件/宏/条件属性按实际 MSBuild 求值解析，核实目标文件存在，再走经授权的真实目标 Release Build 检查有效性。Build 0W0E不等于真实UI/全仓Integration PASS。

## 正式入库文件与身份

- 知识正文（XYK 独立正式路径）：`xyk/experience/lessons.md`，`L-TEST-001` 的“2026-10-31 正式强化：物理路径迁移必须核验 MSBuild 路径属性并完成真实 Build”段。
- 用户交接指定路径的 XYK 分支镜像：`docs/knowledge/lessons.md`，同 ID、同标题强化段。
- 正式索引：`xyk/index/knowledge-index.md`；交接指定索引镜像：`docs/knowledge/knowledge-index.md`；专项目录：`xyk/index/lesson-index.md`。
- K Event：`K-20261031-R5-001`（STRENGTHEN，Product Commit=`-`）；K 正文 Git Commit：`faa6fc825cf001bd4448722b48ca87cb56d92166`。
- 本审核回执、索引与事件由另一个继承上述正文提交的 `xyk/main` 后续 Commit 正式固化；以最终远端 HEAD 为准。

## 状态解释

**XYK 正式入库：ACCEPT / STRENGTHEN / WRITTEN（以远端 Commit 核验为准）。** **产品本地 Changelog：PENDING、GIT UNCOMMITTED**，未授权在本轮修改/提交 XYE 产品工作树。两个状态可以同时成立，不得混写。
