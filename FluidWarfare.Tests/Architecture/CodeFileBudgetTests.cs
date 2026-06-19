namespace FluidWarfare.Tests.Architecture;

/// <summary>
/// 代码架构宪法测试。逐步推行 100 行硬线 + 每目录 ≤5 直属文件。
/// 白名单标记到期阶段。每拆一个就删除一项。
/// </summary>
public sealed class CodeFileBudgetTests
{
    /// <summary>仓库根目录（从测试项目所在目录向上查找）。</summary>
    static readonly string s_root = FindRoot();
    /// <summary>用于比较路径，确保大小写无关。</summary>
    static readonly StringComparer s_cmp = StringComparer.OrdinalIgnoreCase;

    static readonly HashSet<string> s_lineWhitelist = new(s_cmp)
    {
        // ═══ EditorShell 主线拆解（到期：8.7.6.8）═══════════════
        @"FluidWarfare.Editor.Windows\Shell\EditorShell.axaml.cs",

        // ═══ UI God 面板（到期：8.7.7A）══════════════════════════
        @"FluidWarfare.Editor.Windows\Panels\Inspector\InspectorPanel.axaml.cs",
        @"FluidWarfare.Editor.Windows\Panels\DebugDock\DebugDockPanel.axaml.cs",
        @"FluidWarfare.Editor.Windows\Panels\Viewport\ViewportPlaceholderPanel.axaml.cs",
        @"FluidWarfare.Editor.Windows\Panels\Viewport\VulkanViewportHostPanel.axaml.cs",
        @"FluidWarfare.Editor.Windows\Panels\Viewport\Input\WindowsViewportInputTranslator.cs",
        @"FluidWarfare.Editor.Windows\Panels\Viewport\NativeHost\WindowsVulkanViewportHostControl.cs",

        // UI 树面板（到期：8.7.7D）
        @"FluidWarfare.Editor.Windows\Panels\LeftDock\ProjectWorldDockPanel.axaml.cs",
        @"FluidWarfare.Editor.Windows\Panels\WorldHierarchy\WorldHierarchyTreePanel.axaml.cs",
        @"FluidWarfare.Editor.Windows\Panels\WorldHierarchy\WorldHierarchyTreeIndex.cs",
        @"FluidWarfare.Editor.Windows\Panels\ProjectContentTree\ProjectContentTreePanel.axaml.cs",
        @"FluidWarfare.Editor.Windows\Panels\ProjectContentTree\ProjectContentNodeView.cs",
        @"FluidWarfare.Editor.Windows\Preferences\EditorPreferencesWindow.axaml.cs",

        // Editor 业务层（到期：8.7.8）
        @"FluidWarfare.Editor\WorldHierarchy\WorldHierarchyTreeBuilder.cs",
        @"FluidWarfare.Editor\Input\Runtime\EditorInputBindingSnapshot.cs",
        @"FluidWarfare.Editor\Input\Actions\EditorInputActionCatalog.cs",

        // Vulkan（到期：8.7.7C）
        @"FluidWarfare.Render.Vulkan\Scene3D\Session\VulkanScene3dSession.cs",
        @"FluidWarfare.Render.Vulkan\Scene3D\VulkanScene3dRenderer.cs",
        @"FluidWarfare.Render.Vulkan\Scene3D\Session\VulkanScene3dSwapchainResources.cs",
        @"FluidWarfare.Render.Vulkan\Scene3D\VulkanScene3dVertexBuffers.cs",
        @"FluidWarfare.Render.Vulkan\Scene3D\VulkanScene3dVertex.cs",
        @"FluidWarfare.Render.Vulkan\Scene3D\VulkanScene3dPipelines.cs",
        @"FluidWarfare.Render.Vulkan\Scene3D\VulkanScene3dCommandRecorder.cs",
        @"FluidWarfare.Render.Vulkan\Scene3D\VulkanScene3dRenderResources.cs",
        @"FluidWarfare.Render.Vulkan\Scene3D\Overlay\VulkanNavigationOverlayGeometry.cs",
        @"FluidWarfare.Render.Vulkan\Scene3D\Overlay\VulkanOverlayResources.cs",
        @"FluidWarfare.Render.Vulkan\Scene3D\Overlay\VulkanOverlayPipeline.cs",
        @"FluidWarfare.Render.Vulkan\Scene3D\Depth\VulkanScene3dDepthAttachments.cs",
        @"FluidWarfare.Render.Vulkan\Context\VulkanRenderContext.cs",
        @"FluidWarfare.Render.Vulkan\Clear\VulkanClearProbe.cs",
        @"FluidWarfare.Render.Vulkan\Swapchain\VulkanSwapchainProbe.cs",
        @"FluidWarfare.Render.Vulkan\Device\VulkanDeviceProbe.cs",
        @"FluidWarfare.Render.Vulkan\Surface\VulkanSurfaceProbe.cs",
        @"FluidWarfare.Render.Vulkan\Camera\VulkanCameraMatrices.cs",
        @"FluidWarfare.Render.Vulkan\Camera\VulkanSceneRayBuilder.cs",
        @"FluidWarfare.Render.Vulkan\Instance\VulkanInstanceProbe.cs",
        @"FluidWarfare.Render.Vulkan\Validation\VulkanDebugMessengerScope.cs",
        @"FluidWarfare.Render.Vulkan\Validation\VulkanValidationAvailabilityProbe.cs",

        // Render 层（到期：8.7.7E）
        @"FluidWarfare.Render\Camera\SceneOrbitCameraMotion.cs",
        @"FluidWarfare.Render\Camera\SceneCameraPose.cs",
        @"FluidWarfare.Render\Camera\Navigation\SceneNavigationCameraMotion.cs",

        // Project / Engine（到期：8.7.8）
        @"FluidWarfare.Project\Loading\GameProjectLoader.cs",
        @"FluidWarfare.Project\Content\GameContentFileScanner.cs",
        @"FluidWarfare.Engine\World\WorldState.cs",

        // 测试文件（允许保留，不强制拆）
        @"FluidWarfare.Tests\Architecture\CodeFileBudgetTests.cs",
        @"FluidWarfare.Tests\Architecture\ProjectDependencyDirectionTests.cs",
        @"FluidWarfare.Tests\Bridge\ProjectEngine\World\ProjectContentWorldSeederTests.cs",
        @"FluidWarfare.Tests\Core\Logging\EngineLogEntryTests.cs",
        @"FluidWarfare.Tests\Core\Results\EngineResultTests.cs",
        @"FluidWarfare.Tests\Editor\Input\Bindings\EditorInputConflictDetectorTests.cs",
        @"FluidWarfare.Tests\Editor\Input\Runtime\EditorInputBindingSnapshotTests.cs",
        @"FluidWarfare.Tests\Editor\Input\Runtime\EditorInputServiceTests.cs",
        @"FluidWarfare.Tests\Editor\Transform\Gizmo\MoveGizmoHitTestTests.cs",
        @"FluidWarfare.Tests\Editor\Transform\Gizmo\PresentedMoveGizmoSnapshotTests.cs",
        @"FluidWarfare.Tests\Editor\Transform\Translation\Axis\AxisTranslationEventCountTests.cs",
        @"FluidWarfare.Tests\Editor\WorldHierarchy\WorldHierarchyTreeBuilderTests.cs",
        @"FluidWarfare.Tests\Engine\World\WorldStateTests.cs",
        @"FluidWarfare.Tests\Project\Content\GameContentFileScannerTests.cs",
        @"FluidWarfare.Tests\Project\Loading\GameProjectLoaderTests.cs",
        @"FluidWarfare.Tests\Render\Camera\SceneCameraMotionTests.cs",
        @"FluidWarfare.Tests\Render\Camera\SceneOrbitCameraMotionTests.cs",
        @"FluidWarfare.Tests\Render\Camera\Navigation\SceneNavigationCameraMotionTests.cs",
        @"FluidWarfare.Tests\Render\Scene\Position\RenderSceneObjectPositionWriterTests.cs",
        @"FluidWarfare.Tests\Render\Selection\Ground\SceneRayGroundIntersectionTests.cs",
        @"FluidWarfare.Tests\Render\Selection\Pointer\ScenePointerPickerTests.cs",
        @"FluidWarfare.Tests\Render\Selection\Presented\PresentedScenePickLifecycleTests.cs",
        @"FluidWarfare.Tests\Render\ViewportNavigation\ViewportNavigationLayoutTests.cs",
        @"FluidWarfare.Tests\Render\Vulkan\Camera\PerspectiveOrthographicPickingTests.cs",
        @"FluidWarfare.Tests\Render\Vulkan\Camera\ProjectionUnprojectionRoundTripTests.cs",
        @"FluidWarfare.Tests\Render\Vulkan\Device\VulkanDeviceInfoTests.cs",
        @"FluidWarfare.Tests\Render\Vulkan\Scene3D\VulkanScene3dInfoTests.cs",
        @"FluidWarfare.Tests\Render\Vulkan\Scene3D\VulkanScene3dRunGateTests.cs",
        @"FluidWarfare.Tests\Render\Vulkan\Scene3D\VulkanScene3dVertexTests.cs",
        @"FluidWarfare.Tests\Render\World\WorldToRenderSceneBuilderTests.cs",
    };

