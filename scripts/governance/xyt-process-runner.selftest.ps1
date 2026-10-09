[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'xyt-process-runner.ps1')
$shells = @(Get-Command powershell.exe, pwsh.exe -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source -Unique)
$git = (Get-Command git.exe -ErrorAction SilentlyContinue).Source
if (!$shells.Count -or !$git) { throw 'PowerShell and Git executables are required.' }
function Assert-True([bool]$condition, [string]$message) { if (!$condition) { throw $message } }
foreach ($shell in $shells) {
    $run = Invoke-XytProcess -FilePath $shell -Arguments @('-NoLogo','-NoProfile','-NonInteractive','-Command','[Console]::Error.Write("runner-error"); Write-Output "runner-output"; exit 7')
    Assert-True ($run.ExitCode -eq 7) 'PowerShell exit code was not propagated.'
    Assert-True ($run.Stdout -match 'runner-output' -and $run.Stderr -match 'runner-error') 'PowerShell streams were not captured.'
    Assert-True ($run.Command -eq $shell -and $run.Arguments -match '-Command' -and $run.Duration -ge 0) 'Process metadata was not retained.'
}
$gitRun = Invoke-XytProcess -FilePath $git -Arguments @('--version')
Assert-True ($gitRun.ExitCode -eq 0 -and $gitRun.Stdout -match 'git version') 'Git did not execute through XYT runner.'
$gitFailure = Invoke-XytProcess -FilePath $git -Arguments @('not-a-real-command')
Assert-True ($gitFailure.ExitCode -ne 0 -and $gitFailure.Stderr.Trim()) 'Git failure details were swallowed.'
Write-Output 'XYT PROCESS RUNNER SELFTEST: PASS'
