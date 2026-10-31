$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$entry = Join-Path $root 'scripts\xye-dotnet.ps1'
$source = Get-Content -LiteralPath $entry -Raw
if ($source -match 'xye-workspace|XYE_WORKSPACE_ROOT|XYE_BUILD_DIRECTORY') {
    throw 'The dotnet entrypoint still contains task workspace routing.'
}
if ($source -notmatch 'Rebuild' -or $source -notmatch 'Custom output or NuGet cache paths are forbidden') {
    throw 'The dotnet entrypoint is missing a destructive-target or output-path guard.'
}

$project = 'XuanYu.World.Tests\XuanYu.World.Tests.csproj'
function Invoke-Wrapper([string[]]$Arguments) {
    $output = @(& powershell.exe -NoProfile -ExecutionPolicy Bypass -File $script:entry @Arguments 2>&1)
    return [pscustomobject]@{ ExitCode=$LASTEXITCODE; Output=($output -join "`n") }
}
function Assert-Refused([string]$Name, [string[]]$Arguments, [string]$Message) {
    $result = Invoke-Wrapper $Arguments
    if ($result.ExitCode -eq 0 -or $result.Output -notmatch [regex]::Escape($Message)) {
        throw "$Name was not refused before dotnet execution: exit=$($result.ExitCode); $($result.Output)"
    }
    Write-Output "PASS: $Name refused (exit $($result.ExitCode))."
}

$rebuildForms = @(
    @('-t:Rebuild'), @('/t:Rebuild'), @('-target:Rebuild'), @('/target:Rebuild'),
    @('-t','Rebuild'), @('/t','Rebuild'), @('-target','Rebuild'),
    @('--target','Rebuild'), @('--target=Rebuild'), @('-t=Rebuild'),
    @('-t:Build;rebuild'), @('-target:Rebuild;Build'), @('--target','Build;REBUILD')
)
foreach ($form in $rebuildForms) {
    Assert-Refused "Rebuild $($form -join ' ')" (@('build',$project,'--no-restore') + $form + @('--help')) 'Rebuild is forbidden'
}

$outputForms = @(
    @('--artifacts-path','C:\temp\xy'), @('--artifacts-path=C:\temp\xy'),
    @('--output','C:\temp\xy'), @('--output=C:\temp\xy'), @('-o','C:\temp\xy'),
    @('-o:C:\temp\xy'), @('/p:OutDir=C:\temp\xy'), @('-p:BaseOutputPath=C:\temp\xy'),
    @('/p:BaseIntermediateOutputPath=C:\temp\xy'),
    @('--property:MSBuildProjectExtensionsPath=C:\temp\xy'),
    @('-property:OutputPath=C:\temp\xy'), @('/property:PublishDir=C:\temp\xy'),
    @('--property','RestorePackagesPath=C:\temp\xy'),
    @('-p:ArtifactsPath=C:\temp\xy'), @('-p:NuGetPackageRoot=C:\temp\xy'),
    @('-p','RestorePackagesPath=C:\temp\xy')
)
foreach ($form in $outputForms) {
    $command = if ($form[0] -in @('--output','-o')) { 'publish' } else { 'build' }
    Assert-Refused "output override $($form -join ' ')" (@($command,$project,'--no-restore') + $form + @('--help')) 'Custom output or NuGet cache paths are forbidden'
}

foreach ($arguments in @(
    @('build',$project,'--no-restore','--help'),
    @('restore',$project,'--help'),
    @('test',$project,'--no-build','--no-restore','--help')
)) {
    $result = Invoke-Wrapper $arguments
    if ($result.ExitCode -ne 0) { throw "A permitted command was rejected: $($arguments -join ' '); $($result.Output)" }
}

$activeBuilds = @(Get-CimInstance Win32_Process -Filter "Name='dotnet.exe' OR Name='MSBuild.exe'" |
    Where-Object { $_.CommandLine -and $_.CommandLine.IndexOf($root,[StringComparison]::OrdinalIgnoreCase) -ge 0 -and
        $_.CommandLine -notmatch '/nodemode:' })
if ($activeBuilds.Count) { throw 'A refusal test started a repository build process.' }
Write-Output 'PASS: normal Build, Restore, and no-build Test help paths remain accepted; no build process was started.'
