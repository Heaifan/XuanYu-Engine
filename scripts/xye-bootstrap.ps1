[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$required = @('AGENTS.md', 'run.bat', 'scripts\resolve-dotnet.ps1')
foreach ($path in $required) {
    if (-not (Test-Path (Join-Path $repo $path) -PathType Leaf)) {
        Write-Error "Required repository file is missing: $path"
        exit 1
    }
}

$resolver = Join-Path $repo 'scripts\resolve-dotnet.ps1'
$dotnet = & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $resolver
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($dotnet)) {
    Write-Error 'Canonical toolchain resolver failed.'
    exit $(if ($LASTEXITCODE -ne 0) { $LASTEXITCODE } else { 1 })
}

$sdk = (& $dotnet --version).Trim()
if ($LASTEXITCODE -ne 0) { Write-Error 'Resolved dotnet could not run.'; exit $LASTEXITCODE }
$branch = (& git -C $repo branch --show-current).Trim()
$head = (& git -C $repo rev-parse HEAD).Trim()
$statusOutput = & git -C $repo status --short
$status = if ($null -eq $statusOutput) { '' } else { $statusOutput.Trim() }
$origin = (& git -C $repo remote get-url origin).Trim()
if ($LASTEXITCODE -ne 0) { Write-Error 'Git repository facts could not be read.'; exit $LASTEXITCODE }

Write-Output 'XYE BOOTSTRAP REPORT'
Write-Output "Repository: $repo"
Write-Output "Branch: $branch"
Write-Output "HEAD: $head"
Write-Output "WorkingTree: $(if ([string]::IsNullOrWhiteSpace($status)) { 'clean' } else { 'dirty' })"
Write-Output "Origin: $origin"
Write-Output "DOTNET_EXE: $dotnet"
Write-Output "SDK: $sdk"
Write-Output 'Resolver: scripts/resolve-dotnet.ps1'
Write-Output 'State: READY'
