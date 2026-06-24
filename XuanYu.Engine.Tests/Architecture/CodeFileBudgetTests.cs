namespace FluidWarfare.Tests.Architecture;

/// <summary>
/// 代码架构宪法测试。逐步推行 100 行硬线 + 每目录 ≤5 直属文件。
/// 白名单标记到期阶段。每拆一个就删除一项。
/// </summary>
public sealed class CodeFileBudgetTests
{
    /// <summary>仓库根目录（从测试项目所在目录向上查找）。</summary>
    static readonly string s_root = FindRoot();
// ── 白名单债务计数器 ──────────────────────────────────
    // 每次新增白名单条目必须同时递增此值。
    // 目的：防止 AI 无监督扩大债务。
    // 每次新增白名单条目必须同步递增
    const int LineWhitelistBudget = 49;
    const int DirectoryWhitelistBudget = 3;

    static readonly HashSet<string> s_lineWhitelist = new(StringComparer.OrdinalIgnoreCase)
    {
        // Editor 业务层（到期：8.7.8）

        // Render 层（到期：8.7.7E）
        @"XuanYu.Engine.Render\Camera\Orbit\SceneOrbitCameraMotion.cs",
        @"XuanYu.Engine.Render\Camera\Navigation\SceneNavigationCameraMotion.cs",

        // 测试文件（允许保留，不强制拆）
        @"XuanYu.Engine.Tests\Architecture\CodeFileBudgetTests.cs",
        @"XuanYu.Engine.Tests\Architecture\ProjectDependencyDirectionTests.cs",
        @"XuanYu.Engine.Tests\Bridge\ProjectEngine\World\ProjectContentWorldSeederTests.cs",
        @"XuanYu.Engine.Tests\Core\Logging\EngineLogEntryTests.cs",
        @"XuanYu.Engine.Tests\Core\Results\EngineResultTests.cs",
        @"XuanYu.Engine.Tests\Editor\Input\Bindings\EditorInputConflictDetectorTests.cs",
        @"XuanYu.Engine.Tests\Editor\Input\Runtime\EditorInputBindingSnapshotTests.cs",
        @"XuanYu.Engine.Tests\Editor\Input\Runtime\EditorInputServiceTests.cs",
        @"XuanYu.Engine.Tests\Editor\Transform\Gizmo\MoveGizmoHitTestTests.cs",
        @"XuanYu.Engine.Tests\Editor\Transform\Gizmo\PresentedMoveGizmoSnapshotTests.cs",
        @"XuanYu.Engine.Tests\Editor\Transform\Translation\Axis\AxisTranslationEventCountTests.cs",
        @"XuanYu.Engine.Tests\Editor\WorldHierarchy\WorldHierarchyTreeBuilderTests.cs",
        @"XuanYu.Engine.Tests\Engine\World\WorldStateTests.cs",
        @"XuanYu.Engine.Tests\Project\Content\GameContentFileScannerTests.cs",
        @"XuanYu.Engine.Tests\Project\Loading\GameProjectLoaderTests.cs",
        @"XuanYu.Engine.Tests\Render\Camera\SceneCameraMotionTests.cs",
        @"XuanYu.Engine.Tests\Render\Camera\SceneOrbitCameraMotionTests.cs",
        @"XuanYu.Engine.Tests\Render\Camera\Navigation\SceneNavigationCameraMotionTests.cs",
        @"XuanYu.Engine.Tests\Render\Scene\Position\RenderSceneObjectPositionWriterTests.cs",
        @"XuanYu.Engine.Tests\Render\Selection\Ground\SceneRayGroundIntersectionTests.cs",
        @"XuanYu.Engine.Tests\Render\Selection\Pointer\ScenePointerPickerTests.cs",
        @"XuanYu.Engine.Tests\Render\Selection\Presented\PresentedScenePickLifecycleTests.cs",
        @"XuanYu.Engine.Tests\Render\ViewportNavigation\ViewportNavigationLayoutTests.cs",
        @"XuanYu.Engine.Tests\Render\Vulkan\Camera\PerspectiveOrthographicPickingTests.cs",
        @"XuanYu.Engine.Tests\Render\Vulkan\Camera\ProjectionUnprojectionRoundTripTests.cs",
        @"XuanYu.Engine.Tests\Render\Vulkan\Device\VulkanDeviceInfoTests.cs",
        @"XuanYu.Engine.Tests\Render\Vulkan\Scene3D\VulkanScene3dInfoTests.cs",
        @"XuanYu.Engine.Tests\Render\Vulkan\Scene3D\VulkanScene3dRunGateTests.cs",
        @"XuanYu.Engine.Tests\Render\Vulkan\Scene3D\VulkanScene3dVertexTests.cs",
        @"XuanYu.Engine.Tests\Render\World\WorldToRenderSceneBuilderTests.cs",
    };

