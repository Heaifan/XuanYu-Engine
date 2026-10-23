[CmdletBinding()]
param(
    [string]$RepositoryRoot
)

$ErrorActionPreference = 'Stop'

function Get-GitValue([string]$Directory, [string[]]$Arguments) {
    $output = @(& git -C $Directory @Arguments 2>&1)
    $exitCode = $LASTEXITCODE
    if ($exitCode -ne 0) {
        throw "Git command failed in '$Directory' (exit $exitCode): git $($Arguments -join ' ')`n$($output -join "`n")"
    }
    return ($output -join "`n").Trim()
}

function Resolve-ExistingDirectory([string]$Path, [string]$Label) {
    if ([string]::IsNullOrWhiteSpace($Path)) { throw "$Label is empty." }
    $resolved = Resolve-Path -LiteralPath $Path -ErrorAction Stop
    if ($resolved.Provider.Name -ne 'FileSystem') { throw "$Label is not a filesystem path: $Path" }
    $item = Get-Item -LiteralPath $resolved.Path -ErrorAction Stop
    if (-not $item.PSIsContainer) { throw "$Label is not a directory: $Path" }
    return [IO.Path]::GetFullPath($item.FullName).TrimEnd('\', '/')
}

function Test-SamePath([string]$Left, [string]$Right) {
    return [string]::Equals($Left.TrimEnd('\', '/'), $Right.TrimEnd('\', '/'), [StringComparison]::OrdinalIgnoreCase)
}

try {
    $entryPath = (Resolve-Path -LiteralPath $PSCommandPath -ErrorAction Stop).Path
    $scriptDirectory = Split-Path -Parent $entryPath
    $scriptSourceRoot = Get-GitValue $scriptDirectory @('rev-parse', '--show-toplevel')
    $scriptSourceRoot = Resolve-ExistingDirectory $scriptSourceRoot 'Bootstrap script SourceRoot'

    $workingDirectory = (Get-Location).ProviderPath
    if ([string]::IsNullOrWhiteSpace($workingDirectory)) { throw 'Current location must be a filesystem directory.' }
    $workingSourceRoot = Get-GitValue $workingDirectory @('rev-parse', '--show-toplevel')
    $workingSourceRoot = Resolve-ExistingDirectory $workingSourceRoot 'Current worktree SourceRoot'

    if (-not (Test-SamePath $scriptSourceRoot $workingSourceRoot)) {
        throw "Bootstrap script belongs to '$scriptSourceRoot', but the current worktree is '$workingSourceRoot'. Cross-worktree invocation is refused."
    }

    $expectedEntry = [IO.Path]::GetFullPath((Join-Path $scriptSourceRoot 'scripts\xye-bootstrap.ps1'))
    if (-not (Test-SamePath $entryPath $expectedEntry)) {
        throw "Actual entry script is not the Bootstrap owned by SourceRoot '$scriptSourceRoot': $entryPath"
    }

    if (-not [string]::IsNullOrWhiteSpace($RepositoryRoot)) {
        $explicitRoot = Resolve-ExistingDirectory $RepositoryRoot 'RepositoryRoot'
        if (-not (Test-SamePath $explicitRoot $workingSourceRoot)) {
            throw "Explicit RepositoryRoot '$explicitRoot' does not match current worktree SourceRoot '$workingSourceRoot'."
        }
    }

    $repo = $workingSourceRoot
    $commonDir = Get-GitValue $repo @('rev-parse', '--path-format=absolute', '--git-common-dir')
    if (-not [IO.Path]::IsPathRooted($commonDir)) { $commonDir = Join-Path $repo $commonDir }
    $commonDir = [IO.Path]::GetFullPath($commonDir)
    if (-not (Test-Path -LiteralPath $commonDir -PathType Container)) { throw "Git CommonDir is not an existing directory: $commonDir" }

    $required = @('AGENTS.md', 'run.bat', 'scripts\resolve-dotnet.ps1')
    foreach ($path in $required) {
        if (-not (Test-Path -LiteralPath (Join-Path $repo $path) -PathType Leaf)) {
            throw "Required repository file is missing: $path"
        }
    }

    $resolver = Join-Path $repo 'scripts\resolve-dotnet.ps1'
    $powershell = (Get-Command powershell.exe -ErrorAction Stop).Source
    $dotnetOutput = @(& $powershell -NoProfile -ExecutionPolicy Bypass -File $resolver 2>&1)
    $resolverExitCode = $LASTEXITCODE
    $dotnet = ($dotnetOutput -join "`n").Trim()
    if ($resolverExitCode -ne 0 -or [string]::IsNullOrWhiteSpace($dotnet)) {
        throw "Worktree toolchain resolver failed (exit $resolverExitCode): $($dotnetOutput -join "`n")"
    }

    $sdkOutput = @(& $dotnet --version 2>&1)
    $sdkExitCode = $LASTEXITCODE
    if ($sdkExitCode -ne 0) { throw "Resolved dotnet could not run (exit $sdkExitCode): $($sdkOutput -join "`n")" }
    $sdk = ($sdkOutput -join "`n").Trim()

    $branch = Get-GitValue $repo @('branch', '--show-current')
    if ([string]::IsNullOrWhiteSpace($branch)) { $branch = '(detached)' }
    $head = Get-GitValue $repo @('rev-parse', 'HEAD')
    $status = Get-GitValue $repo @('status', '--porcelain=v1', '--untracked-files=all')
    $origin = Get-GitValue $repo @('remote', 'get-url', 'origin')

    Write-Output 'XYE BOOTSTRAP REPORT'
    Write-Output "Repository: $repo"
    Write-Output "SourceRoot: $repo"
    Write-Output "GitCommonDir: $commonDir"
    Write-Output "Branch: $branch"
    Write-Output "HEAD: $head"
    Write-Output "WorkingTree: $(if ([string]::IsNullOrWhiteSpace($status)) { 'clean' } else { 'dirty' })"
    Write-Output "Origin: $origin"
    Write-Output "DOTNET_EXE: $dotnet"
    Write-Output "SDK: $sdk"
    Write-Output 'Resolver: scripts/resolve-dotnet.ps1'
    Write-Output 'State: READY'
    exit 0
}
catch {
    [Console]::Error.WriteLine("XYE BOOTSTRAP REFUSED: $($_.Exception.Message)")
    exit 2
}
