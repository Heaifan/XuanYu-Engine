using XuanYu.Engine.Bridge.ProjectEngine.World;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.World;
using XuanYu.Engine.Project.Content;

namespace XuanYu.Engine.Tests.Bridge.ProjectEngine.World;

public sealed class ProjectContentWorldSeederTests
{
    [Fact]
    public void SeedUnitTemplatePlaceholders_WithUnitTemplateFile_ShouldCreateEntity()
    {
        var world = new WorldState();
        var contentFiles = new List<GameContentFileInfo>
        {
            new("units", "unitTemplate", "sample_unit.json", "units/sample_unit.json", ".json")
        };

        var result = ProjectContentWorldSeeder.SeedUnitTemplatePlaceholders(world, contentFiles);

        Assert.Equal(1, result.CreatedEntityCount);
        Assert.Single(result.SourcePaths);
        Assert.Single(world.ListEntities());
    }

    [Fact]
    public void SeedUnitTemplatePlaceholders_WithNonUnitFiles_ShouldIgnoreThem()
    {
        var world = new WorldState();
        var contentFiles = new List<GameContentFileInfo>
        {
            new("weapons", "weapon", "sample_weapon.json", "weapons/sample_weapon.json", ".json"),
            new("icons", "icon", "sample_icon.svg", "icons/sample_icon.svg", ".svg")
        };

        var result = ProjectContentWorldSeeder.SeedUnitTemplatePlaceholders(world, contentFiles);

        Assert.Equal(0, result.CreatedEntityCount);
        Assert.Empty(result.SourcePaths);
        Assert.Empty(world.ListEntities());
    }

    [Fact]
    public void SeedUnitTemplatePlaceholders_WithMultipleUnitFiles_ShouldCreateMultipleEntities()
    {
        var world = new WorldState();
        var contentFiles = new List<GameContentFileInfo>
        {
            new("units", "unitTemplate", "alpha.json", "units/alpha.json", ".json"),
            new("units", "unitTemplate", "bravo.json", "units/bravo.json", ".json")
        };

        var result = ProjectContentWorldSeeder.SeedUnitTemplatePlaceholders(world, contentFiles);

        Assert.Equal(2, result.CreatedEntityCount);
        Assert.Equal(2, world.ListEntities().Count);
        Assert.Equal(2, result.SourcePaths.Count);
    }

    [Fact]
    public void SeedUnitTemplatePlaceholders_ShouldUseFileNameWithoutExtensionAsDisplayName()
    {
        var world = new WorldState();
        var contentFiles = new List<GameContentFileInfo>
        {
            new("units", "unitTemplate", "sample_unit.json", "units/sample_unit.json", ".json")
        };

        ProjectContentWorldSeeder.SeedUnitTemplatePlaceholders(world, contentFiles);

        var entity = world.ListEntities()[0];
        Assert.Equal("sample_unit", entity.DisplayName);
    }

    [Fact]
    public void SeedUnitTemplatePlaceholders_ShouldStoreSourcePath()
    {
        var world = new WorldState();
        var contentFiles = new List<GameContentFileInfo>
        {
            new("units", "unitTemplate", "sample_unit.json", "units/sample_unit.json", ".json")
        };

        ProjectContentWorldSeeder.SeedUnitTemplatePlaceholders(world, contentFiles);

        var entity = world.ListEntities()[0];
        Assert.NotNull(entity.Source);
        Assert.Equal("units/sample_unit.json", entity.Source.RelativePath);
        Assert.Equal("unitTemplate", entity.Source.ContentKind);
    }

    [Fact]
    public void SeedUnitTemplatePlaceholders_ShouldUseStableOrder()
    {
        var world = new WorldState();
        var contentFiles = new List<GameContentFileInfo>
        {
            new("units", "unitTemplate", "zulu.json", "units/zulu.json", ".json"),
            new("units", "unitTemplate", "alpha.json", "units/alpha.json", ".json"),
            new("units", "unitTemplate", "bravo.json", "units/bravo.json", ".json")
        };

        var result = ProjectContentWorldSeeder.SeedUnitTemplatePlaceholders(world, contentFiles);

        Assert.Equal(3, result.CreatedEntityCount);
        Assert.Equal("units/alpha.json", result.SourcePaths[0]);
        Assert.Equal("units/bravo.json", result.SourcePaths[1]);
        Assert.Equal("units/zulu.json", result.SourcePaths[2]);
    }

    [Fact]
    public void SeedUnitTemplatePlaceholders_WithEmptyFiles_ShouldReturnZero()
    {
        var world = new WorldState();

        var result = ProjectContentWorldSeeder.SeedUnitTemplatePlaceholders(world, []);

        Assert.Equal(0, result.CreatedEntityCount);
        Assert.Empty(result.SourcePaths);
    }

    [Fact]
    public void SeedUnitTemplatePlaceholders_WithNullWorld_ShouldThrow()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => ProjectContentWorldSeeder.SeedUnitTemplatePlaceholders(null!, []));

        Assert.Contains("worldState", ex.Message);
    }
}
