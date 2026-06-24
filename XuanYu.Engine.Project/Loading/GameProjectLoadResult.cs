using XuanYu.Engine.Core.Results;
using XuanYu.Engine.Project.Metadata;
using XuanYu.Engine.Project.Validation;

namespace XuanYu.Engine.Project.Loading;

public sealed record GameProjectLoadResult(
    EngineResult Result,
    GameProjectInfo? Project,
    ProjectValidationReport ValidationReport);
