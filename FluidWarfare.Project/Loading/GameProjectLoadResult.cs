using FluidWarfare.Core.Results;
using FluidWarfare.Project.Metadata;

namespace FluidWarfare.Project.Loading;

public sealed record GameProjectLoadResult(
    EngineResult Result,
    GameProjectInfo? Project);