    static readonly HashSet<string> s_directoryWhitelist = new(StringComparer.OrdinalIgnoreCase)
    {
        // (No directory whitelist entries remain for Scene3D/Navigation)
    };

    static readonly string[] s_forbiddenNames =
        ["Manager", "Helper", "Utils", "Processor", "Factory", "Creator"];

    [Fact]
    public void WhitelistBudget_NotExceeded()
    {
        Assert.True(s_lineWhitelist.Count <= LineWhitelistBudget,
            $"Line whitelist {s_lineWhitelist.Count} > budget {LineWhitelistBudget}");
        Assert.True(s_directoryWhitelist.Count <= DirectoryWhitelistBudget,
            $"Dir whitelist {s_directoryWhitelist.Count} > budget {DirectoryWhitelistBudget}");
    }

    [Fact]
    public void ProductionWhitelist_OnlyApproved()
    {
        var expected = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            @"XuanYu.Engine.Render\Camera\Orbit\SceneOrbitCameraMotion.cs",
            @"XuanYu.Engine.Render\Camera\Navigation\SceneNavigationCameraMotion.cs",
        };

        var actual = new HashSet<string>(
            s_lineWhitelist.Where(e =>
                !e.StartsWith("XuanYu.Engine.Tests", StringComparison.OrdinalIgnoreCase)),
            StringComparer.OrdinalIgnoreCase);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GlobalUsings_Max100Lines()
    {
        var path = Path.Combine(s_root, @"XuanYu.Engine.Editor.Windows\GlobalUsings.cs");
        var lines = File.ReadAllLines(path).Length;
        Assert.True(lines <= 100,
            $"GlobalUsings.cs has {lines} lines, exceeds 100-line limit");
    }

    [Fact]
    public void EditorShellContext_Max95Lines()
    {
        var path = Path.Combine(s_root,
            @"XuanYu.Engine.Editor.Windows\Shell\Composition\Core\EditorShellContext.cs");
        var lines = File.ReadAllLines(path).Length;
        Assert.True(lines <= 95,
            $"EditorShellContext.cs has {lines} lines, exceeds 95-line redline — must split before adding new fields");
    }

    [Fact]
    public void EditorShell_NotInWhitelist()
    {
        var shellEntries = s_lineWhitelist.Where(e =>
            e.StartsWith("XuanYu.Engine.Editor.Windows\\Shell", StringComparison.OrdinalIgnoreCase));
        Assert.Empty(shellEntries);
    }

    [Fact]
    public void DirectoryWhitelist_RemainsZero()
    {
        Assert.Empty(s_directoryWhitelist);
    }

    [Fact]
    public void ProductionFiles_Max100Lines()
    {
        var items = Directory.EnumerateFiles(s_root, "*.cs", SearchOption.AllDirectories);
        var bad = new List<string>();
        foreach (var f in items)
        {
            var r = Path.GetRelativePath(s_root, f);
            if (IsBuildArtifact(r)) continue;
            if (r.StartsWith("XuanYu.Engine.Tests", StringComparison.OrdinalIgnoreCase)) continue; // tests have own rule
            if (s_lineWhitelist.Contains(r)) continue;
            var lines = File.ReadAllLines(f).Length;
            if (lines > 100) bad.Add($"{r} ({lines})");
        }
        Assert.Empty(bad);
    }

