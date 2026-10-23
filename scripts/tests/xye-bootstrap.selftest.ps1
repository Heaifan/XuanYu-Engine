$ErrorActionPreference = 'Stop'

$repo = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path
$workspace = Join-Path $repo '.xye-workspace'
$tempRoot = Join-Path $workspace ('Temp/XYW-BOOTSTRAP-R2-selftest-' + [guid]::NewGuid().ToString('N'))
$null = New-Item -ItemType Directory -Path $tempRoot -Force
$foreignWorktree = Join-Path $tempRoot 'foreign-worktree'

function Assert-True([bool]$Condition, [string]$Message) {
    if (-not $Condition) { throw $Message }
}

function Quote-Argument([string]$Value) {
    return '"' + $Value.Replace('"', '\"') + '"'
}

function Invoke-Bootstrap([string]$WorkingDirectory, [string[]]$ExtraArguments, [string]$ScriptPath = '') {
    $stdout = Join-Path $tempRoot ('stdout-' + [guid]::NewGuid().ToString('N') + '.txt')
    $stderr = Join-Path $tempRoot ('stderr-' + [guid]::NewGuid().ToString('N') + '.txt')
    $pwsh = (Get-Command pwsh.exe -ErrorAction Stop).Source
    if ([string]::IsNullOrWhiteSpace($ScriptPath)) { $ScriptPath = Join-Path $repo 'scripts/xye-bootstrap.ps1' }
    $childArguments = @('-NoLogo', '-NoProfile', '-ExecutionPolicy', 'Bypass', '-File', $ScriptPath) + $ExtraArguments
    $argumentLine = ($childArguments | ForEach-Object { Quote-Argument ([string]$_) }) -join ' '
    try {
        $process = Start-Process -FilePath $pwsh -ArgumentList $argumentLine -WorkingDirectory $WorkingDirectory -Wait -PassThru -WindowStyle Hidden -RedirectStandardOutput $stdout -RedirectStandardError $stderr
        return [pscustomobject]@{
            ExitCode = $process.ExitCode
            Output = ((Get-Content -LiteralPath $stdout -Raw -ErrorAction SilentlyContinue) + (Get-Content -LiteralPath $stderr -Raw -ErrorAction SilentlyContinue))
        }
    }
    finally {
        Remove-Item -LiteralPath $stdout, $stderr -Force -ErrorAction SilentlyContinue
    }
}

