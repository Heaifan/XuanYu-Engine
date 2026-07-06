# Milestone 1 公开验收记录

创建时间：2026-06-10

## 验收目标

本文件记录 Milestone 1 第三次修复后的公开验收口径。

验收重点不是新增引擎代码，而是确认仓库结构和 Markdown 文件格式可以被 GitHub 公开页面与 Raw URL 正确读取。

## 必须验证的公开 Raw 文件

```text
https://raw.githubusercontent.com/Heaifan/FluidWarfare/main/.gitattributes
https://raw.githubusercontent.com/Heaifan/FluidWarfare/main/file-tree.md
https://raw.githubusercontent.com/Heaifan/FluidWarfare/main/docs/PROJECT_CHARTER.md
https://raw.githubusercontent.com/Heaifan/FluidWarfare/main/docs/ENGINE_ARCHITECTURE.md
https://raw.githubusercontent.com/Heaifan/FluidWarfare/main/docs/CODE_CONSTITUTION.md
```

## 本地字节级验收命令

```powershell
@'
from pathlib import Path
for p in [
    ".gitattributes",
    "file-tree.md",
    "docs/PROJECT_CHARTER.md",
    "docs/ENGINE_ARCHITECTURE.md",
    "docs/CODE_CONSTITUTION.md",
]:
    data = Path(p).read_bytes()
    print(p, "LF count =", data.count(b"\n"), "CRLF count =", data.count(b"\r\n"))
'@ | python -
```

## 公开 Raw 验收命令

```powershell
curl.exe -L https://raw.githubusercontent.com/Heaifan/FluidWarfare/main/file-tree.md |
  python -c "import sys; print(len(sys.stdin.read().splitlines()))"

curl.exe -L https://raw.githubusercontent.com/Heaifan/FluidWarfare/main/.gitattributes |
  python -c "import sys; print(len(sys.stdin.read().splitlines()))"
```

## 旧目录验收命令

```powershell
git fetch origin
git ls-tree -r --name-only origin/main
```

以下旧目录和旧文件不得出现在 `origin/main`：

```text
.dotnet_home/
.vscode/
Docs/
Prj_Graphics/
Prj_UI/
FluidWarfare.slnx
get_tree.bat
```

## 当前阶段禁止项

本轮修复不进入以下实现：

1. Core 类型。
2. ECS。
3. Vulkan。
4. Android。
5. Avalonia UI。
6. 第三方框架。
