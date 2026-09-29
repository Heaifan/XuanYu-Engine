[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$runner = Join-Path $PSScriptRoot 'process-runner.ps1'
. $runner

function Assert-True([bool]$condition, [string]$message) {
    if (!$condition) { throw $message }
}

function Find-Executable([string]$name) {
    $command = Get-Command $name -ErrorAction SilentlyContinue
    if ($null -eq $command) { return $null }
    return $command.Source
}

$powershell = Find-Executable 'powershell.exe'
$pwsh = Find-Executable 'pwsh.exe'
$git = Find-Executable 'git.exe'
Assert-True ($powershell -or $pwsh) 'No PowerShell executable available.'
Assert-True ([bool]$git) 'Git executable unavailable.'

function Test-PowerShell([string]$shell) {
    $run = Invoke-HandoffProcess -FilePath $shell -Arguments @(
        '-NoLogo', '-NoProfile', '-NonInteractive', '-Command',
        '[Console]::Error.Write("runner-error"); Write-Output "runner-output"; exit 7')
    Assert-True ($run.ExitCode -eq 7) "$shell exit code was not propagated."
    Assert-True ($run.Stdout -match 'runner-output') "$shell stdout was not captured."
    Assert-True ($run.Stderr -match 'runner-error') "$shell stderr was not captured."
    Assert-True ($run.Command -eq $shell) 'Command was not retained.'
    Assert-True ($run.Arguments -match '-Command') 'Arguments were not retained.'
    Assert-True ($run.Duration -ge 0) 'Duration was not retained.'
}

if ($powershell) { Test-PowerShell $powershell }
if ($pwsh) { Test-PowerShell $pwsh }

$gitRun = Invoke-HandoffProcess -FilePath $git -Arguments @('--version')
Assert-True ($gitRun.ExitCode -eq 0) 'Git did not execute through the runner.'
Assert-True ($gitRun.Stdout -match 'git version') 'Git stdout was not captured.'

$gitFailure = Invoke-HandoffProcess -FilePath $git -Arguments @('not-a-real-command')
Assert-True ($gitFailure.ExitCode -ne 0) 'Git failure exit code was swallowed.'
Assert-True (-not [string]::IsNullOrWhiteSpace($gitFailure.Stderr)) 'Git failure stderr was swallowed.'

Write-Output 'PROCESS RUNNER SELFTEST: PASS'