try {
    $head = (& git -C $repo rev-parse HEAD).Trim()
    Assert-True ($LASTEXITCODE -eq 0) 'Cannot resolve current worktree HEAD.'
    $branchOutput = @(& git -C $repo branch --show-current)
    $branch = ($branchOutput -join '').Trim()
    Assert-True ([string]::IsNullOrWhiteSpace($branch)) 'Detached HEAD fixture is required.'
    $commonDir = (& git -C $repo rev-parse --git-common-dir).Trim()
    if (-not [IO.Path]::IsPathRooted($commonDir)) { $commonDir = Join-Path $repo $commonDir }
    $commonDir = [IO.Path]::GetFullPath($commonDir)

    $nested = Join-Path $tempRoot 'nested/level'
    $null = New-Item -ItemType Directory -Path $nested -Force
    $legal = Invoke-Bootstrap $nested @('-RepositoryRoot', $repo)
    Assert-True ($legal.ExitCode -eq 0) "Legal nested invocation failed with $($legal.ExitCode): $($legal.Output)"
    Assert-True ($legal.Output.Contains("SourceRoot: $repo")) 'Nested invocation did not resolve the worktree SourceRoot.'
    Assert-True ($legal.Output.Contains("GitCommonDir: $commonDir")) 'Git CommonDir was not reported separately.'
    Assert-True ($legal.Output.Contains("HEAD: $head")) 'Nested invocation reported the wrong HEAD.'
    Assert-True ($legal.Output.Contains('Branch: (detached)')) 'Detached HEAD was not reported explicitly.'

    $external = Invoke-Bootstrap ([IO.Path]::GetPathRoot($repo)) @()
    Assert-True ($external.ExitCode -eq 2) "External directory refusal returned unexpected exit code $($external.ExitCode)."

    $wrongRoot = Invoke-Bootstrap $repo @('-RepositoryRoot', (Split-Path -Parent (Split-Path -Parent $repo)))
    Assert-True ($wrongRoot.ExitCode -eq 2) "Wrong RepositoryRoot refusal returned unexpected exit code $($wrongRoot.ExitCode)."

    $fakeScriptDirectory = Join-Path $tempRoot 'spoof/scripts'
    $null = New-Item -ItemType Directory -Path $fakeScriptDirectory -Force
    $fakeBootstrap = Join-Path $fakeScriptDirectory 'xye-bootstrap.ps1'
    Copy-Item -LiteralPath (Join-Path $repo 'scripts/xye-bootstrap.ps1') -Destination $fakeBootstrap
    $wrongEntry = Invoke-Bootstrap $repo @() $fakeBootstrap
    Assert-True ($wrongEntry.ExitCode -eq 2) "Wrong entry refusal returned unexpected exit code $($wrongEntry.ExitCode)."
    Assert-True ($wrongEntry.Output.Contains('Actual entry script is not the Bootstrap owned')) 'Wrong-entry refusal reason was not reported.'

    $null = & git -C $repo worktree add -q --detach --no-checkout $foreignWorktree $head 2>$null
    $worktreeExitCode = $LASTEXITCODE
    Assert-True ($worktreeExitCode -eq 0) "Could not create isolated cross-worktree fixture (exit $worktreeExitCode)."
    $foreignRoot = (& git -C $foreignWorktree rev-parse --show-toplevel).Trim()
    Assert-True ($LASTEXITCODE -eq 0) 'Temporary cross-worktree fixture is invalid.'
    $cross = Invoke-Bootstrap $foreignRoot @()
    Assert-True ($cross.ExitCode -eq 2) "Cross-worktree refusal returned unexpected exit code $($cross.ExitCode)."
    Assert-True ($cross.Output.Contains('Cross-worktree invocation is refused')) 'Cross-worktree refusal reason was not reported.'

    $versionOut = Join-Path $tempRoot 'xye-dotnet-version.txt'
    $dotnetScript = Join-Path $repo 'scripts/xye-dotnet.ps1'
    $versionLines = @(& pwsh -NoLogo -NoProfile -ExecutionPolicy Bypass -File $dotnetScript --version 2>&1)
    $versionExit = $LASTEXITCODE
    $versionLines | Set-Content -LiteralPath $versionOut -Encoding utf8
    Assert-True ($versionExit -eq 0) "Worktree xye-dotnet resolver failed with ${versionExit}: $($versionLines -join "`n")"
    $versionText = Get-Content -LiteralPath $versionOut -Raw
    Assert-True (-not [string]::IsNullOrWhiteSpace($versionText.Trim())) 'Worktree xye-dotnet returned no SDK version.'

    Write-Output 'XYE BOOTSTRAP SELFTEST: PASS'
    Write-Output "Nested current-worktree path: PASS (exit $($legal.ExitCode))"
    Write-Output "External directory refusal: PASS (exit $($external.ExitCode))"
    Write-Output "Wrong RepositoryRoot refusal: PASS (exit $($wrongRoot.ExitCode))"
    Write-Output "Actual entry ownership: PASS (exit $($wrongEntry.ExitCode))"
    Write-Output "Detached HEAD: PASS (branch='$branch', exit $($legal.ExitCode))"
    Write-Output "Cross-worktree script invocation refusal: PASS (exit $($cross.ExitCode))"
    Write-Output 'Worktree xye-dotnet entry: PASS'
    exit 0
}
catch {
    [Console]::Error.WriteLine("XYE BOOTSTRAP SELFTEST: FAIL`n$($_.Exception.Message)`n$($_.ScriptStackTrace)")
    exit 1
}
finally {
    if (Test-Path -LiteralPath $foreignWorktree) {
        $null = & git -C $repo worktree remove --force $foreignWorktree 2>&1
        if ($LASTEXITCODE -ne 0) { [Console]::Error.WriteLine('Could not remove temporary cross-worktree fixture.') }
    }
    Remove-Item -LiteralPath $tempRoot -Recurse -Force -ErrorAction SilentlyContinue
}