    [Fact]
    public void TestFiles_Max180Lines()
    {
        // 测试文件不强制 100 行，但不得无限增长（软上限 180）
        var items = Directory.EnumerateFiles(s_root, "*.cs", SearchOption.AllDirectories);
        var bad = new List<string>();
        foreach (var f in items)
        {
            var r = Path.GetRelativePath(s_root, f);
            if (IsBuildArtifact(r)) continue;
            if (!r.StartsWith("XuanYu.Engine.Tests", StringComparison.OrdinalIgnoreCase)) continue;
            // Architecture tests are allowed to be larger
            if (r.StartsWith("XuanYu.Engine.Tests\\Architecture", StringComparison.OrdinalIgnoreCase)) continue;
            if (s_lineWhitelist.Contains(r)) continue; // uses same whitelist
            var lines = File.ReadAllLines(f).Length;
            if (lines > 180) bad.Add($"{r} ({lines})");
        }
        Assert.Empty(bad);
    }

    [Fact]
    public void AllDirectories_Max5CsFiles()
    {
        var dirs = Directory.EnumerateDirectories(s_root, "*", SearchOption.AllDirectories);
        var bad = new List<string>();
        foreach (var d in dirs)
        {
            var r = Path.GetRelativePath(s_root, d);
            if (IsBuildArtifact(r)) continue;
            if (s_directoryWhitelist.Contains(r)) continue;
            var n = Directory.GetFiles(d, "*.cs").Length;
            if (n > 5) bad.Add($"{r} ({n})");
        }
        Assert.Empty(bad);
    }

    [Fact]
    public void NoForbiddenNames_FileAndType()
    {
        var items = Directory.EnumerateFiles(s_root, "*.cs", SearchOption.AllDirectories);
        var bad = new List<string>();
        foreach (var f in items)
        {
            var r = Path.GetRelativePath(s_root, f);
            if (IsBuildArtifact(r)) continue;
            // 跳过架构测试文件自身（它包含禁用词列表的定义）
            if (r.StartsWith("XuanYu.Engine.Tests\\Architecture", StringComparison.OrdinalIgnoreCase))
                continue;

            // 检查文件名
            var name = Path.GetFileNameWithoutExtension(f);
            foreach (var p in s_forbiddenNames)
                if (name.Contains(p, StringComparison.OrdinalIgnoreCase))
                    bad.Add($"{r}: filename contains \"{p}\"");

            // 检查类型声明
            var content = File.ReadAllText(f);
            foreach (var p in s_forbiddenNames)
            {
                var pattern = $@"\b(class|record|struct|interface)\s+\w*{p}\w*";
                if (System.Text.RegularExpressions.Regex.IsMatch(content, pattern))
                    bad.Add($"{r}: type name contains \"{p}\"");
            }
        }
        Assert.Empty(bad);
    }

    static bool IsBuildArtifact(string p) =>
        p.StartsWith("XuanYu.Engine.Tests\\bin\\", StringComparison.Ordinal) ||
        p.StartsWith("XuanYu.Engine.Tests\\obj\\", StringComparison.Ordinal) ||
        p.Contains("\\bin\\Debug\\") || p.Contains("\\obj\\") ||
        p.Contains("\\bin\\Release\\");

    static string FindRoot()
    {
        var dir = AppDomain.CurrentDomain.BaseDirectory;
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir, "XuanYu.Engine.sln"))) return dir;
            dir = Path.GetDirectoryName(dir);
        }
        // Fallback for IDE: solution dir is 3 levels up from test bin
        var fallback = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "..", "..");
        return Path.GetFullPath(fallback);
    }
}