    static readonly HashSet<string> s_directoryWhitelist = new(s_cmp)
    {
        @"FluidWarfare.Render.Vulkan\Scene3D",
        @"FluidWarfare.Render.Vulkan\Scene3D\Session",
        @"FluidWarfare.Render.Vulkan\Scene3D\Overlay",
        @"FluidWarfare.Render.Vulkan\Validation",
        @"FluidWarfare.Render\Camera",
        @"FluidWarfare.Render\ViewportNavigation",
        @"FluidWarfare.Editor.Windows\Panels\Viewport",
        @"FluidWarfare.Editor.Windows\Panels\WorldHierarchy",
        @"FluidWarfare.Editor.Windows\Panels\ProjectContentTree",
        @"FluidWarfare.Editor.Windows\Viewport\Transform\Gizmo",
        @"FluidWarfare.Editor.Windows\Viewport\Transform\Drag",
    };

    static readonly string[] s_forbiddenNames =
        ["Manager", "Helper", "Utils", "Processor", "Factory", "Creator"];

    [Fact]
    public void AllFiles_Max100Lines()
    {
        var items = Directory.EnumerateFiles(s_root, "*.cs", SearchOption.AllDirectories);
        var bad = new List<string>();
        foreach (var f in items)
        {
            var r = Path.GetRelativePath(s_root, f);
            if (IsBuildArtifact(r)) continue;
            if (s_lineWhitelist.Contains(r)) continue;
            var lines = File.ReadAllLines(f).Length;
            if (lines > 100) bad.Add($"{r} ({lines})");
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
    public void NoForbiddenFileNames()
    {
        var items = Directory.EnumerateFiles(s_root, "*.cs", SearchOption.AllDirectories);
        var bad = new List<string>();
        foreach (var f in items)
        {
            var r = Path.GetRelativePath(s_root, f);
            if (IsBuildArtifact(r)) continue;
            var name = Path.GetFileNameWithoutExtension(f);
            foreach (var p in s_forbiddenNames)
                if (name.Contains(p, StringComparison.OrdinalIgnoreCase))
                    bad.Add($"{r} contains \"{p}\"");
        }
        Assert.Empty(bad);
    }

    static bool IsBuildArtifact(string p) =>
        p.StartsWith("FluidWarfare.Tests\\bin\\", StringComparison.Ordinal) ||
        p.StartsWith("FluidWarfare.Tests\\obj\\", StringComparison.Ordinal) ||
        p.Contains("\\bin\\Debug\\") || p.Contains("\\obj\\") ||
        p.Contains("\\bin\\Release\\");

    static string FindRoot()
    {
        var dir = AppDomain.CurrentDomain.BaseDirectory;
        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir, "FluidWarfare.sln"))) return dir;
            dir = Path.GetDirectoryName(dir);
        }
        // Fallback for IDE: solution dir is 3 levels up from test bin
        var fallback = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "..", "..");
        return Path.GetFullPath(fallback);
    }
}
