using FluidWarfare.Core.Results;
using FluidWarfare.Project.Metadata;
using FluidWarfare.Project.Validation;

namespace FluidWarfare.Project.Loading;

public sealed record GameProjectLoadResult(
    EngineResult Result,
    GameProjectInfo? Project,
    ProjectValidationReport ValidationReport);
