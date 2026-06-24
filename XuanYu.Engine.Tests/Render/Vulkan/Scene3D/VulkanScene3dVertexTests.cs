using FluidWarfare.Render.Vulkan.Scene3D;

namespace FluidWarfare.Tests.Render.Vulkan.Scene3D;

public sealed class VulkanScene3dVertexTests
{
    [Fact]
    public void BuildGrid_ShouldGenerateCorrectVertexCount()
    {
        // Range -20 to +20, spacing 2: 21 values per axis
        // X lines: 21 * 2 vertices = 42
        // Y lines: 21 * 2 vertices = 42
        // Total: 84
        var vertices = VulkanScene3dVertices.BuildGrid(20, 2);
        Assert.Equal(84, vertices.Length);
    }

    [Fact]
    public void BuildGrid_ShouldUseDefaultZOffset()
    {
        var vertices = VulkanScene3dVertices.BuildGrid(10, 5);
        foreach (var v in vertices)
        {
            Assert.Equal(-0.01f, v.Z);
        }
    }

    [Fact]
    public void BuildGrid_ShouldAllowCustomZOffset()
    {
        var vertices = VulkanScene3dVertices.BuildGrid(10, 5, groundOffsetZ: 0);
        foreach (var v in vertices)
        {
            Assert.Equal(0, v.Z);
        }
    }

    [Fact]
    public void BuildCube_ShouldHave36Vertices()
    {
        var vertices = VulkanScene3dVertices.BuildCube(0, 0, 0, 1);
        Assert.Equal(36, vertices.Length); // 12 triangles * 3
    }

    [Fact]
    public void BuildCube_ShouldUseDefaultYellowColor()
    {
        var vertices = VulkanScene3dVertices.BuildCube(0, 0.5f, 0, 1);
        foreach (var v in vertices)
        {
            Assert.Equal(1.00f, v.R);
            Assert.Equal(0.82f, v.G);
            Assert.Equal(0.20f, v.B);
            Assert.Equal(1.00f, v.A);
        }
    }

    [Fact]
    public void BuildCube_Center_ShouldBeAtCorrectPosition()
    {
        var vertices = VulkanScene3dVertices.BuildCube(5, 2, 3, 2);
        // All vertices should be between the bounds
        foreach (var v in vertices)
        {
            Assert.InRange(v.X, 4, 6);
            Assert.InRange(v.Y, 1, 3);
            Assert.InRange(v.Z, 2, 4);
        }
    }

    [Fact]
    public void BuildAxes_ShouldHave6Vertices()
    {
        var vertices = VulkanScene3dVertices.BuildAxes(20, 8);
        Assert.Equal(6, vertices.Length); // X, Y, Z each as a line
    }

    [Fact]
    public void BuildAxes_XAxis_IsRed()
    {
        var vertices = VulkanScene3dVertices.BuildAxes(20, 8);
        // X axis: first 2 vertices, red (1,0,0)
        Assert.Equal(1f, vertices[0].R);
        Assert.Equal(0f, vertices[0].G);
        Assert.Equal(0f, vertices[0].B);
    }

    [Fact]
    public void BuildAxes_YAxis_IsGreen()
    {
        var vertices = VulkanScene3dVertices.BuildAxes(20, 8);
        // Y axis: vertices 2-3, green (0,1,0)
        Assert.Equal(0f, vertices[2].R);
        Assert.Equal(1f, vertices[2].G);
        Assert.Equal(0f, vertices[2].B);
    }

    [Fact]
    public void BuildAxes_ZAxis_IsBlue()
    {
        var vertices = VulkanScene3dVertices.BuildAxes(20, 8);
        // Z axis: vertices 4-5, blue (0,0,1)
        Assert.Equal(0f, vertices[4].R);
        Assert.Equal(0f, vertices[4].G);
        Assert.Equal(1f, vertices[4].B);
    }

    [Fact]
    public void BuildAxes_ZAxis_StartsFromOrigin()
    {
        var vertices = VulkanScene3dVertices.BuildAxes(20, 8);
        // Z axis starts at origin (0,0,0) and goes up
        Assert.Equal(0, vertices[4].X);
        Assert.Equal(0, vertices[4].Y);
        Assert.Equal(0, vertices[4].Z);
        Assert.Equal(8, vertices[5].Z);
    }

    [Fact]
    public void ToInterleaved_ShouldConvertCorrectly()
    {
        var verts = new VulkanScene3dVertex[]
        {
            new(1, 2, 3, 0.5f, 0.6f, 0.7f, 1),
            new(4, 5, 6, 0.8f, 0.9f, 1.0f, 1)
        };
        var interleaved = VulkanScene3dVertices.ToInterleaved(verts);
        Assert.Equal(14, interleaved.Length); // 2 * 7
        Assert.Equal(1, interleaved[0]);
        Assert.Equal(2, interleaved[1]);
        Assert.Equal(3, interleaved[2]);
        Assert.Equal(0.5f, interleaved[3]);
        Assert.Equal(0.6f, interleaved[4]);
        Assert.Equal(0.7f, interleaved[5]);
        Assert.Equal(1f, interleaved[6]);
    }
}
