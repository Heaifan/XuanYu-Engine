using FluidWarfare.Project.Validation;

namespace FluidWarfare.Tests.Project.Validation;

public sealed class ProjectValidationReportTests
{
    [Fact]
    public void Empty_ShouldHaveNoIssues()
    {
        var report = ProjectValidationReport.Empty;

        Assert.False(report.HasIssues);
        Assert.Equal(0, report.IssueCount);
        Assert.Null(report.FirstIssue);
    }

    [Fact]
    public void WithIssues_ShouldExposeIssueCount()
    {
        var issues = new List<ProjectValidationIssue>
        {
            new("Project.UndeclaredContentFolder", "未声明目录。", "backup"),
            new("Project.ContentFileExtensionNotAllowed", "扩展名不允许。", "units/bad.txt")
        };

        var report = new ProjectValidationReport(issues);

        Assert.True(report.HasIssues);
        Assert.Equal(2, report.IssueCount);
    }

    [Fact]
    public void WithIssues_ShouldExposeFirstIssue()
    {
        var issues = new List<ProjectValidationIssue>
        {
            new("Project.UndeclaredContentFolder", "未声明目录。", "backup"),
            new("Project.ContentFileExtensionNotAllowed", "扩展名不允许。", "units/bad.txt")
        };

        var report = new ProjectValidationReport(issues);

        Assert.NotNull(report.FirstIssue);
        Assert.Equal("Project.UndeclaredContentFolder", report.FirstIssue.Code);
        Assert.Equal("backup", report.FirstIssue.Path);
    }
}
