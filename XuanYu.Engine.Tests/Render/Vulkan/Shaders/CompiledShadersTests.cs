using FluidWarfare.Render.Vulkan.Shaders;

namespace FluidWarfare.Tests.Render.Vulkan.Shaders;

[Collection("CompiledShadersState")]
public sealed class CompiledShadersTests
{
    public CompiledShadersTests()
    {
        // 确保测试前重置为干净的默认状态
        CompiledShaders.Reset();
    }

    [Fact]
    public void HasValidatedBasic3dShaders_ShouldBeFalse_WhenArraysEmpty()
    {
        CompiledShaders.Reset();
        Assert.False(CompiledShaders.HasValidatedBasic3dShaders);
    }

    [Fact]
    public void HasValidatedBasic3dShaders_ShouldBeFalse_WhenVertMissing()
    {
        CompiledShaders.Reset();
        CompiledShaders.Basic3dFrag = new uint[] { 0x07230203 };
        CompiledShaders.Basic3dFragmentValidatedBySpirvVal = true;
        Assert.False(CompiledShaders.HasValidatedBasic3dShaders);
    }

    [Fact]
    public void HasValidatedBasic3dShaders_ShouldBeFalse_WhenFragMissing()
    {
        CompiledShaders.Reset();
        CompiledShaders.Basic3dVert = new uint[] { 0x07230203 };
        CompiledShaders.Basic3dVertexValidatedBySpirvVal = true;
        Assert.False(CompiledShaders.HasValidatedBasic3dShaders);
    }

    [Fact]
    public void HasValidatedBasic3dShaders_ShouldBeFalse_WhenVertNotValidated()
    {
        CompiledShaders.Reset();
        CompiledShaders.Basic3dVert = new uint[] { 0x07230203 };
        CompiledShaders.Basic3dFrag = new uint[] { 0x07230203 };
        CompiledShaders.Basic3dFragmentValidatedBySpirvVal = true;
        Assert.False(CompiledShaders.HasValidatedBasic3dShaders);
    }

    [Fact]
    public void HasValidatedBasic3dShaders_ShouldBeTrue_WhenBothPresentAndValidated()
    {
        CompiledShaders.Reset();
        CompiledShaders.Basic3dVert = new uint[] { 0x07230203 };
        CompiledShaders.Basic3dFrag = new uint[] { 0x07230203 };
        CompiledShaders.Basic3dVertexValidatedBySpirvVal = true;
        CompiledShaders.Basic3dFragmentValidatedBySpirvVal = true;
        Assert.True(CompiledShaders.HasValidatedBasic3dShaders);
    }

    [Fact]
    public void Reset_ShouldClearAllState()
    {
        CompiledShaders.Basic3dVert = new uint[] { 0x07230203 };
        CompiledShaders.Basic3dFrag = new uint[] { 0x07230203 };
        CompiledShaders.Basic3dVertexValidatedBySpirvVal = true;
        CompiledShaders.Basic3dFragmentValidatedBySpirvVal = true;

        CompiledShaders.Reset();

        Assert.Empty(CompiledShaders.Basic3dVert);
        Assert.Empty(CompiledShaders.Basic3dFrag);
        Assert.False(CompiledShaders.Basic3dVertexValidatedBySpirvVal);
        Assert.False(CompiledShaders.Basic3dFragmentValidatedBySpirvVal);
        Assert.False(CompiledShaders.HasValidatedBasic3dShaders);
    }
}
