# ARCH-WORLD red-line guards (R1-R1 hardening), dot-sourced by arch-a-guard.ps1.
# Enforces the physical layering introduced by ARCH-WORLD-R1:
#   World -> Core (allowed)   Core -/-> World   World -/-> Editor/Vulkan/Avalonia/Silk
# ProjectReference elements are parsed explicitly (not whole-file substring) so the
# legitimate InternalsVisibleTo("XuanYu.World.Tests") attribute is NOT mistaken for a
# production->test dependency.
function Get-ProjectReferences([string]$path) {
    $text = Read-Text $path
    $refs = New-Object System.Collections.Generic.List[string]
    $matches = [regex]::Matches($text, '<ProjectReference\s+Include="([^"]+)"')
    foreach ($m in $matches) { $refs.Add($m.Groups[1].Value) }
    return $refs
}

$coreCsproj = "XuanYu.Core/XuanYu.Core.csproj"
$worldCsproj = "XuanYu.World/XuanYu.World.csproj"

# Core is the pure base layer: it must NOT reference World / Editor / Vulkan via ProjectReference.
foreach ($ref in (Get-ProjectReferences $coreCsproj)) {
    if ($ref -match "XuanYu\.World|XuanYu\.Editor|XuanYu\.Render\.Vulkan|Silk\.NET\.Vulkan") {
        Add-Failure "Core project reference forbidden: $ref ($coreCsproj)"
    }
}

# World may ONLY depend on Core via ProjectReference (this also rejects World->World.Tests).
foreach ($ref in (Get-ProjectReferences $worldCsproj)) {
    if ($ref -notmatch "XuanYu\.Core\\XuanYu\.Core\.csproj$") {
        Add-Failure "World project reference forbidden (only Core allowed): $ref ($worldCsproj)"
    }
}

# World production source must not import Editor / Vulkan / Avalonia / Silk namespaces.
# NOTE: XuanYu.Core.Gizmo is still mis-placed (debt D1) but is NOT XuanYu.Editor.*, so this
# passes today. New World->Core.Gizmo dependencies are forbidden by the R1-R1 ruling.
foreach ($file in Get-SourceFiles "XuanYu.World") {
    Assert-NotContains $file.FullName @("using XuanYu.Editor.", "using XuanYu.Render.Vulkan", "using Silk.NET.Vulkan", "using Avalonia") "World source reference"
}

# R2-R1 single spatial authority: the WHOLE World assembly may contain exactly ONE
# authorized SpatialIndexOwner instance path. Only WorldQuery.cs may construct it; any
# other World file (Scene/Picking/Streaming/Partition/...) creating a second index is
# rejected. This locks "single spatial index" as a machine-enforced rule, not a convention.
foreach ($file in Get-SourceFiles "XuanYu.World") {
    if ($file.Name -eq "WorldQuery.cs") { continue }
    Assert-NotContains $file.FullName @("new SpatialIndexOwner") "Only WorldQuery may own the unique SpatialIndexOwner"
}

# R2-R1: WorldQuery mutation API (Insert/Update/Remove/Rebuild) is `internal` -- that only
# restricts it to the XuanYu.World assembly, but a SECOND in-assembly caller (e.g. a future
# Streaming/Partition system) would still compile. Lock the production call sites to the
# World authority whitelist so "single Writer" is machine-enforced, not merely conventional.
# NOTE: this keys on the `_query` field name used by GlobalWorld; if that field is renamed
# the guard must be updated to keep matching the real call sites.
$writeWhitelist = @("GlobalWorld.cs", "GlobalWorld.Query.cs", "GlobalWorld.Authoring.cs", "WorldQuery.cs")
$mutationPatterns = @("_query.Insert(", "_query.Update(", "_query.Remove(", "_query.Rebuild(")
foreach ($file in Get-SourceFiles "XuanYu.World") {
    if ($writeWhitelist -contains $file.Name) { continue }
    Assert-NotContains $file.FullName $mutationPatterns "WorldQuery mutation only callable from World authority (GlobalWorld)"
}

# Solution must contain the new World assemblies; otherwise the physical boundary is gone
# without the guard noticing (the old $projects list omitted them).
Assert-Contains "XuanYu.Engine.slnx" "XuanYu.World/XuanYu.World.csproj" "solution contains World"
Assert-Contains "XuanYu.Engine.slnx" "XuanYu.World.Tests/XuanYu.World.Tests.csproj" "solution contains World.Tests"
