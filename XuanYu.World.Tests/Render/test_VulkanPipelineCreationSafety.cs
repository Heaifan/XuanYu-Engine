using System.IO;
using System.Reflection;
using XuanYu.Render.Vulkan.Pipeline;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanPipelineCreationSafetyTests
{
    [Fact]
    public void Log_failure_rolls_back_created_resource_and_preserves_the_failure()
    {
        var rollbackCount = 0;
        var type = typeof(VulkanGraphicsPipelineOwner).Assembly.GetType(
            "XuanYu.Render.Vulkan.Pipeline.VulkanPipelineCreationCompletion");
        Assert.NotNull(type);
        var method = type.GetMethod("Complete", BindingFlags.Static | BindingFlags.NonPublic);
        Assert.NotNull(method);
        var failure = Assert.Throws<TargetInvocationException>(() => method
            .MakeGenericMethod(typeof(object)).Invoke(null,
            [new object(), (Action<string>)(_ => throw new InvalidOperationException("log failed")),
                (Action)(() => rollbackCount++), new[] { "created" }]));

        Assert.IsType<InvalidOperationException>(failure.InnerException);
        Assert.Equal("log failed", failure.InnerException!.Message);
        Assert.Equal(1, rollbackCount);
    }

    [Fact]
    public void Grid_pipeline_set_rolls_back_owners_in_reverse_creation_order()
    {
        var source = ReadPipelineFile("Sets", "GridPipelineSet.cs");
        var create = source.IndexOf("public static GridPipelineSet Create", StringComparison.Ordinal);
        var catcher = source.IndexOf("catch (Exception", create, StringComparison.Ordinal);
        var rollback = catcher < 0
            ? Enumerable.Repeat(-1, 6).ToArray()
            : new[] { "viewPlaneGrid", "scaleIndicator", "navGizmo", "origin", "axes", "grid" }
                .Select(name => source.IndexOf($"{name}?.Dispose()", catcher, StringComparison.Ordinal))
                .ToArray();

        Assert.True(create >= 0 && catcher > create && rollback.All(index => index > catcher),
            "Every locally created pipeline owner must be disposed if set creation fails.");
        Assert.True(rollback.Zip(rollback.Skip(1), (left, right) => left < right).All(value => value),
            "Pipeline owners must be released in reverse creation order.");
    }

    static string ReadPipelineFile(params string[] path)
    {
        var segments = new[] { Path.GetFullPath(TestSourceRoot.Root), "XuanYu.Render.Vulkan", "Pipeline" }
            .Concat(path).ToArray();
        var fullPath = Path.Combine(segments);
        Assert.True(File.Exists(fullPath), $"文件缺失：{fullPath}");
        return File.ReadAllText(fullPath);
    }
}
